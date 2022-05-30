using MDispatch.Models;
using MDispatch.Models.Enum;
using MDispatch.Models.ModelDataBase;
using MDispatch.Service.Auth;
using MDispatch.Service.DatabaseContext;
using MDispatch.Service.DriverInspection;
using MDispatch.Service.GoogleApi;
using MDispatch.Service.Inspection;
using MDispatch.Service.OrderGet;
using MDispatch.Vidget.VM;
using Plugin.Connectivity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.Service.ManagerDispatchMob
{
    public class ManagerDispatchMobService : IManagerDispatchMobService
    {
        public delegate void InitDasbordDelegate();
        public delegate List<VehiclwInformation> GetVechicleDelegate();
        public delegate Shipping GetShiping();
        private Photo photo = null;
        private int CountReqvest = 0;
        private readonly IAuth _auth;
        private readonly IDatabaseContextService _dataBaseContext;
        private readonly IDriverInspectionService _driverInspection;
        private readonly IGoogleApiService _googleApi;
        private readonly IOrderGetService _orderGet;
        private readonly IInspectionService _inspection;

        public ManagerDispatchMobService()
        {
            _orderGet = DependencyService.Get<IOrderGetService>();
            _auth = DependencyService.Get<IAuth>();
            _dataBaseContext = DependencyService.Get<IDatabaseContextService>();
            _driverInspection = DependencyService.Get<IDriverInspectionService>();
            _googleApi = DependencyService.Get<IGoogleApiService>();
            _inspection = DependencyService.Get<IInspectionService>();
        }

        public int DriverWork(string typeDriver, string token, ref string description, ref bool isInspection, ref int indexPhoto, ref TruckCar truckCar)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateDriver = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeDriver == "CheckInspeacktion")
                {
                    stateDriver = _driverInspection.CheckInspectionDriver(token, ref description, ref isInspection, ref indexPhoto, ref truckCar);
                }
            }
            CountReqvest--;
            return stateDriver;
        }

        public int DriverWork(string typeDriver, string token, ref string description, string idDriver, InspectionDriver inspectionDriver = null)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateDriver = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeDriver == "SetInspectionDriver")
                {
                    stateDriver = _driverInspection.SetInspectionDriver(token, ref description, inspectionDriver, idDriver);
                }
                else if (typeDriver == "UpdateInspectionDriver")
                {
                    stateDriver = _driverInspection.UpdateInspectionDriver(token, ref description, idDriver);
                }
            }
            CountReqvest--;
            return stateDriver;
        }

        public int GetLastInspaction(string token, string idDriver, ref string latsInspection, ref string plateTruck, ref string plateTrailer, ref string description)
        {
            //CountReqvest++;
            int statePay = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                statePay = _driverInspection.GetLastInspaction(token, idDriver, ref latsInspection, ref plateTruck, ref plateTrailer, ref description);
            }
            CountReqvest--;
            return statePay;
        }

        public async Task<FolderOffline> GetPhotoInspectionByOptinsInDB(string idShip, string id, FolderOflineType folderOflineType, InspactionType inspactionType, int index)
        {
            List<FolderOffline> folderOfflines = await _dataBaseContext.GetFolderOfflines();
            return folderOfflines.FirstOrDefault(f => f.IdShiping == idShip && f.IdVech == id && f.Index == index && f.FolderOflineType == folderOflineType && f.InspactionType == inspactionType);
        }

        public async Task DeleteFolderOfflinesById(int idFolderOffline)
        {
            await _dataBaseContext.DeleteFolderOfflines(idFolderOffline);
        }

        public int A_RWork(string typeR_A, string login, string password, ref string description, ref string token)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateA_R = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeR_A == "authorisation")
                {
                    stateA_R = _auth.Avthorization(login, password, ref description, ref token);
                }
                else if (typeR_A == "Clear")
                {
                    stateA_R = _auth.ClearAvt(token);
                }
                else if (typeR_A == "RequestPasswordChanges")
                {
                    stateA_R = _auth.RequestPasswordChanges(login, password, ref description);
                }
            }
            CountReqvest--;
            return stateA_R;
        }

        public int OrderWork(string typeOrder, string token, ref string description, ref List<Shipping> shippings)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeOrder == "OrderGet")
                {
                    stateOrder = _orderGet.ActiveOreder(token, ref description, ref shippings);
                }
                else if (typeOrder == "OrderDelyveryGet")
                {
                    stateOrder = _orderGet.DelyveryOreder(token, ref description, ref shippings);
                }
                else if (typeOrder == "OrderArchiveGet")
                {
                    stateOrder = _orderGet.ActiveOreder(token, ref description, ref shippings);
                }
            }
            CountReqvest--;
            return stateOrder;
        }

        public int InspectionStatus(string token, string idShipping, string statusInspection, ref string description, ref Shipping shipping)
        {
            //CountReqvest++;
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                stateOrder = _inspection.InspectionStatus(token, idShipping, statusInspection, ref description, ref shipping);
            }
            // CountReqvest--;
            return stateOrder;
        }

        public int OrderOneWork(string typeOrder, string id, string token, string idOrder, string name, string contactName, string address,
            string city, string state, string zip, string phone, string email, string typeSave, ref string description)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeOrder == "Save")
                {
                    stateOrder = _orderGet.Save(token, id, idOrder, name, contactName, address, city, state, zip, phone, email, typeSave, ref description);
                }
            }
            CountReqvest--;
            return stateOrder;
        }

        public int OrderOneWork(string typeOrder, string id, string token, string typeSave, string payment, string paymentTeams, ref string description)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeOrder == "Save")
                {
                    stateOrder = _orderGet.Save(token, id, typeSave, payment, paymentTeams, ref description);
                }
            }
            CountReqvest--;
            return stateOrder;
        }

        public int OrderWork(string typeOrder, int idVech, ref VehiclwInformation vehiclwInformation, string token, ref string description)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeOrder == "GetVechicleInffo")
                {
                    stateOrder = _orderGet.GetVehiclwInformation(token, idVech, ref description, ref vehiclwInformation);
                }
            }
            CountReqvest--;
            return stateOrder;
        }

        public int Recurent(string token, string id, string status, ref string description)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateInspection = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                stateInspection = _inspection.ReCurentStatus(token, id, ref description, status);
            }
            CountReqvest--;
            return stateInspection;
        }

        public int AskWork(string typeInspection, string token, string id, object obj, ref string description, string idShiping = null, int indexPhoto = 0, string typeTransportVehicle = null)
        {
            int stateInspection = 1;
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeInspection == "SaveAsk")
                {
                    stateInspection = _inspection.SaveAsk(token, id, (Models.Ask)obj, ref description);
                }
                else if (typeInspection == "SavePhoto")
                {
                    stateInspection = _inspection.SavePhoto(token, id, (Models.PhotoInspection)obj, ref description);
                }
                else if (typeInspection == "SaveAsk1")
                {
                    stateInspection = _inspection.SaveAsk(token, id, (Models.Ask1)obj, ref description);
                }
                else if (typeInspection == "SaveAsk2")
                {
                    stateInspection = _inspection.SaveAsk(token, id, (Models.Ask2)obj, ref description);
                }
                else if (typeInspection == "AskFromUser")
                {
                    stateInspection = _inspection.SaveAsk(token, id, (AskFromUser)obj, ref description);
                }
                else if (typeInspection == "FeedBack")
                {
                    stateInspection = _inspection.SaveAsk(token, (Models.Feedback)obj, ref description);
                }
                else if (typeInspection == "AskDelyvery")
                {
                    stateInspection = _inspection.SaveAsk(token, id, (Models.AskDelyvery)obj, ref description);
                }
                else if (typeInspection == "AskForUserDelyvery")
                {
                    stateInspection = _inspection.SaveAsk(token, id, (Models.AskForUserDelyveryM)obj, ref description);
                }
                else if (typeInspection == "AskPikedUpSig")
                {
                    stateInspection = _inspection.SaveSigPikedUp(token, (Photo)obj, id, ref description);
                }
                else if (typeInspection == "DamageForUser")
                {
                    stateInspection = _inspection.SaveDamageForuser(token, id, idShiping, (List<DamageForUser>)obj, ref description);
                }
                else if (typeInspection == "SaveInspactionDriver")
                {
                    stateInspection = _driverInspection.SaveInspactionDriver(token, ref description, id, (Photo)obj, indexPhoto, typeTransportVehicle);
                }
                else if (typeInspection == "SendBolMail")
                {
                    stateInspection = _inspection.SendBolEmaile(token, id, ref description, (string)obj);
                }
                else if (typeInspection == "SendCouponMail")
                {
                    stateInspection = _inspection.SendCouponEmaile(token, ref description, (string)obj);
                }
                else if (typeInspection == "SaveInTruck")
                {
                    stateInspection = _inspection.SaveInTruck(token, id, (List<Photo>)obj, ref description);
                }
                else if (typeInspection == "SaveStrap")
                {
                    stateInspection = _inspection.SaveStrap(token, id, (List<Photo>)obj, ref description);
                }
            }
            CountReqvest--;
            return stateInspection;
        }

        public int SetPlate(string token, string plateTruck, string plateTrailer, string nowCheck, ref string description, ref bool isPlate, ref TruckCar truckCar)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int statePay = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                statePay = _driverInspection.SetPlate(token, plateTruck, plateTrailer, nowCheck, ref description, ref isPlate, ref truckCar);
            }
            CountReqvest--;
            return statePay;
        }

        public int GetShipping(string token, string id, ref string description, ref Shipping shipping)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateInspection = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                stateInspection = _inspection.GetShipping(token, id, ref description, ref shipping);
            }
            CountReqvest--;
            return stateInspection;
        }

        public int CheckPlate(string token, ref string description, ref string plateTruckAndTrailer)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int statePay = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                statePay = _driverInspection.CheckPlate(token, ref description, ref plateTruckAndTrailer);
            }
            CountReqvest--;
            return statePay;
        }

        public int GetShippingPhoto(string token, string id, ref string description, ref Shipping shipping)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateInspection = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                stateInspection = _inspection.GetShippingPhoto(token, id, ref description, ref shipping);
            }
            CountReqvest--;
            return stateInspection;
        }

        public int SavePay(string typeReqvest, string token, string idShiping, int type, object obj, ref string description)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int statePay = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeReqvest == "SaveSig")
                {
                    statePay = _inspection.SavePhotPay(token, idShiping, type, (Photo)obj, ref description);
                }
                else if (typeReqvest == "SaveRecount")
                {
                    statePay = _inspection.SaveVideoRecount(token, idShiping, type, (Video)obj, ref description);
                }
            }
            CountReqvest--;
            return statePay;
        }

        public int SaveMethodPay(string token, string idShiping, string payMethod, string countPay, ref string description)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateProplem = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                stateProplem = _inspection.SaveMethodPay(token, idShiping, payMethod, countPay, ref description);
            }
            CountReqvest--;
            return stateProplem;
        }

        public int CheckProblem(string token, string idShiping, ref bool isProplem)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateProplem = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                stateProplem = _inspection.CheckProblem(token, idShiping, ref isProplem);
            }
            CountReqvest--;
            return stateProplem;
        }

        public int SetProblem(string token, string idShiping)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int statePay = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                statePay = _inspection.SetProblem(token, idShiping);
            }
            CountReqvest--;
            return statePay;
        }

        public int SetInstaraction(string token, string idShiping, ref string description)
        {
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int statePay = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                statePay = _inspection.SetInstaraction(token, idShiping, ref description);
            }
            CountReqvest--;
            return statePay;
        }

        public int DetectPlate(string token, string image, string idDriver, string type, string plate)
        {
            //CountReqvest++;
            int statePay = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                statePay = _googleApi.DetectPlate(token, image, idDriver, type, plate).Result;
            }
            CountReqvest--;
            return statePay;
        }

        private void WaiteNoramalReqvestCount()
        {
            while (CountReqvest >= 2)
            {

            }
        }

        public async Task AddPhotoInspection(FolderOffline folder)
        {
            await _dataBaseContext.AddPhotoInspection(folder);
        }
    }
}
