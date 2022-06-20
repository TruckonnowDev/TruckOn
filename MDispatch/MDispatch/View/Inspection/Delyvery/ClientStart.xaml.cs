using MDispatch.Models;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.View.Inspection.Delyvery
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientStart : ContentPage
    {
        private AskForUserDelyvery askForUserDelyvery = null;
        private readonly IHelperViewService _helperView;

        public ClientStart(IManagerDispatchMobService managerDispatchMob, string idShip, InitDasbordDelegate initDasbordDelegate, string onDeliveryToCarrier, string totalPaymentToCarrier, VehiclwInformation vehiclwInformation, GetShiping getShiping, GetVechicleDelegate getVechicleDelegate, bool isproplem)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            askForUserDelyvery = new AskForUserDelyvery(managerDispatchMob, idShip, initDasbordDelegate, onDeliveryToCarrier, totalPaymentToCarrier, vehiclwInformation, getShiping, getVechicleDelegate, isproplem);
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(askForUserDelyvery);
            Navigation.RemovePage(Navigation.NavigationStack[1]);
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            //await PopupNavigation.PushAsync(new ContactInfo());
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _helperView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _helperView.Hidden();
        }
    }
}