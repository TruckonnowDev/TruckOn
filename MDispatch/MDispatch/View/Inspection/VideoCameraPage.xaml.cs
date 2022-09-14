using MDispatch.NewElement;
using MDispatch.View.Inspection.PickedUp;
using MDispatch.View.PageApp;
using MDispatch.ViewModels.AskPhoto;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using MDispatch.ViewModels.InspectionMV.Servise.Models;
using MDispatch.ViewModels.InspectionMV.Servise.Paymmant;
using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoCameraPage : NewElement.CustomVideoCam.VideoCameraPage
    {
        private object paymmant = null;

        public VideoCameraPage(object paymmant, string instructionAndNamePaymmant)
        {
            this.paymmant = paymmant;
            InitializeComponent();
            On<iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True)
                .SetPreferredStatusBarUpdateAnimation(UIStatusBarAnimation.Fade);
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
        }

        [Obsolete]
        private async void VideoCameraPage_OnPhotoResult(NewElement.PhotoResultEventArgs result)
        {
            if (!result.Success)
                return;
            if (paymmant is AskForUsersDelyveryMW)
            {
                ((AskForUsersDelyveryMW)paymmant).VideoRecount = new Models.Video()
                {
                    path = $"../Video/{((AskForUsersDelyveryMW)paymmant).VehiclwInformation.Id}/RecountPay.mp4",
                    VideoBase64 = Convert.ToBase64String(result.Result)
                };
                IVehicle Car = ((AskForUsersDelyveryMW)paymmant).GetTypeCar(((AskForUsersDelyveryMW)paymmant).vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
                FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(((AskForUsersDelyveryMW)paymmant).managerDispatchMob, ((AskForUsersDelyveryMW)paymmant).vehiclwInformation, ((AskForUsersDelyveryMW)paymmant).IdShip,
                    $"{((AskForUsersDelyveryMW)paymmant).vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", ((AskForUsersDelyveryMW)paymmant).vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""),
                    ((AskForUsersDelyveryMW)paymmant).InderxPhotoInspektion + 1, ((AskForUsersDelyveryMW)paymmant).initDasbordDelegate, ((AskForUsersDelyveryMW)paymmant).getVechicleDelegate, Car.GetNameLayout(1),
                    ((AskForUsersDelyveryMW)paymmant).Payment, ((AskForUsersDelyveryMW)paymmant).TotalPaymentToCarrier);
                await Navigation.PushAsync(fullPagePhotoDelyvery, true);
                await Navigation.PushAsync(new CameraPagePhoto1($"{((AskForUsersDelyveryMW)paymmant).vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", fullPagePhotoDelyvery, "PhotoIspection"));
                ((AskForUsersDelyveryMW)paymmant).SaveRecountVideo();
            }
            else
            {
                ((LiabilityAndInsuranceMV)paymmant).VideoRecount = new Models.Video()
                {
                    path = $"../Video/{((LiabilityAndInsuranceMV)paymmant).IdVech}/RecountPay.mp4",
                    VideoBase64 = Convert.ToBase64String(result.Result)
                };
                bool isProblem = await ((LiabilityAndInsuranceMV)paymmant).CheckProplem();

                await Navigation.PushAsync(new Ask2Page(((LiabilityAndInsuranceMV)paymmant).managerDispatchMob, ((LiabilityAndInsuranceMV)paymmant).IdVech, ((LiabilityAndInsuranceMV)paymmant).IdShip, ((LiabilityAndInsuranceMV)paymmant).initDasbordDelegate, isProblem));
                ((LiabilityAndInsuranceMV)paymmant).SaveRecountVideo();
            }

        }

        protected override void OnDisappearing()
        {
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            base.OnDisappearing();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}