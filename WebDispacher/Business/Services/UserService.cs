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
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebDispacher.Business.Interfaces;
using WebDispacher.Models;
using WebDispacher.Service;
using WebDispacher.Service.EmailSmtp;

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
        
        public Users GetUserByEmailAndPasswrod(string email, string password)
        {
            Users users = GetUserByEmailAndPasswrodDb(email, password);
            if(users != null)
            {
                AddUserToDispatch(users);
            }
            return users;
        }

        public bool IsPermission(string key, string idCompany, string route)
        {
            bool isPermission = false;
            Users users = GetUserByKey(key);
            Commpany commpany = GetCompanyById(idCompany);
            if(users != null && commpany != null)
            {
                isPermission = ValidCompanyRoute(commpany.Type, route);
            }
            return isPermission;
        }
        
        public Commpany GetUserByKeyUser(int key)
        {
            Commpany commpany = null;
            Users users = db.User.FirstOrDefault(u => u.KeyAuthorized == key.ToString());
            commpany = db.Commpanies.FirstOrDefault(c => c.Id == users.CompanyId);
            return commpany;
        }
        
        public bool CheckEmail(string email)
        {
            bool isEmail = CheckEmail(email);
            if (isEmail)
            {
                string token = CreateToken(email);
                int idUser = AddRecoveryPassword(email, token);
                string patern = new PaternSourse().GetPaternRecoveryPassword($"{Config.BaseReqvesteUrl}/Recovery/Password?idUser={idUser}&token={token}");
                Task.Run(async () => await new AuthMessageSender().Execute(email, "Password recovery", patern));
            }
            return isEmail;
        }
        
        public async Task<int> ResetPasswordFoUser(string newPassword, string idUser, string token)
        {
            int isStateActual = ResetPasswordFoUserDb(newPassword, idUser, token);
            
            if (isStateActual == 2)
            {
                string emailUser = GetEmailUserDb(idUser);
                string patern = new PaternSourse().GetPaternDataAccountUser(emailUser, newPassword);
                await new AuthMessageSender().Execute(emailUser, "Password changed successfully", patern);
            }
            else
            {
                string emailDriver = GetEmailUserDb(idUser);
                string patern = new PaternSourse().GetPaternNoRestoreDataAccountUser();
                await new AuthMessageSender().Execute(emailDriver, "Password reset attempt failed", patern);
            }
            
            return isStateActual;
        }
        
        public Commpany GetCompanyById(string idCompany)
        {
            return db.Commpanies.First(c => c.Id.ToString() == idCompany);
        }
        
        public void CreateUserForCompanyId(int id, string nameCompany, string password)
        {
            db.User.Add(new Users()
            {
                CompanyId = id,
                Login = nameCompany + "Admin",
                Password = password,
                Date = DateTime.Now.ToString()
            });
            db.SaveChanges();
        }

        public void AddUser(string idCompany, string login, string password)
        {
            Users users = new Users()
            {
                CompanyId = Convert.ToInt32(idCompany),
                Date = DateTime.Now.ToString(),
                Login = login,
                Password = password
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
        
        public int Createkey(string login, string password)
        {
            Random random = new Random();
            int key = random.Next(1000, 1000000000);
            SaveKeyDatabays(login, password, key);
            return key;
        }
        
        public bool CheckKey(string key)
        {
            return key != null && CheckKeyDb(key);
        }
        
        public List<Layouts> GetLayoutsByTransportVehicle(ITransportVehicle transportVehicle)
        {
            List<Layouts> layouts = new List<Layouts>();
            
            for(int i = 0; i < transportVehicle.Layouts.Count; i++)
            {
                layouts.Add(new Layouts()
                {
                    Id = 0,
                    Index = transportVehicle.Layouts[i],
                    Name = transportVehicle.NamePatern[i],
                    OrdinalIndex = i+1,
                    IsUsed = true,
                });
            }
            
            return layouts;
        }
        
        public int CheckTokenFoUser(string idUser, string token)
        {
            int isStateActual = 0;
            
            PasswordRecovery passwordRecovery = db.PasswordRecoveries.ToList().FirstOrDefault(p => p.IdUser.ToString() == idUser && p.Token == token);
            
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
                Users users = db.User.FirstOrDefault(u => u.Login == login && u.Password == password);
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
        
        private void AddUserDb(Users users)
        {
            db.User.Add(users);
            db.SaveChanges();
        }
        
        private int ResetPasswordFoUserDb(string newPassword, string idUser, string token)
        {
            int isStateActual = 0;
            PasswordRecovery passwordRecovery = db.PasswordRecoveries.ToList().FirstOrDefault(p => p.IdUser.ToString() == idUser && p.Token == token);
            
            if (passwordRecovery != null && Convert.ToDateTime(passwordRecovery.Date) > DateTime.Now.AddHours(-2))
            {
                Users users = db.User.FirstOrDefault(d => d.Id.ToString() == idUser);
                users.Password = newPassword;
                isStateActual = 2;
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
            string emailDriver = "";
            Users users = db.User.FirstOrDefault(d => d.Id.ToString() == idUser);
            
            if (users != null)
            {
                emailDriver = users.Login;
            }
            
            return emailDriver;
        }
        
        private int AddRecoveryPassword(string email, string token)
        {
            Users users = db.User.FirstOrDefault(u => u.Login == email);
            PasswordRecovery passwordRecovery1 = db.PasswordRecoveries.FirstOrDefault(p => p.IdUser == users.Id);
            
            if (passwordRecovery1 == null)
            {
                PasswordRecovery passwordRecovery = new PasswordRecovery()
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
            string token = "";
            
            for (int i = 0; i < email.Length; i++)
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
            bool validCompany = false;
            
            if(route == "Company")
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
            Dispatcher dispatcher = db.Dispatchers.FirstOrDefault(d => d.IdCompany == users.CompanyId);
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
        
        private Users GetUserByEmailAndPasswrodDb(string email, string password)
        {
            return db.User.FirstOrDefault(u => u.Login == email && u.Password == password);
        }
    }
}