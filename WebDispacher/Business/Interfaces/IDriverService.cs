using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using DaoModels.DAO.Models.Settings;
using Microsoft.AspNetCore.Http;
using WebDispacher.Models;
using WebDispacher.ViewModels.Driver;

namespace WebDispacher.Business.Interfaces
{
    public interface IDriverService
    {
        Task<List<DriverReportViewModel>> GetDriverReportsByCompnayId(DriverSearchViewModel model, string companyId);
        List<Driver> GetDriversByIdCompany(string idCompany);
        void RemoveDrive(string idCompany, DriverReportModel model, string localDate);
        int CheckTokenFoDriver(string idDriver, string token);
        InspectionDriver GetInspectionTruck(string idInspection);

        void EditDriver(DriverViewModel driver);

        Task CreateDriver(DriverViewModel driver,
            IFormFile dLDoc, IFormFile medicalCardDoc, IFormFile sSNDoc, IFormFile proofOfWorkAuthorizationOrGCDoc,
            IFormFile dQLDoc, IFormFile contractDoc, IFormFile drugTestResultsDo, string dateTimeLocal);

        Driver GetDriver(string idInspection);
        Driver GetDriverById(int id);
        DriverViewModel GetDriverByIdViewModel(int id);
        void RemoveDocDriver(string idDock);
        Task<List<Driver>> GetDrivers(int page, string idCompany);
        Task<int> GetCountDriversPages(string idCompany);
        Task<List<Driver>> GetDrivers(string idCompany);

        void AddNewReportDriver(DriverReportViewModel driverReport);

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
        Task RemoveCompany(string idCompany);
        Task SaveDocDriver(IFormFile uploadedFile, string nameDoc, string id);
    }
}