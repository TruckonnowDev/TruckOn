using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BaceModel.ModelInspertionDriver;
using DaoModels.DAO;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebDispacher.Business.Interfaces;
using WebDispacher.Constants;
using WebDispacher.Service.EmailSmtp;
using WebDispacher.ViewModels.RA.Carrier.Login;
using WebDispacher.ViewModels.Settings;

namespace WebDispacher.Business.Services
{
    public class UserService : IUserService
    {
        private readonly Context db;
        private readonly IMapper mapper;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public UserService(
            IMapper mapper,
            Context db, 
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            this.mapper = mapper;
            this.db = db;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public bool CheckPermissions(string key, string idCompany, string route)
        {
            return IsPermission(key, idCompany, route);
        }

        public async Task<User> CreateUser(User model)
        {
            await db.Users.AddAsync(model);

            await db.SaveChangesAsync();

            return model;
        }

        public async Task SendEmailSuccessCarrierRegistration(string email)
        {
            await new AuthMessageSender().Execute(email,
                UserConstants.ThankRegistrationSubject,
                new PaternSourse().GetPatternRegistrationMail());
        }

        public User GetUserByEmailAndPasswrod(string email, string password)
        {
            var users = GetUserByEmailAndPasswordDb(email, password);
            
            if(users != null)
            {
                AddUserToDispatch(users);
            }
            
            return users;
        }

        public bool IsPermission(string key, string idCompany, string route)
        {
            var company = GetCompanyById(idCompany);

            if (company == null) return false;

            /* if (company.Type == TypeCompany.DeactivateCompany) return false;

             return ValidCompanyRoute(company.Type, route) && CheckKey(key);*/
            return false;
        }
        
       /* public Company GetUserByKeyUser(int key)
        {
            var users = db.Users.FirstOrDefault(u => u.KeyAuthorized == key.ToString());
            var commpany = db.Companies.FirstOrDefault(c => c.Id == users.CompanyId);
            
            return commpany;
        }*/

        public bool CheckEmailDb(string email)
        {
            return db.Users.FirstOrDefault(u => u.Email == email) != null;
        }

        public async Task<bool> SendRecoveryPasswordToEmail(string email, string url)
        {
            try
            {
                var pattern = new PaternSourse().GetPaternRecoveryPassword(url);

                await new AuthMessageSender().Execute(email, UserConstants.PasswordRecoverySubject, pattern);
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreatePasswordResets(User user, string code,string localDate)
        {
            try
            {
                var entityTypeId = await GetEntityTypeIdResetPassword(UserConstants.UserEntityTypeResetPassword);

                var dateTimeAction= !string.IsNullOrEmpty(localDate) ? DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture) : DateTime.Now;

                var pr = new PasswordRecovery
                {
                    EntityRecoveryId = user.Id,
                    DateTimeAction = dateTimeAction,
                    Token = code,
                    RecoveryTypeId = entityTypeId
                };

                await db.PasswordsRecoveries.AddAsync(pr);

                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> ResetPasswordFoUser(string newPassword, string idUser, string token)
        {
            var isStateActual = ResetPasswordFoUserDb(newPassword, idUser, token);
            
            if (isStateActual == 2)
            {
                var emailUser = GetEmailUserDb(idUser);
                var pattern = new PaternSourse().GetPaternDataAccountUser(emailUser, newPassword);
                await new AuthMessageSender().Execute(emailUser, UserConstants.PasswordChanged, pattern);
            }
            else
            {
                var emailDriver = GetEmailUserDb(idUser);
                var pattern = new PaternSourse().GetPaternNoRestoreDataAccountUser();
                await new AuthMessageSender().Execute(emailDriver, UserConstants.PasswordResetFailed, pattern);
            }
            
            return isStateActual;
        }
        
        public void EditUser(SettingsUserViewModel user)
        {
            var userEdit = db.Users.FirstOrDefault(c => c.Id == user.Id.ToString());
            if (userEdit == null) return;
            
            userEdit.Email = user.Login.ToLower();
            //userEdit.Password = user.Password;

            db.SaveChanges();
        }

        public Company GetCompanyById(string companyId)
        {
            var companyIndex = Int32.Parse(companyId);

            using(var context = new Context())
            {
                var company = context.Companies.FirstOrDefault(c => c.Id == companyIndex);

                return company;
            }
        }
        
        public User GetUserById(string id)
        {
            var user = db.Users.FirstOrDefault(c => c.Id == id);

            if (user == null) return null;

            return user;
        } 
        
        public async Task<User> GetFirstUserByCompanyId(int id)
        {
            var companyUser = await db.CompanyUsers.FirstOrDefaultAsync(cu => cu.CompanyId == id);

            if (companyUser == null) return null;

            var user = await db.Users.FirstOrDefaultAsync(c => c.Id == companyUser.UserId);

            if (user == null) return null;

            return user;
        }

        public async Task<string> GetFirstUserEmailByCompanyId(int id)
        {
            var companyUser = await db.CompanyUsers.FirstOrDefaultAsync(cu => cu.CompanyId == id);

            if (companyUser == null) return null;

            var user = await db.Users.FirstOrDefaultAsync(c => c.Id == companyUser.UserId);

            if (user == null) return null;

            return user.Email;
        }

        public async Task SetLastLoginDateToUser(LoginViewModel model, string localDate)
        {
            if (model == null) return;

            var dateTimeLastLogin = !string.IsNullOrEmpty(localDate) ? DateTime.ParseExact(localDate, DateTimeFormats.FullDateTimeInfo, CultureInfo.InvariantCulture) : DateTime.Now;

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email.ToLower());

            if (user != null)
            {
                user.DateTimeLastLogin = dateTimeLastLogin;
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetUsers(int page)
        {
            return await GetUsersDb(page);
        }

        public void CreateUserForCompanyId(int id, string emailCompany, string password)
        {
            db.Users.Add(new User()
            {
                /*CompanyId = id,
                Login = emailCompany.ToLower(),
                Password = password,
                Date = DateTime.Now.ToString()*/
            });
            
            db.SaveChanges();
        }

        public void AddUser(string idCompany, SettingsUserViewModel user)
        {
            var users = new User()
            {
               /* CompanyId = Convert.ToInt32(idCompany),
                Date = user.Date,
                Login = user.Login,
                Password = user.Password*/
            };
            
            //AddUserDb(users);
        }

        public void RemoveUserById(string idUser)
        {
            RemoveUserByIdDb(idUser);
        }
        
        public bool Authorization(string login, string password)
        {
            return ExistsDataUser(login, password);
        }
        
        public int CreateKey(string login, string password)
        {
            var random = new Random();
            var key = random.Next(1000, 1000000000);
            
            SaveKeyDatabays(login, password, key);
            
            return key;
        }

        public bool CheckKey(string key)
        {
            return key != null && CheckKeyDb(key);
        }
        
        public List<Layouts> GetLayoutsByTransportVehicle(ITransportVehicle transportVehicle)
        {
            return transportVehicle.Layouts.Select((t, i) => new Layouts()
                {
                    Id = 0,
                    Index = t,
                    Name = transportVehicle.NamePatern[i],
                    OrdinalIndex = i + 1,
                    IsUsed = true,
                })
                .ToList();
        }
        
        public int CheckTokenFoUser(string idUser, string token)
        {
            var isStateActual = 0;
            /*
            var passwordRecovery = db.PasswordsRecoveries.ToList().FirstOrDefault(p => p.IdUser.ToString() == idUser && p.Token == token);
            
            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                isStateActual = 1;
            }*/

            return isStateActual;
        }
        
        private bool CheckKeyDb(string key)
        {
            return false;
            //return db.Users.FirstOrDefault(u => u.KeyAuthorized == key) != null;
        }
        
        private void SaveKeyDatabays(string login, string password, int key)
        {
            Init();
            try
            {
                /*var users = db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
                
                if (users == null) return;
                
                users.KeyAuthorized = key.ToString();
                */
                db.SaveChanges();
            }
            catch (Exception)
            {
               
            }
        }
        
        private void Init()
        {
            db.Orders.Load();
            db.VehiclesInspections.Load();
            db.Drivers.Load();
        }

        private async Task<int> GetEntityTypeIdResetPassword(string name)
        {
            var entityType = await db.RecoveriesTypes.FirstOrDefaultAsync(rt => rt.Name == name);

            return entityType.Id;
        }

        private async Task<List<User>> GetUsersDb(int page)
        {
            var users = db.Users.AsQueryable();

            if (page == UserConstants.AllPagesNumber) return await users.ToListAsync();

            try
            {
                users = users.Skip(UserConstants.NormalPageCount * page - UserConstants.NormalPageCount);

                users = users.Take(UserConstants.NormalPageCount);
            }
            catch (Exception)
            {
                users = users.Skip((UserConstants.NormalPageCount * page) - UserConstants.NormalPageCount);
            }

            var listCompanies = await users.ToListAsync();

            return listCompanies;
        }

        private bool ExistsDataUser(string login, string password)
        {
            return false;
            //return db.Users.FirstOrDefault(u => u.Login == login && u.Password == password) != null;
        }
        
        private void RemoveUserByIdDb(string idUser)
        {
            db.Users.Remove(db.Users.First(u => u.Id.ToString() == idUser));
            
            db.SaveChanges();
        }
        
        private void AddUserDb(User user)
        {
            db.Users.Add(user);

            db.SaveChanges();
        }

        private int ResetPasswordFoUserDb(string newPassword, string idUser, string token)
        {
            var isStateActual = 0;
            /*var passwordRecovery = db.PasswordsRecoveries.ToList().FirstOrDefault(p => p.IdUser.ToString() == idUser && p.Token == token);

            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                var users = db.Users.FirstOrDefault(d => d.Id.ToString() == idUser);
                if (users != null)
                {
                    users.Password = newPassword;
                    isStateActual = 2;
                }
            }
            
            if (passwordRecovery != null)
            {
                db.PasswordsRecoveries.Remove(passwordRecovery);
            }
            
            db.SaveChanges();*/
            
            return isStateActual;
        }
        
        private string GetEmailUserDb(string idUser)
        {
            var emailDriver = string.Empty;
            var users = db.Users.FirstOrDefault(d => d.Id.ToString() == idUser);
            
            if (users != null)
            {
                emailDriver = users.Email;
            }
            
            return emailDriver;
        }
        
        private int AddRecoveryPassword(string email, string token)
        {
            var users = db.Users.FirstOrDefault(u => u.Email == email);
            if (users == null) return 0;
            /*
             var passwordRecovery1 = db.PasswordsRecoveries.FirstOrDefault(p => p.IdUser == users.Id);

             if (passwordRecovery1 == null)
             {
                 var passwordRecovery = new PasswordRecovery()
                 {
                     Date = DateTime.Now.ToString(),
                     IdUser = users.Id,
                     Token = token
         };
                 db.PasswordsRecoveries.Add(passwordRecovery);
             }
             else
             {
                 passwordRecovery1.Token = token;
                 passwordRecovery1.Date = DateTime.Now.ToString();
             }
             */
            db.SaveChanges();
            
            return 10;
            //return users.Id;
        }
        
        private string CreateToken(string email)
        {
            var token = string.Empty;
            
            for (var i = 0; i < email.Length; i++)
            {
                token += i * new Random().Next(1, 1000) + email[i];
            }
            
            return token;
        }
        
        public User GetUserByKey(string key)
        {
            return null;
            //return db.Users.First(c => c.KeyAuthorized == key);
        }

        /*private bool ValidCompanyRoute(TypeCompany typeCompany, string route)
        {
            var validCompany = false;

            if (route == RouteConstants.Company)
            {
                if (typeCompany == TypeCompany.BaseCommpany)
                {
                    validCompany = true;
                }
            }
            else
            {
                if (typeCompany == TypeCompany.NormalCompany)
                {
                    validCompany = true;
                }
            }

            if (route == RouteConstants.SettingsUser || route == RouteConstants.SettingsExtension || route == RouteConstants.Settings) validCompany = true;

            return validCompany;
        }*/
        
        private void AddUserToDispatch(User users)
        {
            //var dispatcher = db.Dispatchers.FirstOrDefault(d => d.CompanyId == users.CompanyId);
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
        
        private User GetUserByEmailAndPasswordDb(string email, string password)
        {
            return null;
            //return db.Users.FirstOrDefault(u => u.Login == email && u.Password == password);
        }
    }
}