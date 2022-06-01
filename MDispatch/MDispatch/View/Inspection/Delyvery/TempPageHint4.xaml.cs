using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection.PickedUp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TempPageHint4 : PopupPage
    {
		public TempPageHint4 ()
		{
			InitializeComponent ();
		}

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}