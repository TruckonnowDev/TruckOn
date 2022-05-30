using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection.Delyvery.CameraPage;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.View.Inspection.Delyvery
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AskPageDelyvery : ContentPage
	{
        private AskDelyveryMV askDelyveryMV = null;
        private readonly IHelperViewService _helperView;
        public AskPageDelyvery (IManagerDispatchMobService managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, InitDasbordDelegate initDasbordDelegate, GetVechicleDelegate getVechicleDelegate,
             string onDeliveryToCarrier, string totalPaymentToCarrier, GetShiping getShiping)
		{
            _helperView = DependencyService.Get<IHelperViewService>();
            askDelyveryMV = new AskDelyveryMV(managerDispatchMob, vehiclwInformation, idShip, Navigation, getShiping, initDasbordDelegate, getVechicleDelegate, onDeliveryToCarrier, totalPaymentToCarrier);
            askDelyveryMV.AskDelyvery = new AskDelyvery();
            InitializeComponent ();
            BindingContext = askDelyveryMV;
            InitAsk();
        }

        #region Ask19
        string curentTime = null;
        string currentDate = null;

        private void InitAsk()
        {
            var ssssss = App.time.GetDateTimeFormats();
            curentTime = App.time.TimeOfDay.ToString("hh':'mm");
            currentDate = App.time.ToString("dd/MM/yyyy");
            askDelyveryMV.AskDelyvery.Time_Of_Delivery = $"{currentDate} {curentTime}";
            time.Text = $"{currentDate} {curentTime}";
            methodExitPik.ItemsSource = new List<string>()
            {
                LanguageHelper.DoorAnswer,
                LanguageHelper.WindowAnswer,
                LanguageHelper.SunroofAnswer,
                LanguageHelper.ConvertibleAnswer,
            };
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
            askDelyveryMV.AskDelyvery.Lightbrightness = button.Text;
            if (button2 != null)
            {
                button2.TextColor = Color.FromHex("#C1C1C1");
            }
            button2 = button;
        }
        #endregion

        #region Ask10
        Button button10 = null;
        bool isAsk10 = false;
        private void Button_Clicked_2(object sender, EventArgs e)
        {
            isAsk10 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askDelyveryMV.AskDelyvery.Vehicle_Condition_on_delivery = button.Text;
            if (button10 != null)
            {
                button10.TextColor = Color.FromHex("#C1C1C1");
            }
            button10 = button;
        }
        #endregion

        #region Ask3
        Button button3 = null;
        bool isAsk3 = false;
        private void RadioButton_Clicked_1(object sender, EventArgs e)
        {
            isAsk3 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askDelyveryMV.AskDelyvery.Vehicle_Condition_on_delivery = button.Text;
            if (button3 != null)
            {
                button3.TextColor = Color.FromHex("#C1C1C1");
            }
            button3 = button;
        }
        #endregion

        #region Ask4
        Button button4 = null;
        bool isAsk4 = false;
        private void Button_Clicked_3(object sender, EventArgs e)
        {
            isAsk4 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askDelyveryMV.AskDelyvery.Did_you_meet_the_client = button.Text;
            if (button4 != null)
            {
                button4.TextColor = Color.FromHex("#C1C1C1");
            }
            button4 = button;
        }
        #endregion

        #region Ask5
        Button button5 = null;
        bool isAsk5 = false;
        private void Button_Clicked(object sender, EventArgs e)
        {
            isAsk5 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askDelyveryMV.AskDelyvery.Truck_on_emergency_brake = button.Text;
            if (button5 != null)
            {
                button5.TextColor = Color.FromHex("#C1C1C1");
            }
            button5 = button;
        }
        #endregion

        #region Ask6
        Button button6 = null;
        bool isAsk6 = false;
        private void Button_Clicked_4(object sender, EventArgs e)
        {
            isAsk6 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askDelyveryMV.AskDelyvery.Truck_locked = button.Text;
            if (button6 != null)
            {
                button6.TextColor = Color.FromHex("#C1C1C1");
            }
            button6 = button;
        }
        #endregion

        #region Ask7
        Button button7 = null;
        bool isAsk7 = false;
        private async void Button_Clicked_5(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new IDpersonFace(this));
        }

        public void AddPhoto(byte[] result)
        {
            if (askDelyveryMV.AskDelyvery.Please_take_a_picture_Id_of_the_person_taking_the_delivery == null)
            {
                askDelyveryMV.AskDelyvery.Please_take_a_picture_Id_of_the_person_taking_the_delivery = new List<Models.Photo>();
            }
            Models.Photo photo = new Models.Photo();
            photo.Base64 = Convert.ToBase64String(result);
            photo.path = $"../Photo/{askDelyveryMV.VehiclwInformation.Id}/PikedUp/Items/{askDelyveryMV.AskDelyvery.Please_take_a_picture_Id_of_the_person_taking_the_delivery.Count + 1}.jpg";
            askDelyveryMV.AskDelyvery.Please_take_a_picture_Id_of_the_person_taking_the_delivery.Add(photo);
            Image image = new Image()
            {
                Source = ImageSource.FromStream(() => new MemoryStream(result)),
                HeightRequest = 50,
                WidthRequest = 50,
            };
            //image.GestureRecognizers.Add(new TapGestureRecognizer(ViewPhotoForRetacke1));
            blockPhoto.Children.Add(image);
            isAsk7 = true;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        #endregion

        #region Ask8
        Button button8 = null;
        bool isAsk8 = false;
        private void Button_Clicked_8(object sender, EventArgs e)
        {
            isAsk8 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askDelyveryMV.AskDelyvery.Truck_on_emergency_brake = button.Text;
            if (button8 != null)
            {
                button8.TextColor = Color.FromHex("#C1C1C1");
            }
            button8 = button;
        }
        #endregion

        #region Ask9
        Button button9 = null;
        bool isAsk9 = false;
        string btnAnswer = "";
        private void Button_Clicked_11(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            btnAnswer = button.Text;
            if (button9 != null)
            {
                button9.TextColor = Color.FromHex("#C1C1C1");
            }
            button9 = button;
            if (button.Text == LanguageHelper.YesBtnText)
            {
                askBlock9v2.IsVisible = true;
                askDelyveryMV.AskDelyvery.Anyone_Rushing_you_to_perform_the_delivery = $"{btnAnswer}, {txtAnswer}";
                if (txtAnswer != "")
                {
                    isAsk9 = true;
                }
                else
                {
                    isAsk9 = false;
                }
            }
            else
            {
                askDelyveryMV.AskDelyvery.Anyone_Rushing_you_to_perform_the_delivery = $"{btnAnswer}";
                askBlock9v2.IsVisible = false;
                isAsk9 = true;
            }
        }

        string txtAnswer = "";
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtAnswer = e.NewTextValue;
            askDelyveryMV.AskDelyvery.Anyone_Rushing_you_to_perform_the_delivery = $"{btnAnswer}, {txtAnswer}";
            isAsk9 = true;
        }
        #endregion



        #region Ask11
        bool isAsk11 = false;
        private void Dropdown_SelectedItemChanged_1(object sender, Plugin.InputKit.Shared.Utils.SelectedItemChangedArgs e)
        {
            isAsk11 = true;
            askDelyveryMV.AskDelyvery.How_did_you_get_inside_of_the_vehicle = (string)e.NewItem;
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            isAsk11 = true;
            askDelyveryMV.AskDelyvery.How_did_you_get_inside_of_the_vehicle = (string)((Picker)sender).SelectedItem;
        }
        #endregion

        #region Ask1
        bool isAsk1 = false;
        Button button1 = null;
        private void Dropdown_SelectedItemChanged_2(object sender, EventArgs e)
        {
            isAsk1 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askDelyveryMV.AskDelyvery.Weather_Conditions = button.Text;
            if (button1 != null)
            {
                button1.TextColor = Color.FromHex("#C1C1C1");
            }
            button1 = button;
        }
        #endregion

        #region Ask13
        Button button13 = null;
        bool isAsk13 = false;
        private void Button_Clicked_6(object sender, EventArgs e)
        {
            isAsk13 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            if (button13 != null)
            {
                button13.TextColor = Color.FromHex("#C1C1C1");
            }
            button13 = button;
            askDelyveryMV.AskDelyvery.Does_the_vehicle_Drives = button.Text;
        }
        #endregion

        #region Ask14
        Button button14 = null;
        bool isAsk14 = false;
        string answer = "";
        string answerStr = "";
        private void Button_Clicked_7(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            answer = button.Text;
            if (button14 != null)
            {
                button14.TextColor = Color.FromHex("#C1C1C1");
            }
            button14 = button;
            if (button.Text == LanguageHelper.YesBtnText)
            {
                isAsk14 = true;
                startST.IsVisible = false;
            }
            else
            {
                if(button14V2 != null)
                {
                    isAsk14 = true;
                }
                else
                {
                    isAsk14 = false;
                }
                startST.IsVisible = true;
            }
            answer = button.Text;
            askDelyveryMV.AskDelyvery.Did_the_vehicle_starts = $"{answer}, {answerStr}";
        }

        Button button14V2 = null;
        private void Button_Clicked_12(object sender, EventArgs e)
        {
            isAsk14 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            answerStr = button.Text;
            if (button14V2 != null)
            {
                button14V2.TextColor = Color.FromHex("#C1C1C1");
            }
            button14V2 = button;
            answerStr = button.Text;
            askDelyveryMV.AskDelyvery.Did_the_vehicle_starts = $"{answer}, {answerStr}";
        }
        #endregion

        #region Ask18
        Button button18 = null;
        bool isAsk18 = false;
        private void Button_Clicked_9(object sender, EventArgs e)
        {
            isAsk18 = true;
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askDelyveryMV.AskDelyvery.Does_the_vehicle_Drives = button.Text;
            if (button18 != null)
            {
                button18.TextColor = Color.FromHex("#C1C1C1");
            }
            button18 = button;
        }
        #endregion

        #region Ask20
        bool isAsk20 = false;
        private void Button_Clicked_10(object sender, EventArgs e)
        {
            isAsk20 = true;
            Button button = (Button)sender;
            button.IsEnabled = false;
        }
        #endregion

        

        #region Ask15
        bool isAsk15 = false;
        private void Entry_TextChanged_10(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != "")
            {
                isAsk15 = true;
            }
            else
            {
                isAsk15 = false;
            }
            askDelyveryMV.AskDelyvery.Exact_mileage_after_unloading = e.NewTextValue;
        }
        #endregion

        #region Ask16
        bool isAsk16 = false;
        string txtAnswer16 = "";
        string btnAnswer16 = "";
        Button button16 = null;
        private void Entry_TextChanged_11(object sender, TextChangedEventArgs e)
        {
            txtAnswer = e.NewTextValue;
            askDelyveryMV.AskDelyvery.Anyone_helping_you_unload = $"{btnAnswer16}, {txtAnswer16}";
            isAsk16 = true;
        }

        private void Button_Clicked_13(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            btnAnswer16 = button.Text;
            if (button16 != null)
            {
                button16.TextColor = Color.FromHex("#C1C1C1");
            }
            button16 = button;
            if (button.Text == LanguageHelper.YesBtnText)
            {
                entUnload.IsVisible = true;
                askDelyveryMV.AskDelyvery.Anyone_helping_you_unload = $"{btnAnswer16}, {txtAnswer16}";
                if (txtAnswer16 != "")
                {
                    isAsk16 = true;
                }
                else
                {
                    isAsk16 = false;
                }
            }
            else
            {
                askDelyveryMV.AskDelyvery.Anyone_helping_you_unload = $"{btnAnswer16}";
                entUnload.IsVisible = false;
                isAsk16 = true;
            }
        }
        #endregion

        #region Ask17
        bool isAsk17 = false;
        string txtAnswer17 = "";
        string btnAnswer17 = "";
        Button button17 = null;
        private void Entry_TextChanged_12(object sender, TextChangedEventArgs e)
        {
            txtAnswer = e.NewTextValue;
            askDelyveryMV.AskDelyvery.Did_someone_else_unloaded_the_vehicle_for_you = $"{btnAnswer17}, {txtAnswer17}";
            isAsk17 = true;
        }

        private void Button_Clicked_14(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            btnAnswer17 = button.Text;
            if (button17 != null)
            {
                button17.TextColor = Color.FromHex("#C1C1C1");
            }
            button17 = button;
            if (button.Text == LanguageHelper.YesBtnText)
            {
                entUnloaded.IsVisible = true;
                askDelyveryMV.AskDelyvery.Did_someone_else_unloaded_the_vehicle_for_you = $"{btnAnswer17}, {txtAnswer17}";
                if (txtAnswer17 != "")
                {
                    isAsk17 = true;
                }
                else
                {
                    isAsk17 = false;
                }
            }
            else
            {
                askDelyveryMV.AskDelyvery.Did_someone_else_unloaded_the_vehicle_for_you = $"{btnAnswer17}";
                entUnloaded.IsVisible = false;
                isAsk17 = true;
            }
        }
        #endregion

        #region Ask12
        bool isAsk12 = false;
        string txtAnswer12 = "";
        string btnAnswer12 = "";
        Button button12 = null;
        private void Entry_TextChanged_13(object sender, TextChangedEventArgs e)
        {
            txtAnswer12 = e.NewTextValue;
            askDelyveryMV.AskDelyvery.Did_you_notice_any_imperfections_on_body_wile_vehicle_been_transported = $"{btnAnswer12}, {txtAnswer12}";
            isAsk12 = true;
        }

        private void Button_Clicked_15(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            btnAnswer12 = button.Text;
            if (button12 != null)
            {
                button12.TextColor = Color.FromHex("#C1C1C1");
            }
            button12 = button;
            if (button.Text == LanguageHelper.YesBtnText)
            {
                entFlaws.IsVisible = true;
                askDelyveryMV.AskDelyvery.Did_you_notice_any_imperfections_on_body_wile_vehicle_been_transported = $"{btnAnswer12}, {txtAnswer12}";
                if (txtAnswer17 != "")
                {
                    isAsk12 = true;
                }
                else
                {
                    isAsk12 = false;
                }
            }
            else
            {
                askDelyveryMV.AskDelyvery.Did_you_notice_any_imperfections_on_body_wile_vehicle_been_transported = $"{btnAnswer12}";
                entFlaws.IsVisible = false;
                isAsk12 = true;
            }
        }
        #endregion


        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (isAsk1 && isAsk2 && isAsk3 && isAsk4 && isAsk5 && isAsk6 && isAsk7 && isAsk8 && isAsk9 && isAsk10 && isAsk11 && isAsk12 && isAsk13 && isAsk14 && isAsk15 && isAsk16 && isAsk17 && isAsk18 && isAsk20)
            {
                askDelyveryMV.SaveAsk();
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
            if (!isAsk2)
            {
                askBlock2.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock2.BorderColor = Color.White;
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
            if (!isAsk12)
            {
                askBlock12.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock12.BorderColor = Color.White;
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
            if (!isAsk15)
            {
                askBlock15.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock15.BorderColor = Color.White;
            }
            if (!isAsk16)
            {
                askBlock16.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock16.BorderColor = Color.White;
            }
            if (!isAsk17)
            {
                askBlock17.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock17.BorderColor = Color.White;
            }
            if (!isAsk18)
            {
                askBlock18.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock18.BorderColor = Color.White;
            }
            if (!isAsk20)
            {
                askBlock20.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock20.BorderColor = Color.White;
            }
        }

        private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BOLPage(askDelyveryMV.managerDispatchMob, askDelyveryMV.IdShip, askDelyveryMV.initDasbordDelegate));
        }

        private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
        {
            //await PopupNavigation.PushAsync(new ContactInfo());
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            _helperView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _helperView.Hidden();
        }
    }
}