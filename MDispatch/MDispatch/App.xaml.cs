using FormsControls.Base;
using MDispatch.Helpers;
using MDispatch.Service.Auth;
using MDispatch.Service.DatabaseContext;
using MDispatch.Service.DriverInspection;
using MDispatch.Service.GlobalHelper;
using MDispatch.Service.GoogleApi;
using MDispatch.Service.HelperView;
using MDispatch.Service.Inspection;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.Service.ManagerStore;
using MDispatch.Service.OpacityTouchView;
using MDispatch.Service.OrderGet;
using MDispatch.Service.RequestQueue;
using MDispatch.Service.StoreNotify;
using MDispatch.Service.Tasks;
using MDispatch.Service.TimeSync;
using MDispatch.Service.Utils;
using MDispatch.View.A_R;
using MDispatch.View.TabPage;
using Plugin.Settings;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch
{
    public partial class App : Application
	{
        public static bool isAvtorization;
        public static bool isNetwork;
        public static bool isStart;
        public static DateTime time = DateTime.Now;
        private readonly IUtilsService _utils;
        private readonly ITimeSyncService _untils;
        public App ()
        {
            RegisterServices();
            _utils = DependencyService.Get<IUtilsService>();
            _untils = DependencyService.Get<ITimeSyncService>();
            SetCurrentCultureThread();
            LanguageHelper.InitLanguage();
            InitializeComponent();
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
                MainPage = new AnimationNavigationPage(new TabPage());
            }

        }

        private void RegisterServices()
        {
            DependencyService.Register<IAuth, Auth>();
            DependencyService.Register<IManagerStore, ManagerStore>();
            DependencyService.Register<IOrderGetService, OrderGetService>();
            DependencyService.Register<IManagerDispatchMobService, ManagerDispatchMobService>();
            DependencyService.Register<IDatabaseContextService, DatabaseContextService>();
            DependencyService.Register<IDriverInspectionService, DriverInspectionService>();
            DependencyService.Register<IGoogleApiService, GoogleApiService>();
            DependencyService.Register<IInspectionService, InspectionService>();
            DependencyService.Register<IUtilsService, UtilsService>();
            DependencyService.Register<ITimeSyncService, TimeSyncService>();
            DependencyService.Register<IManagerQueueService, ManagerQueueService>();
            DependencyService.Register<IOpacityTouchViewService, OpacityTouchViewService>();
            DependencyService.Register<IGlobalHelperService, GlobalHelperService>();
            DependencyService.Register<IHelperViewService, HelperViewService>();
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
                _untils.Start();
                await _utils.StartListening();
                TaskManager.CommandToDo("CheckTask");
            }
        }

		protected override async void OnSleep ()
        {
            if (isAvtorization)
            {
                isStart = false;
                _untils.Stop();
                await _utils.StopListening();
            }
        }

		protected override async void OnResume ()
        {
            if (isAvtorization)
            {
                isStart = true;
                _untils.Start();
                await _utils.StartListening();
            }
        }

        private async void SetCurrentCultureThread()
        {

        }
    }
}