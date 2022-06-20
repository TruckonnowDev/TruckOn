using MDispatch.Models;
using MDispatch.Service;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View.PageApp.DialogPage;
using MDispatch.View.ServiceView.ResizeImage;
using MDispatch.ViewModels.PageAppMV.VehicleDetals;
using Rg.Plugins.Popup.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.PageApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VechicleDetails : ContentPage
    {

        VehicleDetailsMV vehicleDetailsMV = null;

        public VechicleDetails(VehiclwInformation vehiclwInformation, IManagerDispatchMobService managerDispatchMob)
        {
            vehicleDetailsMV = new VehicleDetailsMV(managerDispatchMob, Convert.ToInt32(vehiclwInformation.Id), this, Navigation);
            InitializeComponent();
            BindingContext = vehicleDetailsMV;
        }

        public async Task InitPhoto(VehiclwInformation vehiclwInformation)
        {
            await PopupNavigation.Instance.PushAsync(new LoadPage());
            AddScan(vehiclwInformation);
            AddBlocItemPhoto(vehiclwInformation);
            AddBlocSeatBelts(vehiclwInformation);
            AddBlocTakePictures(vehiclwInformation);
            AddBlocInspectionPhoto(vehiclwInformation);
            AddBlocInspectionPhoto1(vehiclwInformation);
            AddBlocPhotoClient(vehiclwInformation);
            await PopupNavigation.Instance.PopAsync();
        }

        private void AddScan(VehiclwInformation vehiclwInformation)
        {
            if(vehiclwInformation.Scan != null)
            {
                VechInfoSt3.Children.Add(new Image()
                {
                    Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(vehiclwInformation.Scan.Base64)))
                });
                VechInfoSt1.Children.Add(new StackLayout()
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
            }
            else
            {
                VechInfoSt3.IsVisible = false;
                VechInfoSt1.IsVisible = false;
                VechInfoSt2.IsVisible = true;
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

        [Obsolete]
        private void AddBlocItemPhoto(VehiclwInformation vehiclwInformation)
        {
            if (vehiclwInformation.Ask != null && vehiclwInformation.Ask.Any_personal_or_additional_items_with_or_in_vehicle != null)
            {
                foreach (var item in vehiclwInformation.Ask.Any_personal_or_additional_items_with_or_in_vehicle)
                {
                    Image image = new Image()
                    {
                        Source = ImageSource.FromStream(() => new MemoryStream(ResizeImage(item.Base64))),
                        HeightRequest = 100,
                        WidthRequest = 100,
                        Margin = 3
                    };
                    image.GestureRecognizers.Add(new TapGestureRecognizer(VievFull));
                    blockItems.Children.Add(image);
                }
            }
            else
            {
                blockItems.IsVisible = false;
                itemsNotContent.IsVisible = true;
            }
        }

        [Obsolete]
        private void AddBlocSeatBelts(VehiclwInformation vehiclwInformation)
        {
            if (vehiclwInformation.Ask1 != null && vehiclwInformation.Ask1.App_will_force_driver_to_take_pictures_of_each_strap != null)
            {
                foreach (var item in vehiclwInformation.Ask1.App_will_force_driver_to_take_pictures_of_each_strap)
                {
                    Image image = new Image()
                    {
                        Source = ImageSource.FromStream(() => new MemoryStream(ResizeImage(item.Base64))),
                        HeightRequest = 100,
                        WidthRequest = 100,
                        Margin = 3
                    };
                    image.GestureRecognizers.Add(new TapGestureRecognizer(VievFull));
                    blockSeatBelts.Children.Add(image);
                }
            }
            else
            {
                blockSeatBelts.IsVisible = false;
                strapNotContent.IsVisible = true;
            }
        }

        [Obsolete]
        private void AddBlocTakePictures(VehiclwInformation vehiclwInformation)
        {
            if (vehiclwInformation.Ask1 != null && vehiclwInformation.Ask1.Photo_after_loading_in_the_truck != null)
            {
                foreach (var item in vehiclwInformation.Ask1.Photo_after_loading_in_the_truck)
                {
                    Image image = new Image()
                    {
                        Source = ImageSource.FromStream(() => new MemoryStream(ResizeImage(item.Base64))),
                        HeightRequest = 100,
                        WidthRequest = 100,
                        Margin = 3
                    };
                    image.GestureRecognizers.Add(new TapGestureRecognizer(VievFull));
                    blockTakePictures.Children.Add(image);
                }
            }
            else
            {
                blockTakePictures.IsVisible = false;
                TakePicturesNotContent.IsVisible = true;
            }
        }

        [Obsolete]
        private void AddBlocPhotoClient(VehiclwInformation vehiclwInformation)
        {
            //if (vehiclwInformation.askForUserDelyveryM != null && vehiclwInformation.askForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo != null)
            //{
            //    foreach (var item in vehiclwInformation.askForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up_photo)
            //    {
            //        Image image = new Image()
            //        {
            //            Source = ImageSource.FromStream(() => new MemoryStream(ResizeImage(item.Base64))),
            //            HeightRequest = 100,
            //            WidthRequest = 100,
            //            Margin = 3
            //        };
            //        image.GestureRecognizers.Add(new TapGestureRecognizer(VievFull));
            //        blockPhotoInspectedClient.Children.Add(image);
            //    }
            //}
            //else if(vehiclwInformation.askForUserDelyveryM != null && vehiclwInformation.askForUserDelyveryM.Have_you_inspected_the_vehicle_For_any_additional_imperfections_other_than_listed_at_the_pick_up != null)
            //{
            //    blockPhotoInspectedClient.IsVisible = false;
            //    answerClient.IsVisible = true;
            //}
            //else
            //{
            //    blockPhotoInspectedClient.IsVisible = false;
            //    photoClientNotContent.IsVisible = true;
            //}
        }

        [Obsolete]
        private void AddBlocInspectionPhoto(VehiclwInformation vehiclwInformation)
        {
            if (vehiclwInformation.PhotoInspections != null && vehiclwInformation.PhotoInspections.FirstOrDefault(p => p.CurrentStatusPhoto == "PikedUp") != null)
            {
                foreach (var item in vehiclwInformation.PhotoInspections)
                {
                    if (item.CurrentStatusPhoto == "PikedUp")
                    {
                        foreach (var photo in item.Photos)
                        {
                            Image image = new Image()
                            {
                                Source = ImageSource.FromStream(() => new MemoryStream(ResizeImage(photo.Base64))),
                                HeightRequest = 70,
                                WidthRequest = 70,
                                Margin = 3
                            };
                            image.GestureRecognizers.Add(new TapGestureRecognizer(VievFull));
                            blockPhotoInspection.Children.Add(image);
                        }
                    }
                }
            }
            else
            {
                blockPhotoInspection.IsVisible = false;
                photoInspectionNotContent.IsVisible = true;
            }
        }

        private void AddBlocInspectionPhoto1(VehiclwInformation vehiclwInformation)
        {
            if (vehiclwInformation.PhotoInspections != null && vehiclwInformation.PhotoInspections.FirstOrDefault(p => p.CurrentStatusPhoto == "Delyvery") != null)
            {
                foreach (var item in vehiclwInformation.PhotoInspections)
                {
                    if (item.CurrentStatusPhoto == "Delyvery")
                    {
                        foreach (var photo in item.Photos)
                        {
                            Image image = new Image()
                            {
                                Source = ImageSource.FromStream(() => new MemoryStream(ResizeImage(photo.Base64))),
                                HeightRequest = 70,
                                WidthRequest = 70,
                                Margin = 3
                            };
                            blockPhotoInspection1.Children.Add(image);
                        }
                    }
                }
            }
            else
            {
                blockPhotoInspection1.IsVisible = false;
                photoInspectionNotContent1.IsVisible = true;
            }
        }

        protected byte[] ResizeImage(string base64)
        {
            var assembly = typeof(VechicleDetails).GetTypeInfo().Assembly;
            byte[] imageData;
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(base64)))
            {
                imageData = ms.ToArray();;
            }
            int width = DependencyService.Get<IResizeImage>().GetWidthImage(imageData);
            int heigth = DependencyService.Get<IResizeImage>().GetHeigthImage(imageData);
            return DependencyService.Get<IResizeImage>().ResizeImage(imageData, (width / 100) * 44, (heigth / 100) * 44);
        }
    }
}