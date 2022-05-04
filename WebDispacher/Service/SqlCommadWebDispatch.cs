using DaoModels.DAO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebDispacher.Models;

namespace WebDispacher.Dao
{
    public class SqlCommadWebDispatch
    {
        private Context context = null;

        public SqlCommadWebDispatch()
        {
            context = new Context();
            if (Context._cache == null) { Context._cache = new MemoryCache(new MemoryCacheOptions()); }
            InitCommpanyOne();
            InitUserOne();
        }

        private void InitCommpanyOne()
        {
            try
            {
                if (context.Commpanies.FirstOrDefault(c => c.Type == TypeCompany.BaseCommpany) == null)
                {
                    context.Commpanies.Add(new Commpany()
                    {
                        Name = "Truckonnow",
                        Type = TypeCompany.BaseCommpany,
                        DateRegistration = DateTime.Now.ToString()
                    });
                    context.SaveChanges();
                }
            }
            catch 
            { }
        }

        internal Dispatcher GetDispatcherByKeyUsers(string key)
        {
            return context.Dispatchers.FirstOrDefault(d => d.key != null && d.key == key);
        }

        internal Subscribe_ST GetSubscriptionIdCompany(string idCompany, ActiveType activeType = ActiveType.Active)
        {
            return context.Subscribe_STs.FirstOrDefault(s => s.IdCompany.ToString() == idCompany && s.ActiveType == activeType);
        }

        private async void InitUserOne()
        {
            try
            {
                Commpany commpany = context.Commpanies.First(c => c.Type == TypeCompany.BaseCommpany);
                if (context.User.Count(u => u.CompanyId == commpany.Id) == 0)
                {
                    Users users = new Users();
                    users.Login = "DevRoma";
                    users.Password = "polkilo123";
                    users.CompanyId = commpany.Id;
                    users.Date = DateTime.Now.ToString();
                    await context.User.AddAsync(users);
                    users = new Users();
                    users.Login = "ArtemManager";
                    users.Password = "truckon777";
                    users.CompanyId = commpany.Id;
                    users.Date = DateTime.Now.ToString();
                    await context.User.AddAsync(users);
                    users = new Users();
                    users.Login = "Designer";
                    users.Password = "truckon777";
                    users.CompanyId = commpany.Id;
                    users.Date = DateTime.Now.ToString();
                    await context.User.AddAsync(users);
                    users = new Users();
                    users.Login = "Truckonnow";
                    users.Password = "truckon777";
                    users.CompanyId = commpany.Id;
                    users.Date = DateTime.Now.ToString();
                    await context.User.AddAsync(users);
                    users = new Users();
                    users.Login = "Truckonnow1";
                    users.Password = "truckon777";
                    users.CompanyId = commpany.Id;
                    users.Date = DateTime.Now.ToString();
                    await context.User.AddAsync(users);
                    await context.SaveChangesAsync();
                }

            }
            catch
            {

            } 
        }

        internal void UpdateTypeActiveSubById(int idSub, ActiveType activeType)
        {
            Subscribe_ST subscribe_ST = context.Subscribe_STs.FirstOrDefault(s => s.Id == idSub);
            if(subscribe_ST != null)
            {
                subscribe_ST.ActiveType = activeType;
                context.SaveChanges();
            }
        }

        internal List<Dispatcher> GetDispatchersDB(int idCompany)
        {
            return context.Dispatchers
                .Where(d => d.IdCompany == idCompany)
                .ToList();
        }

        internal ProfileSetting GetIdProfile(int idTr, string typeTransport)
        {
            return context.ProfileSettings
                .Where(p => p.IdTr == idTr && p.TypeTransportVehikle == typeTransport)
                .Include(p => p.TransportVehicle.Layouts)
                .FirstOrDefault();
        }

        internal Users GetUserByEmailAndPasswrodDB(string email, string password)
        {
            return context.User.FirstOrDefault(u => u.Login == email && u.Password == password);
        }

        internal void AddUserToDispatchDB(Users users)
        {
            Dispatcher dispatcher = context.Dispatchers.FirstOrDefault(d => d.IdCompany == users.CompanyId);
            //if(dispatcher != null)
            //{
            //    if(dispatcher.Users == null)
            //    {
            //        dispatcher.Users = new List<Users>();
            //    }
            //    dispatcher.Users.Add(users);
            //    context.SaveChanges();
            //}
        }

        internal List<ProfileSetting> GetSetingsDb(string idCompany, TypeTransportVehikle typeTransportVehikle, int idTr)
        {
            List<ProfileSetting> profileSettings = null;
            profileSettings = context.ProfileSettings
                .Where(p => p.IdCompany.ToString() == idCompany && p.TypeTransportVehikle == typeTransportVehikle.ToString() && p.IdTr == idTr)
                .ToList();
            return profileSettings;
             
        }

        internal Commpany GetCompanyById(string idCompany)
        {
            return context.Commpanies.First(c => c.Id.ToString() == idCompany);
        }

        internal Users GetUserByKey(string key)
        {
            return context.User.First(c => c.KeyAuthorized == key);
        }

        internal List<Commpany> GetCompanies()
        {
            return context.Commpanies.ToList();
        }

        internal Commpany GetUserByKeyUser(int key)
        {
            Commpany commpany = null;
            Users users = context.User.FirstOrDefault(u => u.KeyAuthorized == key.ToString());
            commpany = context.Commpanies.FirstOrDefault(c => c.Id == users.CompanyId);
            return commpany;
        }

        internal ProfileSetting GetSelectSeting(string idCompany, int idProfile)
        {
            return context.ProfileSettings
                .Where(p => p.IdCompany.ToString() == idCompany && p.Id == idProfile)
                .Include(p => p.TransportVehicle.Layouts)
                .FirstOrDefault();
        }

        internal Contact GetContactById(int id)
        {
            return context.Contacts.FirstOrDefault(c => c.ID == id) ;
        }

        internal void RefreshTokenDispatchDB(string idDispatch, string token)
        {
            Dispatcher dispatcher = context.Dispatchers.FirstOrDefault(d => d.Id.ToString() == idDispatch);
            if(dispatcher != null)
            {
                dispatcher.key = token;
                context.SaveChanges();
            }
        }

        internal bool CheckEmail(string email)
        {
            return context.User.FirstOrDefault(u => u.Login == email) != null;
        }
        
        internal int AddRecoveryPassword(string email, string token)
        {
            Users users = context.User.FirstOrDefault(u => u.Login == email);
            PasswordRecovery passwordRecovery1 = context.PasswordRecoveries.FirstOrDefault(p => p.IdUser == users.Id);
            if (passwordRecovery1 == null)
            {
                PasswordRecovery passwordRecovery = new PasswordRecovery()
                {
                    Date = DateTime.Now.ToString(),
                    IdUser = users.Id,
                    Token = token
                };
                context.PasswordRecoveries.Add(passwordRecovery);
            }
            else
            {
                passwordRecovery1.Token = token;
                passwordRecovery1.Date = DateTime.Now.ToString();
            }
            context.SaveChanges();
            return users.Id;
        }

