using MDispatch.Helpers;
using MDispatch.Service;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.A_R;
using MDispatch.View.GlobalDialogView;
using Plugin.LatestVersion;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.PageAppMV.Settings
{
    public class SettingsMV : BaseViewModel
    {
        private readonly IManagerDispatchMobService managerDispatchMob;
        private readonly IHelperViewService _helperView;

        public SettingsMV(
            IManagerDispatchMobService managerDispatchMob,
            INavigation navigation)
            : base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            this.managerDispatchMob = managerDispatchMob;
            Init();
            SetCurrentVersion();
            SetLatestVersionNumber();
        }
        
        public INavigation Navigation { get; set; }

        private string latsInspection = "--.--.--";
        public string LatsInspection
        {
            get { return latsInspection; }
            set { SetProperty(ref latsInspection, value); }
        }

        private string plateTruck = "---------";
        public string PlateTruck
        {
            get { return plateTruck; }
            set { SetProperty(ref plateTruck, value); }
        }

        private string plateTrailer = "---------";
        public string PlateTrailer
        {
            get { return plateTrailer; }
            set { SetProperty(ref plateTrailer, value); }
        }

        private string plateTruck1 = "";
        public string PlateTruck1
        {
            get { return plateTruck1; }
            set { SetProperty(ref plateTruck1, value); }
        }

        private string plateTrailer1 = "";
        public string PlateTrailer1
        {
            get { return plateTrailer1; }
            set
            { 
                SetProperty(ref plateTrailer1, value);
            }
        }

        public string CurrentVersion
        {
            get { return CrossLatestVersion.Current.InstalledVersionNumber; }
        }

        private string lastUpdateAvailable = "";
        public string LastUpdateAvailable
        {
            get { return lastUpdateAvailable; }
            set { SetProperty(ref lastUpdateAvailable, value); }
        }

        private bool isUpdateVersion = default;
        public bool IsUpdateVersion
        {
            get { return isUpdateVersion; }
            set { SetProperty(ref isUpdateVersion, value); }
        }

        private async void SetCurrentVersion()
        {
            try
            {
                IsUpdateVersion = await CrossLatestVersion.Current.IsUsingLatestVersion();
            }
            catch
            {
                IsUpdateVersion = true;
            }
        }

        private async void SetLatestVersionNumber()
        {
            try
            {
                LastUpdateAvailable = await CrossLatestVersion.Current.GetLatestVersionNumber();
            }
            catch
            {
                LastUpdateAvailable = "Check stor app";
            }
        }

        private async void Init()
        {
            await _popupNavigation.PushAsync(new LoadPage());
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string idDriver = CrossSettings.Current.GetValueOrDefault("IdDriver", "");
            string description = null;
            int state = 0; 
            string latsInspection = "--.--.--";
            string plateTruck = "---------";
            string plateTrailer = "---------";
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.GetLastInspaction(token, idDriver, ref latsInspection, ref plateTruck, ref plateTrailer, ref description);
                });
                await _popupNavigation.PopAsync();
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                {
                    await _popupNavigation.PushAsync(new Alert(description, null));
                    //HelpersView.CallError(description);
                }
                else if (state == 3)
                {
                    LatsInspection = latsInspection;
                    PlateTruck = plateTruck;
                    PlateTrailer = plateTrailer;
                }
                else if (state == 4)
                {
                    await _popupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                    //HelpersView.CallError("Technical work on the service");
                }
            }
            else
            {
                await _popupNavigation.PopAsync();
            }
        }

        public async void OutAccount()
        {
            await _popupNavigation.PushAsync(new LoadPage());
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            bool isInspection = false;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.A_RWork("Clear", null, null, ref description, ref token);
                });

                await _popupNavigation.PopAsync();
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await _popupNavigation.PushAsync(new Alert("Error", null));
                }
                else if (state == 3)
                {
                    await Navigation.PopModalAsync();
                    CrossSettings.Current.Remove("Token");
                    App.isAvtorization = false;
                    App.Current.MainPage = new NavigationPage(new Avtorization());
                }
                else if (state == 4)
                {
                    await _popupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
        }

        internal async void DetectText(byte[] result, string type)
        {
            await _popupNavigation.PushAsync(new LoadPage());
            string idDriver = CrossSettings.Current.GetValueOrDefault("IdDriver", "");
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            int state = 0;
            string plate = null;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                state = managerDispatchMob.DetectPlate(token, Convert.ToBase64String(result), idDriver, type, plate);

                if (state == 1)
                {
                    await _popupNavigation.PopAsync();
                    _globalHelperService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(LanguageHelper.NoAvtorisationAlert, null));
                }
                else if (state == 3)
                {
                    await _popupNavigation.PopAsync();
                    if (type == "truck")
                    {
                        PlateTruck1 = plate;
                    }
                    else if (type == "trailer")
                    {
                        PlateTrailer1 = plate;
                    }
                }
                else if (state == 4)
                {
                    await _popupNavigation.PopAsync();
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service scan", null));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
        }
    }
}