﻿using FormsControls.Base;
using MDispatch.NewElement;
using MDispatch.NewElement.Tabs;
using MDispatch.Service;
using MDispatch.View.TabPage.Tab;
using MDispatch.ViewModels.TAbbPage;
using Plugin.Badge.Abstractions;
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

        public TabPage (ManagerDispatchMob managerDispatchMob)  : base()
        {
            tablePageMV = new TablePageMV(managerDispatchMob);
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<iOS>().SetUseSafeArea(true);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            BindingContext = this.tablePageMV;
            On<Android>().SetBarItemColor(Color.FromHex("#A1A1A1"));
            On<Android>().SetBarSelectedItemColor(Color.FromHex("#2C5DEB"));
            InitActivePage(managerDispatchMob);
            InitDeiveredPage(managerDispatchMob);
            InitArchivedPage(managerDispatchMob);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<IOrientationHandler>().ForceSensor();
        }

        private void InitActivePage(ManagerDispatchMob managerDispatchMob)
        {
            AnimationNavigationPage navigationPage = new AnimationNavigationPage(new ActivePage(managerDispatchMob, Navigation));
            navigationPage.Title = "Active";
            navigationPage.IconImageSource = "Aktiv.png";
            Children.Add(navigationPage);
        }

        private void InitDeiveredPage(ManagerDispatchMob managerDispatchMob)
        {
            AnimationNavigationPage navigationPage = new AnimationNavigationPage(new DeiveredPage(managerDispatchMob, Navigation));
            navigationPage.Title = "Delivery";
            navigationPage.IconImageSource = "Delivery.png";
            Children.Add(navigationPage);
        }

        private void InitArchivedPage(ManagerDispatchMob managerDispatchMob)
        {
            AnimationNavigationPage navigationPage = new AnimationNavigationPage(new ArchivedPage(managerDispatchMob, Navigation));
            navigationPage.Title = "Archived";
            navigationPage.IconImageSource = "Archive.png";
            Children.Add(navigationPage);
        }
    }
}