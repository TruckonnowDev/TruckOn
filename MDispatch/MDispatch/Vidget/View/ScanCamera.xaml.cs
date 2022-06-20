using MDispatch.Vidget.VM;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using MDispatch.View.Popups;

namespace MDispatch.Vidget.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanCamera : MDispatch.NewElement.CameraPage
    {
        private FullPhotoTruckVM fullPhotoTruckVM = null;
        private string typeDetect = null;

        public ScanCamera(FullPhotoTruckVM fullPhotoTruckVM, string typeDetect)
        {
            this.typeDetect = typeDetect;
            this.fullPhotoTruckVM = fullPhotoTruckVM;
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True)
               .SetPreferredStatusBarUpdateAnimation(UIStatusBarAnimation.Fade);
        }

        [Obsolete]
        private async void CameraPage_OnScan(NewElement.PhotoResultEventArgs result)
        {
            if (!result.Success)
            {
                return;
            }
            fullPhotoTruckVM.DetectText(result.Result, typeDetect);
            await Navigation.PopAsync();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            if (fullPhotoTruckVM.PlateTruck == null || fullPhotoTruckVM.PlateTruck == "")
            {
                await PopupNavigation.Instance.PushAsync(new PlateTruckWrite(fullPhotoTruckVM));
            }
            else if (fullPhotoTruckVM.PlateTrailer == null || fullPhotoTruckVM.PlateTrailer == "")
            {
                await PopupNavigation.Instance.PushAsync(new MDispatch.View.Popups.PlateTrailerWritePopupView(fullPhotoTruckVM));
            }
        }
    }
}