        internal void UpdateTypeInactiveByIdCompany(string idCompany)
        {
            List<Subscribe_ST> subscribe_STs = context.Subscribe_STs.Where(s => s.IdCompany.ToString() == idCompany && s.ActiveType != ActiveType.Inactive).ToList();
            foreach(Subscribe_ST subscribe_ST in subscribe_STs)
            {
                subscribe_ST.ActiveType = ActiveType.Inactive;
            }
            context.SaveChanges();
        }

        internal int AddProfileDb(ProfileSetting profileSetting)
        {
            context.ProfileSettings.Add(profileSetting);
            context.SaveChanges();
            return profileSetting.Id;
        }

        internal void CreateDispatchDB(Dispatcher dispatcher)
        {
            context.Dispatchers.Add(dispatcher);
            context.SaveChanges();
        }

        internal int GetComapnyIdByKeyUser(int key)
        {
            return context.User.First(u => u.KeyAuthorized == key.ToString()).CompanyId;
        }

        internal async Task<Truck> GetTruckDb(string idDriver)
        {
            Truck truck = null;
            Driver driver = await context.Drivers.Include(d => d.InspectionDrivers).FirstOrDefaultAsync(d => d.Id.ToString() == idDriver);
            if (driver != null && driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0)
            {
                InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
                truck = await context.Trucks.FirstOrDefaultAsync(t => t.Id == inspectionDriver.IdITruck);
            }
            return truck; 
        }

        internal void EditContact(int id, string fullName, string emailAddress, string phoneNumbe)
        {
            Contact contact = context.Contacts.FirstOrDefault(c => c.ID == id);
            if(contact != null)
            {
                contact.Email = emailAddress;
                contact.Name = fullName;
                contact.Phone = phoneNumbe;
                context.SaveChanges();
            }
        }

        internal void RemoveProfiledb(string idCompany, int idProfile)
        {
            ProfileSetting profileSetting = context.ProfileSettings
                .Where(p => p.IdCompany.ToString() == idCompany && p.Id == idProfile)
                .Include(p => p.TransportVehicle.Layouts)
                .FirstOrDefault();
            if (profileSetting != null)
            {
                context.Layouts.RemoveRange(profileSetting.TransportVehicle.Layouts);
                context.TransportVehicles.RemoveRange(profileSetting.TransportVehicle);
                context.ProfileSettings.Remove(context.ProfileSettings.First(p => p.IdCompany.ToString() == idCompany && p.Id == idProfile));
                context.SaveChanges();
            }
        }

        internal string GetEmailUserDb(string idUser)
        {
            string emailDriver = "";
            Users users = context.User.FirstOrDefault(d => d.Id.ToString() == idUser);
            if (users != null)
            {
                emailDriver = users.Login;
            }
            return emailDriver;
        }

        internal int ResetPasswordFoUser(string newPassword, string idUser, string token)
        {
            int isStateActual = 0;
            PasswordRecovery passwordRecovery = context.PasswordRecoveries.ToList().FirstOrDefault(p => p.IdUser.ToString() == idUser && p.Token == token);
            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                Users users = context.User.FirstOrDefault(d => d.Id.ToString() == idUser);
                users.Password = newPassword;
                isStateActual = 2;
            }
            if (passwordRecovery != null)
            {
                context.PasswordRecoveries.Remove(passwordRecovery);
            }
            context.SaveChanges();
            return isStateActual;
        }

