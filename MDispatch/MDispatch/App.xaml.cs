using System;
using System.Threading.Tasks;
using FormsControls.Base;
using MDispatch.Helpers;
using MDispatch.Service.Cache;
using MDispatch.Service.GeloctionGPS;
using MDispatch.Service.Tasks;
using MDispatch.StoreNotify;
using MDispatch.View.A_R;
using MDispatch.View.TabPage;
using MonkeyCache.SQLite;
using Plugin.Settings;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MDispatch
{
    public partial class App : Application
	{
        public static bool isAvtorization;
        public static bool isNetwork;
        public static bool isStart;

        private static DateTime _time = DateTime.Now;
        public static DateTime Time
        {
            get => _time;
            set => _time = value;
        }

        public App ()
        {
            SetCurrentCultureThread();
            LanguageHelper.InitLanguage();
            InitializeComponent();
            GetPermissions();
            Barrel.ApplicationId = "TruckOnNow";
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            if (token == "")
            {
                isAvtorization = false;
                MainPage = new AnimationNavigationPage(new Avtorization());
            }
            else
            {
                TaskManager.isWorkTask = true;
                isAvtorization = true;
                MainPage = new AnimationNavigationPage(new TabPage(new Service.ManagerDispatchMob()));
            }


            //Services
            DependencyService.Register<ICacheService, CacheService>();
        }

        private async void GetPermissions()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                await Xamarin.Essentials.Permissions.RequestAsync<Permissions.Camera>();
            }
        }

        [Obsolete]
        protected override async void OnStart()
        {
            if (isAvtorization)
            {
                TaskManager.isWorkTask = true;
                Task.Run(() =>
                {
                    DependencyService.Get<IStore>().OnTokenRefresh();
                });
                isStart = true;
                MDispatch.Service.TimeSync.Untils.Start();
                await Utils.StartListening();
                TaskManager.CommandToDo("CheckTask");
            }
        }

		protected override async void OnSleep ()
        {
            if (isAvtorization)
            {
                isStart = false;
                MDispatch.Service.TimeSync.Untils.Stop();
                await Utils.StopListening();
            }
        }

		protected override async void OnResume ()
        {
            if (isAvtorization)
            {
                isStart = true;
                MDispatch.Service.TimeSync.Untils.Start();
                await Utils.StartListening();
            }
        }

        private async void SetCurrentCultureThread()
        {

        }
    }
}