using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection.PickedUp.CameraPageFolder;
using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using Plugin.InputKit.Shared.Controls;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.View.Inspection.PickedUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Ask2Page : ContentPage
    {
        private Ask2PageMW ask2PageMW = null;

        public Ask2Page(ManagerDispatchMob managerDispatchMob, string idVech, string idShip, InitDasbordDelegate initDasbordDelegate, bool isProblem)
        {
            ask2PageMW = new Ask2PageMW(managerDispatchMob, idVech, idShip, Navigation, initDasbordDelegate, isProblem);
            ask2PageMW.Ask2 = new Ask2();
            InitializeComponent();
            BindingContext = ask2PageMW;
        }

        #region Ask1
        bool isAsk1 = false;
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != "")
            {
                isAsk1 = true;
            }
            else
            {
                isAsk1 = false;
            }
            ask2PageMW.Ask2.How_many_keys_total_you_been_given = e.NewTextValue;
        }
        #endregion

        #region Ask2
        bool isAsk2 = false;
        Button button2 = null;
        private void Button_Clicked_2(object sender, EventArgs e)
        {
            isAsk2 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            if (button2 != null)
            {
                button2.TextColor = Color.FromHex("#C1C1C1");
            }
            button2 = button;
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            if (button2 != null)
            {
                button2.TextColor = Color.FromHex("#C1C1C1");
            }
            button2 = button;
            await Navigation.PushAsync(new CameraDocumment(this));
        }

        internal void AddPhotoDocumments(byte[] result)
        {
            isAsk2 = true;
            if (ask2PageMW.Ask2.Any_additional_documentation_been_given_after_loading == null)
            {
                ask2PageMW.Ask2.Any_additional_documentation_been_given_after_loading = new List<Models.Photo>();
            }
            Models.Photo photo = new Models.Photo();
            photo.Base64 = Convert.ToBase64String(result);
            photo.path = $"../Photo/{ask2PageMW.IdVech}/PikedUp/Documment/{ask2PageMW.Ask2.Any_additional_documentation_been_given_after_loading.Count + 1}.jpg";
            ask2PageMW.Ask2.Any_additional_documentation_been_given_after_loading.Add(photo);
            Image image = new Image()
            {
                Source = ImageSource.FromStream(() => new MemoryStream(result)),
                HeightRequest = 50,
                WidthRequest = 50,
            };
            //image.GestureRecognizers.Add(new TapGestureRecognizer(ViewPhotoForRetacke1));
            blockAskPhotoDocumments.Children.Add(image);
        }
        #endregion

        #region Ask3
        bool isAsk3 = false;
        Button button3 = null;
        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            if (button3 != null)
            {
                button3.TextColor = Color.FromHex("#C1C1C1");
            }
            button3 = button;
            await Navigation.PushAsync(new CameraPartsBeen(this));

        }

        private async void Button_Clicked_4(object sender, EventArgs e)
        {
            isAsk3 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            if (button3 != null)
            {
                button3.TextColor = Color.FromHex("#C1C1C1");
            }
            button3 = button;
        }

        internal void AddPhotoPartsBeen(byte[] result)
        {
            isAsk3 = true;
            if (ask2PageMW.Ask2.Any_additional_parts_been_given_to_you == null)
            {
                ask2PageMW.Ask2.Any_additional_parts_been_given_to_you = new List<Models.Photo>();
            }
            Models.Photo photo = new Models.Photo();
            photo.Base64 = Convert.ToBase64String(result);
            photo.path = $"../Photo/{ask2PageMW.IdVech}/PikedUp/Documment/{ask2PageMW.Ask2.Any_additional_parts_been_given_to_you.Count + 1}.jpg";
            ask2PageMW.Ask2.Any_additional_parts_been_given_to_you.Add(photo);
            Image image = new Image()
            {
                Source = ImageSource.FromStream(() => new MemoryStream(result)),
                HeightRequest = 50,
                WidthRequest = 50,
            };
            //image.GestureRecognizers.Add(new TapGestureRecognizer(ViewPhotoForRetacke1));
            blockAskPhotoPartsBeen.Children.Add(image);
        }
        #endregion

        #region Ask4
        Button button4 = null;
        bool isAsk4 = false;
        private void Button_Clicked4(object sender, EventArgs e)
        {
            isAsk4 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            ask2PageMW.Ask2.Car_locked = button.Text;
            if (button4 != null)
            {
                button4.TextColor = Color.FromHex("#C1C1C1");
            }
            button4 = button;
        }
        #endregion

        #region Ask5
        bool isAsk5 = false;
        Button button5= null;
        private void Button_Clicked(object sender, EventArgs e)
        {
            isAsk5 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            ask2PageMW.Ask2.Car_locked = button.Text;
            if (button5 != null)
            {
                button5.TextColor = Color.FromHex("#C1C1C1");
            }
            button5 = button;
        }
        #endregion

        #region Ask6
        bool isAsk6 = false;
        private void AdvancedSlider_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                isAsk6 = true;
                ask2PageMW.Ask2.Client_friendliness = ((AdvancedSlider)sender).Value.ToString();
            }
        }
        #endregion  

        private void PhoneNumber_clicked(object sender, EventArgs e)
        {
            PhoneDialer.Open("+17734305155");
        }

        [Obsolete]
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (isAsk1 && isAsk3 && isAsk4 && isAsk5 && isAsk6)
            {
                ask2PageMW.SaveAsk();
            }
            else
            {
                await PopupNavigation.PushAsync(new Alert(LanguageHelper.AskErrorAlert, null));
                CheckAsk();
            }
        }

        private void CheckAsk()
        {
            if (!isAsk1)
            {
                askBlock1.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock1.BorderColor = Color.White;
            }
            if (!isAsk3)
            {
                askBlock3.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock3.BorderColor = Color.White;
            }
            if (!isAsk4)
            {
                askBlock4.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock4.BorderColor = Color.White;
            }
            if (!isAsk5)
            {
                askBlock5.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock5.BorderColor = Color.White;
            }
            if (!isAsk6)
            {
                askBlock6.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock6.BorderColor = Color.White;
            }
        }

        private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BOLPage(ask2PageMW.managerDispatchMob, ask2PageMW.IdShip, ask2PageMW.initDasbordDelegate));
        }

        private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
        {
            //await PopupNavigation.PushAsync(new ContactInfo());
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            HelpersView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            HelpersView.Hidden();
        }
    }
}