using MDispatch.Models;
using MDispatch.Service;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.ViewModels.PageAppMV;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.PageApp.DialogPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPayment : PopupPage
    {
        EditPaymentInfoMV editPaymentInfoMV = null;

        public EditPayment(IManagerDispatchMobService managerDispatchMob, Shipping shipping)
        {
            editPaymentInfoMV = new EditPaymentInfoMV(managerDispatchMob, shipping, Navigation);
            InitializeComponent();
            BindingContext = editPaymentInfoMV;
            Init(shipping);
        }

        private async void Init(Shipping shipping)
        {
            if (shipping.PriceListed == null || shipping.PriceListed == "")
            {
                await lAmount.TranslateTo(0, 13, 0);
                await lAmount.FadeTo(0.5, 0);
            }
            else
            {
                await lAmount.TranslateTo(0, 8, 0);
                lAmount.TextColor = Color.FromHex("#131313");
            }
        }

        private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length != 0)
            {
                await Task.WhenAll(
                    lAmount.TranslateTo(0, 8, 150),
                    lAmount.FadeTo(1, 150)
                    );
                lAmount.TextColor = Color.FromHex("#131313");
            }
            else
            {
                await Task.WhenAll(
                    lAmount.TranslateTo(0, 33, 150),
                    lAmount.FadeTo(0.5, 150)
                    );
                lAmount.TextColor = Color.FromHex("#b8babb");
            }
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync(true);
        }
    }
}