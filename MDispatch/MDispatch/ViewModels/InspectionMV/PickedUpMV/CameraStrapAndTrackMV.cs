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
                await Navigation.PushAsync(new ClientStart(managerDispatchMob, VehiclwInformation, IdShip, initDasbordDelegate, OnDeliveryToCarrier, TotalPaymentToCarrier));
                await _popupNavigation.PushAsync(new Alert(LanguageHelper.PassTheDeviceAlert, null));
            }
            else
            {
                await _popupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.ContinuingInspectionPickedUp, vehiclwInformation1s[indexCurrentVechecle + 1]));
                await Navigation.PushAsync(new AskPage(managerDispatchMob, vehiclwInformation1s[indexCurrentVechecle + 1], IdShip, initDasbordDelegate, getVechicleDelegate, OnDeliveryToCarrier, TotalPaymentToCarrier), true);
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
            if (Navigation.NavigationStack.Count > 2)
            {
                await _popupNavigation.PushAsync(new LoadPage());
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
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        await Navigation.PopAsync();
                    }
                    await _popupNavigation.PushAsync(new Alert(description, Navigation));
                }
                else if (state == 3)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.AnswersSaved);
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[1]);
                    }
                }
                else if (state == 4)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        await Navigation.PopAsync();
                    }
                    await _popupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, Navigation));
                }
            }
            else
            {
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopAsync();
                }
            }
        }
    }
}