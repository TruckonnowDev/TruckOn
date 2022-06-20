using MDispatch.ViewModels;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.A_R
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Avtorization : ContentPage
	{
        private AvtorizationMV avtorizationMV = null;

        public Avtorization ()
		{
            InitializeComponent ();
            avtorizationMV = new AvtorizationMV(Navigation);
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = avtorizationMV;
        }

        private async void Entry_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            ValidationInpEmail(e.NewTextValue);
        }

        private void ValidationInpEmail(string email)
        {
            if (IsValidEmail(email))
            {
                doneImg.IsVisible = true;
                unDoneImg.IsVisible = false;
            }
            else
            {
                doneImg.IsVisible = false;
                unDoneImg.IsVisible = true;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            await avtorizationMV._popupNavigation.PushAsync(new ForgotPassword(avtorizationMV));
        }

        void TapGestureRecognizer_Tapped_1(System.Object sender, System.EventArgs e)
        {
            if(passwordEnt.IsPassword)
            {
                hdPswImg.Source = ImageSource.FromFile("hiddenPsw.png");
                passwordEnt.IsPassword = false;
            }
            else
            {
                hdPswImg.Source = ImageSource.FromFile("unHiddenPsw.png");
                passwordEnt.IsPassword = true;
            }
        }
    }
}