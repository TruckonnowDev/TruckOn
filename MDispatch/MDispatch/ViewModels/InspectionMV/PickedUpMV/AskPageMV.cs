using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.PageApp;
using Newtonsoft.Json;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.AskPhoto
{
    public class AskPageMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        private InitDasbordDelegate initDasbordDelegate = null;
        private GetVechicleDelegate getVechicleDelegate = null;
        private readonly IHelperViewService _helperView;

        public AskPageMV(
            IManagerDispatchMobService managerDispatchMob, 
            VehiclwInformation vehiclwInformation, 
            string idShip, INavigation navigation, 
            InitDasbordDelegate initDasbordDelegate, 
            GetVechicleDelegate getVechicleDelegate,
             string onDeliveryToCarrier, string totalPaymentToCarrier)
            : base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            this.getVechicleDelegate = getVechicleDelegate;
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
            IdShip = idShip;
            OnDeliveryToCarrier = onDeliveryToCarrier;
            TotalPaymentToCarrier = totalPaymentToCarrier;
            IdShip = idShip;
            Ask = new Ask();
            Init();
        }

        private void Init()
        {
            if(vehiclwInformation.Type == "Coupe" || vehiclwInformation.Type == "Suv" || vehiclwInformation.Type == "PickUp" || vehiclwInformation.Type == "Sedan" || vehiclwInformation.Type == "Tricycle"
                || vehiclwInformation.Type == "Sport bike" || vehiclwInformation.Type == "Touring motorcycle" || vehiclwInformation.Type == "Cruise motorcycle")
            {
                Ask.TypeVehicle = VehiclwInformation.Type;
            }
        }

        public string IdShip { get; set; }
        public string OnDeliveryToCarrier { get; set; }
        public string TotalPaymentToCarrier { get; set; }

        private Models.Ask ask = null;
        public Models.Ask Ask
        {
            get => ask;
            set => SetProperty(ref ask, value);
        }

        private VehiclwInformation vehiclwInformation = null;
        public VehiclwInformation VehiclwInformation
        {
            get => vehiclwInformation;
            set => SetProperty(ref vehiclwInformation, value);
        }

        public void ResetAskPhotoItem(byte[] oldRes, byte[] newRetake)
        {
            string base64 = JsonConvert.SerializeObject(newRetake);
            Photo photo = Ask.Any_personal_or_additional_items_with_or_in_vehicle.FirstOrDefault(a => a.Base64 == Convert.ToBase64String(oldRes));
            if (photo != null)
            {
                photo.Base64 = base64;
            }
        }


        public async void SaveAsk(string indexTypeCar)
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
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            FullPagePhoto fullPagePhoto = new FullPagePhoto(managerDispatchMob, VehiclwInformation, IdShip, $"{indexTypeCar}1.png", indexTypeCar, 1, initDasbordDelegate, getVechicleDelegate, "", OnDeliveryToCarrier, TotalPaymentToCarrier);
            await _navigation.PushAsync(fullPagePhoto);
            await _navigation.PushAsync(new CameraPagePhoto($"{indexTypeCar}1.png", fullPagePhoto, "PhotoIspection"));
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {

                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("SaveAsk", token, VehiclwInformation.Id, Ask, ref description);
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
                    GoToABack();
                    //await PopupNavigation.PushAsync(new Errror(description, Navigation));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    if (isNavigationMany)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[0]);
                        isNavigationMany = false;
                    }
                    _navigation.RemovePage(_navigation.NavigationStack[1]);
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.AnswersSaved);
                }
                else if (state == 4)
                {
                    if (isNavigationMany)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[0]);
                        isNavigationMany = false;
                    }
                    GoToABack();
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigation));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            else
            {
                GoToABack();
            }
        }

        private async void GoToABack()
        {
            if (_navigation.NavigationStack.Count > 2)
            {
                await _navigation.PopAsync();
                await _navigation.PopAsync();
            }
            DependencyService.Get<IOrientationHandler>().ForceSensor();
        }
    }
}