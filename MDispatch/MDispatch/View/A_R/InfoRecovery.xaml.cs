using MDispatch.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.A_R
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoRecovery : PopupPage
    {
        private AvtorizationMV vm;

        public InfoRecovery(AvtorizationMV avtorizationMV)
        {
            InitializeComponent();
            BindingContext = vm = avtorizationMV;
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await vm._popupNavigation.PopAsync();
        }
    }
}