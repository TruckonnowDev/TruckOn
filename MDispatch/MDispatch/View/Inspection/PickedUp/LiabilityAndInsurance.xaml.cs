using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.TouchCordinate;
using MDispatch.Service;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View.GlobalDialogView;
using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using MDispatch.ViewModels.InspectionMV.Servise.Paymmant;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.View.Inspection.PickedUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LiabilityAndInsurance : ContentPage
	{
        public LiabilityAndInsuranceMV liabilityAndInsuranceMV = null;
        private IPaymmant Paymmant = null;
        private Timer timer = null;
        private readonly IHelperViewService _helperView;

        [Obsolete]
        public LiabilityAndInsurance (IManagerDispatchMobService managerDispatchMob, string idVech, string idShip, InitDasbordDelegate initDasbordDelegate, string OnDeliveryToCarrier, string totalPaymentToCarrier, bool isproplem)
		{
            _helperView = DependencyService.Get<IHelperViewService>();
            liabilityAndInsuranceMV = new LiabilityAndInsuranceMV(managerDispatchMob, idVech, idShip, Navigation, initDasbordDelegate, this);
            InitializeComponent ();
            BindingContext = liabilityAndInsuranceMV;
            InitElemnt(isproplem);
            InitPayment(OnDeliveryToCarrier, totalPaymentToCarrier);
        }

        private void InitPayment(string OnDeliveryToCarrier, string totalPaymentToCarrier)
        {
            if (totalPaymentToCarrier == "COP")
            {
                payBlockSelectPatment.IsVisible = true;
            }
            else if (totalPaymentToCarrier == "COD")
            {
                isAsk2 = true;
                askBlock2.IsVisible = false;
                liabilityAndInsuranceMV.What_form_of_payment_are_you_using_to_pay_for_transportation = "COP";
            }
            else
            {
                liabilityAndInsuranceMV.What_form_of_payment_are_you_using_to_pay_for_transportation = "Biling";
                isAsk2 = true;
                instructionL.Text = OnDeliveryToCarrier;
                bilingPay.IsVisible = true;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            return base.OnBackButtonPressed();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (Paymmant != null)
            {
                isAsk2 = Paymmant.IsAskPaymmant;
            }
            if (isSignatureAsk && isAsk2)
            {
                liabilityAndInsuranceMV.SaveSigAndMethodPay();
            }
            else
            {
                await liabilityAndInsuranceMV._popupNavigation.PushAsync(new Alert(LanguageHelper.AskErrorAlert, null));
                CheckAsk();
            }
        }

        [Obsolete]
        public async void InitElemnt(bool isProplem)
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
                timer = new Timer(new TimerCallback(CheckProplem), null, 5000, 5000);
            }
            await Wait();
            if (liabilityAndInsuranceMV.Shipping != null && liabilityAndInsuranceMV.Shipping.VehiclwInformations != null)
            {
                string fontBold = ((OnPlatform<string>)Application.Current.Resources["OpenSans-Bold"]).Platforms.ToList().First(p => p.Platform.FirstOrDefault(pp => pp == Device.RuntimePlatform) != null).Value.ToString();
                string fontRegular = ((OnPlatform<string>)Application.Current.Resources["OpenSans-Regular"]).Platforms.ToList().First(p => p.Platform.FirstOrDefault(pp => pp == Device.RuntimePlatform) != null).Value.ToString();
                foreach (var VehiclwInformation in liabilityAndInsuranceMV.Shipping.VehiclwInformations)
                {
                    VechInfoSt.Children.Add(new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Margin = new Thickness(0, 0, 0, 6),
                        Children =
                        {
                            new Label()
                            {
                                Text = VehiclwInformation.Year ?? "----",
                                FontSize = 16,
                                TextColor = Color.FromHex("#101010"),
                                FontFamily = fontBold
                            },
                            new Label()
                            {
                                Text = VehiclwInformation.Make ?? "--------",
                                FontSize = 16,
                                TextColor = Color.FromHex("#101010"),
                                FontFamily = fontBold
                            },
                            new Label()
                            {
                                Text = VehiclwInformation.Model ?? "--------",
                                FontSize = 16,
                                TextColor = Color.FromHex("#101010"),
                                FontFamily = fontBold
                            },
                        }
                    });
                    VechInfoSt.Children.Add(new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label()
                                    {
                                        Text = "VIN#",
                                        FontSize = 14,
                                        TextColor = Color.FromHex("#101010"),
                                        FontFamily = fontRegular
                                    },
                                    new Label()
                                    {
                                        Text = VehiclwInformation.VIN ?? "---------",
                                        FontSize = 14,
                                        TextColor = Color.FromHex("#101010"),
                                        FontFamily = fontRegular
                                    }
                        }
                    });
                    VechInfoSt.Children.Add(new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label()
                                    {
                                        Text = LanguageHelper.WithoutTranslationTypeInfo,
                                        FontSize = 14,
                                        TextColor = Color.FromHex("#101010"),
                                        FontFamily = fontRegular
                                    },
                                    new Label()
                                    {
                                        Text = VehiclwInformation.Type ?? "-----------",
                                        FontSize = 14,
                                        TextColor = Color.FromHex("#101010"),
                                        FontFamily = fontRegular
                                    }
                        }
                    });
                    VechInfoSt.Children.Add(new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label()
                            {
                                Text = LanguageHelper.WithoutTranslationColorInfo,
                                FontSize = 14,
                                TextColor = Color.FromHex("#101010"),
                                FontFamily = fontRegular
                            },
                            new Label()
                            {
                                Text = VehiclwInformation.Color ?? "---------",
                                FontSize = 14,
                                TextColor = Color.FromHex("#101010"),
                                FontFamily = fontRegular
                            }
                        }
                    });

                    VechInfoSt.Children.Add(new Image()
                    {
                        HeightRequest = 300,
                        Source = ImageSource.FromUri(new Uri($"{Config.BaseReqvesteUrl}/Mobile/Image?name=../Photo/{VehiclwInformation.Id}/scan.jpg&type=jpg"))
                    });

                    VechInfoSt.Children.Add(new FlexLayout()
                    {
                        JustifyContent = FlexJustify.SpaceAround,
                        HeightRequest = 40,
                        Children =
                        {
                            new StackLayout(){
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 0,
                                Children =
                                {
                                    new Image()
                                    {
                                        Source = "DamageP1.png",
                                        HeightRequest = 25,
                                        WidthRequest = 25
                                    },
                                    new Label()
                                    {
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        Text = LanguageHelper.WithoutTranslationHintDamegePickedUp,
                                        FontSize = 12,
                                        TextColor = Color.FromHex("#101010"),
                                        FontFamily = fontRegular
                                    },
                                }
                            },
                            new StackLayout(){
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 0,
                                Children =
                                {
                                    new Image()
                                    {
                                        Source = "DamageD1.png",
                                        HeightRequest = 25,
                                        WidthRequest = 25
                                    },
                                    new Label()
                                    {
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        Text = LanguageHelper.WithoutTranslationHintDamegeDelivery,
                                        FontSize = 12,
                                        TextColor = Color.FromHex("#101010"),
                                        FontFamily = fontRegular
                                    },
                                }
                            },
                        }
                    });

                    FlexLayout flexLayout = new FlexLayout()
                    {
                        JustifyContent = FlexJustify.Center,
                        Children =
                        {
                            new Label()
                            {
                                HorizontalTextAlignment = TextAlignment.Center,
                                Text = LanguageHelper.WithoutTranslationSeeInspactinPhoneBtnText,
                                FontSize = 16,
                                VerticalTextAlignment = TextAlignment.Center,
                                FontFamily = fontBold,
                                TextColor = Color.FromHex("#2C5DEB"),
                            }
                        }
                    };
                    flexLayout.GestureRecognizers.Add(new TapGestureRecognizer(flexLayout_Clicked));
                    VechInfoSt.Children.Add(flexLayout);
                }
            }
            liabilityAndInsuranceMV.StataLoadShip = 0;
        }

        private async void flexLayout_Clicked(Xamarin.Forms.View arg1, object arg2)
        {
            await Navigation.PushAsync(new BOLPage(liabilityAndInsuranceMV.managerDispatchMob, liabilityAndInsuranceMV.IdShip, liabilityAndInsuranceMV.initDasbordDelegate));
        }

        private async void CheckProplem(object o)
        {
            bool isProplem = await liabilityAndInsuranceMV.CheckProplem();
            if(!isProplem)
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

        bool isSignatureAsk = false;
        private async void Sign_StrokeCompleted(object sender, EventArgs e)
        {
            Photo photo = new Photo();
            isSignatureAsk = true;
            Stream stream = await sign.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] image = memoryStream.ToArray();
            photo.Base64 = Convert.ToBase64String(image);
            photo.path = $"../Photo/{liabilityAndInsuranceMV.Shipping.VehiclwInformations.Find(v => v.Id == liabilityAndInsuranceMV.IdVech)}/PikedUp/Signature/PikedUp.jpg";
            liabilityAndInsuranceMV.SigPhoto = photo;
        }

        private void Sign_Cleared(object sender, EventArgs e)
        {
            isSignatureAsk = false;
            liabilityAndInsuranceMV.SigPhoto = null;
        }

        private void Touch(object sender, TouchActionEventArgs args)
        {
        }

        private async Task Wait()
        {
            await Task.Run(() =>
            {
                while (liabilityAndInsuranceMV.StataLoadShip == 0)
                {

                }
            });
        }

        private void CheckAsk()
        {
            if (!isSignatureAsk)
            {
                askBlock3.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock3.BorderColor = Color.White;
            }

            if (!isAsk2)
            {
                askBlock2.BorderColor = Color.FromHex("#FF2C2C");
            }
            else
            {
                askBlock2.BorderColor = Color.White;
            }
        }

        private bool isAsk2 = false;
        private void Dropdown_SelectedItemChanged(object sender, Plugin.InputKit.Shared.Utils.SelectedItemChangedArgs e)
        {
            liabilityAndInsuranceMV.What_form_of_payment_are_you_using_to_pay_for_transportation = (string)e.NewItem;
            Paymmant = GetPaymmant((string)e.NewItem);
            if (payBlockSelectPatment.Children.Count == 4)
            {
                payBlockSelectPatment.Children.RemoveAt(3);
            }
            payBlockSelectPatment.Children.Add(Paymmant.GetStackLayout());
            isAsk2 = false;
        }

        private IPaymmant GetPaymmant(string paymmantName)
        {
            IPaymmant paymmant = null;
            switch (paymmantName)
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

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            liabilityAndInsuranceMV.What_form_of_payment_are_you_using_to_pay_for_transportation = (string)((Picker)sender).SelectedItem;
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
            await Navigation.PushAsync(new BOLPage(liabilityAndInsuranceMV.managerDispatchMob, liabilityAndInsuranceMV.IdShip, liabilityAndInsuranceMV.initDasbordDelegate));
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            //await PopupNavigation.PushAsync(new ContactInfo());
        }

        private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(CrossSettings.Current.GetValueOrDefault("Password", "") == e.NewTextValue)
            {
                if (!payBlockSelectPatment.IsVisible)
                {
                    if (Paymmant != null)
                    {
                        isAsk2 = Paymmant.IsAskPaymmant;
                    }
                    if (isSignatureAsk && isAsk2)
                    {
                        liabilityAndInsuranceMV.SaveSigAndMethodPay();
                        if (liabilityAndInsuranceMV.What_form_of_payment_are_you_using_to_pay_for_transportation == "COD" || liabilityAndInsuranceMV.What_form_of_payment_are_you_using_to_pay_for_transportation == "COP" || liabilityAndInsuranceMV.What_form_of_payment_are_you_using_to_pay_for_transportation == "Biling")
                        {
                            liabilityAndInsuranceMV.GoToContinue();
                        }
                        else
                        {
                            await PopupNavigation.Instance.PopAllAsync(true);
                            await PopupNavigation.Instance.PushAsync(new TempPageHint4());
                            if (liabilityAndInsuranceMV.What_form_of_payment_are_you_using_to_pay_for_transportation == "Cash")
                            {
                                await Navigation.PushAsync(new VideoCameraPage(liabilityAndInsuranceMV, ""));
                            }
                            else if (liabilityAndInsuranceMV.What_form_of_payment_are_you_using_to_pay_for_transportation == "Check")
                            {
                                await Navigation.PushAsync(new CameraPaymmant(liabilityAndInsuranceMV, "", "CheckPaymment.png"));
                            }
                            else
                            {
                                await Navigation.PushAsync(new Ask2Page(liabilityAndInsuranceMV.managerDispatchMob, liabilityAndInsuranceMV.IdVech, liabilityAndInsuranceMV.IdShip, liabilityAndInsuranceMV.initDasbordDelegate));
                            }
                        }
                    }
                    else
                    {
                        ((Entry)sender).Text = "";
                        blockAskPay.IsVisible = false;
                        await liabilityAndInsuranceMV._popupNavigation.PushAsync(new Alert(LanguageHelper.WithoutTranslationAskErrorAlert, null));
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

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            isAsk3 = true;
            if (Paymmant != null)
            {
                isAsk2 = Paymmant.IsAskPaymmant;
            }
            if (isSignatureAsk && isAsk2)
            {
                btnSave.IsVisible = false;
                bloclThank.IsVisible = true;
                blockPsw.IsVisible = true;
                await PopupNavigation.Instance.PushAsync(new CopyLibaryAndInsurance(liabilityAndInsuranceMV));

            }
            else
            {
                await liabilityAndInsuranceMV._popupNavigation.PushAsync(new Alert(LanguageHelper.AskErrorAlert, null));
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
            bloclThank.IsVisible = false;
            blockPsw.IsVisible = false;
            liabilityAndInsuranceMV.SetProblem();
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
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            _helperView.Hidden();
        }
    }
}