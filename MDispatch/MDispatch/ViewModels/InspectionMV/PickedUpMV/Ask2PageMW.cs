using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.InspectionMV.PickedUpMV
{
    public class Ask2PageMW : BaseViewModel
    {

        public readonly IManagerDispatchMobService managerDispatchMob;
        public InitDasbordDelegate initDasbordDelegate = null;
        private readonly IHelperViewService _helperView;
        public Ask2PageMW(
            IManagerDispatchMobService managerDispatchMob, string idVech, 
            string idShip, INavigation navigation, 
            InitDasbordDelegate initDasbordDelegate)
            :base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            IdShip = idShip;
            IdVech = idVech;
        }

        public string IdShip { get; set; }
        public string IdVech { get; set; }

        private Ask2 ask2 = null;
        public Ask2 Ask2
        {
            get => ask2;
            set => SetProperty(ref ask2, value);
        }

        public async void Continue()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await _navigation.PushAsync(new LoadPage());
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.Recurent(token, IdShip, "Picked up", ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    await _navigation.PopAsync();
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await _navigation.PopAsync();
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    await _navigation.PopAsync();
                    await _navigation.PopToRootAsync();
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.AnswersSaved);
                }
                else if (state == 4)
                {
                    await _navigation.PopAsync();
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
        }

        public async void SaveAsk()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("SaveAsk2", token, IdShip, Ask2, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    Continue();
                }
                else if (state == 4)
                {
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
        }

    }
}
    