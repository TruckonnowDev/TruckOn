using System.Collections.Generic;
using System.Threading.Tasks;
using DaoModels.DAO.Models;

namespace WebDispacher.Business.Interfaces
{
    public interface IOrderService
    {
        void DeleteOrder(string id);
        void ArchiveOrder(string id);
        Task<Shipping> AddNewOrder(string urlPage, Dispatcher dispatcher);

        void SaveVechi(string idVech, string vin, string year, string make, string model, string type, string color,
            string lotNumber);

        void AddHistory(string key, string idCompany, string idOrder, string idVech, string idDriver, string action);

        string GetStrAction(string key, string idCompany, string idOrder, string idVech, string idDriver,
            string action);

        void RemoveVechi(string idVech);
        Task<VehiclwInformation> AddVechi(string idOrder);
        Task<Shipping> CreateShipping();
        Shipping GetShippingCurrentVehiclwIn(string id);
        void Assign(string idOrder, string idDriver);
        void Unassign(string idOrder);
        void Solved(string idOrder);
        Task<int> GetCountPage(string status, string name, string address, string phone, string email, string price);

        Task<List<Shipping>> GetOrders(string status, int page, string name, string address, string phone, string email,
            string price);

        Shipping GetOrder(string id);

        void UpdateOrder(string idOrder, string idLoad, string internalLoadID, string driver, string status,
            string instructions, string nameP, string contactP,
            string addressP, string cityP, string stateP, string zipP, string phoneP, string emailP,
            string scheduledPickupDateP, string nameD, string contactD, string addressD,
            string cityD, string stateD, string zipD, string phoneD, string emailD, string ScheduledPickupDateD,
            string paymentMethod, string price, string paymentTerms, string brokerFee,
            string contactId, string phoneC, string faxC, string iccmcC);

        void SavePath(string id, string path);
        Task<string> GetDocument(string id);
        void RemoveDoc(string idDock);
        bool SendRemindInspection(int idDriver);
        List<HistoryOrder> GetHistoryOrder(string idOrder);
    }
}