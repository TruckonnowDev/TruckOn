using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BaceModel.ModelInspertionDriver;
using DaoModels.DAO;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Models;
using WebDispacher.Service;
using WebDispacher.Service.EmailSmtp;
using WebDispacher.ViewModels.Driver;

namespace WebDispacher.Business.Services
{
    public class DriverService : IDriverService
    {
        private readonly Context db;
        private readonly IMapper mapper;
        private readonly StripeApi stripeApi;
        private readonly IUserService userService;
        private readonly ICompanyService companyService;
        private readonly ITruckAndTrailerService truckAndTrailerService;

        public DriverService(
            IMapper mapper,
            Context db,
            ITruckAndTrailerService truckAndTrailerService,
            IUserService userService,
            ICompanyService companyService)
        {
            this.truckAndTrailerService = truckAndTrailerService;
            this.companyService = companyService;
            this.userService = userService;
            this.mapper = mapper;
            this.db = db;
            stripeApi = new StripeApi();
        }

        public List<Driver> GetDriversByIdCompany(string idCompany)
        {
            return db.Drivers.Where(d => !d.IsFired && d.CompanyId.ToString() == idCompany).ToList();
        }

        public async Task RemoveCompany(string idCompany)
        {
            RemoveCompanyDb(idCompany);
            var customerSt = companyService.GetCustomer_STByIdCompany(idCompany);
            var customer = stripeApi.RemoveCustomer(customerSt);

            if (customer != null)
            {
                RemoveCustomerST(customerSt);
                RemoveSubscribeST(customerSt);
            }

            await Task.Run(() =>
            {
                var drivers = GetDriversByIdCompany(idCompany);
                foreach (var driver in drivers)
                {
                    RemoveDrive(idCompany,
                        new DriverReportModel
                        {
                            Id = driver.Id,
                            Description = CompanyConstants.RemoveCompanyDescription
                        });
                }
            });
        }

        private void RemoveCompanyDb(string id)
        {
            db.User.RemoveRange(db.User.Where(u => u.CompanyId.ToString() == id));
            db.Commpanies.Remove(db.Commpanies.First(dc => dc.Id.ToString() == id));
            db.SaveChanges();
        }

        private void RemoveSubscribeST(Customer_ST customer_ST)
        {
            var subscribeSt =
                db.Subscribe_STs.FirstOrDefault(s => s.IdCustomerST == customer_ST.IdCustomerST);
            if (subscribeSt == null) return;
            
            db.Subscribe_STs.Remove(subscribeSt);
            db.SaveChanges();
        }

        private void RemoveCustomerST(Customer_ST customer_ST)
        {
            db.Customer_STs.Remove(customer_ST);
            db.SaveChanges();
        }

        public List<ProfileSettingsDTO> GetSetingsTruck(string idCompany, int idProfile, int idTr, string typeTransport)
        {
            var tr = truckAndTrailerService.GetTr(idTr, typeTransport);
            var typeTransportVehikle =
                tr is Truck ? TypeTransportVehikle.Truck : TypeTransportVehikle.Trailer;
            var profileSettings = new List<ProfileSettingsDTO>();
            var profileSettings1 = GetSetingsDb(idCompany, typeTransportVehikle, idTr);

            profileSettings.Add(new ProfileSettingsDTO()
            {
                Name = DriverConstants.StandartName,
                TypeTransportVehikle = typeTransportVehikle.ToString(),
                Id = 0,
                IsSelect = 0 == idProfile,
                IsUsed = profileSettings1.FirstOrDefault(p => p.IsUsed) == null,
                IdCompany = 0
            });

            if (profileSettings1.Count != 0)
            {
                profileSettings.AddRange(profileSettings1.Select(z => new ProfileSettingsDTO()
                {
                    Id = z.Id,
                    IsChange = true,
                    IsSelect = z.Id == idProfile,
                    Name = z.Name,
                    TransportVehicle = z.TransportVehicle,
                    TypeTransportVehikle = z.TypeTransportVehikle,
                    IsUsed = z.IsUsed,
                    IdCompany = z.IdCompany,
                    IdTr = z.IdTr
                }));
            }

            return profileSettings;
        }

        public int CheckTokenFoDriver(string idDriver, string token)
        {
            var isStateActual = 0;
            var passwordRecovery = db.PasswordRecoveries.ToList()
                .FirstOrDefault(p => p.IdDriver.ToString() == idDriver && p.Token == token);
            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                isStateActual = 1;
            }

