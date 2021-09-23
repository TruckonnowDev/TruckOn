using System;
using System.Threading.Tasks;
using FormsControls.Base;
using MDispatch.Helpers;
using MDispatch.Service.GeloctionGPS;
using MDispatch.Service.Tasks;
using MDispatch.StoreNotify;
using MDispatch.View.A_R;
using MDispatch.View.TabPage;
using Plugin.Settings;
using Xamarin.Forms;

namespace MDispatch
{
    public partial class App : Application
	{
        public static bool isAvtorization;
        public static bool isNetwork;
        public static bool isStart;
        public static DateTime time = DateTime.Now;

        public App ()
        {
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
                MainPage = new AnimationNavigationPage(new TabPage(new Service.ManagerDispatchMob()));
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