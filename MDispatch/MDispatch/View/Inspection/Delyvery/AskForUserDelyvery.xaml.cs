using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection.PickedUp;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using MDispatch.ViewModels.InspectionMV.Servise.Paymmant;
using Plugin.InputKit.Shared.Controls;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.View.Inspection.Delyvery
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AskForUserDelyvery : ContentPage
	{
        public AskForUsersDelyveryMW askForUsersDelyveryMW = null;
        private IPaymmant Paymmant = null;
        private Timer timer = null;
        private readonly IHelperViewService _helperView;

        public AskForUserDelyvery (IManagerDispatchMobService managerDispatchMob, string idShip, InitDasbordDelegate initDasbordDelegate, string OnDeliveryToCarrier, string totalPaymentToCarrier,
            VehiclwInformation vehiclwInformation, GetShiping getShiping, GetVechicleDelegate getVechicleDelegate, bool isproplem)
		{
            _helperView = DependencyService.Get<IHelperViewService>();
            askForUsersDelyveryMW = new AskForUsersDelyveryMW(managerDispatchMob, idShip, Navigation, getShiping, initDasbordDelegate, getVechicleDelegate, vehiclwInformation, totalPaymentToCarrier);
            askForUsersDelyveryMW.AskForUserDelyveryM = new AskForUserDelyveryM();
            InitializeComponent ();
            BindingContext = askForUsersDelyveryMW;
            InitElemnt(isproplem);
            InitPayment(OnDeliveryToCarrier, totalPaymentToCarrier);
        }

        private void InitElemnt(bool isProplem)
        {
            if (isProplem)
            {
                btnSave.IsVisible = false;
                blockAskPay.IsVisible = true;
                btnYesPay.IsVisible = false;
                btnNoPay.IsVisible = false;
                btnNumberOffice.IsVisible = true;
                lReport.IsVisible = true;
                blockAsk.IsVisible = false;
                timer = new Timer(new TimerCallback(CheckProplem), null, 10000, 10000);
            }
        }

        private async void CheckProplem(object state)
        {
            bool isProplem = await askForUsersDelyveryMW.CheckProplem();
            if (!isProplem)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    blockPsw.IsVisible = false;
                    bloclThank.IsVisible = false;
                    btnSave.IsVisible = true;
                    blockAskPay.IsVisible = false;
                    btnYesPay.IsVisible = true;
                    btnNoPay.IsVisible = true;
                    btnNumberOffice.IsVisible = false;
                    lReport.IsVisible = false;
                    blockAsk.IsVisible = true;
                });
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void InitPayment(string OnDeliveryToCarrier, string totalPaymentToCarrier)
        {
            if(totalPaymentToCarrier == "COD")
            {
                payBlockSelectPatment.IsVisible = true;
            }
            else if (totalPaymentToCarrier == "COP")
            {
                isAsk2 = true;
                askBlock2.IsVisible = false;
                askForUsersDelyveryMW.AskForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation = "COP";
            }
            else
            {
                askForUsersDelyveryMW.AskForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation = "Biling";
                isAsk2 = true;
                instructionL.Text = OnDeliveryToCarrier;
                bilingPay.IsVisible = true;
            }
        }

        #region Ask1
        Button button1 = null;
        bool isAsk1 = false;
        private async void Button_Clicked1(object sender, EventArgs e)
        {
            isAsk1 = true;
            if (button1 != null)
            {
                button1.TextColor = Color.FromHex("#C1C1C1");
            }
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            askForUsersDelyveryMW.AskForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up = button.Text;
            button1 = button;
            if (button1.Text == LanguageHelper.FoundIssueBtnText)
            {
                await Navigation.PushAsync(new PageAddDamageFoUser(askForUsersDelyveryMW, blockAskPhoto, this));
            }
            else if(button1.Text == LanguageHelper.YesBtnText)
            {
                askForUsersDelyveryMW.AskForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo = null;
                blockAskPhoto.Children.Clear();
                scrolViewAskPhoto.IsVisible = false;
            }
        }

        public void AddPhotoAdditional(byte[] image)
        {
            if(askForUsersDelyveryMW.AskForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo == null)
            {
                askForUsersDelyveryMW.AskForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo = new List<Photo>();
            }
            Photo photo1 = new Photo();
            photo1.Base64 = Convert.ToBase64String(image);
            photo1.path = $"../Photo/{askForUsersDelyveryMW.VehiclwInformation.Id}/Delyvery/Additional/{askForUsersDelyveryMW.AskForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo.Count + 1}.jpg";
            askForUsersDelyveryMW.AskForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo.Add(photo1);
            blockAskPhoto.Children.Add(new Image() { Source = ImageSource.FromStream(() => new MemoryStream(image)), HeightRequest = 40, WidthRequest = 40 });
            if (!scrolViewAskPhoto.IsVisible)
            {
                scrolViewAskPhoto.IsVisible = true;
            }
        }
        #endregion

        #region Ask2
        bool isAsk2 = false;
        private void Dropdown_SelectedItemChanged(object sender, Plugin.InputKit.Shared.Utils.SelectedItemChangedArgs e)
        {
            askForUsersDelyveryMW.AskForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation = (string)e.NewItem;
            Paymmant = GetPaymmant((string)e.NewItem);
            if(payBlockSelectPatment.Children.Count == 3)
            {
                payBlockSelectPatment.Children.RemoveAt(2);
            }
            payBlockSelectPatment.Children.Add(Paymmant.GetStackLayout());
            isAsk2 = false;
        }

        private IPaymmant GetPaymmant(string paymmantName)
        {
            IPaymmant paymmant = null;
            switch(paymmantName)
            {
                case "Cash":
                    paymmant = new CashPaymmant(this);
                    break;
                case "Check":
                    paymmant = new CheckPaymmant(this);
                    break;
                case "Electronic Money Transfer":
                    paymmant = new CraditCardPaymant(this);
                    break;
            }
            return paymmant;
        }
        #endregion

        #region Ask3
        bool isSignatureAsk = false;
        bool isNameAsk = false;
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != "")
            {
                isNameAsk = true;
            }
            else
            {
                isNameAsk = false;
            }
            askForUsersDelyveryMW.AskForUserDelyveryM.App_will_ask_for_name_of_the_client_signature = e.NewTextValue;
        }

        private async void Sign_StrokeCompleted(object sender, EventArgs e)
        {
            Photo photo = new Photo();
            isSignatureAsk = true;
            Stream stream = await sign.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] image = memoryStream.ToArray();
            photo.Base64 = Convert.ToBase64String(image);
            photo.path = $"../Photo/{askForUsersDelyveryMW.IdShip}/Delyvery/Signature/DelyverySig.jpg";
            askForUsersDelyveryMW.AskForUserDelyveryM.App_will_ask_for_signature_of_the_client_signature = photo;
        }

        private void Sign_Cleared(object sender, EventArgs e)
        {
            isSignatureAsk = false;
            askForUsersDelyveryMW.AskForUserDelyveryM.App_will_ask_for_signature_of_the_client_signature = null;
        }

        private bool GetIsAsk3()
        {
            return isNameAsk == true && isSignatureAsk == true;
        }
        #endregion

        #region Ask4
        Button button4 = null;
        bool isAsk4 = false;
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            isAsk4 = true;
            if (button4 != null)
            {
                button4.TextColor = Color.FromHex("#C1C1C1");
            }
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#2C5DEB");
            button4 = button;
            if(button.Text == LanguageHelper.YesBtnText)
            {
                await Navigation.PushAsync(new View.Inspection.Feedback(askForUsersDelyveryMW.managerDispatchMob, askForUsersDelyveryMW.VehiclwInformation, askForUsersDelyveryMW));
            }
        }
        #endregion

        #region Ask5
        bool isAsk5 = false;
        private void AdvancedSlider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                isAsk5 = true;
                askForUsersDelyveryMW.AskForUserDelyveryM.Please_rate_the_driver = ((AdvancedSlider)sender).Value.ToString();
            }
        }
        #endregion

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (Paymmant != null)
            {
                isAsk2 = Paymmant.IsAskPaymmant;
            }
            if (isAsk1 && isAsk2 && GetIsAsk3() && isAsk4 && isAsk5 )
            {
                askForUsersDelyveryMW.SaveAsk(askForUsersDelyveryMW.AskForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation);
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
            if (!GetIsAsk3())
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
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            askForUsersDelyveryMW.AskForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation = (string)((Picker)sender).SelectedItem;
            Paymmant = GetPaymmant((string)((Picker)sender).SelectedItem);
            if (payBlockSelectPatment.Children.Count == 3)
            {
                payBlockSelectPatment.Children.RemoveAt(2);
            }
            payBlockSelectPatment.Children.Add(Paymmant.GetStackLayout());
            isAsk2 = false;
        }

        private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BOLPage(askForUsersDelyveryMW.managerDispatchMob, askForUsersDelyveryMW.IdShip, askForUsersDelyveryMW.initDasbordDelegate));
        }

        private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
        {
            //await PopupNavigation.PushAsync(new ContactInfo());
        }

        private async void Entry_TextChanged1(object sender, TextChangedEventArgs e)
        {
            if (CrossSettings.Current.GetValueOrDefault("Password", "") == e.NewTextValue)
            {
                if (!payBlockSelectPatment.IsVisible)
                {
                    if (Paymmant != null)
                    {
                        isAsk2 = Paymmant.IsAskPaymmant;
                    }
                    if (isAsk1 && isAsk2 && GetIsAsk3() && isAsk4 && isAsk5)
                    {
                        askForUsersDelyveryMW.SaveAsk(askForUsersDelyveryMW.AskForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation);
                    }
                    else
                    {
                        await Navigation.PushAsync(new Alert(LanguageHelper.AskErrorAlert, null));
                        CheckAsk();
                    }
                }
                else
                {
                    ((Entry)sender).TextColor = Color.Default;
                    //btnConntinue.IsVisible = true;
                    blockAskPay.IsVisible = true;
                }
            }
            else
            {
                ((Entry)sender).TextColor = Color.FromHex("#FF2C2C");
                //btnConntinue.IsVisible = false;
                blockAskPay.IsVisible = false;
            }
        }

        bool isAsk3 = false;
        private async void Button_Clicked2(object sender, EventArgs e)
        {
            isAsk3 = true;
            if (Paymmant != null)
            {
                isAsk2 = Paymmant.IsAskPaymmant;
            }
            if (isAsk1 && isAsk2 && GetIsAsk3() && isAsk4 && isAsk5)
            {
                btnSave.IsVisible = false;
                bloclThank.IsVisible = true;
                blockPsw.IsVisible = true;
            }
            else
            {
                await Navigation.PushAsync(new Alert(LanguageHelper.AskErrorAlert, null));
                CheckAsk();
            }

        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            btnYesPay.IsVisible = false;
            btnNoPay.IsVisible = false;
            btnNumberOffice.IsVisible = true;
            lReport.IsVisible = true;
            blockAsk.IsVisible = false;
            askForUsersDelyveryMW.SetProblem();
            timer = new Timer(new TimerCallback(CheckProplem), null, 10000, 10000);
        }

        private void Button_Clicked_3(object sender, EventArgs e)
        {
            PhoneDialer.Open("+17734305155");
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