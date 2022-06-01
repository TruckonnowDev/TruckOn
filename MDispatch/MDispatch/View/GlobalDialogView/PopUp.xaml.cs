using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.GlobalDialogView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopUp : PopupPage
    {
        public PopUp(string title, string body)
        {
            InitializeComponent();
            titleL.Text = title;
            bodyL.Text = body;
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}
