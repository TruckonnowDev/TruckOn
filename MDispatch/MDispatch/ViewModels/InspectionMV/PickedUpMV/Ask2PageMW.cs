using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.Service.Net;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using Plugin.Settings;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.ViewModels.InspectionMV.PickedUpMV
{
    public class Ask2PageMW : BindableBase
    {

        public ManagerDispatchMob managerDispatchMob = null;
        public INavigation Navigation { get; set; }
        public InitDasbordDelegate initDasbordDelegate = null;

        public Ask2PageMW(ManagerDispatchMob managerDispatchMob, string idVech, string idShip, INavigation navigation, InitDasbordDelegate initDasbordDelegate, bool isProblem)
        {
            OnInitialize();
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            Navigation = navigation;
            IdShip = idShip;
            IdVech = idVech;
            IsProblem = isProblem;
        }

        public string IdShip { get; set; }
        public string IdVech { get; set; }

        private Ask2 ask2 = null;
        public Ask2 Ask2
        {
            get => ask2;
            set => SetProperty(ref ask2, value);
        }

        private bool _isProblem;
        public bool IsProblem
        {
            get => _isProblem;
            set => SetProperty(ref _isProblem, value);
        }

        [System.Obsolete]
        public async void Continue()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await PopupNavigation.PushAsync(new LoadPage());
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.Recurent(token, IdShip, "Picked up", ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    await PopupNavigation.PopAsync();
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PopAsync();
                    HelpersView.CallError(description);
                }
                else if (state == 3)
                {
                    await PopupNavigation.PopAsync();
                    await Navigation.PopToRootAsync();
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.AnswersSaved);
                }
                else if (state == 4)
                {
                    await PopupNavigation.PopAsync();
                    HelpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
        }

        private async void OnInitialize()
        {
            if (IsProblem)
                await PopupNavigation.PushAsync(new Alert("Please contact our support to fix an issue according this number: 17734305155", Navigation));
        }

        [Obsolete]
        public async void SaveAsk()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("SaveAsk2", token, IdShip, Ask2, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    HelpersView.CallError(description);
                }
                else if (state == 3)
                {
                    Continue();
                }
                else if (state == 4)
                {
                    HelpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
        }

    }
}
    