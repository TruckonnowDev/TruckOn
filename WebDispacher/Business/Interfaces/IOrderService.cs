using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.Models;
using WebDispacher.ViewModels;

namespace WebDispacher.Business.Interfaces
{
    public interface IOrderService
    {
        Task DeleteOrder(string id);
        Task ArchiveOrder(string id);
        Task<Shipping> AddNewOrder(string urlPage, Dispatcher dispatcher);

        Task SaveVechi(string idVech, string vin, string year, string make, string model, string type, string color,
            string lotNumber);

        void AddHistory(string key, string idCompany, string idOrder, string idVech, string idDriver, string action);

        string GetStrAction(string key, string idCompany, string idOrder, string idVech, string idDriver,
            string action);

        void RemoveVechi(string idVech);
        Task<VehiclwInformation> AddVechi(string idOrder);
        Task<Shipping> CreateShipping();
        Shipping GetShippingCurrentVehiclwIn(string id);
        Task Assign(string idOrder, string idDriver);
        Task Unassign(string idOrder);
        void Solved(string idOrder);
        Task<int> GetCountPage(string status, string name, string address, string phone, string email, string price);

        Task<List<Shipping>> GetOrders(string status, int page, string name, string address, string phone, string email,
            string price);

        ShippingViewModel GetOrder(string id);

        void UpdateOrder(ShippingViewModel shipping);

        void SavePath(string id, string path);
        Task<string> GetDocument(string id);
        void RemoveDoc(string idDock);
        bool SendRemindInspection(int idDriver);
        List<HistoryOrder> GetHistoryOrder(string idOrder);
    }
}