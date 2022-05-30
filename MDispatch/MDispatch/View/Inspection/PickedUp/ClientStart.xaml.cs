using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.View.Inspection.PickedUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientStart : ContentPage
    {
        private AskForUser askForUser = null;
        private readonly IHelperViewService _helperView;

        public ClientStart(IManagerDispatchMobService managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, InitDasbordDelegate initDasbordDelegate, string onDeliveryToCarrier, string totalPaymentToCarrier)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            askForUser = new AskForUser(managerDispatchMob, vehiclwInformation, idShip, initDasbordDelegate, onDeliveryToCarrier, totalPaymentToCarrier);
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(askForUser);
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
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            _helperView.InitAlert(body);
        } 

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _helperView.Hidden();
        }
    }
}