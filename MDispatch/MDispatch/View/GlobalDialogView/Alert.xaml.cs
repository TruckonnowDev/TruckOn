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
        private bool goBackAfterAnswer;

        public Alert(string info, INavigation navigation, bool goBackAfterAnswer = true)
        {
            this.navigation = navigation;
            this.goBackAfterAnswer = goBackAfterAnswer;
            InitializeComponent();
            infoL.Text = info;
        }

        [System.Obsolete]
        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await PopupNavigation.PopAsync(true);
            if(navigation != null && goBackAfterAnswer)
            {
                await navigation.PopAsync();
            }
        }
    }
}