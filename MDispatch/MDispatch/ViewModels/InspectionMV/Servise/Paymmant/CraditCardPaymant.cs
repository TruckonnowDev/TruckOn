using MDispatch.Helpers;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection.Delyvery;
using MDispatch.View.Inspection.PickedUp;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using Xamarin.Forms;

namespace MDispatch.ViewModels.InspectionMV.Servise.Paymmant
{
    class CraditCardPaymant : IPaymmant
    {
        public AskForUserDelyvery AskForUserDelyvery { get; set; }
        public LiabilityAndInsurance LiabilityAndInsurance { get; set; }

        StackLayout stackLayout = null;
        public bool IsAskPaymmant { get; set; }

        public StackLayout GetStackLayout()
        {
            string fontBold = ((OnPlatform<string>)Application.Current.Resources["OpenSans-Bold"]).Platforms.ToList().First(p => p.Platform.FirstOrDefault(pp => pp == Device.RuntimePlatform) != null).Value.ToString();
            string fontRegular = ((OnPlatform<string>)Application.Current.Resources["OpenSans-Regular"]).Platforms.ToList().First(p => p.Platform.FirstOrDefault(pp => pp == Device.RuntimePlatform) != null).Value.ToString();
            Entry entry = new Entry();
            Button button = new Button();
            FlexLayout flexLayout = new FlexLayout();

            entry.Keyboard = Keyboard.Numeric;
            entry.Placeholder = "$";
            entry.TextChanged += EntryTextChange;
            entry.FontSize = 14;
            entry.PlaceholderColor = Color.FromHex("#101010");
            entry.FontFamily = fontRegular;
            if (Device.RuntimePlatform == Device.iOS)
            {
                entry.HeightRequest = 19;
            }
            else
            {
                entry.HeightRequest = 40;
            }

            button.Text = "Paid";
            button.BackgroundColor = Color.White;
            button.TextColor = Color.FromHex("#2C5DEB"); ;
            button.Clicked += ClickBtn;
            button.FontSize = 16;
            button.FontFamily = fontBold;
            button.HeightRequest = 19;
            button.Padding = new Thickness(0);

            stackLayout = new StackLayout();
            stackLayout.Spacing = 10;
            stackLayout.Children.Add(entry);
            flexLayout.Children.Add(button);
            stackLayout.Children.Add(flexLayout);
            return stackLayout;
        }

        private async void ClickBtn(object sender, EventArgs e)
        {
            if (((Entry)stackLayout.Children[0]).Text != null && ((Entry)stackLayout.Children[0]).Text.Length > 0)
            {
                IsAskPaymmant = true;
                stackLayout.IsEnabled = false;
                await PopupNavigation.PushAsync(new Alert($"{LanguageHelper.GiveMoneyAlert} {((Entry)stackLayout.Children[0]).Text}", null));
            }
            else
            {
                IsAskPaymmant = false;
                await PopupNavigation.PushAsync(new Alert(LanguageHelper.PaymentForDeliveryAlert, null));
            }
        }

        private void EntryTextChange(object sender, TextChangedEventArgs e)
        {
            if (AskForUserDelyvery != null)
            {
                AskForUserDelyvery.askForUsersDelyveryMW.AskForUserDelyveryM.CountPay = e.NewTextValue;
            }
            else
            {
                LiabilityAndInsurance.liabilityAndInsuranceMV.CountPay = e.NewTextValue;
            }
        }

        public CraditCardPaymant(object page)
        {
            AskForUserDelyvery = (page as AskForUserDelyvery);
            if (AskForUserDelyvery == null)
            {
                LiabilityAndInsurance = (LiabilityAndInsurance)page;
            }
        }
    }
}
