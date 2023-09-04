using System.Collections.Generic;
using System.Threading.Tasks;
using BaceModel.ModelInspertionDriver;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using WebDispacher.ViewModels.RA.Carrier.Login;
using WebDispacher.ViewModels.Settings;

namespace WebDispacher.Business.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUser(User model);
        Task SendEmailSuccessCarrierRegistration(string email);
        User GetUserById(string id);
        Task<User> GetFirstUserByCompanyId(int id);
        Task SetLastLoginDateToUser(LoginViewModel model, string localDate);
        Task<bool> CreatePasswordResets(User user, string code, string localDate);
        Company GetCompanyById(string companyId);
        Task<string> GetFirstUserEmailByCompanyId(int id);

        bool CheckPermissions(string key, string idCompany, string route);
        Task<List<User>> GetUsers(int page);
        User GetUserByEmailAndPasswrod(string email, string password);
        bool IsPermission(string key, string idCompany, string route);
        //Company GetUserByKeyUser(int key);
        User GetUserByKey(string key);
        List<Layouts> GetLayoutsByTransportVehicle(ITransportVehicle transportVehicle);
        Task<bool> SendRecoveryPasswordToEmail(string email, string url);
        bool CheckEmailDb(string email);
        Task<int> ResetPasswordFoUser(string newPassword, string idUser, string token);
        void CreateUserForCompanyId(int id, string emailCompany, string password);
        void EditUser(SettingsUserViewModel user);
        void AddUser(string idCompany, SettingsUserViewModel user);
        void RemoveUserById(string idUser);
        bool Authorization(string login, string password);
        int CreateKey(string login, string password);
        bool CheckKey(string key);
        int CheckTokenFoUser(string idUser, string token);
    }
}