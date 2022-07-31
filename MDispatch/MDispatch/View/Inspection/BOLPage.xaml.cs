using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.Service.HelpersViews;
using MDispatch.View.Inspection.PickedUp;
using MDispatch.View.ServiceView.ResizeImage;
using MDispatch.ViewModels.InspectionMV;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.View.Inspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BOLPage : ContentPage
    {
        private BOLMV bOLMV = null;

        public BOLPage(ManagerDispatchMob managerDispatchMob, string idShip, InitDasbordDelegate initDasbordDelegate)
        {
            bOLMV = new BOLMV(managerDispatchMob, idShip, Navigation, initDasbordDelegate, this);
            InitializeComponent();
            BindingContext = bOLMV;

        }

        [Obsolete]
        public async Task InitPhoto(List<VehiclwInformation> vehiclwInformations)
        {
            bOLMV.IsLoad = true;
            InitElemnt();
            foreach (VehiclwInformation vehiclwInformation in vehiclwInformations)
            {
                AddBlocInspectionPhoto(vehiclwInformation);
            }
            bOLMV.IsLoad = false;
        }

        [Obsolete]
        private void AddBlocInspectionPhoto(VehiclwInformation vehiclwInformation)
        {
            if (vehiclwInformation.PhotoInspections != null && vehiclwInformation.PhotoInspections.Count != 0)
            {
                string fontBold = ((OnPlatform<string>)Application.Current.Resources["OpenSans-Bold"]).Platforms.ToList().First(p => p.Platform.FirstOrDefault(pp => pp == Device.RuntimePlatform) != null).Value.ToString();
                string fontRegular = ((OnPlatform<string>)Application.Current.Resources["OpenSans-Regular"]).Platforms.ToList().First(p => p.Platform.FirstOrDefault(pp => pp == Device.RuntimePlatform) != null).Value.ToString();
                FlexLayout flexLayout = new FlexLayout()
                {
                    Wrap = FlexWrap.Wrap,
                };
                blockPhotoInspection.Children.Add(new Label()
                {
                    Text = $"{vehiclwInformation.Year} {vehiclwInformation.Make} {vehiclwInformation.Model} {vehiclwInformation.Type}",
                    FontSize = 16,
                    Margin = new Thickness(0, 20, 0, 5),
                    FontFamily = fontBold,
                });
                if (vehiclwInformation.PhotoInspections.FirstOrDefault(p => p.CurrentStatusPhoto == "PikedUp") != null)
                {
                    blockPhotoInspection.Children.Add(new Label()
                    {
                        Text = LanguageHelper.WithoutTranslationTitlePhotoInspactionPickedUp,
                        FontSize = 14,
                        FontFamily = fontBold,
                        Margin = new Thickness(0, 0, 0, 15),
                    });
                    foreach (var photoInspection in vehiclwInformation.PhotoInspections.Where(p => p.CurrentStatusPhoto == "PikedUp"))
                    {
                        foreach (var photo in photoInspection.Photos)
                        {
                            Frame imagebox = new Frame()
                            {
                                HeightRequest = 50,
                                BackgroundColor = Color.Transparent,
                                WidthRequest = 106,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Padding = new Thickness(0),
                                Margin = new Thickness(0, 0, 5, 5),
                            };
                            Image image = new Image()
                            {
                                Source = ImageSource.FromUri(new Uri($"{Config.BaseReqvesteUrl}/Mobile/Image?name={photo.path}&type=jpg")),
                                HeightRequest = photo.Height,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                WidthRequest = photo.Width,
                                Margin = new Thickness(0, 0, 5, 5)
                            };
                            imagebox.Content = image;
                            image.GestureRecognizers.Add(new TapGestureRecognizer(VievFull));
                            flexLayout.Children.Add(imagebox);
                        }
                        blockPhotoInspection.Children.Add(flexLayout);
                        blockPhotoInspection.Children.Add(flexLayout);
                    }
                }
                if (vehiclwInformation.PhotoInspections.FirstOrDefault(p => p.CurrentStatusPhoto == "Delivery") != null)
                {
                    blockPhotoInspection.Children.Add(new Label()
                    {
                        Text = LanguageHelper.WithoutTranslationTitlePhotoInspactionDelivery,
                        FontSize = 14,
                        FontFamily = fontBold,
                        Margin = new Thickness(0, 5, 0, 15),
                    });
                    foreach (var photoInspection in vehiclwInformation.PhotoInspections.Where(p => p.CurrentStatusPhoto == "Delivery"))
                    {
                        foreach (var photo in photoInspection.Photos)
                        {
                            Frame imagebox = new Frame()
                            {
                                HeightRequest = 50,
                                BackgroundColor = Color.Transparent,
                                WidthRequest = 106,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Padding = new Thickness(0),
                                Margin = new Thickness(0, 0, 5, 5),
                            };
                            Image image = new Image()
                            {
                                Source = ImageSource.FromUri(new Uri($"{Config.BaseReqvesteUrl}/Mobile/Image?name={photo.path}&type=jpg")),
                                HeightRequest = photo.Height,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                WidthRequest = photo.Width,
                                Margin = new Thickness(0, 0, 5, 5)
                            };
                            imagebox.Content = image;
                            image.GestureRecognizers.Add(new TapGestureRecognizer(VievFull));
                            flexLayout.Children.Add(imagebox);
                        }
                        blockPhotoInspection.Children.Add(flexLayout);
                    }
                }
                blockPhotoInspection.IsVisible = true;
            }
            else
            {
                blockPhotoInspection.IsVisible = false;
            }
        }

        private async void VievFull(Xamarin.Forms.View v, object s)
        {
            Image image = ((Image)v);
            await OpacityTouchView.TouchFeedBack(image);
            await PopupNavigation.PushAsync(new ViewPhoto(image.Source));
        }

        private async void InitElemnt()
        {
            if (bOLMV.Shipping != null && bOLMV.Shipping.VehiclwInformations != null)
            {
                string fontBold = ((OnPlatform<string>)Application.Current.Resources["OpenSans-Bold"]).Platforms.ToList().First(p => p.Platform.FirstOrDefault(pp => pp == Device.RuntimePlatform) != null).Value.ToString();
                string fontRegular = ((OnPlatform<string>)Application.Current.Resources["OpenSans-Regular"]).Platforms.ToList().First(p => p.Platform.FirstOrDefault(pp => pp == Device.RuntimePlatform) != null).Value.ToString();
                foreach (var VehiclwInformation in bOLMV.Shipping.VehiclwInformations)
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
                                        Text = VehiclwInformation.VIN ?? "--------",
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
                                        Text = VehiclwInformation.Type ?? "--------",
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
                                Text = VehiclwInformation.Color ?? "--------",
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
                }
            }
            //bOLMV.StataLoadShip = 0;
        }

        private async void GetPagePhotoInspection(Xamarin.Forms.View v, object s)
        {
            Label label = (Label)((FlexLayout)v).Children[1];
            await Navigation.PushAsync(new PhotoInspectionWeb(label.Text));
        }

        protected byte[] ResizeImage(string base64)
        {
            var assembly = typeof(BOLPage).GetTypeInfo().Assembly;
            byte[] imageData;
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                imageData = ms.ToArray();
            }
            int width = DependencyService.Get<IResizeImage>().GetWidthImage(imageData);
            int heigth = DependencyService.Get<IResizeImage>().GetHeigthImage(imageData);
            return DependencyService.Get<IResizeImage>().ResizeImage(imageData, (width / 100) * 40, (heigth / 100) * 40);
        }

        [Obsolete]
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await bOLMV.SendLiabilityAndInsuranceEmaile();
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