using System.Collections.Generic;
using System.Threading.Tasks;
using BaceModel.ModelInspertionDriver;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;

namespace WebDispacher.Business.Interfaces
{
    public interface IUserService
    {
        Users GetUserByEmailAndPasswrod(string email, string password);
        bool IsPermission(string key, string idCompany, string route);
        Commpany GetUserByKeyUser(int key);
        Users GetUserByKey(string key);
        List<Layouts> GetLayoutsByTransportVehicle(ITransportVehicle transportVehicle);
        bool CheckEmail(string email);
        Task<int> ResetPasswordFoUser(string newPassword, string idUser, string token);
        void CreateUserForCompanyId(int id, string nameCompany, string password);
        void AddUser(string idCompany, string login, string password);
        void RemoveUserById(string idUser);
        bool Authorization(string login, string password);
        int CreateKey(string login, string password);
        bool CheckKey(string key);
        Commpany GetCompanyById(string idCompany);
        int CheckTokenFoUser(string idUser, string token);
    }
}