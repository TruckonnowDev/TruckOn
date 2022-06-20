using MDispatch.Models;
using System.Collections.Generic;

namespace MDispatch.Service.OrderGet
{
    public interface IOrderGetService
    {
        int ActiveOreder(string token, ref string description, ref List<Shipping> shippings);
        int DelyveryOreder(string token, ref string description, ref List<Shipping> shippings);
        int ArchiveOreder(string token, ref string description, ref List<Shipping> shippings);
        int GetVehiclwInformation(string token, int idVech, ref string description, ref VehiclwInformation vehiclwInformation);
        int Save(string token, string id, string idOrder, string name, string contactName, string address,
            string city, string state, string zip, string phone, string email, string typeSave, ref string description);
        int Save(string token, string id, string typeSave, string payment, string paymentTeams, ref string description);
    }
}
