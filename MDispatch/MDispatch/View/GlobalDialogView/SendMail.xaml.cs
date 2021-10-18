using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.GlobalDialogView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendMail : PopupPage
    {
        public SendMail()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                if (msg.Text != null && msg.Text != "")
                {
                    string subjStr = subj.Text != null ? subj.Text : "";
                    EmailMessage message = new EmailMessage
                    {
                        Subject = $"App msg ({subjStr})",
                        Body = $"{msg.Text}",
                        To = new List<string>() { "chuprina.r.v@gmail.com" },
                        //Cc = ccRecipients,
                        //Bcc = bccRecipients
                    };
                    await Email.ComposeAsync(message);
                }
                else
                {
                    warn.IsVisible = true;
                }
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {

            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }
    }
}