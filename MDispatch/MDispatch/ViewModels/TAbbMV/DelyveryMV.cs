using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View.GlobalDialogView;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;
using Plugin.Settings;
using Prism.Commands;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.TAbbMV
{
    public class DelyveryMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public DelegateCommand RefreshCommand { get; set; }
        public InitDasbordDelegate initDasbordDelegate;
        private readonly IHelperViewService _helperView;

        public DelyveryMV(
            IManagerDispatchMobService managerDispatchMob,
            INavigation navigation)
            :base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            Shippings = new List<Shipping>();
            initDasbordDelegate = Init;
            this.managerDispatchMob = managerDispatchMob;
            RefreshCommand = new DelegateCommand(PreVibartionLoad);
        }

        private List<Shipping> shippings = null;
        public List<Shipping> Shippings
        {
            get => shippings;
            set => SetProperty(ref shippings, value);
        }

        private bool isRefr = false;
        public bool IsRefr
        {
            get => isRefr;
            set => SetProperty(ref isRefr, value);
        }

        private void PreVibartionLoad()
        {
            if (CrossDeviceInfo.Current.Platform == Platform.Android)
            {
                Vibration.Vibrate(20);
            }
            Init();
        }

        public async void Init()
        {
            IsRefr = true;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            List<Shipping> shippings = null;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.OrderWork("OrderDelyveryGet", token, ref description, ref shippings);
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                     _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    _helperView.Hidden();
                    Shippings = shippings;
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            IsRefr = false;
        }
    }
}
