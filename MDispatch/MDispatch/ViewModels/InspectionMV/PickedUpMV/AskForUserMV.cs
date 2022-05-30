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
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.InspectionMV.PickedUpMV
{
    public class AskForUserMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public InitDasbordDelegate initDasbordDelegate = null;
        private readonly IHelperViewService _helperView;

        public AskForUserMV(
            IManagerDispatchMobService managerDispatchMob, 
            VehiclwInformation vehiclwInformation, 
            string idShip, INavigation navigation, 
            InitDasbordDelegate initDasbordDelegate, 
            string onDeliveryToCarrier, 
            string totalPaymentToCarrier)
            :base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
            IdShip = idShip;
            OnDeliveryToCarrier = onDeliveryToCarrier;
            TotalPaymentToCarrier = totalPaymentToCarrier;
        }

        public string IdShip { get; set; }
        public string OnDeliveryToCarrier { get; set; }
        public string TotalPaymentToCarrier { get; set; }

        private AskFromUser askForUser = null;
        public AskFromUser AskForUser
        {
            get => askForUser;
            set => SetProperty(ref askForUser, value);
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
            if (_navigation.NavigationStack.Count > 2)
            {
                await _navigation.PushAsync(new LoadPage());
                isNavigationMany = true;
            }
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await _navigation.PushAsync(new LiabilityAndInsurance(managerDispatchMob, VehiclwInformation.Id, IdShip, initDasbordDelegate, OnDeliveryToCarrier, TotalPaymentToCarrier, false), true);
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("AskFromUser", token, IdShip, AskForUser, ref description);
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
                    if (_navigation.NavigationStack.Count > 1)
                    {
                        await _navigation.PopAsync();
                    }
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigation));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
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