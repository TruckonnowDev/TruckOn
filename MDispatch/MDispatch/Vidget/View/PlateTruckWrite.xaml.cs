using MDispatch.Vidget.VM;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System.Linq;
using Xamarin.Forms;
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

        [System.Obsolete]
        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            await PopupNavigation.PopAllAsync();
            fullPhotoTruckVM.BackToRootPage();
        }

        [System.Obsolete]
        private void Button_Clicked(object sender, System.EventArgs e)
        {
            fullPhotoTruckVM.SetPlate("Truck");
        }

        private async void Button_Clicked_1(object sender, System.EventArgs e)
        {
            await PopupNavigation.PopAllAsync();
            await fullPhotoTruckVM.Navigation.PushAsync(new ScanCamera(fullPhotoTruckVM, "truck"));
        }

        private void Entry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x)); //Make sure all characters are numbers

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }
        }
    }
}