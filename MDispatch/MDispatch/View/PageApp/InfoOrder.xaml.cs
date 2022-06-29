using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.ViewModels.PageAppMV;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.View.PageApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoOrder : ContentPage
    {
        private InfoOrderMV infoOrderMV = null;
        private bool isNextPage = false;
        private ManagerDispatchMob managerDispatchMob;
        private InitDasbordDelegate initDasbordDelegate;
        private string currentStatus;

        public InfoOrder(ManagerDispatchMob managerDispatchMob, InitDasbordDelegate initDasbordDelegate, string statusInspection, string idShipping)
        {
            this.infoOrderMV = new InfoOrderMV(managerDispatchMob, initDasbordDelegate, statusInspection, idShipping, CallBackVehiclwInformation) { Navigation = this.Navigation };
            InitializeComponent();
            BindingContext = this.infoOrderMV;
            SetupView();
        }

        private void SetupView()
        {
        }

        private void CallBackVehiclwInformation()
        {
            int i = 0;
            countVeclLbl.Text = $"{LanguageHelper.TitleVehicleInfo} ({infoOrderMV.Shipping.VehiclwInformations.Count})";
            foreach (VehiclwInformation vehiclwInformations in infoOrderMV.Shipping.VehiclwInformations)
            {
                StackLayout stackLayout = new StackLayout();
                stackLayout.Spacing = 4;
                string font = ((OnPlatform<string>)Application.Current.Resources["OpenSans-Regular"]).Platforms.ToList().First(p => p.Platform.FirstOrDefault(pp => pp == Device.RuntimePlatform) != null).Value.ToString();
                stackLayout.Children.Add(new Label()
                {
                    Text = $"{vehiclwInformations.Year ?? "---"} {vehiclwInformations.Make ?? "------"} {vehiclwInformations.Model ?? "------"}",
                    FontSize = 14,
                    FontFamily = font,
                    TextColor = Color.FromHex("#101010")
                });;
                stackLayout.Children.Add(new Label()
                {
                    Text = $"VIN: {vehiclwInformations.VIN ?? "-----------"}",
                    FontSize = 14,
                    FontFamily = font,
                    TextColor = Color.FromHex("#101010")
                });
                bloc1.Children.Add(stackLayout);
                if (i != infoOrderMV.Shipping.VehiclwInformations.Count - 1)
                {
                    stackLayout.Padding = new Thickness(0, 4, 0, 7);
                    bloc1.Children.Add(new BoxView()
                    {
                        HeightRequest = 1,
                        BackgroundColor = Color.FromHex("#E5E5E5")
                    });
                }
                else
                {
                    stackLayout.Padding = new Thickness(0, 4, 0, 0);
                }
                i++;
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Button button = ((Button)sender);
            string id = button.FindByName<Label>("idL").Text;
            infoOrderMV.ToVehicleDetails(infoOrderMV.Shipping.VehiclwInformations.Find(v => v.Id == id));
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            if(infoOrderMV.Shipping == null)
            {
                HelpersView.CallError(LanguageHelper.NoDataAlert);
                return;
            }

            if(infoOrderMV.Shipping.CurrentStatus == "Assigned")
            {
                infoOrderMV.ToStartInspection();
            }
            else if(infoOrderMV.Shipping.CurrentStatus == "Picked up")
            {
                infoOrderMV.ToStartInspectionDelyvery();
            }
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
            HelpersView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            HelpersView.Hidden();
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            infoOrderMV.SetIstractions();
        }
    }
}