            return isStateActual;
        }

        public InspectionDriver GetInspectionTruck(string idInspection)
        {
            return db.InspectionDrivers
                .Where(i => i.Id.ToString() == idInspection)
                .Include(i => i.PhotosTruck)
                .First();
        }

        public void RemoveDrive(string idCompany, DriverReportModel model)
        {
            RemoveDriveInDb(model);
            Task.Run(() => UpdatePlanSubscribe(idCompany));
        }

        public void EditDriver(DriverViewModel driver)
        {
            var driverEdit = db.Drivers.FirstOrDefault(d => d.Id == driver.Id);

            if (driverEdit == null) return;
            
            driverEdit.FullName = driver.FullName;
            driverEdit.EmailAddress = driver.EmailAddress;
            driverEdit.Password = driver.Password;
            driverEdit.PhoneNumber = driver.PhoneNumber;
            driverEdit.TrailerCapacity = driver.TrailerCapacity;
            driverEdit.DriversLicenseNumber = driver.DriversLicenseNumber;
            
            db.SaveChanges();
        }

        public async Task CreateDriver(DriverViewModel driver,
            IFormFile dLDoc, IFormFile medicalCardDoc, IFormFile sSNDoc, IFormFile proofOfWorkAuthorizationOrGCDoc,
            IFormFile dQLDoc, IFormFile contractDoc, IFormFile drugTestResultsDo)
        {
            driver.DateRegistration = DateTime.Now.ToString();

            var id = AddDriver(mapper.Map<Driver>(driver));
            
            await SaveDocDriver(dLDoc, DocAndFileConstants.Dl, id.ToString());
            await SaveDocDriver(medicalCardDoc, DocAndFileConstants.MedicalCard, id.ToString());
            await SaveDocDriver(sSNDoc, DocAndFileConstants.Ssn, id.ToString());
            await SaveDocDriver(proofOfWorkAuthorizationOrGCDoc,  DocAndFileConstants.ProofOfWork, id.ToString());
            await SaveDocDriver(dQLDoc, DocAndFileConstants.Dql, id.ToString());
            await SaveDocDriver(contractDoc, DocAndFileConstants.Contract, id.ToString());

            if (drugTestResultsDo != null)
            {
                await SaveDocDriver(drugTestResultsDo, DocAndFileConstants.DrugTestResult, id.ToString());
            }

            await Task.Run(() => UpdatePlanSubscribe(driver.CompanyId.ToString()));
        }

        public Driver GetDriver(string idInspection)
        {
            var inspectionDriver = db.InspectionDrivers.First(i => i.Id.ToString() == idInspection);
            return db.Drivers
                .Include(d => d.InspectionDrivers)
                .First(d => d.InspectionDrivers.FirstOrDefault(ii => ii.Id == inspectionDriver.Id) != null);
        }

        public Driver GetDriverById(int id)
        {
            return db.Drivers.FirstOrDefault(d => d.Id == id);
        }

        public void RemoveDocDriver(string idDock)
        {
            db.DucumentDrivers.Remove(db.DucumentDrivers.FirstOrDefault(d => d.Id.ToString() == idDock));
            db.SaveChanges();
        }

        public async Task<List<Driver>> GetDrivers(string idCompany)
        {
            return await GetDriversInDb(idCompany);
        }

        public List<Driver> GetDrivers(int pag, string idCompany)
        {
            return GetDriversInDb(pag, idCompany);
        }

        public void AddNewReportDriver(DriverReportViewModel driverReport)
        {
            if (!string.IsNullOrEmpty(driverReport.DriversLicenseNumber))
            {
                AddNewReportDriverDb(driverReport);
            }
        }

        public List<DriverReport> GetDriversReport(string nameDriver, string driversLicense)
        {
            var driverReports = new List<DriverReport>();

            if (nameDriver != null || driversLicense != null)
            {
                driverReports.AddRange(db.DriverReports.Where(d =>
                    (nameDriver == d.FullName)
                    && (driversLicense == d.DriversLicenseNumber)));
            }

            return driverReports;
        }

        public async Task<List<DucumentDriver>> GetDriverDoc(string id)
        {
            return await GetDriverDocInDb(id);
        }

        public int CheckReportDriver(string fullName, string driversLicenseNumber)
        {
            var driverReports = new List<DriverReport>();

            if (fullName != null || driversLicenseNumber != null)
            {
                driverReports.AddRange(db.DriverReports.Where(d =>
                    fullName == d.FullName && driversLicenseNumber == d.DriversLicenseNumber));
            }

            return driverReports.Count;
        }

