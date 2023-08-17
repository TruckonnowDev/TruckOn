using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.Models;
using WebDispacher.ViewModels;
using WebDispacher.ViewModels.Order;
using WebDispacher.ViewModels.Vehicles;

namespace WebDispacher.Business.Interfaces
{
    public interface IOrderService
    {
        Task<OrderWithHistoryViewModel> GetOrderWithHistory(string companyId, int id);
        Task DeleteOrder(int id, string localDate);
        Task ArchiveOrder(int id, string localDate);
        Task<List<VehicleDetails>> GetVehicleDetailsByOrderId(int orderId);
        Task<int> GetOrderIdByVehicleId(int id);
        //Task<Shipping> AddNewOrder(string urlPage, Dispatcher dispatcher);

        CurrentStatus GetCurrentStatusByName(string name);
        int GetCurrentStatusIdByName(string name);

        Task SaveVechicle(int idVech, string vin, string year, string make, string model, string type, string body, string color, string lotNumber, string localDate);

        Task AddHistory(string key, string idCompany, string idOrder, string idVech, string idDriver, string action, string localDate);

        string GetStrAction(string key, string idCompany, string idOrder, string idVech, string idDriver,
            string action);

        string CreateFiltersString(string loadId, string name, string address, string phone, string email,
            string price);

        Task RemoveVechicle(int vechicleId);
        Task<Order> GetOrder(string companyId, int id);
        Task<List<string>> GetVehicleTypes(string searchName);
        Task<List<string>> GetVehicleBrands(string searchName, string vehicleType);
        Task<List<string>> GetVehicleModels(string searchName, string vehicleBrand);

        Task<VehicleDetails> AddVechicle(int id, string localDate);
        Task<Order> CreateOrder(string companyId, string localDate);
        //Order GetShippingCurrentVehiclwIn(string id);
        Task Assign(int orderId, string driverId);
        Task Unassign(int orderId);
        void Solved(string idOrder);
        Task<int> GetCountPage(string companyId, string status, string loadId, string name, string address, string phone, string email, decimal price);
        int GetCountPage(int countPage);

        Task<List<Order>> GetOrders(string companyId, string status, int page, string loadId, string name, string address, string phone, string email,
            decimal price);

        Task<Order> GetCompanyOrderById(string companyId, int id);
        Task<EditOrderViewModel> GetEditCompanyOrderById(string companyId, int id);

        Task<EditOrderViewModel> UpdateOrder(EditOrderViewModel model, string localDate);

        void SavePath(string id, string path);
        Task<string> GetDocument(string id);
        bool SendRemindInspection(int driverId);
        List<HistoryOrderAction> GetHistoryOrder(int orderId);
    }
}