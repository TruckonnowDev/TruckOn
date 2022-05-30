using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection.Delyvery
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TempDialogPage : PopupPage
    {
		public TempDialogPage ()
		{
			InitializeComponent ();
		}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
        }
    }
}