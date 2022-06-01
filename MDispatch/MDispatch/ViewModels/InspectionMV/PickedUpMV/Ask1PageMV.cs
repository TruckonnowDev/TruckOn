using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection.PickedUp;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.InspectionMV
{
    public class Ask1PageMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public InitDasbordDelegate initDasbordDelegate = null;
        private GetVechicleDelegate getVechicleDelegate = null;
        private readonly IHelperViewService _helperView;

        public Ask1PageMV(IManagerDispatchMobService managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, INavigation navigation, InitDasbordDelegate initDasbordDelegate, GetVechicleDelegate getVechicleDelegate, 
            string onDeliveryToCarrier, string totalPaymentToCarrier, string typeCar)
            :base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            this.getVechicleDelegate = getVechicleDelegate;
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
            IdShip = idShip;
            OnDeliveryToCarrier = onDeliveryToCarrier;
            TotalPaymentToCarrier = totalPaymentToCarrier;
            TypeCar = typeCar;
        }

        public string IdShip { get; set; }
        public string OnDeliveryToCarrier { get; set; }
        public string TotalPaymentToCarrier { get; set; }
        public string TypeCar { get; set; }

        private Ask1 ask1 = null;
        public Ask1 Ask1
        {
            get => ask1;
            set => SetProperty(ref ask1, value);
        }

        private VehiclwInformation vehiclwInformation = null;
        public VehiclwInformation VehiclwInformation
        {
            get => vehiclwInformation;
            set => SetProperty(ref vehiclwInformation, value);
        }

        public async void SaveAsk()
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
            await Navigation.PushAsync(new CameraStrapAndTrack(managerDispatchMob, VehiclwInformation, IdShip, initDasbordDelegate, getVechicleDelegate, OnDeliveryToCarrier, TotalPaymentToCarrier, TypeCar));
            
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("SaveAsk1", token, VehiclwInformation.Id, Ask1, ref description);
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
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.AnswersSaved);
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
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
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
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

        internal void ResetAskSeatBelts(byte[] oldRes, byte[] newRetake)
        {
            string base64 = Convert.ToBase64String(newRetake);
            Photo photo = Ask1.App_will_force_driver_to_take_pictures_of_each_strap.FirstOrDefault(a => a.Base64 == Convert.ToBase64String(oldRes));
            if (photo != null)
            {
                photo.Base64 = base64;
            }
        }

        internal void ResetAskInTrack(byte[] oldRes, byte[] newRetake)
        {

            string base64 = Convert.ToBase64String(newRetake);
            Photo photo = Ask1.Photo_after_loading_in_the_truck.FirstOrDefault(a => a.Base64 == Convert.ToBase64String(oldRes));
            if (photo != null)
            {
                photo.Base64 = base64;
            }
        }
    }
}