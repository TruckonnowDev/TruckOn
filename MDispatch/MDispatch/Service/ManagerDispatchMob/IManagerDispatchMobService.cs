using MDispatch.Models;
using MDispatch.Models.Enum;
using MDispatch.Models.ModelDataBase;
using MDispatch.Vidget.VM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDispatch.Service.ManagerDispatchMob
{
    public interface IManagerDispatchMobService
    {
        int DriverWork(string typeDriver, string token, ref string description, ref bool isInspection, ref int indexPhoto, ref TruckCar truckCar);
        int DriverWork(string typeDriver, string token, ref string description, string idDriver, InspectionDriver inspectionDriver = null);
        int GetLastInspaction(string token, string idDriver, ref string latsInspection, ref string plateTruck, ref string plateTrailer, ref string description);
        Task<FolderOffline> GetPhotoInspectionByOptinsInDB(string idShip, string id, FolderOflineType folderOflineType, InspactionType inspactionType, int index);
        Task DeleteFolderOfflinesById(int idFolderOffline);
        int A_RWork(string typeR_A, string login, string password, ref string description, ref string token);
        int OrderWork(string typeOrder, string token, ref string description, ref List<Shipping> shippings);
        int InspectionStatus(string token, string idShipping, string statusInspection, ref string description, ref Shipping shipping);
        int OrderOneWork(string typeOrder, string id, string token, string idOrder, string name, string contactName, string address,
            string city, string state, string zip, string phone, string email, string typeSave, ref string description);
        int OrderOneWork(string typeOrder, string id, string token, string typeSave, string payment, string paymentTeams, ref string description);
        int OrderWork(string typeOrder, int idVech, ref VehiclwInformation vehiclwInformation, string token, ref string description);
        int Recurent(string token, string id, string status, ref string description);
        int AskWork(string typeInspection, string token, string id, object obj, ref string description, string idShiping = null, int indexPhoto = 0, string typeTransportVehicle = null);
        int SetPlate(string token, string plateTruck, string plateTrailer, string nowCheck, ref string description, ref bool isPlate, ref TruckCar truckCar);
        int GetShipping(string token, string id, ref string description, ref Shipping shipping);
        int CheckPlate(string token, ref string description, ref string plateTruckAndTrailer);
        int GetShippingPhoto(string token, string id, ref string description, ref Shipping shipping);
        int SavePay(string typeReqvest, string token, string idShiping, int type, object obj, ref string description);
        int SaveMethodPay(string token, string idShiping, string payMethod, string countPay, ref string description);
        int CheckProblem(string token, string idShiping, ref bool isProplem);
        int SetProblem(string token, string idShiping);
        int SetInstaraction(string token, string idShiping, ref string description);
        int DetectPlate(string token, string image, string idDriver, string type, string plate);
        Task AddPhotoInspection(FolderOffline folder);
    }
}
