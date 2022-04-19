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
using WebDispacher.Models;
using WebDispacher.Service;
using WebDispacher.Service.EmailSmtp;

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
        
        public async void RemoveCompany(string idCompany)
        {
            RemoveCompanyDb(idCompany);
            Customer_ST customer_ST = companyService.GetCustomer_STByIdCompany(idCompany);
            Customer customer = stripeApi.RemoveCustomer(customer_ST);
            
            if(customer != null)
            {
                RemoveCustomerST(customer_ST);
                RemoveSubscribeST(customer_ST);
            }
            
            Task.Run(() =>
            {
                List<Driver> drivers = GetDriversByIdCompany(idCompany);
                foreach(Driver driver in drivers)
                {
                    RemoveDrive(idCompany, new DriverReportModel{Id = driver.Id, Description = "The site administration deleted the company in which this driver worked"});
                }
            });
        }
        
        public void RemoveCompanyDb(string id)
        {
            db.User.RemoveRange(db.User.Where(u => u.CompanyId.ToString() == id));
            db.Commpanies.Remove(db.Commpanies.First(dc => dc.Id.ToString() == id));
            db.SaveChanges();
        }
        
        private void RemoveSubscribeST(Customer_ST customer_ST)
        {
            Subscribe_ST subscribe_ST = db.Subscribe_STs.FirstOrDefault(s => s.IdCustomerST == customer_ST.IdCustomerST);
            if(subscribe_ST != null)
            {
                db.Subscribe_STs.Remove(subscribe_ST);
                db.SaveChanges();
            }
        }
        
        private void RemoveCustomerST(Customer_ST customer_ST)
        {
            db.Customer_STs.Remove(customer_ST);
            db.SaveChanges();
        }
        
        public List<ProfileSettingsDTO> GetSetingsTruck(string idCompany, int idProfile, int idTr, string typeTransport)
        {
            ITr tr = truckAndTrailerService.GetTr(idTr, typeTransport);
            TypeTransportVehikle typeTransportVehikle = tr is Truck ? TypeTransportVehikle.Truck : TypeTransportVehikle.Trailer;
            List <ProfileSettingsDTO> profileSettings = new List<ProfileSettingsDTO>();
            List<ProfileSetting> profileSettings1 = GetSetingsDb(idCompany, typeTransportVehikle, idTr);
            
            profileSettings.Add(new ProfileSettingsDTO()
            {
                Name = "Standart",
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
            int isStateActual = 0;
            PasswordRecovery passwordRecovery = db.PasswordRecoveries.ToList().FirstOrDefault(p => p.IdDriver.ToString() == idDriver && p.Token == token);
            if(passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
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
        
        public void EditDrive(int id, string fullName, string emailAddress, 
            string password, string phoneNumber, string trailerCapacity, string driversLicenseNumber)
        {;
            Driver driver = db.Drivers.FirstOrDefault(d => d.Id == id);
            driver.FullName = fullName;
            driver.EmailAddress = emailAddress;
            driver.Password = password;
            driver.PhoneNumber = phoneNumber;
            driver.TrailerCapacity = trailerCapacity;
            driver.DriversLicenseNumber = driversLicenseNumber;
            db.SaveChanges();
        }
        
        public async void CreateDriver(string fullName, string emailAddress, string password, string phoneNumbe, string trailerCapacity, string driversLicenseNumber, string idCompany,
            IFormFile dLDoc, IFormFile medicalCardDoc, IFormFile sSNDoc, IFormFile proofOfWorkAuthorizationOrGCDoc, IFormFile dQLDoc, IFormFile contractDoc, IFormFile drugTestResultsDo)
        {
            Driver driver = new Driver();
            driver.FullName = fullName;
            driver.EmailAddress = emailAddress;
            driver.Password = password;
            driver.PhoneNumber = phoneNumbe;
            driver.TrailerCapacity = trailerCapacity;
            driver.DriversLicenseNumber = driversLicenseNumber;
            driver.DateRegistration = DateTime.Now.ToString();
            driver.CompanyId = Convert.ToInt32(idCompany);
            
            int id = AddDriver(driver);
            await SaveDocDriver(dLDoc, "DL", id.ToString());
            await SaveDocDriver(medicalCardDoc, "Medical card", id.ToString());
            await SaveDocDriver(sSNDoc, "SSN", id.ToString());
            await SaveDocDriver(proofOfWorkAuthorizationOrGCDoc, "Proof of work Authorization or GC", id.ToString());
            await SaveDocDriver(dQLDoc, "DQL (driver qualification file)", id.ToString());
            await SaveDocDriver(contractDoc, "Contract (company and driver)", id.ToString());
            
            if (drugTestResultsDo != null)
            {
                await SaveDocDriver(drugTestResultsDo, "Drug test results", id.ToString());
            }
            
            Task.Run(() => UpdatePlanSubscribe(idCompany));
        }
        
        public Driver GetDriver(string idInspection)
        {
            InspectionDriver inspectionDriver = db.InspectionDrivers.First(i => i.Id.ToString() == idInspection);
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
        
        public void AddNewReportDriver(string fullName, string driversLicenseNumber, string numberOfAccidents, string english, string returnedEquipmen, string workingEfficiency, string eldKnowledge, string drivingSkills,
            string paymentHandling, string alcoholTendency, string drugTendency, string terminated, string experience, string dotViolations, string description)
        {
            if (driversLicenseNumber != null && driversLicenseNumber != "")
            {
                AddNewReportDriverDb(fullName, driversLicenseNumber, numberOfAccidents, english, returnedEquipmen, workingEfficiency, eldKnowledge, drivingSkills, paymentHandling, alcoholTendency, drugTendency, 
                    terminated, experience, dotViolations, description);
            }
        }
        
        public List<DriverReport> GetDriversReport(string nameDriver, string driversLicense)
        {
            List<DriverReport> driverReports = new List<DriverReport>();
            
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
            List<DriverReport> driverReports = new List<DriverReport>();
            
            if (fullName != null || driversLicenseNumber != null)
            {
                driverReports.AddRange(db.DriverReports.Where(d => fullName == d.FullName && driversLicenseNumber == d.DriversLicenseNumber));
            }
            
            return driverReports.Count;
        }
        
        public async Task<int> ResetPasswordFoDriver(string newPassword, string idDriver, string token)
        {
            int isStateActual = await ResetPasswordFoDriver(newPassword, idDriver, token);
            
            if(isStateActual == 2)
            {
                string emailDriver = GetEmailDriverDb(idDriver);
                string patern = new PaternSourse().GetPaternDataAccountDriver(emailDriver, newPassword);
                
                await new AuthMessageSender().Execute(emailDriver, "Password changed successfully", patern);
            }
            else
            {
                string emailDriver = GetEmailDriverDb(idDriver);
                string patern = new PaternSourse().GetPaternNoRestoreDataAccountDriver();
                
                await new AuthMessageSender().Execute(emailDriver, "Password reset attempt failed", patern);
            }
            
            return isStateActual;
        }
        
        public void LayoutDown(int idLayout, int idTransported)
        {
            TransportVehicle transportVehicle = db.TransportVehicles
                .Where(p => p.Id == idTransported)
                .Include(p => p.Layouts)
                .First(p => p.Id == idTransported);
            
            Layouts layoutsCurrent = transportVehicle.Layouts.First(l => l.Id == idLayout);
            Layouts layoutsDown = transportVehicle.Layouts.First(l => l.OrdinalIndex == layoutsCurrent.OrdinalIndex + 1);
            
            layoutsCurrent.OrdinalIndex++;
            layoutsDown.OrdinalIndex--;
            
            db.SaveChanges();
        }
        
        public void UnSelectLayout(int idLayout)
        {
            TransportVehicle transportVehicle = db.TransportVehicles
                .Include(t => t.Layouts)
                .First(t => t.Layouts.FirstOrDefault(l => l.Id == idLayout) != null);
            transportVehicle.CountPhoto--;
            
            db.SaveChanges();
            
            Layouts layouts = db.Layouts.FirstOrDefault(l => l.Id == idLayout);
            
            if(layouts != null)
            {
                layouts.IsUsed = false;
                db.SaveChanges();
            }
        }
        
        public void SelectProfile(int idProfile, string typeTransport, int idTr, string idCompany)
        {
            TypeTransportVehikle typeTransportVehikle = typeTransport == "Truck" ? TypeTransportVehikle.Truck : TypeTransportVehikle.Trailer;
            
            List<ProfileSetting> profileSettings = GetSetingsDb(idCompany, typeTransportVehikle, idTr);
            ProfileSetting profileSetting = profileSettings.FirstOrDefault(p => p.IsUsed);
            
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
            TransportVehicle transportVehicle = db.TransportVehicles
                .Where(p => p.Id == idTransported)
                .Include(p => p.Layouts)
                .First(p => p.Id == idTransported);

            Layouts layoutsCurrent = transportVehicle.Layouts.First(l => l.Id == idLayout);
            Layouts layoutsUp = transportVehicle.Layouts.First(l => l.OrdinalIndex == layoutsCurrent.OrdinalIndex - 1);
            
            layoutsCurrent.OrdinalIndex--;
            layoutsUp.OrdinalIndex++;
            
            db.SaveChanges();
        }
        
        public void RemoveProfile(string idCompany, int idProfile)
        {
            ProfileSetting profileSetting = db.ProfileSettings
                .Where(p => p.IdCompany.ToString() == idCompany && p.Id == idProfile)
                .Include(p => p.TransportVehicle.Layouts)
                .FirstOrDefault();
            
            if (profileSetting != null)
            {
                db.Layouts.RemoveRange(profileSetting.TransportVehicle.Layouts);
                db.TransportVehicles.RemoveRange(profileSetting.TransportVehicle);
                db.ProfileSettings.Remove(db.ProfileSettings.First(p => p.IdCompany.ToString() == idCompany && p.Id == idProfile));
                db.SaveChanges();
            }
        }
        
        public void SelectLayout(int idLayout)
        {
            TransportVehicle transportVehicle = db.TransportVehicles
                .Include(t => t.Layouts)
                .First(t => t.Layouts.FirstOrDefault(l => l.Id == idLayout) != null);
            
            transportVehicle.CountPhoto++;
            db.SaveChanges();
            
            Layouts layouts = db.Layouts.FirstOrDefault(l => l.Id == idLayout);
            
            if (layouts != null)
            {
                layouts.IsUsed = true;
                db.SaveChanges();
            }
        }
        
        public ProfileSettingsDTO GetSelectSetingTruck(string idCompany, int idProfile, int idTr, string typeTransport)
        {
            ProfileSettingsDTO profileSetting = null;
            if(idProfile != 0)
            {
                ProfileSetting profileSetting1 = GetSelectSeting(idCompany, idProfile);
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
                ITr tr = truckAndTrailerService.GetTr(idTr, typeTransport);
                ITransportVehicle transportVehicle = HelperTransport.GetTransportVehicle(tr.Type);
                profileSetting = new ProfileSettingsDTO()
                {
                    Id = 0,
                    Name = "Standart",
                    TransportVehicle = new TransportVehicle()
                    {
                        CountPhoto = transportVehicle.CountPhoto,
                        Id = 0,
                        Layouts = userService.GetLayoutsByTransportVehicle(transportVehicle),
                        Type = transportVehicle.Type,
                        TypeTransportVehicle = transportVehicle.TypeTransportVehicle
                    },
                    TypeTransportVehikle = "Truck",
                    IsChange = false,
                    IsUsed = true,
                    IdCompany = 0
                };
            }
            profileSetting.TransportVehicle.Layouts = profileSetting.TransportVehicle.Layouts.OrderBy(l => l.OrdinalIndex).ToList();
            
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
                .Where(p => p.IdCompany.ToString() == idCompany && p.TypeTransportVehikle == typeTransportVehikle.ToString() && p.IdTr == idTr)
                .ToList();
            
            return profileSettings;
             
        }
        
        private void SelectProfileDb(int idProfile, bool isUsed = true)
        {
            ProfileSetting profileSetting = db.ProfileSettings.FirstOrDefault(p => p.Id == idProfile);
            profileSetting.IsUsed = isUsed;
            db.SaveChanges();
        }
        
        private string GetEmailDriverDb(string idDriver)
        {
            string emailDriver = "";
            Driver driver = db.Drivers.FirstOrDefault(d => d.Id.ToString() == idDriver);
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
        
        private void AddNewReportDriverDb(string fullName, string driversLicenseNumber, string numberOfAccidents, string english, string returnedEquipmen, string workingEfficiency, string eldKnowledge, string drivingSkills,
            string paymentHandling, string alcoholTendency, string drugTendency, string terminated, string experience, string dotViolations, string description)
        {
            int idDriver = 0;
            int idCompany = 0;
            string dateRegistration = "";
            Driver driver = db.Drivers.LastOrDefault(d => d.DriversLicenseNumber == driversLicenseNumber);
            
            if(driver != null)
            {
                idDriver = driver.Id;
                dateRegistration = driver.DateRegistration;
                idCompany = db.Commpanies.FirstOrDefault(c => c.Id == driver.CompanyId).Id;
            }    
            
            db.DriverReports.Add(new DriverReport()
            {
                Comment = description,
                DriversLicenseNumber = driversLicenseNumber,
                FullName = fullName,
                AlcoholTendency = alcoholTendency,
                DateFired = DateTime.Now.ToString(),
                DateRegistration = dateRegistration,
                IdDriver = idDriver,
                DrivingSkills = drivingSkills,
                DrugTendency = drugTendency,
                EldKnowledge = eldKnowledge,
                English = english,
                Experience = experience,
                PaymentHandling = paymentHandling,
                ReturnedEquipmen = returnedEquipmen,
                Terminated = terminated,
                WorkingEfficiency = workingEfficiency,
                IdCompany = idCompany,
                DotViolations = dotViolations,
                NumberOfAccidents = numberOfAccidents
            });
            
            db.SaveChanges();
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
            else if(page != 0)
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
            string path = $"../Document/Driver/{id}/" + uploadedFile.FileName;
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
            string pref = path.Remove(0, path.LastIndexOf(".") + 1);
            DucumentDriver ducumentCompany = new DucumentDriver()
            {
                DocPath = path,
                NameDoc = nameDoc,
                IdDriver = Convert.ToInt32(id)
            };
            db.DucumentDrivers.Add(ducumentCompany);
            db.SaveChanges();
        }
        
        private int AddDriver(Driver driver)
        {
            db.Drivers.Add(driver);
            db.SaveChanges();
            return driver.Id;
        }
        
        private void RemoveDriveInDb(DriverReportModel model)
        {
            Driver driver = db.Drivers
                .FirstOrDefault(d => d.Id == model.Id);
            
            driver.IsFired = true;
            
            DriverReport driverReport = new DriverReport()
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
            TypeCompany typeCurrentCompany = userService.GetCompanyById(idCompany).Type;
            if(typeCurrentCompany != TypeCompany.BaseCommpany)
            {
                int counDriver =  GetDriversByIdCompany(idCompany).Count;
                string idItemSubscribe = companyService.GetSubscriptionIdCompany(idCompany).IdItemSubscribeST;
                stripeApi.UpdateSupsctibe(counDriver, idItemSubscribe);
            }
        }
    }
}