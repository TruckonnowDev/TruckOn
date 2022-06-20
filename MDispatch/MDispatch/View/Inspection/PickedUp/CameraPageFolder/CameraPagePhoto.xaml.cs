using MDispatch.NewElement;
using MDispatch.View.Inspection.PickedUp;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.PageApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CameraPagePhoto : CameraPage
    {
        private FullPagePhoto fullPagePhoto = null;
        private PageAddDamage pageAddDamage = null;

        public CameraPagePhoto(string pngPaternPhoto, FullPagePhoto fullPagePhoto, string typeCamera, PageAddDamage pageAddDamage = null)
		{
            this.TypeCamera = typeCamera;
            this.pageAddDamage = pageAddDamage;
            this.fullPagePhoto = fullPagePhoto;
            InitializeComponent ();
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True)
                .SetPreferredStatusBarUpdateAnimation(UIStatusBarAnimation.Fade);
            if (pngPaternPhoto != null)
            {
                paternPhoto.Source = pngPaternPhoto;
            }
        }

        private async void CameraPage_OnPhotoResult(PhotoResultEventArgs result)
        {
            if (!result.Success)
            {
                return;
            }
            await fullPagePhoto.fullPagePhotoMV.AddNewFotoSourse(result.Result);
            await fullPagePhoto.fullPagePhotoMV.SetPhoto(result.Result, result.Width, result.Height);
            await fullPagePhoto.SetbtnVisable();
            if (pageAddDamage != null)
            {
                pageAddDamage.stateSelect = 0;
            }
            await Navigation.PopAsync(true);
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            if (pageAddDamage != null)
            {
                pageAddDamage.stateSelect = 0;
            }
            await Navigation.PopAsync();
        }

        [System.Obsolete]
        private async void CameraPage_OnPhotoinspectionResult(PhotoResultEventArgs result)
        {
            if (!result.Success)
            {
                return;
            }
            await fullPagePhoto.fullPagePhotoMV.AddNewFotoSourse(result.Result);
            await fullPagePhoto.fullPagePhotoMV.SetPhoto(result.Result, result.Width, result.Height);
            await fullPagePhoto.SetbtnVisable();
            if (pageAddDamage != null)
            {
                pageAddDamage.stateSelect = 0;
            }
            //await Navigation.PopAsync(true);
            fullPagePhoto.fullPagePhotoMV.SavePhoto(true);
        }
    }
}