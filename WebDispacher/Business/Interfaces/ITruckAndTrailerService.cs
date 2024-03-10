using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.DTO;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Http;
using WebDispacher.Models;
using WebDispacher.ViewModels.Trailer;
using WebDispacher.ViewModels.Truck;
using WebDispacher.ViewModels.Widget;

namespace WebDispacher.Business.Interfaces
{
    public interface ITruckAndTrailerService
    {
        Task<List<Truck>> GetTruckWithApproachingPlateExpiration(string companyId, int countDays);
        Task<List<Trailer>> GetTrailerWithApproachingPlateExpiration(string companyId, int countDays);
        Task<List<Truck>> GetTruckWithApproachingAnnualInspectionExpiration(string companyId, int countDays);
        Task<List<Trailer>> GetTrailerWithApproachingAnnualInspectionExpiration(string companyId, int countDays);
        Task RemoveTruckWidget(int entryId, string companyId);
        Task RemoveTrailerWidget(int entryId, string companyId);
        Task<List<TruckStatusTheme>> GetAvailableTruckStatusThemes();
        Task<List<TrailerStatusTheme>> GetAvailableTrailerStatusThemes();
        Task<List<TruckStatusDropdownVm>> GetTruckStatusWithoutWidgetsWithCurrentDropdownItems(int currentItemId, string companyId);
        Task<List<TrailerStatusDropdownVm>> GetTrailerStatusWithoutWidgetsWithCurrentDropdownItems(int currentItemId, string companyId);
        Task<List<TruckStatusDropdownVm>> GetTruckStatusWithoutWidgetsDropdownItems(string companyId);
        Task<List<TrailerStatusDropdownVm>> GetTrailerStatusWithoutWidgetsDropdownItems(string companyId);
        Task UpdateTrailerWidgetInCompany(WidgetViewModel model, string dateTimeLocal, string companyId);
        Task UpdateTruckWidgetInCompany(WidgetViewModel model, string dateTimeLocal, string companyId);
        Task AddTrailerWidgetInCompany(CreateWidgetVm model, string dateTimeLocal, string companyId);
        Task AddTruckWidgetInCompany(CreateWidgetVm model, string dateTimeLocal, string companyId);
        Task<WidgetViewModel> GetTruckWidgetById(int widgetId, string companyId);
        Task<WidgetViewModel> GetTrailerWidgetById(int widgetId, string companyId);
        Task<List<TruckStatusWidget>> GetCurrentCompanyTruckWidgets(string companyId);
        Task<List<TrailerStatusWidget>> GetCurrentCompanyTrailerWidgets(string companyId);
        Task<Truck> GetTruck(string idDriver);
        Task<(Dictionary<TruckGroup, List<Truck>>, int)> GetTrucks(TruckFiltersViewModel filters, string companyId);
        int GetCountTrucksPagesByCountEntites(int countEntites);
        Task<List<DocumentTruck>> GetTruckDoc(int id);
        Task<Dictionary<string, string>> GetBaseTruckDoc(string id);
        Task<Truck> GetTruckByPlate(string truckPlate);
        Task<TruckViewModel> GetTruckById(int truckId);
        Task<Trailer> GetTrailer(string idDriver);
        Task<bool> SaveTruckLocation(int truckId, string truckLocation, string companyId);
        Task<bool> SaveTrailerLocation(int trailerId, string trailerLocation, string companyId);
        Task<bool> SaveTruckStatus(int truckId, int truckStatusId, string companyId);
        Task<bool> SaveTrailerStatus(int trailerId, int trailerStatusId, string companyId);
        TrailerViewModel GetTrailerById(int idTrailer);
        Task<List<TruckType>> GetTruckTypes(string category);
        Task<Trailer> GetTrailerByPlate(string trailerPlate);
        Task<(Dictionary<TrailerGroup, List<Trailer>>, int)> GetTrailers(TrailerFiltersViewModel filters, string companyId);
        int GetCountTrailersPagesByCountEntites(int countEntites);
        Task RemoveTrailer(int id);
        Task<List<VehicleCategory>> GetTruckVehicleCategiries();
        Task<List<VehicleCategory>> GetTrailerVehicleCategiries();
        Task<ITr> GetTr(int idTr, string typeTransport);
        Task<List<DocumentTrailer>> GetTrailerDocsById(int id);
        List<DriverInspection> GetInspectionTrucks(string idDriver, string idTruck, string idTrailer, string date);
        Task RemoveTruck(int id);
        Task SaveDocTrailer(IFormFile uploadedFile, string nameDoc, int trailerId, string dateTimeUpload);
        Task CreateTrailer(TrailerViewModel trailer, string idCompany,
            IFormFile trailerRegistrationDoc, IFormFile trailerAnnualInspectionDoc, IFormFile leaseAgreementDoc, string dateTimeLocal);
        string GetTypeTransport(int idTr, string typeTransport);
        Task RemoveDocTrailer(int docId);
        Task RemoveDocTruck(int docId);

