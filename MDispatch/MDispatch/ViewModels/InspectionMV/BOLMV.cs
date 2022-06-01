using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service;
using MDispatch.Service.GlobalHelper;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.Service.Utils;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using Plugin.Settings;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.InspectionMV
{
    public class BOLMV : BaseViewModel
    {
        private BOLPage bOLPage = null;
        public InitDasbordDelegate initDasbordDelegate = null;
        private readonly IGlobalHelperService _globalHelpersService;
        private readonly IHelperViewService _helperView;
        private readonly IManagerDispatchMobService _managerDispatchMob;
        public BOLMV(
            IManagerDispatchMobService managerDispatchMob, 
            string idShip, INavigation navigation, 
            InitDasbordDelegate initDasbordDelegate, 
            BOLPage bOLPage)
            : base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            _globalHelpersService = DependencyService.Get<IGlobalHelperService>();
            managerDispatchMob = DependencyService.Get<IManagerDispatchMobService>();
            this._managerDispatchMob = managerDispatchMob;
            IdShip = idShip;
            this.initDasbordDelegate = initDasbordDelegate;
            this.bOLPage = bOLPage;
            InitShipping();
        }

        public string IdShip { get; set; }


        private Shipping shipping = null;
        public Shipping Shipping
        {
            get => shipping;
            set => SetProperty(ref shipping, value);
        }

        private bool isLoad = false;
        public bool IsLoad
        {
            get => isLoad;
            set => SetProperty(ref isLoad, value);
        }


        private string email = null;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        private async void InitShipping()
        {
            bool isNavigationMany = false;
            IsLoad = true;
            if (_popupNavigation.PopupStack.Count > 2)
            {
                isNavigationMany = true;
            }
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            Shipping shipping1 = null;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = _managerDispatchMob.GetShippingPhoto(token, IdShip, ref description, ref shipping1);
                });
                if (state == 1)
                {
                    _globalHelpersService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    //await PopupNavigation.PushAsync(new Errror("Error", null));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    Shipping = shipping1;
                    await bOLPage.InitPhoto(Shipping.VehiclwInformations);
                }
                else if (state == 4)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
                IsLoad = false;
            }
        }

        public async Task SendLiabilityAndInsuranceEmaile()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = _managerDispatchMob.AskWork("SendBolMail", token, IdShip, Email, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelpersService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await _popupNavigation.PushAsync(new Alert(description, null));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage($"{LanguageHelper.BOLIsSent} {Email}");
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
        }
    }
}