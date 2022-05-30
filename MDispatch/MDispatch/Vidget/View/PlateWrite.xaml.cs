using MDispatch.Vidget.VM;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace MDispatch.Vidget.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlateWrite : PopupPage
    {
        private FullPhotoTruckVM fullPhotoTruckVM = null;

        public PlateWrite(FullPhotoTruckVM fullPhotoTruckVM)
        {
            this.fullPhotoTruckVM = fullPhotoTruckVM;
            InitializeComponent();
            BindingContext = fullPhotoTruckVM;
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        [System.Obsolete]
        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
            fullPhotoTruckVM.BackToRootPage();
        }

        [System.Obsolete]
        private void Button_Clicked(object sender, System.EventArgs e)
        {
            //fullPhotoTruckVM.SetPlate();
        }
    }
}