        public async Task<int> ResetPasswordFoDriver(string newPassword, string idDriver, string token)
        {
            var isStateActual = await ResetPasswordFoDriver(newPassword, idDriver, token);

            if (isStateActual == 2)
            {
                var emailDriver = GetEmailDriverDb(idDriver);
                var patern = new PaternSourse().GetPaternDataAccountDriver(emailDriver, newPassword);

                await new AuthMessageSender().Execute(emailDriver, DriverConstants.SuccessfullyChangedPassword, patern);
            }
            else
            {
                var emailDriver = GetEmailDriverDb(idDriver);
                var patern = new PaternSourse().GetPaternNoRestoreDataAccountDriver();

                await new AuthMessageSender().Execute(emailDriver, DriverConstants.PasswordResetFailed, patern);
            }

            return isStateActual;
        }

        public void LayoutDown(int idLayout, int idTransported)
        {
            var transportVehicle = db.TransportVehicles
                .Where(p => p.Id == idTransported)
                .Include(p => p.Layouts)
                .First(p => p.Id == idTransported);

            var layoutsCurrent = transportVehicle.Layouts.First(l => l.Id == idLayout);
            var layoutsDown =
                transportVehicle.Layouts.First(l => l.OrdinalIndex == layoutsCurrent.OrdinalIndex + 1);

            layoutsCurrent.OrdinalIndex++;
            layoutsDown.OrdinalIndex--;

            db.SaveChanges();
        }

        public void UnSelectLayout(int idLayout)
        {
            var transportVehicle = db.TransportVehicles
                .Include(t => t.Layouts)
                .First(t => t.Layouts.FirstOrDefault(l => l.Id == idLayout) != null);
            transportVehicle.CountPhoto--;

            db.SaveChanges();

            var layouts = db.Layouts.FirstOrDefault(l => l.Id == idLayout);

            if (layouts == null) return;
            
            layouts.IsUsed = false;
            db.SaveChanges();
        }

        public void SelectProfile(int idProfile, string typeTransport, int idTr, string idCompany)
        {
            var typeTransportVehikle =
                typeTransport == TruckAndTrailerConstants.Truck ? TypeTransportVehikle.Truck : TypeTransportVehikle.Trailer;

            var profileSettings = GetSetingsDb(idCompany, typeTransportVehikle, idTr);
            var profileSetting = profileSettings.FirstOrDefault(p => p.IsUsed);

            if (idProfile != 0)
            {
                SelectProfileDb(idProfile);
            }

            if (profileSetting != null)
            {
                SelectProfileDb(profileSetting.Id, false);
            }
        }

        public void LayoutUp(int idLayout, int idTransported)
        {
            var transportVehicle = db.TransportVehicles
                .Where(p => p.Id == idTransported)
                .Include(p => p.Layouts)
                .First(p => p.Id == idTransported);

            var layoutsCurrent = transportVehicle.Layouts.First(l => l.Id == idLayout);
            var layoutsUp = transportVehicle.Layouts.First(l => l.OrdinalIndex == layoutsCurrent.OrdinalIndex - 1);

            layoutsCurrent.OrdinalIndex--;
            layoutsUp.OrdinalIndex++;

            db.SaveChanges();
        }

        public void RemoveProfile(string idCompany, int idProfile)
        {
            var profileSetting = db.ProfileSettings
                .Where(p => p.IdCompany.ToString() == idCompany && p.Id == idProfile)
                .Include(p => p.TransportVehicle.Layouts)
                .FirstOrDefault();

            if (profileSetting == null) return;
            
                db.Layouts.RemoveRange(profileSetting.TransportVehicle.Layouts);
                db.TransportVehicles.RemoveRange(profileSetting.TransportVehicle);
                db.ProfileSettings.Remove(db.ProfileSettings.First(p =>
                    p.IdCompany.ToString() == idCompany && p.Id == idProfile));
                db.SaveChanges();

        }

        public void SelectLayout(int idLayout)
        {
            var transportVehicle = db.TransportVehicles
                .Include(t => t.Layouts)
                .First(t => t.Layouts.FirstOrDefault(l => l.Id == idLayout) != null);

            transportVehicle.CountPhoto++;
            db.SaveChanges();

            var layouts = db.Layouts.FirstOrDefault(l => l.Id == idLayout);

            if (layouts == null) return;
            
            layouts.IsUsed = true;
            db.SaveChanges();
        }

