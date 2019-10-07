﻿using MDispatch.Models;
using Plugin.Connectivity;
using System.Collections.Generic;

namespace MDispatch.Service
{
    public class ManagerDispatchMob
    {
        public delegate void InitDasbordDelegate();
        public delegate List<VehiclwInformation> GetVechicleDelegate();
        public delegate Shipping GetShiping();
        private A_R a_R = null;
        private OrderGet orderGet = null;
        private Photo photo = null;
        private Inspection inspection = null;
        private DriverInspecktion driverInspecktion = null;
        private int CountReqvest = 0;

        public int DriverWork(string typeDriver, string token, ref string description, ref bool isInspection, ref int indexPhoto)
        {
            driverInspecktion = new DriverInspecktion();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateDriver = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeDriver == "CheckInspeacktion")
                {
                    stateDriver = driverInspecktion.CheckInspectionDriver(token, ref description, ref isInspection, ref indexPhoto);
                }
            }
            driverInspecktion = null;
            CountReqvest--;
            return stateDriver;
        }

        public int DriverWork(string typeDriver, string token, ref string description, string idDriver, InspectionDriver inspectionDriver = null)
        {
            driverInspecktion = new DriverInspecktion();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateDriver = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeDriver == "SetInspectionDriver")
                {
                    stateDriver = driverInspecktion.SetInspectionDriver(token, ref description, inspectionDriver, idDriver);
                }
                else if (typeDriver == "UpdateInspectionDriver")
                {
                    stateDriver = driverInspecktion.UpdateInspectionDriver(token, ref description, idDriver);
                }
            }
            driverInspecktion = null;
            CountReqvest--;
            return stateDriver;
        }

        public int A_RWork(string typeR_A, string login, string password, ref string description, ref string token)
        {
            a_R = new A_R();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateA_R = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeR_A == "authorisation")
                {
                    stateA_R = a_R.Avthorization(login, password, ref description, ref token);
                }
                else if (typeR_A == "Clear")
                {
                    stateA_R = a_R.ClearAvt(token);
                }
            }
            a_R = null;
            CountReqvest--;
            return stateA_R;
        }

        public int OrderWork(string typeOrder, string token, ref string description, ref List<Shipping> shippings)
        {
            orderGet = new OrderGet();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeOrder == "OrderGet")
                {
                    stateOrder = orderGet.ActiveOreder(token, ref description, ref shippings);
                }
                else if (typeOrder == "OrderDelyveryGet")
                {
                    stateOrder = orderGet.DelyveryOreder(token, ref description, ref shippings);
                }
                else if (typeOrder == "OrderArchiveGet")
                {
                    stateOrder = orderGet.ActiveOreder(token, ref description, ref shippings);
                }
            }
            orderGet = null;
            CountReqvest--;
            return stateOrder;
        }


        public int OrderOneWork(string typeOrder, string id, string token, string idOrder, string name, string contactName, string address,
            string city, string state, string zip, string phone, string email, string typeSave, ref string description)
        {
            orderGet = new OrderGet();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeOrder == "Save")
                {
                    stateOrder = orderGet.Save(token, id, idOrder, name, contactName, address, city, state, zip, phone, email, typeSave, ref description);
                }
            }
            orderGet = null;
            CountReqvest--;
            return stateOrder;
        }

        public int OrderOneWork(string typeOrder, string id, string token, string typeSave, string payment, string paymentTeams, ref string description)
        {
            orderGet = new OrderGet();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeOrder == "Save")
                {
                    stateOrder = orderGet.Save(token, id, typeSave, payment, paymentTeams, ref description);
                }
            }
            orderGet = null;
            CountReqvest--;
            return stateOrder;
        }

        public int OrderWork(string typeOrder, int idVech, ref VehiclwInformation vehiclwInformation, string token, ref string description)
        {
            orderGet = new OrderGet();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateOrder = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeOrder == "GetVechicleInffo")
                {
                    stateOrder = orderGet.GetVehiclwInformation(token, idVech, ref description, ref vehiclwInformation);
                }
            }
            orderGet = null;
            CountReqvest--;
            return stateOrder;
        }

        public int Recurent(string token, string id, string status, ref string description)
        {
            inspection = new Inspection();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateInspection = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                stateInspection = inspection.ReCurentStatus(token, id, ref description, status);
            }
            inspection = null;
            CountReqvest--;
            return stateInspection;
        }

        public int AskWork(string typeInspection, string token, string id, object obj, ref string description, string idShiping = null, int indexPhoto = 0)
        {
            int stateInspection = 1;
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeInspection == "SaveAsk")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SaveAsk(token, id, (Models.Ask)obj, ref description);
                }
                else if(typeInspection == "SavePhoto")
                {
                    inspection = new Inspection();
                    stateInspection =  inspection.SavePhoto(token, id, (Models.PhotoInspection)obj, ref description);
                }
                else if (typeInspection == "SaveAsk1")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SaveAsk(token, id, (Models.Ask1)obj, ref description);
                }
                else if (typeInspection == "SaveAsk2")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SaveAsk(token, id, (Models.Ask2)obj, ref description);
                }
                else if (typeInspection == "AskFromUser")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SaveAsk(token, id, (AskFromUser)obj, ref description);
                }
                else if(typeInspection == "FeedBack")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SaveAsk(token, (Models.Feedback)obj, ref description);
                }
                else if(typeInspection == "AskDelyvery")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SaveAsk(token, id, (Models.AskDelyvery)obj, ref description);
                }
                else if (typeInspection == "AskForUserDelyvery")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SaveAsk(token, id, (Models.AskForUserDelyveryM)obj, ref description);
                }
                else if(typeInspection == "AskPikedUpSig")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SaveSigPikedUp(token, (Photo)obj, id, ref description);
                }
                else if (typeInspection == "DamageForUser")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SaveDamageForuser(token, id, idShiping, (List<DamageForUser>)obj, ref description);
                }
                else if (typeInspection == "SaveInspactionDriver")
                {
                    driverInspecktion = new DriverInspecktion();
                    stateInspection = driverInspecktion.SaveInspactionDriver(token, ref description, id, (Photo)obj, indexPhoto);
                }
                else if(typeInspection == "SendBolMail")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SendBolEmaile(token, id, ref description, (string)obj);
                }
                else if (typeInspection == "SendCouponMail")
                {
                    inspection = new Inspection();
                    stateInspection = inspection.SendCouponEmaile(token, ref description, (string)obj);
                }
            }
            driverInspecktion = null;
            inspection = null;
            CountReqvest--;
            return stateInspection;
        }

        public int GetShipping(string token, string id, ref string description, ref Shipping shipping)
        {
            inspection = new Inspection();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateInspection = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                stateInspection = inspection.GetShipping(token, id, ref description, ref shipping);
            }
            inspection = null;
            CountReqvest--;
            return stateInspection;
        }

        public int GetShippingPhoto(string token, string id, ref string description, ref Shipping shipping)
        {
            inspection = new Inspection();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int stateInspection = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                stateInspection = inspection.GetShippingPhoto(token, id, ref description, ref shipping);
            }
            inspection = null;
            CountReqvest--;
            return stateInspection;
        }

        public int SavePay(string typeReqvest, string token, string idShiping, int type, object obj, ref string description)
        {
            inspection = new Inspection();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int statePay = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (typeReqvest == "SaveSig")
                {
                    statePay = inspection.SavePhotPay(token, idShiping, type, (Photo)obj, ref description);
                }
                else if (typeReqvest == "SaveRecount")
                {
                    statePay = inspection.SaveVideoRecount(token, idShiping, type, (Video)obj, ref description);
                }
            }
            inspection = null;
            CountReqvest--;
            return statePay;
        }

        public int SaveMethodPay(string token, string idShiping, string payMethod, string countPay, ref string description)
        {
            inspection = new Inspection();
            //WaiteNoramalReqvestCount();
            CountReqvest++;
            int statePay = 1;
            if (CrossConnectivity.Current.IsConnected)
            {
                statePay = inspection.SaveMethodPay(token, idShiping, payMethod, countPay, ref description);
            }
            inspection = null;
            CountReqvest--;
            return statePay;
        }

        private void WaiteNoramalReqvestCount()
        {
            while(CountReqvest >= 2)
            {

            }
        }
    }
}