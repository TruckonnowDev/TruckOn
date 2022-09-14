using MDispatch.NewElement;
using MDispatch.View.Inspection.PickedUp;
using MDispatch.View.PageApp;
using MDispatch.ViewModels.AskPhoto;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using MDispatch.ViewModels.InspectionMV.Servise.Models;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPaymmant : CameraPage
    {
        private object paymmant = null;

        public CameraPaymmant(object paymmant, string instructionAndNamePaymmant, string paymmentpattern)
        {
            this.paymmant = paymmant;
            InitializeComponent();
            paternPayment.Source = paymmentpattern;
            On<iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True)
                .SetPreferredStatusBarUpdateAnimation(UIStatusBarAnimation.Fade);
            NamaPayment.Text = instructionAndNamePaymmant;
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
        }

        [System.Obsolete]
        private async void CameraPage_OnPhotoResult(NewElement.PhotoResultEventArgs result)
        {
            if (!result.Success)
                return;
            if(paymmant is AskForUsersDelyveryMW)
            {
                IVehicle Car = ((AskForUsersDelyveryMW)paymmant).GetTypeCar(((AskForUsersDelyveryMW)paymmant).vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
                FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(((AskForUsersDelyveryMW)paymmant).managerDispatchMob, ((AskForUsersDelyveryMW)paymmant).vehiclwInformation, ((AskForUsersDelyveryMW)paymmant).IdShip, 
                    $"{((AskForUsersDelyveryMW)paymmant).vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", ((AskForUsersDelyveryMW)paymmant).vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""), 
                    ((AskForUsersDelyveryMW)paymmant).InderxPhotoInspektion + 1, ((AskForUsersDelyveryMW)paymmant).initDasbordDelegate, ((AskForUsersDelyveryMW)paymmant).getVechicleDelegate, Car.GetNameLayout(1), 
                    ((AskForUsersDelyveryMW)paymmant).Payment, ((AskForUsersDelyveryMW)paymmant).TotalPaymentToCarrier);
                await Navigation.PushAsync(fullPagePhotoDelyvery, true);
                await Navigation.PushAsync(new CameraPagePhoto1($"{((AskForUsersDelyveryMW)paymmant).vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", fullPagePhotoDelyvery, "PhotoIspection"));
                ((AskForUsersDelyveryMW)paymmant).AddPhoto(result.Result);
            }
            else
            {
                bool isProblem = await ((LiabilityAndInsuranceMV)paymmant).CheckProplem();
                await Navigation.PushAsync(new Ask2Page(((LiabilityAndInsuranceMV)paymmant).managerDispatchMob, ((LiabilityAndInsuranceMV)paymmant).IdVech, ((LiabilityAndInsuranceMV)paymmant).IdShip, ((LiabilityAndInsuranceMV)paymmant).initDasbordDelegate, isProblem));
                ((LiabilityAndInsuranceMV)paymmant).AddPhoto(result.Result);
            }
        }

        protected override void OnDisappearing()
        {
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            base.OnDisappearing();
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}