        public ProfileSettingsDTO GetSelectSetingTruck(string idCompany, int idProfile, int idTr, string typeTransport)
        {
            ProfileSettingsDTO profileSetting = null;
            
            if (idProfile != 0)
            {
                var profileSetting1 = GetSelectSeting(idCompany, idProfile);
                if (profileSetting1 != null)
                {
                    profileSetting = new ProfileSettingsDTO()
                    {
                        Id = profileSetting1.Id,
                        IsChange = true,
                        IsSelect = idProfile == profileSetting1.Id,
                        Name = profileSetting1.Name,
                        TransportVehicle = profileSetting1.TransportVehicle,
                        TypeTransportVehikle = profileSetting1.TypeTransportVehikle,
                        IsUsed = profileSetting1.IsUsed,
                        IdCompany = profileSetting1.IdCompany
                    };
                }
            }
            else
            {
                var tr = truckAndTrailerService.GetTr(idTr, typeTransport);
                var transportVehicle = HelperTransport.GetTransportVehicle(tr.Type);
                profileSetting = new ProfileSettingsDTO()
                {
                    Id = 0,
                    Name = DriverConstants.StandartName,
                    TransportVehicle = new TransportVehicle()
                    {
                        CountPhoto = transportVehicle.CountPhoto,
                        Id = 0,
                        Layouts = userService.GetLayoutsByTransportVehicle(transportVehicle),
                        Type = transportVehicle.Type,
                        TypeTransportVehicle = transportVehicle.TypeTransportVehicle
                    },
                    TypeTransportVehikle = TruckAndTrailerConstants.Truck,
                    IsChange = false,
                    IsUsed = true,
                    IdCompany = 0
                };
            }

            profileSetting.TransportVehicle.Layouts =
                profileSetting.TransportVehicle.Layouts.OrderBy(l => l.OrdinalIndex).ToList();

            return profileSetting;
        }

        private ProfileSetting GetSelectSeting(string idCompany, int idProfile)
        {
            return db.ProfileSettings
                .Where(p => p.IdCompany.ToString() == idCompany && p.Id == idProfile)
                .Include(p => p.TransportVehicle.Layouts)
                .FirstOrDefault();
        }

        private List<ProfileSetting> GetSetingsDb(string idCompany, TypeTransportVehikle typeTransportVehikle, int idTr)
        {
            List<ProfileSetting> profileSettings = null;

            profileSettings = db.ProfileSettings
                .Where(p => p.IdCompany.ToString() == idCompany &&
                            p.TypeTransportVehikle == typeTransportVehikle.ToString() && p.IdTr == idTr)
                .ToList();

            return profileSettings;
        }

        private void SelectProfileDb(int idProfile, bool isUsed = true)
        {
            var profileSetting = db.ProfileSettings.FirstOrDefault(p => p.Id == idProfile);
            if (profileSetting == null) return;
            
            profileSetting.IsUsed = isUsed;

            db.SaveChanges();
        }

        private string GetEmailDriverDb(string idDriver)
        {
            var emailDriver = "";
            var driver = db.Drivers.FirstOrDefault(d => d.Id.ToString() == idDriver);
            
            if (driver != null)
            {
                emailDriver = driver.EmailAddress;
            }

            return emailDriver;
        }

        private async Task<List<DucumentDriver>> GetDriverDocInDb(string id)
        {
            return await db.DucumentDrivers.Where(d => d.IdDriver.ToString() == id).ToListAsync();
        }

        private async Task<List<Driver>> GetDriversInDb(string idCommpany)
        {
            List<Driver> drivers = null;
            drivers = await db.Drivers
                .Where(d => !d.IsFired && d.CompanyId.ToString() == idCommpany)
                .Include(d => d.InspectionDrivers)
                .Include(d => d.geolocations)
                .ToListAsync();

            return drivers;
        }

        private List<Driver> GetDriversInDb(int page, string idCompany)
        {
            List<Driver> drivers = null;
            drivers = db.Drivers.Where(d => !d.IsFired && d.CompanyId.ToString() == idCompany).ToList();
            if (page == -1)
            {
            }
            else if (page != 0)
            {
                try
                {
                    drivers = drivers.GetRange((20 * page) - 20, 20);
                }
                catch (Exception)
                {
                    drivers = drivers.GetRange((20 * page) - 20, drivers.Count % 20);
                }
            }
            else
            {
                try
                {
                    drivers = drivers.GetRange(0, 20);
                }
                catch (Exception)
                {
                    drivers = drivers.GetRange(0, drivers.Count % 20);
                }
            }

            return drivers;
        }

