using System;
using System.Collections.Generic;
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
using WebDispacher.ViewModels.Settings;

namespace WebDispacher.Business.Services
{
    public class UserService : IUserService
    {
        private readonly Context db;
        private readonly IMapper mapper;

        public UserService(
            IMapper mapper,
            Context db)
        {
            this.mapper = mapper;
            this.db = db;
        }
        
        public bool CheckPermissions(string key, string idCompany, string route)
        {
            return CheckKey(key) && IsPermission(key, idCompany, route);
        }
        
        public Users GetUserByEmailAndPasswrod(string email, string password)
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
            var isPermission = false;
            var users = GetUserByKey(key);
            var company = GetCompanyById(idCompany);
            
            if(users != null && company != null)
            {
                isPermission = ValidCompanyRoute(company.Type, route);
            }
            
            return isPermission;
        }
        
        public Commpany GetUserByKeyUser(int key)
        {
            var users = db.User.FirstOrDefault(u => u.KeyAuthorized == key.ToString());
            var commpany = db.Commpanies.FirstOrDefault(c => c.Id == users.CompanyId);
            
            return commpany;
        }

        public bool CheckEmailDb(string email)
        {
            return db.User.FirstOrDefault(u => u.Login == email) != null;
        }

        public bool CheckEmail(string email)
        {
            var isEmail = CheckEmailDb(email);

            if (isEmail)
            { 
                var token = CreateToken(email);
                var idUser = AddRecoveryPassword(email, token);
                var pattern = new PaternSourse().GetPaternRecoveryPassword($"{Config.BaseReqvesteUrl}/Recovery/Password?idUser={idUser}&token={token}");
                
                Task.Run(async () => await new AuthMessageSender().Execute(email,  UserConstants.PasswordRecoverySubject, pattern));
            }
            
            return isEmail;
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
            var userEdit = db.User.FirstOrDefault(c => c.Id == user.Id);
            if (userEdit == null) return;
            
            userEdit.Login = user.Login.ToLower();
            userEdit.Password = user.Password;

            db.SaveChanges();
        }
        
        public Commpany GetCompanyById(string idCompany)
        {
            return db.Commpanies.First(c => c.Id.ToString() == idCompany);
        }
        
        public SettingsUserViewModel GetUserById(int id)
        {
            var user = db.User.FirstOrDefault(c => c.Id == id);

            return mapper.Map<SettingsUserViewModel>(user);
        }
        
        public void CreateUserForCompanyId(int id, string emailCompany, string password)
        {
            db.User.Add(new Users()
            {
                CompanyId = id,
                Login = emailCompany.ToLower(),
                Password = password,
                Date = DateTime.Now.ToString()
            });
            
            db.SaveChanges();
        }

        public void AddUser(string idCompany, SettingsUserViewModel user)
        {
            var users = new Users()
            {
                CompanyId = Convert.ToInt32(idCompany),
                Date = user.Date,
                Login = user.Login,
                Password = user.Password
            };
            
            AddUserDb(users);
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
            
            var passwordRecovery = db.PasswordRecoveries.ToList().FirstOrDefault(p => p.IdUser.ToString() == idUser && p.Token == token);
            
            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                isStateActual = 1;
            }
            
            return isStateActual;
        }
        
        private bool CheckKeyDb(string key)
        {
            return db.User.FirstOrDefault(u => u.KeyAuthorized == key) != null;
        }
        
        private void SaveKeyDatabays(string login, string password, int key)
        {
            Init();
            try
            {
                var users = db.User.FirstOrDefault(u => u.Login == login && u.Password == password);
                
                if (users == null) return;
                
                users.KeyAuthorized = key.ToString();
                
                db.SaveChanges();
            }
            catch (Exception)
            {
               
            }
        }
        
        private void Init()
        {
            db.Shipping.Load();
            db.VehiclwInformation.Load();
            db.Drivers.Load();
        }
        
        private bool ExistsDataUser(string login, string password)
        {
            return db.User.FirstOrDefault(u => u.Login == login && u.Password == password) != null;
        }
        
        private void RemoveUserByIdDb(string idUser)
        {
            db.User.Remove(db.User.First(u => u.Id.ToString() == idUser));
            
            db.SaveChanges();
        }
        
        private void AddUserDb(Users user)
        {
            user.Login = user.Login != null ? user.Login.ToLower() : user.Login;

            db.User.Add(user);

            db.SaveChanges();
        }

        private int ResetPasswordFoUserDb(string newPassword, string idUser, string token)
        {
            var isStateActual = 0;
            var passwordRecovery = db.PasswordRecoveries.ToList().FirstOrDefault(p => p.IdUser.ToString() == idUser && p.Token == token);

            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                var users = db.User.FirstOrDefault(d => d.Id.ToString() == idUser);
                if (users != null)
                {
                    users.Password = newPassword;
                    isStateActual = 2;
                }
            }
            
            if (passwordRecovery != null)
            {
                db.PasswordRecoveries.Remove(passwordRecovery);
            }
            
            db.SaveChanges();
            
            return isStateActual;
        }
        
        private string GetEmailUserDb(string idUser)
        {
            var emailDriver = string.Empty;
            var users = db.User.FirstOrDefault(d => d.Id.ToString() == idUser);
            
            if (users != null)
            {
                emailDriver = users.Login;
            }
            
            return emailDriver;
        }
        
        private int AddRecoveryPassword(string email, string token)
        {
            var users = db.User.FirstOrDefault(u => u.Login == email);
            if (users == null) return 0;
            
            var passwordRecovery1 = db.PasswordRecoveries.FirstOrDefault(p => p.IdUser == users.Id);
            
            if (passwordRecovery1 == null)
            {
                var passwordRecovery = new PasswordRecovery()
                {
                    Date = DateTime.Now.ToString(),
                    IdUser = users.Id,
                    Token = token
                };
                db.PasswordRecoveries.Add(passwordRecovery);
            }
            else
            {
                passwordRecovery1.Token = token;
                passwordRecovery1.Date = DateTime.Now.ToString();
            }
            
            db.SaveChanges();
            
            return users.Id;
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
        
        public Users GetUserByKey(string key)
        {
            return db.User.First(c => c.KeyAuthorized == key);
        }

        private bool ValidCompanyRoute(TypeCompany typeCompany, string route)
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
                validCompany = true;
            }
            
            return validCompany;
        }
        
        private void AddUserToDispatch(Users users)
        {
            var dispatcher = db.Dispatchers.FirstOrDefault(d => d.IdCompany == users.CompanyId);
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
        
        private Users GetUserByEmailAndPasswordDb(string email, string password)
        {
            return db.User.FirstOrDefault(u => u.Login == email && u.Password == password);
        }
    }
}