        Task CreateTruck(TruckViewModel truck, string idCompany, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string dateTimeLocal);

        Task EditTruck(TruckViewModel model, IFormFile truckRegistrationDoc,
            IFormFile truckLeaseAgreementDoc, IFormFile truckAnnualInspection, IFormFile bobTailPhysicalDamage,
            IFormFile nYHUTDoc, string companyId, string localDate);

        Task EditTrailer(TrailerViewModel model, string companyId, string localDate);

        Task<int> AddProfile(string idCompany, int idTr, string typeTransport);
        Task SaveDocTruck(IFormFile uploadedFile, string nameDoc, int truckId, string dateTimeUpload);

        Task<string> GetTruckTypeSlugByName(string truckName);
        Task AddTruckGroupInCompany(string groupName, string dateTimeLocalTruck, string companyId);
        Task AddTrailerGroupInCompany(string groupName, string dateTimeLocalTruck, string companyId);
        Task<bool> CheckTruckHaveGroup(string companyId);
        Task<bool> CheckTrailerHaveGroup(string companyId);
        Task<List<TruckGroupDropdownVm>> GetTruckGroupsDropdownItems(string companyId);
        Task<List<TruckStatusDropdownVm>> GetTruckStatusDropdownItems(string companyId);
        Task<List<TrailerStatusDropdownVm>> GetTrailerStatusDropdownItems(string companyId);
        Task<List<TrailerGroupDropdownVm>> GetTrailerGroupsDropdownItems(string companyId);
        Task AddCustomTruckStatusInCompany(CreateTruckStatusVm model, string dateTimeLocalTruck, string companyId);
        Task AddCustomTrailerStatusInCompany(CreateTrailerStatusVm model, string dateTimeLocalTrailer, string companyId);
        Task<List<TruckGroup>> GetActualTruckGroupByCompanyId(string companyId);
        Task<List<TrailerGroup>> GetActualTrailerGroupByCompanyId(string companyId);
        Task<(TruckGroup, List<Truck>)> GetTrucksInGroupByFilters(TruckFilterViewModel filters, string companyId);
        Task<(TrailerGroup, List<Trailer>)> GetTrailerInGroupByFilters(TrailerFilterViewModel filters, string companyId);
        Task<bool> RemoveTruckGroupByCompanyId(int id);
        Task<bool> RemoveTrailerGroupByCompanyId(int id);
        Task<bool> RenameTruckGroupByCompanyId(int id, string name, string companyId);
        Task<bool> RenameTrailerGroupByCompanyId(int id, string name, string companyId);
        Task<string> GetTrailerTypeSlugByName(string trailerName);
        Task<List<TrailerType>> GetTrailerTypes(string category);
        Task<bool> IsVisibleTruckLocation(LocationType selectedLocationType);
        Task<bool> IsVisibleTrailerLocation(LocationType selectedLocationType);
        Task UpdateTruckAnnualInspectionWithDoc(int entryId, DateTime docExp, IFormFile annualInspection);
        Task UpdateTruckPlateWithDoc(int entryId, DateTime docExp, IFormFile plate);
        Task UpdateTrailerPlateWithDoc(int entryId, DateTime docExp, IFormFile plate);
        Task UpdateTrailerAnnualInspectionWithDoc(int entryId, DateTime docExp, IFormFile annualInspection);
    }
}