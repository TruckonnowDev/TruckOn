﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MDispatch.Models;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.View.Inspection.PickedUp;
using MDispatch.View.PageApp;
using MDispatch.View.ServiceView.ResizeImage;
using MDispatch.ViewModels.InspectionMV;
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
                FlexLayout flexLayout = new FlexLayout()
                {
                    Wrap = FlexWrap.Wrap,
                };
                blockPhotoInspection.Children.Add(new Label()
                {
                    Text = $"{vehiclwInformation.Year} {vehiclwInformation.Make} {vehiclwInformation.Model} {vehiclwInformation.Type}",
                    FontSize = 18,
                    Margin = new Thickness(5, 5, 0, 0),
                });
                if (vehiclwInformation.PhotoInspections.FirstOrDefault(p => p.CurrentStatusPhoto == "PikedUp") != null)
                {
                    blockPhotoInspection.Children.Add(new Label()
                    {
                        Text = "Photo inspection Picked Up",
                        Margin = new Thickness(10, 5, 0, 0),
                        FontSize = 15
                    });
                    foreach (var photoInspection in vehiclwInformation.PhotoInspections.Where(p => p.CurrentStatusPhoto == "PikedUp"))
                    {
                        foreach (var photo in photoInspection.Photos)
                        {
                            Image image = new Image()
                            {
                                Source = ImageSource.FromStream(() => new MemoryStream(ResizeImage(photo.Base64))),
                                HeightRequest = 70,
                                WidthRequest = 70,
                                Margin = new Thickness(6, 0, 0, 0)
                            };
                            //image.GestureRecognizers.Add(new TapGestureRecognizer(VievFull));
                            flexLayout.Children.Add(image);
                        }
                        blockPhotoInspection.Children.Add(flexLayout);
                    }
                }
                //blockPhotoInspection.Children.Add(new BoxView()
                //{
                //    Color = Color.BlueViolet,
                //    HeightRequest = 1,
                //    Margin = new Thickness(5, 0, 5, 0)
                //});
                if (vehiclwInformation.PhotoInspections.FirstOrDefault(p => p.CurrentStatusPhoto == "Delivery") != null)
                {
                    blockPhotoInspection.Children.Add(new Label()
                    {
                        Text = "Photo inspection Delivery",
                        Margin = new Thickness(10, 5, 0, 0),
                        FontSize = 15
                    });
                    foreach (var photoInspection in vehiclwInformation.PhotoInspections.Where(p => p.CurrentStatusPhoto == "Delivery"))
                    {
                        foreach (var photo in photoInspection.Photos)
                        {
                            Image image = new Image()
                            {
                                Source = ImageSource.FromStream(() => new MemoryStream(ResizeImage(photo.Base64))),
                                HeightRequest = 70,
                                WidthRequest = 70,
                                Margin = 3
                            };
                            //image.GestureRecognizers.Add(new TapGestureRecognizer(VievFull));
                            flexLayout.Children.Add(image);
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
            MemoryStream memoryStream = new MemoryStream();
            StreamImageSource streamImageSource = (StreamImageSource)image.Source;
            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
            Stream stream = await streamImageSource.Stream(cancellationToken);
            stream.CopyTo(memoryStream);
            await Navigation.PushAsync(new ViewPhoto(memoryStream.ToArray()));
        }

        private async void InitElemnt()
        {
            if (bOLMV.Shipping != null && bOLMV.Shipping.VehiclwInformations != null)
            {
                foreach (var VehiclwInformation in bOLMV.Shipping.VehiclwInformations)
                {
                    VechInfoSt.Children.Add(new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label()
                            {
                                Text = VehiclwInformation.Year,
                                FontSize = 18,
                                TextColor = Color.Black
                            },
                            new Label()
                            {
                                Text = VehiclwInformation.Make,
                                FontSize = 18,
                                TextColor = Color.Black
                            },
                            new Label()
                            {
                                Text = VehiclwInformation.Model,
                                FontSize = 18,
                                TextColor = Color.Black
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
                                        FontSize = 18,
                                    },
                                    new Label()
                                    {
                                        Text = VehiclwInformation.VIN,
                                        FontSize = 18,
                                        TextColor = Color.Black
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
                                        Text = "Type:",
                                        FontSize = 18,
                                    },
                                    new Label()
                                    {
                                        Text = VehiclwInformation.Type,
                                        FontSize = 18,
                                        TextColor = Color.Black
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
                                Text = "Color:",
                                FontSize = 18,
                            },
                            new Label()
                            {
                                Text = VehiclwInformation.Color,
                                FontSize = 18,
                                TextColor = Color.Black
                            }
                        }
                    });

                    VechInfoSt.Children.Add(new Image()
                    {
                        Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(VehiclwInformation.Scan.Base64)))
                    });

                    VechInfoSt.Children.Add(new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                                    new Image()
                                    {
                                        Source = "DamageP1.png",
                                        HeightRequest = 18,
                                        WidthRequest = 18
                                    },
                                    new Label()
                                    {
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        Text = "Circles Yellow — pickup damages;",
                                        FontSize = 13
                                    },
                                    new Image()
                                    {
                                        Source = "DamageD1.png",
                                        HeightRequest = 18,
                                        WidthRequest = 18
                                    },
                                    new Label()
                                    {
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        Text = "Circles Green — delivery damages;",
                                        FontSize = 13
                                    },
                        }
                    });

                    FlexLayout flexLayout = new FlexLayout()
                    {
                        Wrap = FlexWrap.Wrap,
                        Opacity = 0.7,
                        BackgroundColor = Color.FromHex("#F3F781"),
                        Children =
                        {
                            new Label()
                            {
                                HorizontalTextAlignment = TextAlignment.Center,
                                Text = "See inspection photo:",
                                FontSize = 16
                            },
                            new Label()
                            {
                                HorizontalTextAlignment = TextAlignment.Center,
                                TextColor = Color.Blue,
                                Text = $"{Config.BaseReqvesteUrl}Photo/BOL/{VehiclwInformation.Id}",
                                FontSize = 16
                            }
                        }
                    };
                    flexLayout.GestureRecognizers.Add(new TapGestureRecognizer(GetPagePhotoInspection));
                    VechInfoSt.Children.Add(flexLayout);
                }
            }
            //bOLMV.StataLoadShip = 0;
        }

        private async void GetPagePhotoInspection(Xamarin.Forms.View v, object s)
        {
            Label label = (Label)((FlexLayout)v).Children[1];
            await Navigation.PushAsync(new PhotoInspectionWeb(label.Text));
        }

        //private async Task Wait()
        //{
        //    await Task.Run(() =>
        //    {
        //        while (bOLMV.StataLoadShip == 0)
        //        {

        //        }
        //    });
        //}

        protected byte[] ResizeImage(string base64)
        {
            var assembly = typeof(BOLPage).GetTypeInfo().Assembly;
            byte[] imageData;
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                imageData = ms.ToArray(); ;
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

        protected override  void OnDisappearing()
        {
            base.OnDisappearing();
             HelpersView.Hidden();
        }
    }
}