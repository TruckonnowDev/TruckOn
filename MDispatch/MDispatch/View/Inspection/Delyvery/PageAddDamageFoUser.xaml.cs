using MDispatch.NewElement;
using MDispatch.NewElement.ResIzeImage;
using MDispatch.View.Inspection.Delyvery.CameraPage;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using Rg.Plugins.Popup.Services;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection.PickedUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageAddDamageFoUser : ContentPage
	{
        private AskForUsersDelyveryMW askForUsersDelyveryMW = null;
        public int stateSelect = 0;
        public int indexSelectDamage = 0;
        public string nameDamage = null;
        public string prefNameDamage = null;
        private StackLayout stackLayout = null;
        private Delyvery.AskForUserDelyvery askForUserDelyvery = null;

        public PageAddDamageFoUser(AskForUsersDelyveryMW askForUsersDelyveryMW, StackLayout stackLayout, Delyvery.AskForUserDelyvery askForUserDelyvery)
        {
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
            this.askForUserDelyvery = askForUserDelyvery;
            this.stackLayout = stackLayout;
            this.askForUsersDelyveryMW = askForUsersDelyveryMW;
            InitializeComponent();
            touchImage.Source = $"scan{askForUsersDelyveryMW.VehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}.jpg";
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True)
                .SetPreferredStatusBarUpdateAnimation(UIStatusBarAnimation.Fade);
        }

        private async void TouchImage_TouchAction(object sender, NewElement.TouchCordinate.TouchActionEventArgs e)
        {
            stateSelect = 1;
            await askForUsersDelyveryMW._popupNavigation.PushAsync(new DamageSelecter1(this, null), true);
            await WaiteSelectDamage();
            await askForUsersDelyveryMW._popupNavigation.PopAsync(true);
            if (stateSelect == 0)
            {
                stateSelect = 1;
                await Navigation.PushAsync(new CameraAdditionalPhoto(askForUserDelyvery, this));
                await WaiteSelectDamage();
                ImgResize image = new ImgResize()
                {
                    Source = $"DamageD{indexSelectDamage}.png",
                    WidthRequest = 25,
                    HeightRequest = 25,
                };
                image.OneTabAction += RemovedDamag;
                AbsoluteLayout.SetLayoutBounds(image, new Rectangle(e.XInterest * 0.0001, e.YInterest * 0.0001, 25, 25));
                AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.PositionProportional);
                absla.Children.Add(image);
                await Task.Run(() =>
                {
                    askForUsersDelyveryMW.SetDamage(nameDamage, indexSelectDamage, prefNameDamage, e.XInterest * 0.0001, e.YInterest * 0.0001, image, askForUsersDelyveryMW.AskForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo.Last().Base64);
                });
            }
            else
            {
                stateSelect = 0;
            }
        }

        private async void RemovedDamag(object sender)
        {
            absla.Children.Remove((ImgResize)sender);
            askForUsersDelyveryMW.RemmoveDamage((ImgResize)sender, stackLayout);
        }

        private async Task WaiteSelectDamage()
        {
            await Task.Run(() =>
            {
                while(stateSelect == 1)
                { }
            });
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            TapGestureRecognizer_Tapped(null, null);
            return base.OnBackButtonPressed();
        }
    }
}