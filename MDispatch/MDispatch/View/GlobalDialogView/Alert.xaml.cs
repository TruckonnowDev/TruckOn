using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.GlobalDialogView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Alert : PopupPage
    {
        private INavigation navigation = null;

        public Alert(string info, INavigation navigation)
        {
            this.navigation = navigation;
            InitializeComponent();
            infoL.Text = info;
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(true);
            if(navigation != null)
            {
                await navigation.PopAsync();
            }
        }
    }
}