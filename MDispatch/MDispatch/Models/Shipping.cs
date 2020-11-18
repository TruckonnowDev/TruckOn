﻿using System.Collections.Generic;

namespace MDispatch.Models
{
    public class Shipping
    {
        public string Id { get; set; }
        public string idOrder { get; set; }
        public string InternalLoadID { get; set; }
        public string Driver { get; set; }
        public string CurrentStatus { get; set; }
        public string LastUpdated { get; set; }
        public string CDReference { get; set; }
        public string UrlReqvest { get; set; }

        //////////////////////////ORDER INFORMATION

        public string DispatchDate { get; set; }
        public string PickupExactly { get; set; }
        public string DeliveryEstimated { get; set; }
        public string ShipVia { get; set; }
        public string Condition { get; set; }
        public string PriceListed { get; set; }
        public string TotalPaymentToCarrier { get; set; }
        public string OnDeliveryToCarrier { get; set; }
        public string CompanyOwesCarrier { get; set; }
        public string Description { get; set; }
        public string BrokerFee { get; set; }

        /////////////////////////CONTACT INFORMATION

        public string ContactC { get; set; }
        public string PhoneC { get; set; }
        public string FaxC { get; set; }
        public string IccmcC { get; set; }

        /////////////////////////VEHICLE INFORMATION

        public string YearV { get; set; }
        public string MakeV { get; set; }
        public string ModelV { get; set; }
        public string TypeV { get; set; }
        public string ColorV { get; set; }
        public string PlateV { get; set; }
        public string VIN_V { get; set; }
        public string LotV { get; set; }
        public string AdditionalInfoV { get; set; }

        /////////////////////////PICKUP INFORMATION

        public string NameP { get; set; }
        public string ContactNameP { get; set; }
        public string AddresP { get; set; }
        public string StateP { get; set; }
        public string ZipP { get; set; }
        public string CityP { get; set; }
        public string PhoneP { get; set; }
        public string EmailP { get; set; }

        /////////////////////////DELIVERY INFORMATION

        public string NameD { get; set; }
        public string ContactNameD { get; set; }
        public string AddresD { get; set; }
        public string StateD { get; set; }
        public string ZipD { get; set; }
        public string CityD { get; set; }
        public string PhoneD { get; set; }
        public string EmailD { get; set; }

        /////////////////////////DISPATCH INSTRUCTIONS

        private string titl1DI = "";
        public string Titl1DI
        {
            get { return titl1DI; }
            set { titl1DI = value == null || value == "" ? "Not instructions" : value; }
        }
        public List<VehiclwInformation> VehiclwInformations { get; set; }

        ////////////////////////////////////////////////

        public Driver Driverr { get; set; }

        /////////////////////////////////////////////

        public AskFromUser AskFromUser { get; set; }
        public Ask2 Ask2 { get; set; }
        public AskForUserDelyveryM askForUserDelyveryM { get; set; }
        public List<DamageForUser> DamageForUsers { get; set; }
        public bool IsProblem { get; set; }
        ///////////////////////////////////////////////

        public string ColorCurrentStatus
        {
            get
            {
                string color = null;
                if (CurrentStatus == "Assigned")
                {
                    color = "#2C5DEB";
                }
                else if (CurrentStatus == "Picked up")
                {
                    color = "#FF9314";
                }
                else if (CurrentStatus == "Delivered,Billed" || CurrentStatus == "Delivered,Paid")
                {
                    color = "#69EB2C";
                }
                return color;
            }
        }

        public string ColorOpacityCurrentStatus
        {
            get
            {
                string color = null;
                if (CurrentStatus == "Assigned")
                {
                    color = "#eef2fd";
                }
                else if (CurrentStatus == "Picked up")
                {
                    color = "#fff6ec";
                }
                else if (CurrentStatus == "Delivered,Billed" || CurrentStatus == "Delivered,Paid")
                {
                    color = "#f3fdee";
                }
                return color;
            }
        }

        public VehiclwInformation VehiclwInformation1
        {
            get
            {
                VehiclwInformation VehiclwInformation = null;
                if (VehiclwInformations.Count > 0)
                {
                    VehiclwInformation = VehiclwInformations[0];
                }
                else
                {
                    VehiclwInformation = new VehiclwInformation();
                }
                return VehiclwInformation;
            }
        }
        public VehiclwInformation VehiclwInformation2
        {
            get
            {
                VehiclwInformation VehiclwInformation = null;
                if (VehiclwInformations.Count > 1)
                {
                    VehiclwInformation = VehiclwInformations[1];
                }
                else
                {
                    VehiclwInformation = new VehiclwInformation();
                }
                return VehiclwInformation;
            }
        }
        public int CountVehiclw
        {
            get => VehiclwInformations != null && VehiclwInformations.Count != 0 ? VehiclwInformations.Count - 2 : 0;
        }
        public bool IsVehiclw1
        {
            get => VehiclwInformations != null && VehiclwInformations.Count != 0 && VehiclwInformations[0] != null;
        }
        public bool IsVehiclw2
        {
            get
            {
                bool isVehiclw = false;
                if(VehiclwInformations.Count > 1)
                {
                    isVehiclw = true;
                }
                return isVehiclw;
            }
        }
        public bool IsVehiclw3
        {
            get
            {
                bool isVehiclw = false;
                if (VehiclwInformations.Count > 2)
                {
                    isVehiclw = true;
                }
                return isVehiclw;
            }
        }

        public string IcoStatus
        {
            get
            {
                string ico = "";
                if(CurrentStatus == "Assigned")
                {
                    ico = "newOrder.png";
                }
                else if(CurrentStatus == "Picked up")
                {
                    ico = "pickedUpOrder1.png";
                }
                else if(CurrentStatus == "Delivered,Billed" || CurrentStatus == "Delivered,Paid")
                {
                    ico = "deliveredOrder.png";
                }
                return ico;
            }
        }

        public string IcoViewStatus
        {
            get
            {
                string ico = "";
                if (CurrentStatus == "Assigned")
                {
                    ico = "newViewOrder.png";
                }
                else if (CurrentStatus == "Picked up")
                {
                    ico = "pickedUpViewOrder.png";
                }
                else if (CurrentStatus == "Delivered,Billed" || CurrentStatus == "Delivered,Paid")
                {
                    ico = "deliveredViewOrder.png";
                }
                return ico;
            }
        }

        public bool IsStartInspection
        {
            get
            {
                bool isStartInspection = false;
                if (VehiclwInformations != null && VehiclwInformations.Count != 0)
                {
                    if (CurrentStatus == "Assigned")
                    {
                        if (VehiclwInformations[0].Ask != null)
                        {
                            isStartInspection = true;
                        }
                    }
                    else if (CurrentStatus == "Picked up")
                    {
                        if (VehiclwInformations[0].AskDelyvery != null)
                        {
                            isStartInspection = true;
                        }
                    }
                }
                else
                {
                    isStartInspection = true;
                }
                return isStartInspection;
            }
        }
    }
}