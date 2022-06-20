using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.View.Inspection.PickedUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraStrapAndTrack : CameraPage
    {
        private CameraStrapAndTrackMV cameraStrapAndTrackMV = null;
        private int countPhoto = 1;
        private string nameVech = null;

        public CameraStrapAndTrack(IManagerDispatchMobService managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, InitDasbordDelegate initDasbordDelegate, GetVechicleDelegate getVechicleDelegate,
            string onDeliveryToCarrier, string totalPaymentToCarrier, string nameVehicl)
        {
            cameraStrapAndTrackMV = new CameraStrapAndTrackMV(
                managerDispatchMob, vehiclwInformation,
                idShip, initDasbordDelegate, getVechicleDelegate,
                onDeliveryToCarrier, totalPaymentToCarrier, nameVehicl,
                Navigation);
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True)
                .SetPreferredStatusBarUpdateAnimation(UIStatusBarAnimation.Fade);
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            paternPhoto.Source = $"{cameraStrapAndTrackMV.Car.TypeIndex.Replace(" ", "")}{cameraStrapAndTrackMV.Car.GetIndexCar(countPhoto)}.png";
            titlePhoto.Text = cameraStrapAndTrackMV.Car.GetNameLayout(cameraStrapAndTrackMV.Car.GetIndexCar(countPhoto));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        [Obsolete]
        private async void CameraPage_OnPhotoResult(PhotoResultEventArgs result)
        {
            if (!result.Success)
                return;
            Photo photo1 = new Photo();
            photo1.Base64 = Convert.ToBase64String(result.Result);
            countPhoto++;
            if (cameraStrapAndTrackMV.Car.GetIndexCar(countPhoto) == 0)
            {
                photo1.path = $"../Photo/{cameraStrapAndTrackMV.VehiclwInformation.Id}/PikedUp/CameraSeatBelts/{countPhoto}.jpg";
                cameraStrapAndTrackMV.straps.Add(photo1);
                cameraStrapAndTrackMV.SavePhotoInTruck();
                return;
            }
            else if (cameraStrapAndTrackMV.Car.GetIndexCar(countPhoto) > 0)
            {
                photo1.path = $"../Photo/{cameraStrapAndTrackMV.VehiclwInformation.Id}/PikedUp/CameraTrack/{countPhoto}.jpg"; 
                if (cameraStrapAndTrackMV.Car.GetIndexCar(countPhoto) > cameraStrapAndTrackMV.Car.CountCarImg)
                {
                    cameraStrapAndTrackMV.straps.Add(photo1);
                }
                else
                {
                    cameraStrapAndTrackMV.inTruck.Add(photo1);
                }
                paternPhoto.Source = $"{cameraStrapAndTrackMV.Car.TypeIndex.Replace(" ", "")}{cameraStrapAndTrackMV.Car.GetIndexCar(countPhoto)}.png";
                titlePhoto.Text = cameraStrapAndTrackMV.Car.GetNameLayout(cameraStrapAndTrackMV.Car.GetIndexCar(countPhoto));
            }
        }
    }
}