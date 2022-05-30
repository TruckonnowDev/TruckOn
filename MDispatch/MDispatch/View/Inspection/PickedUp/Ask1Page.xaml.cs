using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View.GlobalDialogView;
using MDispatch.ViewModels.InspectionMV;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.View.Inspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Ask1Page : ContentPage
	{
        public Ask1PageMV ask1PageMV = null;
        private readonly IHelperViewService _helperView;

        public Ask1Page(IManagerDispatchMobService managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, InitDasbordDelegate initDasbordDelegate, GetVechicleDelegate getVechicleDelegate, string typeCar,
            string onDeliveryToCarrier, string totalPaymentToCarrier)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            ask1PageMV = new Ask1PageMV(managerDispatchMob, vehiclwInformation, idShip, Navigation, initDasbordDelegate, getVechicleDelegate, onDeliveryToCarrier, totalPaymentToCarrier, typeCar);
            ask1PageMV.Ask1 = new Ask1();
            InitializeComponent();
            BindingContext = ask1PageMV;
            methodPicker.ItemsSource = new List<string>()
            {
                LanguageHelper.DoorAnswer,
                LanguageHelper.WindowAnswer,
                LanguageHelper.SunroofAnswer,
                LanguageHelper.ConvertibleAnswer,
            };
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
            ask1PageMV.Ask1.Exact_Mileage = e.NewTextValue;
        }
        #endregion

        #region Ask3
        Button button3 = null;
        bool isAsk3 = false;
        private void Button_Clicked(object sender, EventArgs e)
        {
            isAsk3 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            ask1PageMV.Ask1.Did_someone_help_you_load_it = button.Text;
            if (button3 != null)
            {
                button3.TextColor = Color.FromHex("#C1C1C1");
            }
            button3 = button;
            if(button.Text == LanguageHelper.YesBtnText)
            {
                askBlock4.IsVisible = true;
                if(ask1PageMV.Ask1.Did_someone_load_the_vehicle_for_you != null && ask1PageMV.Ask1.Did_someone_load_the_vehicle_for_you != "")
                {
                    isAsk4 = true;
                }
                else
                {
                    isAsk4 = false;
                }
            }
            else
            {
                isAsk4 = true;
                askBlock4.IsVisible = false;
            }
        }
        #endregion

        #region Ask4
        bool isAsk4 = true;
        private void Entry_TextChanged2(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != "")
            {
                isAsk4 = true;
            }
            else
            {
                isAsk4 = false;
            }
            ask1PageMV.Ask1.Did_someone_load_the_vehicle_for_you = e.NewTextValue;
        }
        #endregion

        #region Ask5
        bool isAsk5 = false;
        bool isTypeText5 = false;
        string btnText5 = "";
        string entryText5 = "";
        Button button5 = null;
        private void Entry_TextChanged3(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != "")
            {
                isAsk5 = true;
                isTypeText5 = true;
            }
            else
            {
                isAsk5 = false;
                isTypeText5 = false;
            }
            entryText = e.NewTextValue;
            ask1PageMV.Ask1.Did_you_Damage_anything_at_the_pick_up = $"{btnText5}, {entryText5}";
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            btnText = button.Text;
            if (button5 != null)
            {
                button5.TextColor = Color.FromHex("#C1C1C1");
            }
            if (button.Text == LanguageHelper.YesBtnText)
            {
                entDamage.IsVisible = true;
                if (isTypeText5)
                {
                    isAsk5 = true;
                }
                else
                {
                    isAsk5 = false;
                }
            }
            else
            {
                isAsk5 = true;
                entDamage.IsVisible = false;
            }
            button5 = button;

            ask1PageMV.Ask1.Did_you_Damage_anything_at_the_pick_up = $"{btnText5}, {entryText5}";
        }
        #endregion

        #region Ask6
        bool isAsk6 = false;
        private void Dropdown_SelectedItemChanged(object sender, Plugin.InputKit.Shared.Utils.SelectedItemChangedArgs e)
        {
            isAsk6 = true;
            ask1PageMV.Ask1.What_method_of_exit_did_you_use = (string)e.NewItem;
        }
        #endregion

        #region Ask7
        Button button1 = null;
        bool isAsk7 = false;
        private void Button_Clicked1(object sender, EventArgs e)
        {
            isAsk7 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            ask1PageMV.Ask1.Did_you_jumped_the_vehicle_to_start = button.Text;
            if (button1 != null)
            {
                button1.TextColor = Color.FromHex("#C1C1C1");
            }
            button1 = button;
        }
        #endregion

        #region Ask8
        Button button2 = null;
        bool isAsk8 = false;
        private void Button_Clicked2(object sender, EventArgs e)
        {
            isAsk8 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            ask1PageMV.Ask1.Have_you_used_winch = button.Text;
            if (button2 != null)
            {
                button2.TextColor = Color.FromHex("#C1C1C1");
            }
            button2 = button;
        }
        #endregion

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (isAsk1 && isAsk14 && isAsk3 && isAsk4 && isAsk5 && isAsk6 && isAsk7 && isAsk8)
            {
                ask1PageMV.SaveAsk();
            }
            else
            {
                await Navigation.PushAsync(new Alert(LanguageHelper.AskErrorAlert, null));
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
            if (!isAsk14)
            {
                askBlock14.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock14.BorderColor = Color.White;
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
                askText4.TextColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askText4.TextColor = Color.FromHex("#101010");
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
            if (!isAsk7)
            {
                askBlock7.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock7.BorderColor = Color.White;
            }
            if (!isAsk8)
            {
                askBlock8.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock8.BorderColor = Color.White;
            }
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
            isAsk6 = true;
            ask1PageMV.Ask1.What_method_of_exit_did_you_use = (string)((Picker)sender).SelectedItem;
        }

        #region Ask14
        Button button14 = null;
        bool isAsk14 = false;
        bool isTypeText = false;
        string btnText = "";
        string entryText = "";
        private void Entry_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != "")
            {
                isAsk14 = true;
                isTypeText = true;
            }
            else
            {
                isAsk14 = false;
                isTypeText = false;
            }
            entryText = e.NewTextValue;
            ask1PageMV.Ask1.Did_you_notice_any_mechanical_imperfections_wile_loading = $"{btnText}, {entryText}";
        }

        private void Button_Clicked_5(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            btnText = button.Text;
            if (button14 != null)
            {
                button14.TextColor = Color.FromHex("#C1C1C1");
            }
            if(button.Text == LanguageHelper.YesBtnText)
            {
                if(isTypeText)
                {
                    isAsk14 = true;
                }
                else
                {
                    isAsk14 = false;
                }
                nameE.IsVisible = true;
            }
            else
            {
                isAsk14 = true;
                nameE.IsVisible = false;
            }
            button14 = button;

            ask1PageMV.Ask1.Did_you_notice_any_mechanical_imperfections_wile_loading = $"{btnText}, {entryText}";
        }
        #endregion



        private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BOLPage(ask1PageMV.managerDispatchMob, ask1PageMV.IdShip, ask1PageMV.initDasbordDelegate));
        }

        private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
        {
            //await PopupNavigation.PushAsync(new ContactInfo());
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _helperView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _helperView.Hidden();
        }
    }
}