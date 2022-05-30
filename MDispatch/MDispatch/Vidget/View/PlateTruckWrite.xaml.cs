using MDispatch.Vidget.VM;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;


namespace MDispatch.Vidget.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlateTruckWrite : PopupPage
    {
        private FullPhotoTruckVM fullPhotoTruckVM = null;

        public PlateTruckWrite(FullPhotoTruckVM fullPhotoTruckVM)
        {
            this.fullPhotoTruckVM = fullPhotoTruckVM;
            InitializeComponent();
            BindingContext = fullPhotoTruckVM;
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
            fullPhotoTruckVM.BackToRootPage();
        }

        [System.Obsolete]
        private void Button_Clicked(object sender, System.EventArgs e)
        {
            fullPhotoTruckVM.SetPlate("Truck");
        }

        private async void Button_Clicked_1(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
            await fullPhotoTruckVM.Navigation.PushAsync(new ScanCamera(fullPhotoTruckVM, "truck"));
        }
    }
}