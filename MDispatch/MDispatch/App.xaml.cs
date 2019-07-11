using FormsControls.Base;
using MDispatch.Service.GCFolder;
using MDispatch.Service.GeloctionGPS;
using MDispatch.StoreNotify;
using MDispatch.View.A_R;
using MDispatch.View.TabPage;
using MDispatch.View.TabPage.Tab;
using Plugin.Settings;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch
{
    public partial class App : Application
	{
        public static bool isAvtorization;
        public static bool isNetwork;
        public static bool isStart;
            
        public App ()
        {
			InitializeComponent();
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            if (token == "")
            {
                isAvtorization = false;
                MainPage = new AnimationNavigationPage(new Avtorization());
            }
            else
            {
                isAvtorization = true;
                MainPage = new AnimationNavigationPage(new TabPage(new Service.ManagerDispatchMob()));
            }
        }

        //private async void SelecterPage(bool isNotify)
        //{
        //    if(isNotify)
        //    {
        //        TabPage tabPage = new TabPage(new Service.ManagerDispatchMob());
        //        NavigationPage navigation = (NavigationPage)tabPage.Children.ToList()[0];
        //        ActivePage activePage = (ActivePage)navigation.Navigation.NavigationStack[0];
        //        MainPage = new NavigationPage(tabPage);
        //       // await MainPage.Navigation.PushAsync(new FullPhotoTruck(activePage.activeMV.managerDispatchMob, activePage.activeMV.UnTimeOfInspection.IdDriver, 1, activePage.activeMV.initDasbordDelegate));
        //    }
        //    else
        //    {
        //        string token = CrossSettings.Current.GetValueOrDefault("Token", "");
        //        if (token == "")
        //        {
        //            isAvtorization = false;
        //            MainPage = new NavigationPage(new Avtorization());
        //        }
        //        else
        //        {
        //            isAvtorization = true;
        //            MainPage = new NavigationPage(new TabPage(new Service.ManagerDispatchMob()));
        //        }
        //    }
        //}

		protected override async void OnStart()
        {
            
            if (isAvtorization)
            {
                //Task.Run(async () =>
                //{
                //    DependencyService.Get<IStore>().OnTokenRefresh();
                //});
                isStart = true;
                await Utils.StartListening();
               GCUntil.StartClereing();
            }
        }

		protected override async void OnSleep ()
        {
            if (isAvtorization)
            {
                isStart = false;
                await Utils.StopListening();
                GCUntil.StopClereing();
            }
        }

		protected override async void OnResume ()
        {
            if (isAvtorization)
            {
                isStart = true;
                await Utils.StartListening();
                GCUntil.StartClereing();
            }
        }
	}
}