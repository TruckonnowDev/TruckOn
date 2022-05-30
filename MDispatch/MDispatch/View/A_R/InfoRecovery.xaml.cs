using MDispatch.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.A_R
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoRecovery : PopupPage
    {
        public InfoRecovery(AvtorizationMV avtorizationMV)
        {
            InitializeComponent();
            BindingContext = avtorizationMV;
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}