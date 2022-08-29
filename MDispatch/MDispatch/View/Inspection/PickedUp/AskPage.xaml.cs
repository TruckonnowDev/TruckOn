using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.View.AskPhoto.CameraPageFolder;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using MDispatch.ViewModels.AskPhoto;
using Newtonsoft.Json;
using Plugin.InputKit.Shared.Controls;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.View.AskPhoto
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AskPage : ContentPage
    {
        AskPageMV askPageMV = null;

        public AskPage(ManagerDispatchMob managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, InitDasbordDelegate initDasbordDelegate, GetVechicleDelegate getVechicleDelegate,
            string onDeliveryToCarrier, string totalPaymentToCarrier)
        {
            askPageMV = new AskPageMV(managerDispatchMob, vehiclwInformation, idShip, Navigation, initDasbordDelegate, getVechicleDelegate, onDeliveryToCarrier, totalPaymentToCarrier);
            InitializeComponent();
            BindingContext = askPageMV;
            NavigationPage.SetBackButtonTitle(this, "LOL");
        }


        #region Ask1
        Button button1 = null;
        bool isAsk1 = false;
        private void Button_Clicked(object sender, EventArgs e)
        {
            isAsk1 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askPageMV.Ask.Lightbrightness = button.Text;
            if (button1 != null)
            {
                button1.TextColor = Color.FromHex("#C1C1C1");
            }
            button1 = button;
        }
        #endregion

        #region Ask2
        Button button2 = null;
        bool isAsk2 = false;
        private void Button_Clicked_1(object sender, EventArgs e)
        {
            isAsk2 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askPageMV.Ask.Vehicle = button.Text;
            if (button2 != null)
            {
                button2.TextColor = Color.FromHex("#C1C1C1");
            }
            button2 = button;
        }
        #endregion

        #region Ask12
        Button button12 = null;
        bool isAsk12 = false;
        private void Button_Clicked_7(object sender, EventArgs e)
        {
            isAsk12 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askPageMV.Ask.Safe_delivery_location = button.Text;
            if (button12 != null)
            {
                button12.TextColor = Color.FromHex("#C1C1C1");
            }
            button12 = button;
        }
        #endregion

        #region Ask15
        bool isAsk15 = false;
        Button button15 = null;
        private void Button_Clicked_8(object sender, EventArgs e)
        {
            isAsk15 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askPageMV.Ask.Enough_space_to_take_pictures = button.Text;
            if (button15 != null)
            {
                button15.TextColor = Color.FromHex("#C1C1C1");
            }
            button15 = button;
        }
        #endregion

        #region Ask4
        bool isAsk4 = false;
        Button button4 = null;
        private void RadioButton_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askPageMV.Ask.Weather_conditions = button.Text;
            if (button4 != null)
            {
                button4.TextColor = Color.FromHex("#C1C1C1");
            }
            button4 = button;
            isAsk4 = true;
        }
        #endregion

        #region Ask7
        bool isAsk7 = false;
        string txtAnswer7 = "";
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            isAsk7 = true;
            txtAnswer7 = e.NewTextValue;
            askPageMV.Ask.Anyone_Rushing_you_to_perform_the_inspection = $"{btnAnswer7} {txtAnswer7}";
        }

        Button button7 = null;
        string btnAnswer7 = "";
        private void Button_Clicked_2(object sender, EventArgs e)
        {
            isAsk1 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            btnAnswer7 = button.Text;
            askPageMV.Ask.Anyone_Rushing_you_to_perform_the_inspection = $"{btnAnswer7} {txtAnswer7}";
            if (button7 != null)
            {
                button7.TextColor = Color.FromHex("#C1C1C1");
            }
            button7 = button;
            if(button.Text == LanguageHelper.YesBtnText)
            {
                askBlock7v2.IsVisible = true;
                if (txtAnswer7 != "")
                {
                    isAsk7 = true;
                }
                else
                {
                    isAsk7 = false;
                }
            }
            else
            {
                askBlock7v2.IsVisible = false;
                isAsk7 = true;
            }
        }
        #endregion

        #region Ask9
        bool isAsk9 = false;
        private void Entry_TextChanged_3(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != "")
            {
                isAsk9 = true;
            }
            else
            {
                isAsk9 = false;
            }
            askPageMV.Ask.Plate = e.NewTextValue;
        }
        #endregion

        #region Ask13
        bool isAsk13 = false;
        private void Entry_TextChanged_4(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != "")
            {
                isAsk13 = true;
            }
            else
            {
                isAsk13 = false;
            }
            askPageMV.Ask.How_far_from_trailer = e.NewTextValue;
        }
        #endregion

        #region Ask14
        bool isAsk14 = false;
        private void Entry_TextChanged_5(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != "")
            {
                isAsk14 = true;
            }
            else
            {
                isAsk14 = false;
            }
            askPageMV.Ask.Name_of_the_person_who_gave_you_keys = e.NewTextValue;
        }
        #endregion

        #region Ask11
        bool isAsk11 = false;
        private void Dropdown_SelectedItemChanged_1(object sender, Plugin.InputKit.Shared.Utils.SelectedItemChangedArgs e)
        {
            isAsk11 = true;
            askPageMV.Ask.TypeVehicle = (string)e.NewItem;
            askPageMV.VehiclwInformation.Type = askPageMV.Ask.TypeVehicle;
        }
        #endregion

        [Obsolete]
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (isAsk1 && isAsk2 && isAsk12 && isAsk4 && isAsk13 && isAsk14 && isAsk7 && isAsk14 && isAsk9 && isAsk10 && isAsk11)
            {
                askPageMV.SaveAsk(askPageMV.Ask.TypeVehicle.Replace(" ", ""));
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
            if (!isAsk2)
            {
                askBlock2.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock2.BorderColor = Color.White;
            }
            if (!isAsk12)
            {
                askBlock12.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock12.BorderColor = Color.White;
            }
            if (!isAsk4)
            {
                askBlock4.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock4.BorderColor = Color.White;
            }
            if (!isAsk13)
            {
                askBlock13.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock13.BorderColor = Color.White;
            }
            if (!isAsk14)
            {
                askBlock14.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock14.BorderColor = Color.White;
            }
            if (!isAsk7)
            {
                askBlock7.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock7.BorderColor = Color.White;
            }
            if (!isAsk15)
            {
                askBlock15.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock15.BorderColor = Color.White;
            }
            if (!isAsk9)
            {
                askBlock9.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock9.BorderColor = Color.White;
            }
            if (!isAsk10)
            {
                askBlock10.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock10.BorderColor = Color.White;
            }
            if (!isAsk11)
            {
                askBlock11.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock11.BorderColor = Color.White;
            }
        }

        #region Ask10
        bool isAsk10 = false;
        Button button10 = null;
        private async void Button_Clicked_6(object sender, EventArgs e)
        {
            isAsk10 = true;
            await Navigation.PushAsync(new CameraItems(this));
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            if (button10 != null)
            {
                button10.TextColor = Color.FromHex("#C1C1C1");
            }
            button10 = button;
        }

        private async void Button_Clicked_6v1(object sender, EventArgs e)
        {
            additionalYesButton.TextColor = Color.FromHex("#C1C1C1");
            isAsk10 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            if (button10 != null &&
                button1 != null)
            {
                button1.TextColor = Color.FromHex("#C1C1C1");
            }
            button10 = button;
        }
        #endregion

        public void AddPhotoItems(byte[] photob)
        {
            if (askPageMV.Ask.Any_personal_or_additional_items_with_or_in_vehicle == null)
            {
                askPageMV.Ask.Any_personal_or_additional_items_with_or_in_vehicle = new List<Models.Photo>();
            }
            Models.Photo photo = new Models.Photo();
            photo.Base64 = Convert.ToBase64String(photob);
            photo.path = $"../Photo/{askPageMV.VehiclwInformation.Id}/PikedUp/Items/{askPageMV.Ask.Any_personal_or_additional_items_with_or_in_vehicle.Count + 1}.jpg";
            askPageMV.Ask.Any_personal_or_additional_items_with_or_in_vehicle.Add(photo);
            Image image = new Image()
            {
                Source = ImageSource.FromStream(() => new MemoryStream(photob)),
                HeightRequest = 50,
                WidthRequest = 50,
            };
            //image.GestureRecognizers.Add(new TapGestureRecognizer(ViewPhotoForRetacke1));
            blockAskPhotoItem.Children.Add(image);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private async void ViewPhotoForRetacke1(Xamarin.Forms.View v, object s)
        {
            if (v != null && blockAskPhotoItem.Children.Contains(v))
            {
                await Navigation.PushAsync(new ViewPhotForAsk(v, this, "Ask1"));
            }
        }

        public void ReSetPhoto2(Xamarin.Forms.View view, byte[] newRetake)
        {
            byte[] r = GetImageBytes(((Image)view).Source);
            askPageMV.ResetAskPhotoItem(r, newRetake);
            blockAskPhotoItem.Children.Remove((Image)view);
            Image image = new Image()
            {
                Source = ImageSource.FromStream(() => new MemoryStream(newRetake)),
                HeightRequest = 50,
                WidthRequest = 50,
            };
            image.GestureRecognizers.Add(new TapGestureRecognizer(ViewPhotoForRetacke1));
            blockAskPhotoItem.Children.Add(image);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private byte[] GetImageBytes(ImageSource imagesource)
        {
            StreamImageSource streamImageSource = (StreamImageSource)imagesource;
            byte[] ImageBytes;
            using (var memoryStream = new System.IO.MemoryStream())
            {
                var stream = streamImageSource.Stream.Invoke(new System.Threading.CancellationToken()).Result;
                stream.CopyTo(memoryStream);
                stream = null;
                ImageBytes = memoryStream.ToArray();
            }
            return ImageBytes;
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            isAsk11 = true;
            askPageMV.Ask.TypeVehicle = (string)((Picker)sender).SelectedItem;
            askPageMV.VehiclwInformation.Type = askPageMV.Ask.TypeVehicle;
        }

        private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            //await PopupNavigation.PushAsync(new ContactInfo());
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            HelpersView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            HelpersView.Hidden();
        }
    }
}