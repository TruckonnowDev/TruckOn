using MDispatch.Vidget.View;
using MDispatch.Vidget.VM;
using MDispatch.ViewModels.Popups;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlateTrailerWritePopupView : PopupPage
    {
        public PlateTrailerWritePopupView(FullPhotoTruckVM fullPhotoTruckVM)
        {
            InitializeComponent();
            BindingContext = new PlateTrailerWritePopupPageViewModel(fullPhotoTruckVM, Navigation);
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        [System.Obsolete]
        private async void Button_Clicked_1(object sender, System.EventArgs e)
        {
            
        }
    }
}