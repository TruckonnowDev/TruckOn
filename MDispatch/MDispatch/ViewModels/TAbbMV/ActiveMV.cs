using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service;
using MDispatch.Service.Cache;
using MDispatch.Service.Helpers;
using MDispatch.Service.Net;
using MDispatch.Vidget.VM;
using MDispatch.View.GlobalDialogView;
using MDispatch.ViewModels.TAbbMV.DialogAsk;
using Plugin.DeviceInfo;
using Plugin.DeviceInfo.Abstractions;
using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.ViewModels.TAbbMV
{
    public class ActiveMV : BindableBase
    {
        public ManagerDispatchMob managerDispatchMob = null;
        public INavigation Navigation { get; set; }
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand GoToInspectionDriveCommand { get; set; }
        public InitDasbordDelegate initDasbordDelegate;

        [Obsolete]
        public ActiveMV(ManagerDispatchMob managerDispatchMob, INavigation navigation)
        {
            Navigation = navigation;
            Shippings = new List<Shipping>();
            initDasbordDelegate = Init;
            this.managerDispatchMob = managerDispatchMob;
            RefreshCommand = new DelegateCommand(PreVibartionLoad);
            UnTimeOfInspection = new UnTimeOfInspection();
            GoToInspectionDriveCommand = new DelegateCommand(GoToInspectionDrive);
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

        private UnTimeOfInspection unTimeOfInspection = new UnTimeOfInspection();
        public UnTimeOfInspection UnTimeOfInspection
        {
            get => unTimeOfInspection;
            set => SetProperty(ref unTimeOfInspection, value);
        }

        private void PreVibartionLoad()
        {
            if (CrossDeviceInfo.Current.Platform == Platform.Android)
            {
                Vibration.Vibrate(20);
            }
            Init();
        }

        [Obsolete]
        public async void Init()
        {
            IsRefr = true;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            List<Shipping> shippings = null;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.OrderWork("OrderGet", token, ref description, ref shippings);
                });
                if(state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                    //HelpersView.CallError(description);
                }
                else if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                    HelpersView.CallError(description);
                }
                else if (state == 3)
                {
                    Shippings = shippings;
                    HelpersView.Hidden();
                    await Task.Run(() =>
                    {
                        UnTimeOfInspection = new UnTimeOfInspection(description);
                        if (UnTimeOfInspection.IsInspection && DependencyService.Get<ICacheService>().IsExpired(Constants.CacheInspection))
                        {
                            Device.BeginInvokeOnMainThread(async () => await PopupNavigation.PushAsync(new AskHint(this)));
                        }
                    });
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    HelpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            IsRefr = false;
        }

        [Obsolete]
        public async void GoToInspectionDrive()
        {
            IsRefr = true;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            bool isInspection = false;
            List<string> plateTruck = null;
            List<string> plateTrailer = null;
            int indexPhoto = 1;
            int state = 0;
            TruckCar truckCar = null;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.DriverWork("CheckInspeacktion", token, ref description, ref isInspection, ref indexPhoto, ref truckCar);
                });
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    //PopupNavigation.PushAsync(new Alert(description, null));
                    HelpersView.CallError(description);
                }
                else if (state == 3)
                {
                    HelpersView.Hidden();
                    if (isInspection)
                    {
                        Init();
                        await PopupNavigation.PushAsync(new Alert(LanguageHelper.InspectionTodayAlert, null));
                        //Add Commplet Alert
                    }
                    else
                    {
                        await Navigation.PushAsync(new Vidget.View.CameraPage(managerDispatchMob, UnTimeOfInspection.IdDriver, indexPhoto, initDasbordDelegate, truckCar));
                    }
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    HelpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            IsRefr = false;
        }
    }
}