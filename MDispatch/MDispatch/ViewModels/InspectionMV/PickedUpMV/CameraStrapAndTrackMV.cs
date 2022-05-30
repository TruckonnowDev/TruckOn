using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.AskPhoto;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using MDispatch.View.Inspection.PickedUp;
using MDispatch.ViewModels.InspectionMV.Servise.Models;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.InspectionMV.PickedUpMV
{
    public class CameraStrapAndTrackMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public InitDasbordDelegate initDasbordDelegate = null;
        private GetVechicleDelegate getVechicleDelegate = null;

        public CameraStrapAndTrackMV(
            IManagerDispatchMobService managerDispatchMob, 
            VehiclwInformation vehiclwInformation, 
            string idShip, InitDasbordDelegate initDasbordDelegate, 
            GetVechicleDelegate getVechicleDelegate,
            string onDeliveryToCarrier, 
            string totalPaymentToCarrier, 
            string nameVehicl,
            INavigation navigation)
            :base(navigation)
        {
            this.getVechicleDelegate = getVechicleDelegate;
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
            IdShip = idShip;
            OnDeliveryToCarrier = onDeliveryToCarrier;
            TotalPaymentToCarrier = totalPaymentToCarrier;
            Car = GetTypeCar(nameVehicl.Replace(" ", ""));
        }

        public string IdShip { get; set; }
        public string OnDeliveryToCarrier { get; set; }
        public string TotalPaymentToCarrier { get; set; }
        public List<Photo> straps = new List<Photo>();
        public List<Photo> inTruck = new List<Photo>();
        public IVehicle Car { get; set; }

        private VehiclwInformation vehiclwInformation = null;
        public VehiclwInformation VehiclwInformation
        {
            get => vehiclwInformation;
            set => SetProperty(ref vehiclwInformation, value);
        }

        private async void CheckVechicleAndGoToResultPage()
        {
            List<VehiclwInformation> vehiclwInformation1s = getVechicleDelegate.Invoke();
            int indexCurrentVechecle = vehiclwInformation1s.FindIndex(v => v == VehiclwInformation);
            if (vehiclwInformation1s.Count - 1 == indexCurrentVechecle)
            {
                await _navigation.PushAsync(new ClientStart(managerDispatchMob, VehiclwInformation, IdShip, initDasbordDelegate, OnDeliveryToCarrier, TotalPaymentToCarrier));
                await _navigation.PushAsync(new Alert(LanguageHelper.PassTheDeviceAlert, null));
            }
            else
            {
                await _navigation.PushAsync(new HintPageVechicle(LanguageHelper.ContinuingInspectionPickedUp, vehiclwInformation1s[indexCurrentVechecle + 1]));
                await _navigation.PushAsync(new AskPage(managerDispatchMob, vehiclwInformation1s[indexCurrentVechecle + 1], IdShip, initDasbordDelegate, getVechicleDelegate, OnDeliveryToCarrier, TotalPaymentToCarrier), true);
            }
        }

        private IVehicle GetTypeCar(string typeCar)
        {
            IVehicle car = null;
            switch (typeCar)
            {
                case "PickUp":
                    {
                        car = new CarPickUp();
                        break;
                    }
                case "Coupe":
                    {
                        car = new CarCoupe();
                        break;
                    }
                case "Suv":
                    {
                        car = new CarSuv();
                        break;
                    }
                case "Sedan":
                    {
                        car = new CarSedan();
                        break;
                    }
                case "Sportbike":
                    {
                        car = new MotorcycleSport();
                        break;
                    }
                case "Touringmotorcycle":
                    {
                        car = new MotorcycleTouring();
                        break;
                    }
                case "Cruisemotorcycle":
                    {
                        car = new MotorcycleСruising();
                        break;
                    }
                case "Tricycle":
                    {
                        car = new MotorcycleTricycle();
                        break;
                    }
            }
            return car;
        }

        internal async void SavePhotoInTruck()
        {
            bool isNavigationMany = false;
            if (_navigation.NavigationStack.Count > 2)
            {
                await _navigation.PushAsync(new LoadPage());
                isNavigationMany = true;
            }
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            CheckVechicleAndGoToResultPage();
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("SaveInTruck", token, VehiclwInformation.Id, inTruck, ref description);
                    if(state == 3)
                    {
                        state = managerDispatchMob.AskWork("SaveStrap", token, VehiclwInformation.Id, straps, ref description);
                    }
                    //state = managerDispatchMob.AskWork("SaveAsk1", token, VehiclwInformation.Id, Ask1, ref description);
                    //SaveStrap
                    //SaveInTruck
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    if (isNavigationMany)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[0]);
                        isNavigationMany = false;
                    }
                    if (_navigation.NavigationStack.Count > 1)
                    {
                        await _navigation.PopAsync();
                    }
                    await _navigation.PushAsync(new Alert(description, _navigation));
                }
                else if (state == 3)
                {
                    if (isNavigationMany)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[0]);
                        isNavigationMany = false;
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.AnswersSaved);
                    if (_navigation.NavigationStack.Count > 1)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[1]);
                    }
                }
                else if (state == 4)
                {
                    if (isNavigationMany)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[0]);
                        isNavigationMany = false;
                    }
                    if (_navigation.NavigationStack.Count > 1)
                    {
                        await _navigation.PopAsync();
                    }
                    await _navigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, _navigation));
                }
            }
            else
            {
                if (_navigation.NavigationStack.Count > 1)
                {
                    await _navigation.PopAsync();
                }
            }
        }
    }
}