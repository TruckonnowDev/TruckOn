using FormsControls.Base;
using MDispatch.Helpers;
using MDispatch.NewElement;
using MDispatch.NewElement.Tabs;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View.TabPage.Tab;
using MDispatch.ViewModels.TAbbPage;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.TabPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabPage : CustomTabbedPage
    {
        private TablePageMV tablePageMV = null;
        private readonly IManagerDispatchMobService _managerDispatchMob;

        public TabPage()
        {
            _managerDispatchMob = DependencyService.Get<IManagerDispatchMobService>();
            tablePageMV = new TablePageMV(_managerDispatchMob, Navigation);
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<iOS>().SetUseSafeArea(true);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            BindingContext = this.tablePageMV;
            On<Android>().SetBarItemColor(Color.FromHex("#A1A1A1"));
            On<Android>().SetBarSelectedItemColor(Color.FromHex("#2C5DEB"));
            InitActivePage(_managerDispatchMob);
            InitDeiveredPage(_managerDispatchMob);
            InitArchivedPage(_managerDispatchMob);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<IOrientationHandler>().ForceSensor();
        }

        private void InitActivePage(IManagerDispatchMobService managerDispatchMob)
        {
            AnimationNavigationPage navigationPage = new AnimationNavigationPage(new ActivePage(managerDispatchMob, Navigation) { Title = LanguageHelper.NamePageTabActive });
            navigationPage.Title = LanguageHelper.NamePageTabActive;
            navigationPage.IconImageSource = "Aktiv.png";
            Children.Add(navigationPage);
        }

        private void InitDeiveredPage(IManagerDispatchMobService managerDispatchMob)
        {
            AnimationNavigationPage navigationPage = new AnimationNavigationPage(new DeiveredPage(managerDispatchMob, Navigation) { Title = LanguageHelper.NamePageTabDelivery } );
            navigationPage.Title = LanguageHelper.NamePageTabDelivery;
            navigationPage.IconImageSource = "Delivery.png";
            Children.Add(navigationPage);
        }

        private void InitArchivedPage(IManagerDispatchMobService managerDispatchMob)
        {
            AnimationNavigationPage navigationPage = new AnimationNavigationPage(new ArchivedPage(managerDispatchMob, Navigation) { Title = LanguageHelper.NamePageTabArchived } );
            navigationPage.Title = LanguageHelper.NamePageTabArchived;
            navigationPage.IconImageSource = "Archive.png";
            Children.Add(navigationPage);
        }
    }
}