        public async Task SaveDocDriver(IFormFile uploadedFile, string nameDoc, string id)
        {
            var path = $"../Document/Driver/{id}/" + uploadedFile.FileName;

            if (!Directory.Exists("../Document/Driver"))
            {
                Directory.CreateDirectory($"../Document/Driver");
            }

            if (!Directory.Exists($"../Document/Driver/{id}"))
            {
                Directory.CreateDirectory($"../Document/Driver/{id}");
            }

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }

            SaveDocDriverDb(path, id, nameDoc);
        }

        private void SaveDocDriverDb(string path, string id, string nameDoc)
        {
            var pref = path.Remove(0, path.LastIndexOf(".") + 1);
            var documentCompany = new DucumentDriver()
            {
                DocPath = path,
                NameDoc = nameDoc,
                IdDriver = Convert.ToInt32(id)
            };

            db.DucumentDrivers.Add(documentCompany);

            db.SaveChanges();
        }

        private int AddDriver(Driver driver)
        {
            db.Drivers.Add(driver);

            db.SaveChanges();

            return driver.Id;
        }

        private void AddNewReportDriverDb(DriverReportViewModel driverReport)
        {
            var driver = db.Drivers.FirstOrDefault(d => d.DriversLicenseNumber == driverReport.DriversLicenseNumber);
            
            if(driver == null) return;

            driver.IsFired = true;

            var driverReportReport = new DriverReport()
            {
                Comment = driverReport.Comment,
                DriversLicenseNumber = driverReport.DriversLicenseNumber,
                IdDriver =  driver.Id,
                FullName = driver.FullName,
                DateRegistration = driver.DateRegistration,
                DateFired = DateTime.Now.ToString(),
                AlcoholTendency = driverReport.AlcoholTendency,
                DrivingSkills = driverReport.DrivingSkills,
                DrugTendency = driverReport.DrugTendency,
                EldKnowledge = driverReport.EldKnowledge,
                English = driverReport.English,
                Experience = driverReport.Experience,
                IdCompany = driver.CompanyId,
                PaymentHandling = driverReport.PaymentHandling,
                ReturnedEquipmen = driverReport.ReturnedEquipmen,
                Terminated = driverReport.Terminated,
                WorkingEfficiency = driverReport.WorkingEfficiency,
                DotViolations = driverReport.DotViolations,
                NumberOfAccidents = driverReport.NumberOfAccidents
                
            };
            
            db.DriverReports.Add(driverReportReport);

            db.SaveChanges();
        }
        
        private void RemoveDriveInDb(DriverReportModel model)
        {
            var driver = db.Drivers
                .FirstOrDefault(d => d.Id == model.Id);

            if(driver == null) return;
            
            driver.IsFired = true;

            var driverReport = new DriverReport()
            {
                Comment = model.Description,
                DriversLicenseNumber = driver.DriversLicenseNumber,
                FullName = driver.FullName,
                IdDriver = driver.Id,
                DateRegistration = driver.DateRegistration,
                DateFired = DateTime.Now.ToString(),
                AlcoholTendency = model.AlcoholTendency,
                DrivingSkills = model.DrivingSkills,
                DrugTendency = model.DrugTendency,
                EldKnowledge = model.EldKnowledge,
                English = model.English,
                Experience = model.Experience,
                IdCompany = driver.CompanyId,
                PaymentHandling = model.PaymentHandling,
                ReturnedEquipmen = model.ReturnedEquipmen,
                Terminated = model.Terminated,
                WorkingEfficiency = model.WorkingEfficiency,
                DotViolations = model.DotViolations,
                NumberOfAccidents = model.NumberOfAccidents
            };

            db.DriverReports.Add(driverReport);

            db.SaveChanges();
        }

        private void UpdatePlanSubscribe(string idCompany)
        {
            var typeCurrentCompany = userService.GetCompanyById(idCompany).Type;
            if (typeCurrentCompany == TypeCompany.BaseCommpany) return;

            var countDriver = GetDriversByIdCompany(idCompany).Count;
            var idItemSubscribe = companyService.GetSubscriptionIdCompany(idCompany).IdItemSubscribeST;
            stripeApi.UpdateSupsctibe(countDriver, idItemSubscribe);
        }
    }
}