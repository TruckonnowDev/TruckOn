using System.Threading.Tasks;

namespace WebDispacher.Business.Interfaces
{
    public interface ISeedDatabaseService
    {
        Task CreateStartRole();
        Task CreateStartAdmin();
        Task CreateStartTestCompany();
        Task CreateBasicOrderStatuses();
        Task CreateVehicleInfo();
        Task CreateDispatcherType();
        Task CreateEntityTypesResetPasswords();
        Task CreateEntityPhotoTypes();
        Task InitVehiclesCategories();
        Task InitTrucksTypesInVehicliesCategories();
        Task InitTrailersTypesInVehicliesCategories();
        Task InitSlugsByTruckTypesNames();
        Task InitResourcesItems();
        Task InitColors();
        Task InitTruckThemesDefault();
        Task InitTruckStatuses();
        Task InitTrailerThemesDefault();
        Task InitTrailerStatuses();
        Task InitSlugsByTrailerTypesNames();
    }
}
