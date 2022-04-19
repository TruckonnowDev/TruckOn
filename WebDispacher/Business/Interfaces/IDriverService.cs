using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.AspNetCore.Http;
using WebDispacher.Models;

namespace WebDispacher.Business.Interfaces
{
    public interface IDriverService
    {
        List<Driver> GetDriversByIdCompany(string idCompany);
        void RemoveDrive(string idCompany, DriverReportModel model);
        int CheckTokenFoDriver(string idDriver, string token);
        InspectionDriver GetInspectionTruck(string idInspection);

        void EditDrive(int id, string fullName, string emailAddress,
            string password, string phoneNumber, string trailerCapacity, string driversLicenseNumber);

        void CreateDriver(string fullName, string emailAddress, string password, string phoneNumbe,
            string trailerCapacity, string driversLicenseNumber, string idCompany,
            IFormFile dLDoc, IFormFile medicalCardDoc, IFormFile sSNDoc, IFormFile proofOfWorkAuthorizationOrGCDoc,
            IFormFile dQLDoc, IFormFile contractDoc, IFormFile drugTestResultsDo);

        Driver GetDriver(string idInspection);
        Driver GetDriverById(int id);
        void RemoveDocDriver(string idDock);
        List<Driver> GetDrivers(int pag, string idCompany);
        Task<List<Driver>> GetDrivers(string idCompany);

        void AddNewReportDriver(string fullName, string driversLicenseNumber, string numberOfAccidents, string english,
            string returnedEquipmen, string workingEfficiency, string eldKnowledge, string drivingSkills,
            string paymentHandling, string alcoholTendency, string drugTendency, string terminated, string experience,
            string dotViolations, string description);

        List<DriverReport> GetDriversReport(string nameDriver, string driversLicense);
        int CheckReportDriver(string fullName, string driversLicenseNumber);
        Task<List<DucumentDriver>> GetDriverDoc(string id);
        Task<int> ResetPasswordFoDriver(string newPassword, string idDriver, string token);
        void LayoutDown(int idLayout, int idTransported);
        void UnSelectLayout(int idLayout);
        void SelectProfile(int idProfile, string typeTransport, int idTr, string idCompany);
        void LayoutUp(int idLayout, int idTransported);
        void RemoveProfile(string idCompany, int idProfile);
        void SelectLayout(int idLayout);
        ProfileSettingsDTO GetSelectSetingTruck(string idCompany, int idProfile, int idTr, string typeTransport);
        List<ProfileSettingsDTO> GetSetingsTruck(string idCompany, int idProfile, int idTr, string typeTransport);
        void RemoveCompany(string idCompany);
        Task SaveDocDriver(IFormFile uploadedFile, string nameDoc, string id);
    }
}