        internal int CheckTokenFoUserDb(string idUser, string token)
        {
            int isStateActual = 0;
            PasswordRecovery passwordRecovery = context.PasswordRecoveries.ToList().FirstOrDefault(p => p.IdUser.ToString() == idUser && p.Token == token);
            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                isStateActual = 1;
            }
            return isStateActual;
        }

        internal ITr GetTrailerById(int idTr)
        {
            return context.Trailers.FirstOrDefault(t => t.Id == idTr);
        }

        internal ITr GetTruckById(int idTr)
        {
            return context.Trucks.FirstOrDefault(t => t.Id == idTr);
        }

        internal void SelectLayout(int idLayout)
        {
            TransportVehicle transportVehicle = context.TransportVehicles
                .Include(t => t.Layouts)
                .First(t => t.Layouts.FirstOrDefault(l => l.Id == idLayout) != null);
            transportVehicle.CountPhoto++;
            context.SaveChanges();
            Layouts layouts = context.Layouts.FirstOrDefault(l => l.Id == idLayout);
            if (layouts != null)
            {
                layouts.IsUsed = true;
                context.SaveChanges();
            }
        }

        internal void EditDispatchById(int idDispatch, string typeDispatcher, string login, string password)
        {
            Dispatcher dispatcher = context.Dispatchers.FirstOrDefault(d => d.Id == idDispatch);
            if(dispatcher != null)
            {
                dispatcher.Login = login;
                dispatcher.Password = password;
                dispatcher.Type = typeDispatcher;
                context.SaveChanges();
            }
        }

        internal void DeleteContactById(int id)
        {
            Contact contact = context.Contacts.FirstOrDefault(c => c.ID == id);
            if(contact != null)
            {
                context.Contacts.Remove(contact);
                context.SaveChanges();
            }
        }

        internal Dispatcher GetDispatcherById(int idDispatch)
        {
            return context.Dispatchers.FirstOrDefault(d => d.Id == idDispatch);
        }

        internal void RemoveByIdDispatcher(int idDispatch)
        {
            context.Dispatchers.Remove(context.Dispatchers.First(d => d.Id == idDispatch));
            context.SaveChanges();
        }

        internal void SaveSubscribeST(Subscribe_ST subscribe_ST)
        {
            context.Subscribe_STs.Add(subscribe_ST);
            context.SaveChanges();
        }

        internal void SaveCustomerST(Customer_ST customer_ST)
        {
            context.Customer_STs.Add(customer_ST);
            context.SaveChanges();
        }

        internal void CreateUserForCompanyId(int id, string nameCommpany, string password)
        {
            context.User.Add(new Users()
            {
                CompanyId = id,
                Login = nameCommpany + "Admin",
                Password = password,
                Date = DateTime.Now.ToString()
            });
            context.SaveChanges();
        }

        internal void AddPaymentMethod_ST(PaymentMethod_ST paymentMethod_ST)
        {
            context.PaymentMethods.Add(paymentMethod_ST);
            context.SaveChanges();
        }

        internal List<PaymentMethod_ST> GetPaymentMethod_STsByIdCompany(string idCompany)
        {
            return context.PaymentMethods.Where(pm => pm.IdCompany.ToString() == idCompany).ToList();
        }

        internal void AddUserDb(Users users)
        {
            context.User.Add(users);
            context.SaveChanges();
        }

        internal int AddCommpany(Commpany commpany)
        {
            context.Commpanies.Add(commpany);
            context.SaveChanges();
            return commpany.Id;
        }

        internal List<Driver> GetDriversByIdCompany(string idCompany)
        {
            return context.Drivers.Where(d => !d.IsFired && d.CompanyId.ToString() == idCompany).ToList();
        }

        internal void RemoveCompanyDb(string id)
        {
            context.User.RemoveRange(context.User.Where(u => u.CompanyId.ToString() == id));
            context.Commpanies.Remove(context.Commpanies.First(dc => dc.Id.ToString() == id));
            context.SaveChanges();
        }

        internal void UnSelectPaymentMethod_ST(string idCompany)
        {
            PaymentMethod_ST paymentMethod_ST = context.PaymentMethods.FirstOrDefault(pm => pm.IdCompany.ToString() == idCompany && pm.IsDefault);
            if(paymentMethod_ST != null)
            {
                paymentMethod_ST.IsDefault = false;
                context.SaveChanges();
            }
        }

        internal void SelectPaymentMethod_ST(string idCompany, string idPayment)
        {
            PaymentMethod_ST paymentMethod_ST = context.PaymentMethods.FirstOrDefault(pm => pm.IdCompany.ToString() == idCompany && pm.IdPaymentMethod_ST == idPayment);
            if (paymentMethod_ST != null)
            {
                paymentMethod_ST.IsDefault = true;
                context.SaveChanges();
            }
        }

        internal async Task<Trailer> GetTrailerDb(string idDriver)
        {
            Trailer trailer = null;
            Driver driver = await context.Drivers.Include(d => d.InspectionDrivers).FirstOrDefaultAsync(d => d.Id.ToString() == idDriver);
            if (driver != null && driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0)
            {
                InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
                trailer = await context.Trailers.FirstOrDefaultAsync(t => t.Id == inspectionDriver.IdITrailer);
            }
            return trailer;
        }

        internal string RemovePaymentMethod(string idCompany, string idPayment)
        {
            string idPaymentNewSelect = null;
            PaymentMethod_ST paymentMethod_ST = context.PaymentMethods.FirstOrDefault(pm => pm.IdCompany.ToString() == idCompany && pm.IdPaymentMethod_ST == idPayment);
            context.PaymentMethods.Remove(paymentMethod_ST);
            context.SaveChanges();
            if(paymentMethod_ST.IsDefault)
            {
                paymentMethod_ST = context.PaymentMethods.FirstOrDefault();
                if(paymentMethod_ST != null)
                {
                    idPaymentNewSelect = paymentMethod_ST.IdPaymentMethod_ST;
                }
            }
            return idPaymentNewSelect;
        }

        internal void EditTruckDB(int idTruck, string nameTruk, string yera, string make, string model, string typeTruk, string state, string exp, string vin, string owner, string plateTruk, string color)
        {
            Truck truck = context.Trucks.FirstOrDefault(t => t.Id == idTruck);
            if(truck != null)
            {
                truck.NameTruk = nameTruk;
                truck.Yera = yera;
                truck.Make = make;
                truck.Model = model;
                truck.Type = typeTruk;
                truck.Satet = state;
                truck.Exp = exp;
                truck.Vin = vin;
                truck.Owner = owner;
                truck.PlateTruk = plateTruk;
                truck.ColorTruk = color;
                context.SaveChanges();
            }
        }

        internal void EditTrailerDB(int idTrailer, string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns)
        {
            Trailer trailer = context.Trailers.FirstOrDefault(t => t.Id == idTrailer);
            if (trailer != null)
            {
                trailer.Name = name;
                trailer.Type = typeTrailer;
                trailer.Year = year;
                trailer.Make = make;
                trailer.HowLong = howLong;
                trailer.Vin = vin;
                trailer.Owner = owner;
                trailer.Color = color;
                trailer.Plate = plate;
                trailer.Exp = exp;
                trailer.AnnualIns = annualIns;
                context.SaveChanges();
            }
        }

        internal void LayoutUPDb(int idLayout, int idTransported)
        {
            TransportVehicle transportVehicle = context.TransportVehicles
                .Where(p => p.Id == idTransported)
                .Include(p => p.Layouts)
                .First(p => p.Id == idTransported);
            Layouts layoutsCurrent = transportVehicle.Layouts.First(l => l.Id == idLayout);
            Layouts layoutsUp = transportVehicle.Layouts.First(l => l.OrdinalIndex == layoutsCurrent.OrdinalIndex - 1);
            layoutsCurrent.OrdinalIndex--;
            layoutsUp.OrdinalIndex++;
            context.SaveChanges();
        }

        internal void UnSelectLayout(int idLayout)
        {
            TransportVehicle transportVehicle = context.TransportVehicles
                   .Include(t => t.Layouts)
                   .First(t => t.Layouts.FirstOrDefault(l => l.Id == idLayout) != null);
            transportVehicle.CountPhoto--;
            context.SaveChanges();
            Layouts layouts = context.Layouts.FirstOrDefault(l => l.Id == idLayout);
            if(layouts != null)
            {
                layouts.IsUsed = false;
                context.SaveChanges();
            }
        }

        internal int ResetPasswordFoDriver(string newPassword, string idDriver, string token)
        {
            int isStateActual = 0;
            PasswordRecovery passwordRecovery = context.PasswordRecoveries.ToList().FirstOrDefault(p => p.IdDriver.ToString() == idDriver && p.Token == token);
            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                Driver driver = context.Drivers.FirstOrDefault(d => d.Id.ToString() == idDriver);
                driver.Password = newPassword;
                isStateActual = 2;
            }
            if(passwordRecovery != null)
            {
                context.PasswordRecoveries.Remove(passwordRecovery);
            }
            context.SaveChanges();
            return isStateActual;
        }

        internal void SelectProfileDb(int idProfile, bool isUsed = true)
        {
            ProfileSetting profileSetting = context.ProfileSettings.FirstOrDefault(p => p.Id == idProfile);
            profileSetting.IsUsed = isUsed;
            context.SaveChanges();
        }

        internal void LayoutDownDb(int idLayout, int idTransported)
        {
            TransportVehicle transportVehicle = context.TransportVehicles
                   .Where(p => p.Id == idTransported)
                   .Include(p => p.Layouts)
                   .First(p => p.Id == idTransported);
            Layouts layoutsCurrent = transportVehicle.Layouts.First(l => l.Id == idLayout);
            Layouts layoutsDown = transportVehicle.Layouts.First(l => l.OrdinalIndex == layoutsCurrent.OrdinalIndex + 1);
            layoutsCurrent.OrdinalIndex++;
            layoutsDown.OrdinalIndex--;
            context.SaveChanges();
        }

        internal void RemoveUserByIdDb(string idUser)
        {
            context.User.Remove(context.User.First(u => u.Id.ToString() == idUser));
            context.SaveChanges();
        }

        internal int GetUserIdByKey(string key)
        {
            using (Context _context = new Context())
            {
                return _context.User.First(u => u.KeyAuthorized == key).Id;
            }
        }

        internal string GetFullNameDriverById(string idDriver)
        {
            return context.Drivers.First(d => d.Id.ToString() == idDriver).FullName;
        }

        internal string GetDriverIdByIdOrder(string idOrder)
        {
            Shipping shipping = context.Shipping
                .First(s => s.Id.ToString() == idOrder);
            return shipping.IdDriver.ToString();
        }

        internal string GetFullNameUserByKey(string key)
        {
            return context.User.First(u => u.KeyAuthorized == key).Login;
        }

        internal int CheckReportDriverDb(string nameDriver, string driversLicense)
        {
            List<DriverReport> driverReports = new List<DriverReport>();
            if (nameDriver != null || driversLicense != null)
            {
                driverReports.AddRange(context.DriverReports.Where(d => nameDriver == d.FullName && driversLicense == d.DriversLicenseNumber));
            }
            return driverReports.Count;
        }

        internal void AddHistory(HistoryOrder historyOrder)
        {
            context.HistoryOrders.Add(historyOrder);
            context.SaveChanges();
        }

        internal string GetEmailDriverDb(string idDriver)
        {
            string emailDriver = "";
            Driver driver = context.Drivers.FirstOrDefault(d => d.Id.ToString() == idDriver);
            if (driver != null)
            {
                emailDriver = driver.EmailAddress;
            }
            return emailDriver;
        }

        internal void AddNewReportDriverDb(string fullName, string driversLicenseNumber, string numberOfAccidents, string english, string returnedEquipmen, string workingEfficiency, string eldKnowledge, string drivingSkills,
            string paymentHandling, string alcoholTendency, string drugTendency, string terminated, string experience, string dotViolations, string description)
        {
            int idDriver = 0;
            int idCompany = 0;
            string dateRegistration = "";
            Driver driver = context.Drivers.LastOrDefault(d => d.DriversLicenseNumber == driversLicenseNumber);
            if(driver != null)
            {
                idDriver = driver.Id;
                dateRegistration = driver.DateRegistration;
                idCompany = context.Commpanies.FirstOrDefault(c => c.Id == driver.CompanyId).Id;
            }    
            context.DriverReports.Add(new DriverReport()
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
            context.SaveChanges();
        }

        internal int CheckTokenFoDriverDb(string idDriver, string token)
        {
            int isStateActual = 0;
            PasswordRecovery passwordRecovery = context.PasswordRecoveries.ToList().FirstOrDefault(p => p.IdDriver.ToString() == idDriver && p.Token == token);
            if(passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                isStateActual = 1;
            }
            return isStateActual;
        }

        internal VehiclwInformation GetVechById(string idVech)
        {
            return context.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == idVech);
        }

        internal string GetIdOrderByIdVech(string idVech)
        {
            Shipping shipping = context.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefault(s => s.VehiclwInformations.FirstOrDefault(v => v.Id.ToString() == idVech) != null);
            return shipping.Id;
        }

        internal async Task<Truck> GetTruckByPlateDb(string truckPlate)
        {
            return await context.Trucks.FirstOrDefaultAsync(t => t.PlateTruk == truckPlate);
        }

        internal async Task<Trailer> GetTrailerByPlateDb(string trailerPlate)
        {
            return await context.Trailers.FirstOrDefaultAsync(t => t.Plate == trailerPlate);
        }

        internal List<DriverReport> GetDriversReportsDb(string nameDriver, string driversLicense)
        {
            List<DriverReport> driverReports = new List<DriverReport>();
            if (nameDriver != null || driversLicense != null)
            {
                driverReports.AddRange(context.DriverReports.Where(d => 
                (nameDriver == d.FullName)
                && (driversLicense == d.DriversLicenseNumber)));
            }
            return driverReports;
        }

        internal void RemoveCustomerST(Customer_ST customer_ST)
        {
            context.Customer_STs.Remove(customer_ST);
            context.SaveChanges();
        }

        internal void RemoveSubscribeST(Customer_ST customer_ST)
        {
            Subscribe_ST subscribe_ST = context.Subscribe_STs.FirstOrDefault(s => s.IdCustomerST == customer_ST.IdCustomerST);
            if(subscribe_ST != null)
            {
                context.Subscribe_STs.Remove(subscribe_ST);
                context.SaveChanges();
            }
        }

        internal Customer_ST GetCustomer_STByIdCompany(string idCompany)
        {
            Customer_ST customer_ST = null;
            customer_ST = context.Customer_STs.FirstOrDefault(c => c.IdCompany.ToString() == idCompany);
            return customer_ST;
        }

        internal int[] GetIdTruckAdnTrailarDb(string idDriver)
        {
            int[] idTruckAdnTrailar = null;
            InspectionDriver inspectionDriver = context.InspectionDrivers.Last();
            if(inspectionDriver != null)
            {
                idTruckAdnTrailar = new int[] { inspectionDriver.IdITruck, inspectionDriver.IdITrailer };
            }
            else
            {
                idTruckAdnTrailar = new int[0];
            }
            return idTruckAdnTrailar;
        }

        internal List<Trailer> GetTrailersDb(string idCompany)
        {
            return context.Trailers.Where(t => t.CompanyId.ToString() == idCompany).ToList();
        }

        internal void RemoveTruck(string id)
        {
            context.Trucks.Remove(context.Trucks.FirstOrDefault(t => t.Id.ToString() == id));
            context.SaveChanges();
        }

        internal List<Truck> GetTrucs(string idCompany)
        {
            return context.Trucks.Where(t => t.CompanyId.ToString() == idCompany).ToList();
        }

        private void Init()
        {
            context.Shipping.Load();
            context.VehiclwInformation.Load();
            context.Drivers.Load();
        }
        
        public async void SaveNewContact(Contact contact)
        {
            await context.Contacts.AddAsync(contact);
            await context.SaveChangesAsync();
        }

        public List<Contact> GetContactsDB(string idCompany)
        {
            return context.Contacts.Where(c => c.CompanyId.ToString() == idCompany).ToList();
        }

        public async Task<List<Driver>> GetDriversInDb()
        {
            List<Driver> drivers = null;
            drivers = await context.Drivers
                .Where(d => !d.IsFired)
                .Include(d => d.InspectionDrivers)
                .Include(d => d.geolocations)
                .ToListAsync();
            return drivers;
        }

        internal List<Users> GetUsers()
        {
            return context.User.ToList();
        }

        public async Task<List<Driver>> GetDriversInDb(string idCommpany)
        {
            List<Driver> drivers = null;
            drivers = await context.Drivers
                .Where(d => !d.IsFired && d.CompanyId.ToString() == idCommpany)
                .Include(d => d.InspectionDrivers)
                .Include(d => d.geolocations)
                .ToListAsync();
            return drivers;
        }

        public async void RecurentOnDeleted(string id)
        {
            Shipping shipping = await context.Shipping.FirstOrDefaultAsync(s => s.Id == id);
            if(shipping != null)
            {
                if(shipping.CurrentStatus == "Delivered,Billed" || shipping.CurrentStatus == "Delivered,Paid")
                {
                    shipping.CurrentStatus = shipping.CurrentStatus.Replace("Delivered", "Deleted");
                }
                else
                {
                    shipping.CurrentStatus = "Deleted";
                }
                await context.SaveChangesAsync();
            }
        }

        public async void RecurentOnArchived(string id)
        {
            Shipping shipping = await context.Shipping.FirstOrDefaultAsync(s => s.Id == id);
            if (shipping != null)
            {
                if (shipping.CurrentStatus == "Delivered,Billed" || shipping.CurrentStatus == "Delivered,Paid")
                {
                    shipping.CurrentStatus = shipping.CurrentStatus.Replace("Delivered", "Archived");
                }
                else
                {
                    shipping.CurrentStatus = "Archived";
                }
                await context.SaveChangesAsync();
            }
        }

        internal int CreateTrukDb(string nameTruk, string yera, string make, string model, string typeTruk, string state, string exp, string vin, string owner, string plateTruk, string color, string idCompany)
        {
            Truck truck = new Truck()
            {
                ColorTruk = color,
                Exp = exp,
                Make = make,
                Model = model,
                NameTruk = nameTruk,
                Owner = owner,
                PlateTruk = plateTruk,
                Satet = state,
                Vin = vin,
                Yera = yera,
                Type = typeTruk,
                CompanyId = Convert.ToInt32(idCompany)
            };
            context.Trucks.Add(truck);
            context.SaveChanges();
            return truck.Id;
        }

        public Shipping GetShipingCurrentVehiclwInDb(string id)
        {
            VehiclwInformation vehiclwInformation = context.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == id);
            Shipping shipping = context.Shipping.Where(s => s.VehiclwInformations.FirstOrDefault(v => v == vehiclwInformation) != null)
                .Include(s => s.VehiclwInformations)
                .Include("VehiclwInformations.Ask")
                .Include("VehiclwInformations.Ask.Any_personal_or_additional_items_with_or_in_vehicle")
                .Include("VehiclwInformations.Ask1")
                .Include("VehiclwInformations.Ask1.App_will_force_driver_to_take_pictures_of_each_strap")
                .Include("VehiclwInformations.Ask1.Photo_after_loading_in_the_truck")
                .Include("VehiclwInformations.AskDelyvery")
                .Include("VehiclwInformations.AskDelyvery.Please_take_a_picture_Id_of_the_person_taking_the_delivery")
                .Include("VehiclwInformations.PhotoInspections.Photos")
                .Include(v => v.AskFromUser)
                .Include(v => v.AskFromUser.App_will_ask_for_signature_of_the_client_signature)
                .Include(v => v.AskFromUser.PhotoPay)
                .Include(v => v.AskFromUser.VideoRecord)
                .Include(v => v.AskFromUser.App_will_ask_for_signature_of_the_client_signature)
                .Include(v => v.askForUserDelyveryM)
                .Include(v => v.askForUserDelyveryM.App_will_ask_for_signature_of_the_client_signature)
                .Include(v => v.askForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo)
                .Include(v => v.askForUserDelyveryM.PhotoPay)
                .Include(v => v.askForUserDelyveryM.VideoRecord)
                .Include("VehiclwInformations.Scan")
                .Include(v => v.Ask2)
                .FirstOrDefault();
            return context.Shipping.FirstOrDefault(s => s.VehiclwInformations.FirstOrDefault(v => v == vehiclwInformation) != null);
        }

        internal void RemoveTrailerDb(string id)
        {
            context.Trailers.Remove(context.Trailers.FirstOrDefault(t => t.Id.ToString() == id));
            context.SaveChanges();
        }

        public void Solved(string idOrder)
        {
            Shipping shipping = context.Shipping.First(s => s.Id == idOrder);
            shipping.IsProblem = false;
            context.SaveChanges();
        }

        public Driver GetDriver(int id)
        {
            return context.Drivers.FirstOrDefault(d => d.Id == id);
        }

        public Driver GetDriver(string idInspection)
        {
            InspectionDriver inspectionDriver = context.InspectionDrivers.First(i => i.Id.ToString() == idInspection);
            return context.Drivers
                .Include(d => d.InspectionDrivers)
                .First(d => d.InspectionDrivers.FirstOrDefault(ii => ii.Id == inspectionDriver.Id) != null);
        }

        public void SavevechInDb(string idVech, VehiclwInformation vehiclwInformation)
        {
            VehiclwInformation vehiclwInformationDb = context.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == idVech);
            vehiclwInformationDb.VIN = vehiclwInformation.VIN;
            vehiclwInformationDb.Year = vehiclwInformation.Year;
            vehiclwInformationDb.Make = vehiclwInformation.Make;
            vehiclwInformationDb.Model = vehiclwInformation.Model;
            vehiclwInformationDb.Type = vehiclwInformation.Type;
            vehiclwInformationDb.Color = vehiclwInformation.Color;
            vehiclwInformationDb.Lot = vehiclwInformation.Lot;
            context.SaveChanges();
        }

        internal int CreateTrailerDb(string name, string typeTrailer, string year, string make, string howLong, string vin, string owner, string color, string plate, string exp, string annualIns, string idCompany)
        {
            Trailer trailer = new Trailer()
            {
                AnnualIns = annualIns,
                Color = color,
                Exp = exp,
                HowLong = howLong,
                Make = make,
                Name = name,
                Owner = owner,
                Plate = plate,
                Vin = vin,
                Year = year,
                Type = typeTrailer,
                CompanyId = Convert.ToInt32(idCompany)
            };
            context.Trailers.Add(trailer);
            context.SaveChanges();
            return trailer.Id;
        }

        internal async Task<List<DucumentDriver>> GetDriverDoc(string id)
        {
            return await context.DucumentDrivers.Where(d => d.IdDriver.ToString() == id).ToListAsync();
        }

        internal void SavePathDb(string id, string path)
        {
            //Truck truck = context.Trucks.First(t => t.Id.ToString() == id);
            //truck.PathDoc = path;
            //context.SaveChanges();
        }

        public async void RemoveVechInDb(string idVech)
        {
            context.VehiclwInformation.Remove(context.VehiclwInformation.FirstOrDefault(v => v.Id.ToString() == idVech));
            context.SaveChanges();
        }

        public async void AddOrder(Shipping shipping)
        {
            bool isCheckOrder = CheckUrlOrder(shipping);
            if (CheckOrder(shipping) && !isCheckOrder)
            {
                shipping.Id += new Random().Next(0, 1000);
            }
            try
            {
                if (!isCheckOrder)
                {
                    await context.Shipping.AddAsync(shipping);
                    await context.SaveChangesAsync();
                }
                else
                {
                }
            }
            catch (Exception e)
            {
            }
        }

        public bool CheckUrlOrder(Shipping shipping)
        {
            return context.Shipping.FirstOrDefault(s => s.UrlReqvest == shipping.UrlReqvest) != null;
        }

        private bool CheckOrder(Shipping shipping)
        {
            return context.Shipping.FirstOrDefault(s => s.Id == shipping.Id) != null;
        }

        internal async Task<List<DocumentTruckAndTrailers>> GetTruckDocDB(string id)
        {
            return await context.DocumentTruckAndTrailers.Where(d => d.TypeTr == "Truck" && d.IdTr.ToString() == id).ToListAsync();
        }

        internal async Task<List<DucumentCompany>> GetCompanyDocDB(string id)
        {
            return await context.DucumentCompanies.Where(d => d.IdCommpany.ToString() == id).ToListAsync();
        }

        internal async Task<List<DocumentTruckAndTrailers>> GetTrailerDocDB(string id)
        {
            return await context.DocumentTruckAndTrailers.Where(d => d.TypeTr == "Trailer" && d.IdTr.ToString() == id).ToListAsync();
        }

        internal void RemoveDocDriverDb(string idDock)
        {
            context.DucumentDrivers.Remove(context.DucumentDrivers.FirstOrDefault(d => d.Id.ToString() == idDock));
            context.SaveChanges();
        }

        internal string GetDocumentDb(string id)
        {
            //string pathDoc = "";
            //Driver driver = context.Drivers
            //    .Include(d => d.InspectionDrivers)
            //    .FirstOrDefault(d => d.Id.ToString() == id);
            //if (driver.InspectionDrivers != null)
            //{
            //    InspectionDriver inspectionDriver = driver.InspectionDrivers.Last();
            //    Truck truck = context.Trucks.FirstOrDefault(t => t.Id == inspectionDriver.IdITruck);
            //    if (truck != null)
            //    {
            //        pathDoc = truck.PathDoc;
            //    }
            //}
            return "";
        }

        internal List<DocumentTruckAndTrailers> GetTraileDocDb(string id)
        {
            throw new NotImplementedException();
        }

        internal void RemoveDocDb(string idDock)
        {
            context.DocumentTruckAndTrailers.Remove(context.DocumentTruckAndTrailers.First(d => d.Id.ToString() == idDock));
            context.SaveChanges();
        }

        internal void RemoveDocCompanyDb(string idDock)
        {
            context.DucumentCompanies.Remove(context.DucumentCompanies.First(d => d.Id.ToString() == idDock));
            context.SaveChanges();
        }

        internal void SaveDocTruckDb(string path, string id, string nameDoc)
        {
            string pref = path.Remove(0, path.LastIndexOf(".") + 1);
            DocumentTruckAndTrailers documentTruckAndTrailers = new DocumentTruckAndTrailers()
            {
                DocPath = path,
                IdTr = Convert.ToInt32(id),
                NameDoc = nameDoc,
                TypeTr = "Truck",
                TypeDoc = pref
            };
            context.DocumentTruckAndTrailers.Add(documentTruckAndTrailers);
            context.SaveChanges();
        }

        internal void SaveDocTrailekDb(string path, string id, string nameDoc)
        {
            string pref = path.Remove(0, path.LastIndexOf(".") + 1);
            DocumentTruckAndTrailers documentTruckAndTrailers = new DocumentTruckAndTrailers()
            {
                DocPath = path,
                IdTr = Convert.ToInt32(id),
                NameDoc = nameDoc,
                TypeTr = "Trailer",
                TypeDoc = pref
            };
            context.DocumentTruckAndTrailers.Add(documentTruckAndTrailers);
            context.SaveChanges();
        }

        public async Task<VehiclwInformation> AddVechInDb(string idOrder)
        {
            Shipping shipping = await context.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefaultAsync(s => s.Id.ToString() == idOrder);
            VehiclwInformation vehiclwInformation = new VehiclwInformation();
            if(shipping.VehiclwInformations == null)
            {
                shipping.VehiclwInformations = new List<VehiclwInformation>();
            }
            shipping.VehiclwInformations.Add(vehiclwInformation);
            await context.SaveChangesAsync();
            return vehiclwInformation;
        }

        internal List<HistoryOrder> GetHistoryOrderByIdOrder(string idOrder)
        {
            return context.HistoryOrders.Where(ho => ho.IdOreder.ToString() == idOrder).ToList();
        }

        public async Task<Shipping> CreateShipping()
        {
            Shipping shipping = new Shipping();
            shipping.Id = CreateIdShipping().ToString();
            shipping.CurrentStatus = "NewLoad";
            context.Shipping.Add(shipping);
            await context.SaveChangesAsync();
            Shipping shipping1 = context.Shipping.FirstOrDefault(s => s.Id == shipping.Id);
            return shipping;
        }

        public List<InspectionDriver> GetInspectionTrucksDb(string idDriver, string idTruck, string idTrailer, string date)
        {

            List<InspectionDriver> inspectionDrivers = new List<InspectionDriver>();
            context.Drivers
                .Include(d => d.InspectionDrivers).ToList()
                .Where(d => idDriver == "0" || d.Id.ToString() == idDriver)
                .ToList()
                .ForEach((item) =>
                {
                    inspectionDrivers.AddRange(item.InspectionDrivers
                        .Where(iD => (date == "0" || (Convert.ToDateTime(iD.Date).Month == Convert.ToDateTime(date).Month && Convert.ToDateTime(iD.Date).Year == Convert.ToDateTime(date).Year))
                        && (idTruck == "0" || iD.IdITruck.ToString() == idTruck)
                        && (idTrailer == "0" || iD.IdITrailer.ToString() == idTrailer)));
                });

            return inspectionDrivers;
        }

        private int CreateIdShipping()
        {
            int id = 1;
            while(context.Shipping.FirstOrDefault(s => s.Id == id.ToString()) != null) { id = new Random().Next(0, 100000000); }
            return id;
        }

        public InspectionDriver GetInspectionTruck(string idInspection)
        {
            return context.InspectionDrivers
                .Where(i => i.Id.ToString() == idInspection)
                .Include(i => i.PhotosTruck)
                .First();
        }

        internal void SaveDocCommpanyDb(string path, string id, string nameDoc)
        {
            string pref = path.Remove(0, path.LastIndexOf(".") + 1);
            DucumentCompany ducumentCompany = new DucumentCompany()
            {
                DocPath = path,
                NameDoc = nameDoc,
                IdCommpany = Convert.ToInt32(id)
            };
            context.DucumentCompanies.Add(ducumentCompany);
            context.SaveChanges();
        }

        public bool ExistsDataUser(string login, string password)
        {
            return context.User.FirstOrDefault(u => u.Login == login && u.Password == password) != null;
        }

        public void SaveKeyDatabays(string login, string password, int key)
        {
            Init();
            try
            {
                Users users = context.User.FirstOrDefault(u => u.Login == login && u.Password == password);
                users.KeyAuthorized = key.ToString();
                context.SaveChanges();
            }
            catch (Exception)
            {
               
            }
        }

        public bool CheckKeyDb(string key)
        {
            return context.User.FirstOrDefault(u => u.KeyAuthorized == key) != null;
        }

        public async Task<List<Shipping>> GetShippings(string status, int page, string name, string address, string phone, string email, string price)
        {
            List<Shipping> shipping = null;
            shipping = await context.Shipping
                .Where(s => s.CurrentStatus == status
                && (name == null || s.NameD.Contains(name) || s.NameP.Contains(name) || s.ContactNameP.Contains(name) || s.ContactNameD.Contains(name))
                && (address == null || s.AddresP.Contains(address) || s.AddresD.Contains(address) || s.CityP.Contains(address) || s.CityD.Contains(address) || s.StateP.Contains(address.ToUpper()) || s.StateD.Contains(address.ToUpper()) || s.ZipP.Contains(address) || s.ZipD.Contains(address))
                && (phone == null || s.PhoneP.Contains(phone) || s.PhoneD.Contains(phone) || s.PhoneC.Contains(phone))
                && (email == null || s.EmailP.Contains(email) || s.EmailD.Contains(email))
                && (price == null || s.PriceListed.Contains(price)))
                .ToListAsync();
            
            if (page != 0)
            {
                try
                {
                    shipping = shipping.GetRange((20 * page) - 20, 20);
                }
                catch(Exception)
                {
                    shipping = shipping.GetRange((20 * page) - 20, shipping.Count % 20);
                }
            }
            else
            {
                try
                {
                    shipping = shipping.GetRange(0, 20);
                }
                catch (Exception)
                {
                    shipping = shipping.GetRange(0, shipping.Count % 20);
                }
            }
            return shipping;
        }

        public async Task<int> GetCountPageInDb(string status, string name, string address, string phone, string email, string price)
        {
            int countPage = 0;
            List<Shipping> shipping = await context.Shipping
                .Where(s => s.CurrentStatus == status
                 && (name == null || s.NameD.Contains(name) || s.NameP.Contains(name) || s.ContactNameP.Contains(name) || s.ContactNameD.Contains(name))
                && (address == null || s.AddresP.Contains(address) || s.AddresD.Contains(address) || s.CityP.Contains(address) || s.CityD.Contains(address) || s.StateP.Contains(address.ToUpper()) || s.StateD.Contains(address.ToUpper()) || s.ZipP.Contains(address) || s.ZipD.Contains(address))
                && (phone == null || s.PhoneP.Contains(phone) || s.PhoneD.Contains(phone) || s.PhoneC.Contains(phone))
                && (email == null || s.EmailP.Contains(email) || s.EmailD.Contains(email))
                && (price == null || s.PriceListed.Contains(price)))
                .ToListAsync();
            countPage = shipping.Count / 20;
            int remainderPage = shipping.Count % 20;
            countPage = remainderPage > 0 ? countPage + 1 : countPage;
            return countPage;
        }

        public bool CheckInspactionDriverToDay(int idDriver)
        {
            Driver driver = context.Drivers.Include(d => d.InspectionDrivers).FirstOrDefault(d => d.Id == idDriver);
            InspectionDriver inspectionDriver = driver.InspectionDrivers != null && driver.InspectionDrivers.Count != 0 ? driver.InspectionDrivers.Last() : null;
            if (inspectionDriver == null)
            {
                driver.IsInspectionDriver = false;
                driver.IsInspectionToDayDriver = false;
                context.SaveChanges();
            }
            else if (Convert.ToDateTime(inspectionDriver.Date).Date != DateTime.Now.Date)
            {
                if (DateTime.Now.Hour >= 12)
                {
                    driver.IsInspectionDriver = false;
                    driver.IsInspectionToDayDriver = false;
                }
                else if (DateTime.Now.Hour <= 12 && 6 >= DateTime.Now.Hour)
                {
                    driver.IsInspectionDriver = true;
                    driver.IsInspectionToDayDriver = false;
                }
                context.SaveChanges();
            }
            return driver.IsInspectionToDayDriver;
        }

        public List<Driver> GetDrivers(int page, string idCompany)
        {
            List<Driver> drivers = null;
            drivers = context.Drivers.Where(d => !d.IsFired && d.CompanyId.ToString() == idCompany).ToList();
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

        public async Task<List<VehiclwInformation>> AddDriversInOrder(string idOrder, string idDriver)
        {
            Shipping shipping = context.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefault(s => s.Id == idOrder);
            Driver driver = context.Drivers.FirstOrDefault(d => d.Id == Convert.ToInt32(idDriver));
            shipping.IdDriver = driver.Id;
            shipping.CurrentStatus = "Assigned";
            await context.SaveChangesAsync();
            return shipping.VehiclwInformations;
        }

        public bool CheckDriverOnShipping(string idShipping)
        {
            bool isDriverAssign = false;
            Shipping shipping = context.Shipping
                .FirstOrDefault(s => s.Id == idShipping);
            if(context.Drivers.FirstOrDefault(d => d.Id == shipping.IdDriver) != null)
            {
                isDriverAssign = true;
            }
            return isDriverAssign;
        }

        public string GerShopToken(string idDriver)
        {
            Driver driver = context.Drivers.FirstOrDefault<Driver>(d => d.Id == Convert.ToInt32(idDriver));
            return driver.TokenShope;
        }

        public string GerShopTokenForShipping(string idOrder)
        {
            Shipping shipping = context.Shipping
                .FirstOrDefault(d => d.Id == idOrder);
            Driver driver = context.Drivers.First(d => d.Id == shipping.IdDriver);
            return driver.TokenShope;
        }

        public async Task<List<VehiclwInformation>> RemoveDriversInOrder(string idOrder)
        {
            Shipping shipping = context.Shipping
                .Include(s => s.VehiclwInformations)
                .FirstOrDefault(s => s.Id == idOrder);
            shipping.IdDriver = 0;
            shipping.CurrentStatus = "NewLoad";
            await context.SaveChangesAsync();
            return shipping.VehiclwInformations;
        }

        public Shipping GetShipping(string id)
        {
            return context.Shipping.Include(s => s.VehiclwInformations).FirstOrDefault(s => s.Id == id);
        }

        public async void UpdateorderInDb(string idOrder, string idLoad, string internalLoadID, string driver, string status, string instructions, string nameP, string contactP,
            string addressP, string cityP, string stateP, string zipP, string phoneP, string emailP, string scheduledPickupDateP, string nameD, string contactD, string addressD,
            string cityD, string stateD, string zipD, string phoneD, string emailD, string ScheduledPickupDateD, string paymentMethod, string price, string paymentTerms, string brokerFee,
            string contactId, string phoneC, string faxC, string iccmcC)
        {
            Shipping shipping = context.Shipping.FirstOrDefault(s => s.Id == idOrder);
            shipping.idOrder = idLoad != null ? idLoad : shipping.Id;
            shipping.InternalLoadID = internalLoadID != null ? internalLoadID : shipping.InternalLoadID;
            //shipping.Driverr = internalLoadID != null ? internalLoadID : shipping.InternalLoadID;
            if(status == "Delivered")
            {
                shipping.CurrentStatus = shipping.CurrentStatus.Replace("Deleted", "Delivered");
            }
            else if(status == "Archived")
            {
                shipping.CurrentStatus = shipping.CurrentStatus.Replace("Archived", "Delivered");
            }
            else
            {
                shipping.CurrentStatus = status != null ? status : shipping.CurrentStatus;
            }
            shipping.Titl1DI = instructions != null ? instructions : shipping.Titl1DI;
            shipping.NameP = nameP != null ? nameP : shipping.NameD;
            shipping.ContactNameP = contactP != null ? contactP : shipping.ContactNameP;
            shipping.AddresP = addressP != null ? addressP : shipping.AddresP;
            shipping.CityP = cityP != null ? cityP : shipping.CityP;
            shipping.StateP = stateP != null ? stateP : shipping.StateP;
            shipping.ZipP = zipP != null ? zipP : shipping.ZipP;
            shipping.PhoneP = phoneP != null ? phoneP : shipping.PhoneP;
            shipping.EmailP = emailP != null ? emailP : shipping.EmailP;
            shipping.PickupExactly = scheduledPickupDateP != null ? scheduledPickupDateP : shipping.PickupExactly;
            shipping.NameD = nameD != null ? nameD : shipping.NameD;
            shipping.ContactNameD = contactD != null ? contactD : shipping.ContactNameD;
            shipping.AddresD = addressD != null ? addressD : shipping.AddresD;
            shipping.CityD = cityD != null ? cityD : shipping.CityD;
            shipping.StateD = stateD != null ? stateD : shipping.StateD;
            shipping.ZipD = zipD != null ? zipD : shipping.ZipD;
            shipping.PhoneD = phoneD != null ? phoneD : shipping.PhoneD;
            shipping.EmailD = emailD != null ? emailD : shipping.EmailD;
            shipping.DeliveryEstimated = ScheduledPickupDateD != null ? ScheduledPickupDateD : shipping.DeliveryEstimated;
            shipping.TotalPaymentToCarrier = paymentMethod != null ? paymentMethod : shipping.TotalPaymentToCarrier;
            shipping.PriceListed = price != null ? price : shipping.PriceListed;
            shipping.BrokerFee = brokerFee != null ? brokerFee : shipping.BrokerFee;
            shipping.ContactC = contactId != null ? contactId : shipping.ContactC;
            shipping.PhoneC = phoneC != null ? phoneC : shipping.PhoneC;
            shipping.FaxC = faxC != null ? faxC : shipping.FaxC;
            shipping.IccmcC = iccmcC != null ? iccmcC : shipping.IccmcC;
            await context.SaveChangesAsync();
        }

        internal void SaveDocDriverDb(string path, string id, string nameDoc)
        {
            string pref = path.Remove(0, path.LastIndexOf(".") + 1);
            DucumentDriver ducumentCompany = new DucumentDriver()
            {
                DocPath = path,
                NameDoc = nameDoc,
                IdDriver = Convert.ToInt32(id)
            };
            context.DucumentDrivers.Add(ducumentCompany);
            context.SaveChanges();
        }

        public async void CreateOrderInDb(string idOrder, string idLoad, string internalLoadID, string driver, string status, string instructions, string nameP, string contactP,
            string addressP, string cityP, string stateP, string zipP, string phoneP, string emailP, string scheduledPickupDateP, string nameD, string contactD, string addressD,
            string cityD, string stateD, string zipD, string phoneD, string emailD, string ScheduledPickupDateD, string paymentMethod, string price, string paymentTerms, string brokerFee)
        {
            Init();
            Shipping shipping = new Shipping();
            shipping.idOrder = idLoad != null ? idLoad : shipping.Id;
            shipping.InternalLoadID = internalLoadID != null ? internalLoadID : shipping.InternalLoadID;
            //shipping.Driverr = internalLoadID != null ? internalLoadID : shipping.InternalLoadID;
            shipping.CurrentStatus = status != null ? status : shipping.CurrentStatus;
            shipping.Titl1DI = instructions != null ? instructions : shipping.Titl1DI;
            shipping.NameP = nameP != null ? nameP : shipping.NameD;
            shipping.ContactNameP = contactP != null ? contactP : shipping.ContactNameP;
            shipping.AddresP = addressP != null ? addressP : shipping.AddresP;
            shipping.CityP = cityP != null ? cityP : shipping.CityP;
            shipping.StateP = stateP != null ? stateP : shipping.StateP;
            shipping.ZipP = zipP != null ? zipP : shipping.ZipP;
            shipping.PhoneP = phoneP != null ? phoneP : shipping.PhoneP;
            shipping.EmailP = emailP != null ? emailP : shipping.EmailP;
            shipping.PickupExactly = scheduledPickupDateP != null ? scheduledPickupDateP : shipping.PickupExactly;
            shipping.NameD = nameD != null ? nameD : shipping.NameD;
            shipping.ContactNameD = contactD != null ? contactD : shipping.ContactNameD;
            shipping.AddresD = addressD != null ? addressD : shipping.AddresD;
            shipping.CityD = cityD != null ? cityD : shipping.CityD;
            shipping.StateD = stateD != null ? stateD : shipping.StateD;
            shipping.ZipD = zipD != null ? zipD : shipping.ZipD;
            shipping.PhoneD = phoneD != null ? phoneD : shipping.PhoneD;
            shipping.EmailD = emailD != null ? emailD : shipping.EmailD;
            shipping.DeliveryEstimated = ScheduledPickupDateD != null ? ScheduledPickupDateD : shipping.DeliveryEstimated;
            shipping.TotalPaymentToCarrier = paymentMethod != null ? paymentMethod : shipping.TotalPaymentToCarrier;
            shipping.PriceListed = price != null ? price : shipping.PriceListed;
            shipping.BrokerFee = brokerFee != null ? brokerFee : shipping.BrokerFee;
            context.Shipping.Add(shipping);
            await context.SaveChangesAsync();
        }

        public int AddDriver(Driver driver)
        {
            context.Drivers.Add(driver);
            context.SaveChanges();
            return driver.Id;
        }



        public  void UpdateDriver(int id, string fullName, string emailAddress, string password, string phoneNumbe, string trailerCapacity, string driversLicenseNumber)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.Id == id);
            driver.FullName = fullName;
            driver.EmailAddress = emailAddress;
            driver.Password = password;
            driver.PhoneNumber = phoneNumbe;
            driver.TrailerCapacity = trailerCapacity;
            driver.DriversLicenseNumber = driversLicenseNumber;
            context.SaveChanges();
        }

        public void RemoveDriveInDb(DriverReportModel model)
        {
            Driver driver = context.Drivers
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
            context.DriverReports.Add(driverReport);
            context.SaveChanges();
        }

        public async void RestoreDriveInDb(int id)
        {
            Driver driver = context.Drivers.FirstOrDefault(d => d.Id == id);
            driver.IsFired = false;
            await context.SaveChangesAsync();
        }
    }
}