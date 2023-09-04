using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using WebDispacher.Models;
using WebDispacher.ViewModels.Driver;

namespace WebDispacher.Business.Interfaces
{
    public interface IDriverService
    {
        Task<List<DriverReportViewModel>> GetDriverReportsByCompnayId(DriverSearchViewModel model, string companyId);
        List<Driver> GetDriversByIdCompany(string companyId);
        Task RemoveDriver(string companyId, DriverReportModel model, string localDate);
        int CheckTokenFoDriver(string idDriver, string token);
        DriverInspection GetInspectionTruck(string idInspection);
        Task RemoveDriversByCompanyId(string companyId, string localDate);

        Task EditDriver(EditDriverViewModel model, string localDate);

        Task CreateDriver(CreateDriverViewModel model,
            IFormFile dLDoc, IFormFile medicalCardDoc, IFormFile sSNDoc, IFormFile proofOfWorkAuthorizationOrGCDoc,
            IFormFile dQLDoc, IFormFile contractDoc, IFormFile drugTestResultsDo, string dateTimeLocal);

        Driver GetDriver(string idInspection);
        Driver GetDriverById(int id);
        Task<EditDriverViewModel> GetEditCompanyDriverById(string companyId, int id);
        Task RemoveDocDriver(int docId);
        Task<List<Driver>> GetDriversByCompanyId(int page, string companyId);
        Task<List<Driver>> GetDriversByCompanyId(string companyId);
        Task<int> GetCountDriversPages(string companyId);

        Task AddNewReportDriver(DriverReportViewModel driverReport, string localDate);

        Task<List<DriverReport>> GetDriversReport(string nameDriver, string driversLicense, string companyId);
        int CheckReportDriver(string fullName, string driversLicenseNumber);
        Task<List<DocumentDriver>> GetDriverDoc(int id);
        Task<int> ResetPasswordFoDriver(string newPassword, string idDriver, string token);
        void LayoutDown(int idLayout, int idTransported);
        void UnSelectLayout(int idLayout);
        void SelectProfile(int idProfile, string typeTransport, int idTr, string idCompany);
        void LayoutUp(int idLayout, int idTransported);
        Task RemoveProfile(string idCompany, int idProfile);
        void SelectLayout(int idLayout);
        Task<ProfileSettingsDTO> GetSelectSettingTruck(string idCompany, int idProfile, int idTr, string typeTransport);
        Task<List<ProfileSettingsDTO>> GetSettingsTruck(string idCompany, int idProfile, int idTr, string typeTransport);
        Task<bool> RemoveCompany(string companyId);
        Task SaveDocDriver(IFormFile uploadedFile, string nameDoc, int driverId, string dateTimeUpload);
    }
}