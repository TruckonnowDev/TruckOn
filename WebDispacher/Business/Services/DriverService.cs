﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
using WebDispacher.ViewModels.Truck;

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
        private readonly int maxFileLength = 6 * 1024 * 1024;
 
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

        public List<Driver> GetDriversByIdCompany(string companyId)
        {
            if (!int.TryParse(companyId, out var result)) return new List<Driver>();

            return db.Drivers.Where(d => d.DriverReportId == null && d.CompanyId == result).ToList();
        }

        public async Task<bool> RemoveCompany(string companyId)
        {
            try
            {
                await RemoveCompanyDb(companyId);

                await RemoveDriversByCompanyId(companyId, string.Empty);
            
                var customerSt = companyService.GetCustomer_STByIdCompany(companyId);
            
                var customer = stripeApi.RemoveCustomer(customerSt);

                if (customer != null)
                {
                    await RemoveCustomerST(customerSt);
                    await RemoveSubscribeST(customerSt);
                }

                var pattern = new PaternSourse().GetPatternSendMessageDeactivateCompany();

                var user = await companyService.GetUserByCompanyId(companyId);
                await new AuthMessageSender().Execute(user.Email, UserConstants.DeactivateEmailSubject, pattern);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task RemoveCompanyDb(string id)
        {
            if (!int.TryParse(id, out var result)) return;
            
            var company = await db.Companies.FirstOrDefaultAsync(dc => dc.Id == result);

            if (company == null) return;

            company.CompanyStatus = CompanyStatus.Deactivate;

            await db.SaveChangesAsync();
        }

        private async Task RemoveSubscribeST(CustomerST customer_ST)
        {
            var subscribeSt = await
                db.SubscribeST.FirstOrDefaultAsync(s => s.CustomerSTId == customer_ST.Id);

            if (subscribeSt == null) return;
            
            db.SubscribeST.Remove(subscribeSt);
            await db.SaveChangesAsync();
        }

        private async Task RemoveCustomerST(CustomerST customer_ST)
        {
            db.CustomerST.Remove(customer_ST);
            await db.SaveChangesAsync();
        }

        public async Task<List<ProfileSettingsDTO>> GetSettingsTruck(string idCompany, int idProfile, int idTr, string typeTransport)
        {
            var tr = await truckAndTrailerService.GetTr(idTr, typeTransport);
            var typeTransportVehikle = TypeTransportVehikle.Truck;
            var profileSettings = new List<ProfileSettingsDTO>();
            var profileSettings1 = GetSettingsDb(idCompany, typeTransportVehikle, idTr);

            profileSettings.Add(new ProfileSettingsDTO()
            {
                Name = DriverConstants.StandardName,
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
            if (string.IsNullOrEmpty(idDriver)) return isStateActual;

            var passwordRecovery = db.PasswordsRecoveries.ToList()
                .FirstOrDefault(p => p.Token.ToString() == idDriver && p.Token == token);

            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.DateTimeAction) > DateTime.Now.AddHours(-2))
            {
                isStateActual = 1;
            }

            return isStateActual;
        }

        public DriverInspection GetInspectionTruck(string idInspection)
        {
            return db.DriversInspections
                .Where(i => i.Id.ToString() == idInspection)
                .Include(i => i.PhotosDriverInspection)
                .First();
        }

        public async Task RemoveDriver(string companyId, DriverReportModel model,string localDate)
        {
            await RemoveDriveInDb(model, localDate);
            await UpdatePlanSubscribe(companyId);
        }
        
        public async Task RemoveDriversByCompanyId(string companyId,string localDate)
        {
            var dateTimeRemove = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);
            if (!int.TryParse(companyId, out var result)) return;

            var drivers = await db.Drivers.Where(d => d.CompanyId == result && d.DriverReport == null).ToListAsync();

            foreach(var driver in drivers)
            {
                await RemoveDriverInCompanyById(new DriverReport
                {
                    Driver = driver,
                    Comment = CompanyConstants.RemoveCompanyDescription,
                    DateFired = dateTimeRemove
                });
            }

            await UpdatePlanSubscribe(companyId);
        }


        public async Task EditDriver(EditDriverViewModel model, string localDate)
        {
            if (model == null) return;

            var dateTimeUpdate = DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var driverEdit = await db.Drivers
                .Include(d => d.DriverControl)
                .Include(d => d.PhoneNumber)
                .FirstOrDefaultAsync(d => d.Id == model.Id);

            if (driverEdit == null) return;
            

            driverEdit.FirstName = model.FirstName;
            driverEdit.LastName = model.LastName;
            driverEdit.Email = model.Email.ToLower();
            if(driverEdit.PhoneNumber.DialCode != 0) driverEdit.PhoneNumber.DialCode = model.PhoneNumber.DialCode;
            driverEdit.PhoneNumber.Name = model.PhoneNumber.Name;
            driverEdit.PhoneNumber.Number = model.PhoneNumber.Number;
            driverEdit.PhoneNumber.Iso2 = model.PhoneNumber.Iso2;
            driverEdit.DateOfBirth = model.DateOfBirth;
            driverEdit.DriverLicenseNumber = model.DriverLicenseNumber;
            driverEdit.DriverControl.TrailerCapacity = model.DriverControl.TrailerCapacity;
            driverEdit.DriverControl.Password = model.DriverControl.Password;
            driverEdit.DateLastUpdate = dateTimeUpdate;

            await db.SaveChangesAsync();
        }

        public async Task CreateDriver(CreateDriverViewModel driver,
            IFormFile dLDoc, IFormFile medicalCardDoc, IFormFile sSNDoc, IFormFile proofOfWorkAuthorizationOrGCDoc,
            IFormFile dQLDoc, IFormFile contractDoc, IFormFile drugTestResultsDo, string dateTimeLocal)
        {
            var dateTimeCreate = string.IsNullOrEmpty(dateTimeLocal) ? DateTime.Now : DateTime.ParseExact(dateTimeLocal, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            driver.DateRegistration = dateTimeCreate;

            var driverId = await AddDriver(driver);

            if (dLDoc != null)
            {
                await SaveDocDriver(dLDoc, DocAndFileConstants.Dl, driverId, dateTimeLocal);
            }

            if (medicalCardDoc != null)
            {
                await SaveDocDriver(medicalCardDoc, DocAndFileConstants.MedicalCard, driverId, dateTimeLocal);
            }

            if (sSNDoc != null)
            {
                await SaveDocDriver(sSNDoc, DocAndFileConstants.Ssn, driverId, dateTimeLocal);
            }

            if (proofOfWorkAuthorizationOrGCDoc != null)
            {
                await SaveDocDriver(proofOfWorkAuthorizationOrGCDoc, DocAndFileConstants.ProofOfWork, driverId, dateTimeLocal);
            }

            if (dQLDoc != null)
            {
                await SaveDocDriver(dQLDoc, DocAndFileConstants.Dql, driverId, dateTimeLocal);
            }

            if (contractDoc != null)
            {
                await SaveDocDriver(contractDoc, DocAndFileConstants.Contract, driverId, dateTimeLocal);
            }

            if (drugTestResultsDo != null)
            {
                await SaveDocDriver(drugTestResultsDo, DocAndFileConstants.DrugTestResult, driverId, dateTimeLocal);
            }

            await Task.Run(() => UpdatePlanSubscribe(driver.CompanyId.ToString()));
        }

        public Driver GetDriver(string idInspection)
        {
            // тут вооббще что
            var inspectionDriver = db.DriversInspections.First(i => i.Id.ToString() == idInspection);
            return db.Drivers
                .Include(d => d.Inspections)
                .First(d => d.Inspections.FirstOrDefault(ii => ii.Id == inspectionDriver.Id) != null);
        }

        public Driver GetDriverById(int id)
        {
            return db.Drivers.FirstOrDefault(d => d.Id == id);
        }
        
        public async Task<EditDriverViewModel> GetEditCompanyDriverById(string companyId, int id)
        {
            if (int.TryParse(companyId, out int result))
            {
                var driver = await db.Drivers
                    .Include(d => d.DriverControl)
                    .Include(d => d.PhoneNumber)
                    .FirstOrDefaultAsync(d => d.Id == id && d.CompanyId == result);

                return mapper.Map<EditDriverViewModel>(driver);
            }

           return null;
        }

        public async Task RemoveDocDriver(int docId)
        {
            var documentDriver = await db.DocumentsDrivers.FirstOrDefaultAsync(d => d.Id == docId);

            if (documentDriver == null) return;

            db.DocumentsDrivers.Remove(documentDriver);

            await db.SaveChangesAsync();
        }

        public async Task<List<Driver>> GetDriversByCompanyId(string companyId)
        {
            if(int.TryParse(companyId, out var result))
            {
                return await GetDriversInDb(result);
            }

            return new List<Driver>();
        }

        public async Task<List<Driver>> GetDriversByCompanyId(int page, string companyId)
        {
            if (int.TryParse(companyId, out var result))
            {
                return await GetDriversInDb(page, result);
            }

            return new List<Driver>();
        }

        public async Task<int> GetCountDriversPages(string companyId)
        {
            if(int.TryParse(companyId, out var result))
            {
                return await GetDriversPagesInDb(result);
            }

            return 0;
        }

        public async Task AddNewReportDriver(DriverReportViewModel driverReport, string localDate)
        {
            await AddNewReportDriverDb(driverReport, localDate);
        }

        public async Task<List<DriverReportViewModel>> GetDriverReportsByCompnayId(DriverSearchViewModel model, string companyId)
        {
            if (!int.TryParse(companyId, out var result)) 
                return new List<DriverReportViewModel>();

            var driverReports = db.DriversReports
                .Include(dr => dr.Driver)
                .Where(dr => dr.Driver.CompanyId == result)
                .AsQueryable();

            driverReports = driverReports.Where(dr => dr.Driver.FirstName == model.FirstName &&
                                                    dr.Driver.LastName == model.LastName &&
                                                    dr.Driver.DriverLicenseNumber == model.LicenseNumber)
                                        .OrderByDescending(x => x.Id);

            var driverReportsList = await driverReports.ToListAsync();

            var driverReportViewModels = mapper.Map<List<DriverReportViewModel>>(driverReportsList);

            return driverReportViewModels;
        }

        public async Task<List<DriverReport>> GetDriversReport(string nameDriver, string driversLicense, string companyId)
        {
            if (!int.TryParse(companyId, out var result)) return new List<DriverReport>();

            var driverReports = db.DriversReports
                .Include(dr => dr.Driver)
                .Where(dr => dr.Driver.CompanyId == result)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nameDriver))
            {
                driverReports = driverReports.Where(x => x.Driver.FirstName.Contains(nameDriver) || x.Driver.LastName.Contains(nameDriver));
            }

            if (!string.IsNullOrEmpty(driversLicense))
            {
               driverReports = driverReports.Where(x => x.Driver.DriverLicenseNumber.Contains(driversLicense));
            }

            driverReports = driverReports.OrderByDescending(x => x.Id);

            var driverReportsList = await driverReports.ToListAsync();

            return driverReportsList;
        }

        public async Task<List<DocumentDriver>> GetDriverDoc(int id)
        {
            return await GetDriverDocInDb(id);
        }

        public int CheckReportDriver(string fullName, string driversLicenseNumber)
        {
            var driverReports = new List<DriverReport>();

            if (fullName != null || driversLicenseNumber != null)
            {
                driverReports.AddRange(db.DriversReports.Where(d =>
                    fullName == d.Driver.FirstName && driversLicenseNumber == d.Driver.DriverLicenseNumber));
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
            /*var transportVehicle = db.TransportVehicles
                .Where(p => p.Id == idTransported)
                .Include(p => p.Layouts)
                .First(p => p.Id == idTransported);

            var layoutsCurrent = transportVehicle.Layouts.First(l => l.Id == idLayout);
            var layoutsDown =
                transportVehicle.Layouts.First(l => l.OrdinalIndex == layoutsCurrent.OrdinalIndex + 1);

            layoutsCurrent.OrdinalIndex++;
            layoutsDown.OrdinalIndex--;

            db.SaveChanges();*/
        }

        public void UnSelectLayout(int idLayout)
        {
            /*var transportVehicle = db.TransportVehicles
                .Include(t => t.Layouts)
                .First(t => t.Layouts.FirstOrDefault(l => l.Id == idLayout) != null);
            transportVehicle.CountPhoto--;

            db.SaveChanges();

            var layouts = db.Layouts.FirstOrDefault(l => l.Id == idLayout);

            if (layouts == null) return;
            
            layouts.IsUsed = false;
            db.SaveChanges();*/
        }

        public void SelectProfile(int idProfile, string typeTransport, int idTr, string idCompany)
        {
            /*var typeTransportVehikle =
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
            }*/
        }

        public void LayoutUp(int idLayout, int idTransported)
        {
            /*var transportVehicle = db.TransportVehicles
                .Where(p => p.Id == idTransported)
                .Include(p => p.Layouts)
                .First(p => p.Id == idTransported);

            var layoutsCurrent = transportVehicle.Layouts.First(l => l.Id == idLayout);
            var layoutsUp = transportVehicle.Layouts.First(l => l.OrdinalIndex == layoutsCurrent.OrdinalIndex - 1);

            layoutsCurrent.OrdinalIndex--;
            layoutsUp.OrdinalIndex++;

            db.SaveChanges();*/
        }

        public async Task RemoveProfile(string idCompany, int idProfile)
        {
            var profileSetting = await db.ProfileSettings
                .Where(p => p.IdCompany.ToString() == idCompany && p.Id == idProfile)
                .Include(p => p.TransportVehicle.Layouts)
                .FirstOrDefaultAsync();

            if (profileSetting == null) return;
            
                db.Layouts.RemoveRange(profileSetting.TransportVehicle.Layouts);
                db.TransportVehicles.RemoveRange(profileSetting.TransportVehicle);
                db.ProfileSettings.Remove(db.ProfileSettings.First(p =>
                    p.IdCompany.ToString() == idCompany && p.Id == idProfile));
                await db.SaveChangesAsync();
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

        public async Task<ProfileSettingsDTO> GetSelectSettingTruck(string idCompany, int idProfile, int idTr, string typeTransport)
        {
            ProfileSettingsDTO profileSetting = null;
            
            if (idProfile != 0)
            {
                var profileSetting1 = GetSelectSettings(idCompany, idProfile);
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
                var tr = await truckAndTrailerService.GetTr(idTr, typeTransport);
                var transportVehicle = HelperTransport.GetTransportVehicle(tr.Type);
                profileSetting = new ProfileSettingsDTO()
                {
                    Id = 0,
                    Name = DriverConstants.StandardName,
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

        private ProfileSettings GetSelectSettings(string idCompany, int idProfile)
        {
            return db.ProfileSettings
                .Where(p => p.IdCompany.ToString() == idCompany && p.Id == idProfile)
                .Include(p => p.TransportVehicle.Layouts)
                .FirstOrDefault();
        }

        private List<ProfileSettings> GetSettingsDb(string idCompany, TypeTransportVehikle typeTransportVehikle, int idTr)
        {
            List<ProfileSettings> profileSettings = null;

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
                emailDriver = driver.Email;
            }

            return emailDriver;
        }

        private async Task<List<DocumentDriver>> GetDriverDocInDb(int id)
        {
            return await db.DocumentsDrivers.Where(d => d.DriverId == id).ToListAsync();
        }

        private async Task<List<Driver>> GetDriversInDb(int companyId)
        {
            var drivers = await db.Drivers
                .Where(d => d.CompanyId == companyId && d.DriverReport == null)
                .Include(d => d.Inspections)
                .Include(d => d.Geolocation)
                .Include(d => d.PhoneNumber)
                .OrderByDescending(d => d.Id)
                .ToListAsync();

            return drivers;
        }

        private async Task<List<Driver>> GetDriversInDb(int page, int companyId)
        {
            var drivers = db.Drivers
                .Include(d => d.DriverControl)
                .Include(d => d.PhoneNumber)
                .Where(d => d.CompanyId == companyId && d.DriverReport == null)
                .OrderByDescending(d => d.Id)
                .AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await drivers.ToListAsync();

            try
            {
                drivers = drivers.Skip(UserConstants.NormalPageCount * page - UserConstants.NormalPageCount);

                drivers = drivers.Take(UserConstants.NormalPageCount);
            }
            catch (Exception)
            {
                drivers = drivers.Skip((UserConstants.NormalPageCount * page) - UserConstants.NormalPageCount);
            }

            var listDrivers = await drivers.ToListAsync();
            
            return listDrivers;
        }
        
        private async Task<int> GetDriversPagesInDb(int companyId)
        {
            var countDrivers = await db.Drivers.Where(d => d.CompanyId == companyId).CountAsync();

            var countPages = GetCountPage(countDrivers, UserConstants.NormalPageCount);

            return countPages;
        }

        private int GetCountPage(int countElements, int countElementsInOnePage)
        {
            var countPages = (countElements / countElementsInOnePage) % countElementsInOnePage;

            return countPages > 0 ? countPages + 1 : countPages;
        }

        public async Task SaveDocDriver(IFormFile uploadedFile, string nameDoc, int driverId, string dateTimeUpload)
        {
            if (uploadedFile.Length > maxFileLength) return;

            var path = $"../Document/Driver/{driverId}/" + uploadedFile.FileName;

            if (!Directory.Exists("../Document/Driver"))
            {
                Directory.CreateDirectory($"../Document/Driver");
            }

            if (!Directory.Exists($"../Document/Driver/{driverId}"))
            {
                Directory.CreateDirectory($"../Document/Driver/{driverId}");
            }
            
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                uploadedFile.CopyTo(fileStream);
            }
            
            await SaveDocDriverDb(path, driverId, nameDoc, dateTimeUpload);
        }

        private async Task SaveDocDriverDb(string path, int driverId, string nameDoc, string localDate)
        {
            var dateTimeUpload = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var pref = path.Remove(0, path.LastIndexOf(".") + 1);

            var documentCompany = new DocumentDriver()
            {
                DocPath = path,
                Name = nameDoc,
                DriverId = driverId,
                DateTimeUpload = dateTimeUpload
            };

            await db.DocumentsDrivers.AddAsync(documentCompany);

            await db.SaveChangesAsync();
        }

        private async Task<int> AddDriver(CreateDriverViewModel model)
        {
            model.Email = model.Email != null ? model.Email.ToLower() : model.Email;

            var driverControl = new DriverControl
            {
                Password = model.DriverControl.Password,
                TrailerCapacity = model.DriverControl.TrailerCapacity
            };

            await db.DriversControls.AddAsync(driverControl);

            var phoneContact = new PhoneNumber
            {
                Number = model.PhoneNumber.Number,
                DialCode = model.PhoneNumber.DialCode,
                Iso2 = model.PhoneNumber.Iso2,
                Name = model.PhoneNumber.Name,
            };

            await db.PhonesNumbers.AddAsync(phoneContact);

            await db.SaveChangesAsync();

            var driver = new Driver
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumberId = phoneContact.Id,
                DriverLicenseNumber = model.DriverLicenseNumber,
                DateRegistration = model.DateRegistration,
                DateLastUpdate = model.DateRegistration,
                DateOfBirth = model.DateOfBirth,
                CompanyId = model.CompanyId,
                DriverControlId = driverControl.Id
            };

            await db.Drivers.AddAsync(driver);

            await db.SaveChangesAsync();

            return driver.Id;
        }

        private async Task AddNewReportDriverDb(DriverReportViewModel driverReport, string localDate)
        {
            var dateUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var driver = await db.Drivers.FirstOrDefaultAsync(d => d.DriverLicenseNumber == driverReport.DriverLicenseNumber);
            
            if(driver == null) return;

            driver.DateLastUpdate = dateUpdate;

            var driverReportReport = new DriverReport()
            {
                Comment = driverReport.Comment,
                DateFired = dateUpdate,
                Driver = driver,
                AlcoholTendency = driverReport.AlcoholTendency,
                DriverSkills = driverReport.DrivingSkills,
                DrugTendency = driverReport.DrugTendency,
                EldKnowledge = driverReport.EldKnowledge,
                English = driverReport.English,
                Experience = driverReport.Experience,
                PaymentHandling = driverReport.PaymentHandling,
                ReturnEquipment = driverReport.ReturnedEquipmen,
                Terminated = driverReport.Terminated,
                WorkEffeciency = driverReport.WorkingEfficiency,
                DotViolations = driverReport.DotViolations,
                NumberAccidents = driverReport.NumberOfAccidents
            };
            
            await db.DriversReports.AddAsync(driverReportReport);

            await db.SaveChangesAsync();
        }

        private async Task RemoveDriverInCompanyById(DriverReport driverReport)
        {
            await db.DriversReports.AddAsync(driverReport);

            await db.SaveChangesAsync();
        }


        private async Task RemoveDriveInDb(DriverReportModel model, string localDate)
        {
            var dateUpdate = string.IsNullOrEmpty(localDate) ? DateTime.Now : DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture);

            var driver = db.Drivers
                .FirstOrDefault(d => d.Id == model.Id);

            if(driver == null) return;

            driver.DateLastUpdate = dateUpdate;

            var driverReport = new DriverReport()
            {
                Comment = model.Description,
                DateFired = dateUpdate,
                Driver = driver,
                AlcoholTendency = model.AlcoholTendency,
                DriverSkills = model.DrivingSkills,
                DrugTendency = model.DrugTendency,
                EldKnowledge = model.EldKnowledge,
                English = model.English,
                Experience = model.Experience,
                PaymentHandling = model.PaymentHandling,
                ReturnEquipment = model.ReturnedEquipmen,
                Terminated = model.Terminated,
                WorkEffeciency = model.WorkingEfficiency,
                DotViolations = model.DotViolations,
                NumberAccidents = model.NumberOfAccidents
            };

            await db.DriversReports.AddAsync(driverReport);

            await db.SaveChangesAsync();
        }

        private async Task UpdatePlanSubscribe(string companyId)
        {
            var typeCurrentCompany = userService.GetCompanyById(companyId).CompanyStatus;
            if (typeCurrentCompany == CompanyStatus.Admin) return;

            var countDriver = GetDriversByIdCompany(companyId).Count;

            var subscribeST = await companyService.GetSubscriptionIdCompany(companyId);
            var idItemSubscribe = subscribeST.ItemSubscribeSTId;

            stripeApi.UpdateSupsctibe(countDriver, idItemSubscribe);
        }
    }
}