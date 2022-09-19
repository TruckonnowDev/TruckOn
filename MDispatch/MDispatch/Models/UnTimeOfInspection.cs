using MDispatch.Helpers;
using MDispatch.Service;
using MDispatch.Vidget.VM;
using Plugin.Settings;
using Prism.Mvvm;
using System;

namespace MDispatch.Models
{
    public class UnTimeOfInspection : BindableBase
    {
        public bool IsInspection { get; private set; } = false;
        public bool ISMaybiInspection { get; set; }
        public string TimeOfInspection { get; private set; }
        public string IdDriver { get; private set; }

        public UnTimeOfInspection(string statusInspection = null)
        {
            bool IsInspectionDriver;
            bool IsInspectionToDayDriver;
            if (statusInspection != null)
            {
                string[] arrData = statusInspection.Split(',');
                IsInspectionDriver = Convert.ToBoolean(arrData[0]);
                IsInspectionToDayDriver = Convert.ToBoolean(arrData[1]);
                TimeOfInspection = arrData[2] + $" {LanguageHelper.HoursText}";
                IdDriver = arrData[3];
                if (IsInspectionDriver)
                {
                    ISMaybiInspection = true;
                    if (IsInspectionToDayDriver)
                    {
                        IsInspection = false;
                    }
                    else
                    {
                        IsInspection = true;

                        string token = CrossSettings.Current.GetValueOrDefault("Token", "");
                        ManagerDispatchMob managerDispatchMob = new ManagerDispatchMob();
                        int state;
                        string description = null;
                        bool isInspection = false;
                        int indexPhoto = 1;
                        TruckCar truckCar = null;
                        state = managerDispatchMob.DriverWork("CheckInspeacktion", token, ref description, ref isInspection, ref indexPhoto, ref truckCar);
                        if (indexPhoto == 2 && !isInspection)
                            IsInspection = false;
                    }
                }
                else
                {
                    ISMaybiInspection = false;
                    IsInspection = true;

                    string token = CrossSettings.Current.GetValueOrDefault("Token", "");
                    ManagerDispatchMob managerDispatchMob = new ManagerDispatchMob();
                    int state;
                    string description = null;
                    bool isInspection = false;
                    int indexPhoto = 1;
                    TruckCar truckCar = null;
                    state = managerDispatchMob.DriverWork("CheckInspeacktion", token, ref description, ref isInspection, ref indexPhoto, ref truckCar);
                    if (indexPhoto == 2 && !isInspection)
                        IsInspection = false;
                }
            }
        }

        public string GetColorBlock
        {
            get
            {
                string color = "";
                if (TimeOfInspection == $"1 {LanguageHelper.HoursText}" || TimeOfInspection == $"0 {LanguageHelper.HoursText}")
                {
                    color = "#FF2C2C";
                }
                else if (TimeOfInspection == $"2 {LanguageHelper.HoursText}")
                {
                    color = "#FF5C00";
                }
                else if (TimeOfInspection == $"3 {LanguageHelper.HoursText}")
                {
                    color = "#FF9314";
                }
                else if (TimeOfInspection == $"4 {LanguageHelper.HoursText}")
                {
                    color = "#FFE600";
                }
                else if (TimeOfInspection == $"5 {LanguageHelper.HoursText}")
                {
                    color = "#BDFF00";
                }
                else if (TimeOfInspection == $"6 {LanguageHelper.HoursText}")
                {
                    color = "#69EB2C";
                }
                else if (TimeOfInspection == $"7 {LanguageHelper.HoursText}")
                {
                    color = "#2C5DEB";
                }
                else
                {
                    color = "#2C5DEB";
                }
                return color;
            }
        }

        public string GetColorBlockOpacity
        {
            get
            {
                string color = "";
                if (TimeOfInspection == $"1 {LanguageHelper.HoursText}" || TimeOfInspection == $"0 {LanguageHelper.HoursText}")
                {
                    color = "#ffeeee";
                }
                else if (TimeOfInspection == $"2 {LanguageHelper.HoursText}")
                {
                    color = "#fff2eb";
                }
                else if (TimeOfInspection == $"3 {LanguageHelper.HoursText}")
                {
                    color = "#fff6ec";
                }
                else if (TimeOfInspection == $"4 {LanguageHelper.HoursText}")
                {
                    color = "#fffdeb";
                }
                else if (TimeOfInspection == $"5 {LanguageHelper.HoursText}")
                {
                    color = "#faffeb";
                }
                else if (TimeOfInspection == $"6 {LanguageHelper.HoursText}")
                {
                    color = "#f3fdee";
                }
                else if (TimeOfInspection == $"7 {LanguageHelper.HoursText}")
                {
                    color = "#eef2fd";
                }
                else
                {
                    color = "#eef2fd";
                }
                return color;
            }
        }


        public string TimeOfStatus
        {
            get
            {
                string status = "";
                if(TimeOfInspection == $"7 {LanguageHelper.HoursText}" || TimeOfInspection == $"6 {LanguageHelper.HoursText}" || TimeOfInspection == $"5 {LanguageHelper.HoursText}" || TimeOfInspection == $"4 {LanguageHelper.HoursText}")
                {
                    status = LanguageHelper.CanPassText;
                }
                else if(TimeOfInspection == $"3 {LanguageHelper.HoursText}" || TimeOfInspection == $"2 {LanguageHelper.HoursText}")
                {
                    status = LanguageHelper.BestTimePassText;
                }
                else if (TimeOfInspection == $"1 {LanguageHelper.HoursText}")
                {
                    status = LanguageHelper.PassNowText;
                }
                else if (TimeOfInspection == $"0 {LanguageHelper.HoursText}")
                {
                    status = LanguageHelper.PassNowText;
                }
                return status;
            }
        }
    }
}