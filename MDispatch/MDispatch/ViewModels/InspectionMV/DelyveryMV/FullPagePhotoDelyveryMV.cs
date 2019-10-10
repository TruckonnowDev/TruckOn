﻿using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service;
using MDispatch.Service.Net;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using MDispatch.View.Inspection.Delyvery;
using MDispatch.View.PageApp;
using MDispatch.ViewModels.AskPhoto;
using MDispatch.ViewModels.InspectionMV.Models;
using MDispatch.ViewModels.InspectionMV.Servise.Models;
using Newtonsoft.Json;
using Plugin.Settings;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.ViewModels.InspectionMV.DelyveryMV
{
    public class FullPagePhotoDelyveryMV : BindableBase
    {
        public ManagerDispatchMob managerDispatchMob = null;
        public INavigation Navigation { get; set; }
        public ICar Car = null;
        private InitDasbordDelegate initDasbordDelegate = null;
        private GetVechicleDelegate getVechicleDelegate = null;

        public FullPagePhotoDelyveryMV(ManagerDispatchMob managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, string typeCar,
            int inderxPhotoInspektion, INavigation navigation, InitDasbordDelegate initDasbordDelegate, GetVechicleDelegate getVechicleDelegate,
            string onDeliveryToCarrier, string totalPaymentToCarrier)
        {
            this.getVechicleDelegate = getVechicleDelegate;
            Navigation = navigation;
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
            this.InderxPhotoInspektion = inderxPhotoInspektion;
            IdShip = idShip;
            OnDeliveryToCarrier = onDeliveryToCarrier;
            TotalPaymentToCarrier = totalPaymentToCarrier;
            Car = GetTypeCar(typeCar.Replace(" ", ""));
            Init();
        }

        private async void Init()
        {
            await Car.OrintableScreen(Car.GetIndexCarFullPhoto(inderxPhotoInspektion));
        }

        public string IdShip { get; set; }
        public string OnDeliveryToCarrier { get; set; }
        public string TotalPaymentToCarrier { get; set; }

        private int inderxPhotoInspektion = 0;
        public int InderxPhotoInspektion
        {
            get => inderxPhotoInspektion;
            set => SetProperty(ref inderxPhotoInspektion, value);
        }

        private VehiclwInformation vehiclwInformation = null;
        public VehiclwInformation VehiclwInformation
        {
            get => vehiclwInformation;
            set => SetProperty(ref vehiclwInformation, value);
        }

        private ImageSource sourseImage = null;
        public ImageSource SourseImage
        {
            get => sourseImage;
            set => SetProperty(ref sourseImage, value);
        }

        private List<ImageSource> allSourseImage = null;
        public List<ImageSource> AllSourseImage
        {
            get => allSourseImage;
            set => SetProperty(ref allSourseImage, value);
        }

        internal void SetDamage(string nameDamage, int indexDamage, string prefNameDamage, double xInterest, double yInterest, int widthDamage, int heightDamage, Image image, ImageSource imageSource1)
        {
            Damage damage = new Damage();
            damage.FullNameDamage = $"{prefNameDamage} - {nameDamage}";
            damage.IndexImageVech = InderxPhotoInspektion.ToString();
            damage.TypeDamage = nameDamage;
            damage.TypePrefDamage = prefNameDamage;
            damage.IndexDamage = indexDamage;
            damage.XInterest = xInterest;
            damage.YInterest = yInterest;
            damage.WidthDamage = widthDamage;
            damage.HeightDamage = heightDamage;
            damage.Image = image;
            damage.TypeCurrentStatus = "D";
            damage.ImageSource = imageSource1;
            if (PhotoInspection.Damages == null)
            {
                PhotoInspection.Damages = new List<Damage>();
            }
            PhotoInspection.Damages.Add(damage);
        }
        
        public void ReSetDamage(Image image, int widthDamage, int heightDamage)
        {
            if (image != null && PhotoInspection.Damages != null && PhotoInspection.Damages.FirstOrDefault(d => d.Image == image) != null)
            {
                int damageIndex = PhotoInspection.Damages.FindIndex(d => d.Image == image);
                PhotoInspection.Damages[damageIndex].WidthDamage = widthDamage;
                PhotoInspection.Damages[damageIndex].HeightDamage = heightDamage;
            }
        }

        public void RemmoveDamage(Image image)
        {
            if (image != null && PhotoInspection.Damages != null && PhotoInspection.Damages.FirstOrDefault(d => d.Image == image) != null)
            {
                List<ImageSource> imageSources2 = new List<ImageSource>(AllSourseImage);
                Damage damage = PhotoInspection.Damages.FirstOrDefault(d => d.Image == image);
                imageSources2.Remove(imageSources2.FirstOrDefault(i => i == damage.ImageSource));
                AllSourseImage = imageSources2;
                PhotoInspection.Damages.Remove(damage);
            }
        }

        public ImageSource SelectPhotoForDamage(Image image)
        {
            if (PhotoInspection != null && PhotoInspection.Damages != null)
            {
                Damage damage1 = PhotoInspection.Damages.FirstOrDefault(d => d.Image == image);
                SourseImage = damage1.ImageSource;
                return damage1.ImageSource;
            }
            return null;
        }

        private List<Damage> damages = null;
        public List<Damage> Damages
        {
            get => damages;
            set => SetProperty(ref damages, value);
        }

        private PhotoInspection photoInspection = null;
        public PhotoInspection PhotoInspection
        {
            get => photoInspection;
            set => SetProperty(ref photoInspection, value);
        }

        private ICar GetTypeCar(string typeCar)
        {
            ICar car = null;
            switch(typeCar)
            {
                case "PickUp":
                    {
                        car = new CarPickUp();
                        break;
                    }
                case "Coupe":
                    {
                        car = new CarCoupe();
                        break;
                    }
                case "Suv":
                    {
                        car = new CarSuv();
                        break;
                    }
                case "Sedan":
                    {
                        car = new CarSedan();
                        break;
                    }
            }
            return car;
        }

        public void AddNewFotoSourse(byte[] imageSorseByte)
        {
            if (AllSourseImage == null)
            {
                AllSourseImage = new List<ImageSource>();
            }
            List<ImageSource> imageSources1 = new List<ImageSource>(AllSourseImage);
            imageSources1.Add(ImageSource.FromStream(() => new MemoryStream(imageSorseByte)));
            AllSourseImage = imageSources1;
        }

        public async void SetPhoto(byte[] PhotoInArrayByte)
        {
            if (PhotoInspection == null)
            {
                PhotoInspection = new PhotoInspection();
            }
            if (PhotoInspection.Photos == null)
            {
                PhotoInspection.Photos = new List<Photo>();
            }
            PhotoInspection.IndexPhoto = InderxPhotoInspektion;
            PhotoInspection.CurrentStatusPhoto = "Delyvery";
            Photo photo = new Photo();
            string photoJson = Convert.ToBase64String(PhotoInArrayByte);
            string pathIndePhoto = PhotoInspection.Photos.Count == 0 ? PhotoInspection.IndexPhoto.ToString() : $"{PhotoInspection.IndexPhoto}.{PhotoInspection.Photos.Count}";
            PhotoInspection.CurrentStatusPhoto = "Delyvery";
            photo.Base64 = photoJson;
            photo.path = $"../Photo/{VehiclwInformation.Id}/Delyvery/PhotoInspection/{pathIndePhoto}.jpg";
            PhotoInspection.Photos.Add(photo);
        }

        [System.Obsolete]
        public async void SavePhoto(bool isNavigWthDamag = false)
        {
            bool isNavigationMany = false;
            int navigationStack_Count = isNavigWthDamag ? Navigation.NavigationStack.Count - 1 : Navigation.NavigationStack.Count;
            if (navigationStack_Count > 2)
            {
                await PopupNavigation.PushAsync(new LoadPage());
                isNavigationMany = true;
            }
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            if (InderxPhotoInspektion >= Car.CountCarImg)
            {
                DependencyService.Get<IOrientationHandler>().ForceSensor();
                await CheckVechicleAndGoToResultPage();
            }
            else
            {
                Car.OrintableScreen(InderxPhotoInspektion);
                FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(managerDispatchMob, VehiclwInformation, IdShip, $"{Car.typeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", Car.typeIndex.Replace(" ", ""), InderxPhotoInspektion + 1, initDasbordDelegate, getVechicleDelegate, Car.GetNameLayout(InderxPhotoInspektion + 1), OnDeliveryToCarrier, TotalPaymentToCarrier);
                await Navigation.PushAsync(fullPagePhotoDelyvery);
                await Navigation.PushAsync(new CameraPagePhoto1($"{Car.typeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", fullPagePhotoDelyvery));
            }
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("SavePhoto", token, VehiclwInformation.Id, PhotoInspection, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (isNavigWthDamag)
                {
                    Navigation.RemovePage(Navigation.NavigationStack[2]);
                }
                if (state == 2)
                {
                    if (isNavigationMany)
                    {
                        await PopupNavigation.RemovePageAsync(PopupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        await Navigation.PopAsync();
                    }
                    await PopupNavigation.PushAsync(new Errror(description, Navigation));
                }
                else if (state == 3)
                {
                    if (isNavigationMany)
                    {
                        await PopupNavigation.RemovePageAsync(PopupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    DependencyService.Get<IToast>().ShowMessage($"Photo {Car.GetNameLayout(Car.GetIndexCarFullPhoto(inderxPhotoInspektion))} saved");
                }
                else if (state == 4)
                {
                    if (isNavigationMany)
                    {
                        await PopupNavigation.RemovePageAsync(PopupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        await Navigation.PopAsync();
                    }
                    await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigation));
                }
            }
            else
            {
                if (Navigation.NavigationStack.Count > 1)
                {
                    await Navigation.PopAsync();
                }
            }
        }

        [System.Obsolete]
        public async void Continue()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    string status = null;
                    if (TotalPaymentToCarrier == "COD" || TotalPaymentToCarrier == "COP")
                    {
                        status = "Delivered,Paid";
                    }
                    else
                    {
                        status = "Delivered,Billed";
                    }
                    state = managerDispatchMob.Recurent(token, IdShip, status, ref description);
                });
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Errror(description, null));
                }
                else if (state == 3)
                {
                    initDasbordDelegate.Invoke();
                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                }
            }
        }

        [System.Obsolete]
        private async Task CheckVechicleAndGoToResultPage()
        {
            List<VehiclwInformation> vehiclwInformation1s = getVechicleDelegate.Invoke();
            int indexCurrentVechecle = vehiclwInformation1s.FindIndex(v => v == VehiclwInformation);
            if (vehiclwInformation1s.Count - 1 == indexCurrentVechecle)
            {
                Continue();
                await Navigation.PopToRootAsync();
            }
            else
            {
                await PopupNavigation.PushAsync(new HintPageVechicle("Continuing inspection Deliveri", vehiclwInformation1s[indexCurrentVechecle + 1]));
                ICar Car = GetTypeCar(vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
                FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(managerDispatchMob, VehiclwInformation, IdShip, $"{Car.typeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", Car.typeIndex.Replace(" ", ""), InderxPhotoInspektion + 1, initDasbordDelegate, getVechicleDelegate, Car.GetNameLayout(Car.GetIndexCarFullPhoto(inderxPhotoInspektion + 1)), OnDeliveryToCarrier, TotalPaymentToCarrier);
                await Navigation.PushAsync(fullPagePhotoDelyvery);
                await Navigation.PushAsync(new CameraPagePhoto1($"{Car.typeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", fullPagePhotoDelyvery));
            }
        }

        public void ReSetPhoto(byte[] newPhoto, byte[] oldPhoto)
        {
            List<ImageSource> imageSources1 = new List<ImageSource>(AllSourseImage);
            Photo photo = new Photo();
            int Index = PhotoInspection.Photos.FindIndex(p => p.Base64 == JsonConvert.SerializeObject(oldPhoto));
            Photo photoOld = PhotoInspection.Photos.FirstOrDefault(p => p.Base64 == JsonConvert.SerializeObject(oldPhoto));
            photo.path = photoOld.path;
            photoOld = null;
            photo.Base64 = JsonConvert.SerializeObject(newPhoto);
            PhotoInspection.Photos.RemoveAt(Index);
            PhotoInspection.Photos.Insert(Index, photo);
            Index = imageSources1.FindIndex(a => Convert.ToBase64String(GetBytesInImageSourse(a)) == Convert.ToBase64String(oldPhoto));
            imageSources1.RemoveAt(Index);
            imageSources1.Insert(Index, ImageSource.FromStream(() => new MemoryStream(newPhoto)));
            AllSourseImage = imageSources1;
            SourseImage = AllSourseImage[Index];
            if (PhotoInspection.Damages != null)
            {
                PhotoInspection.Damages.FirstOrDefault(d => Convert.ToBase64String(GetBytesInImageSourse(d.ImageSource)) == Convert.ToBase64String(oldPhoto)).ImageSource = AllSourseImage[Index];
            }
        }

        private byte[] GetBytesInImageSourse(ImageSource imageSource)
        {
            byte[] sourseImage = null;
            StreamImageSource streamImageSource = (StreamImageSource)imageSource;
            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
            Task<Stream> task = streamImageSource.Stream(cancellationToken);
            Stream stream = task.Result;
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            sourseImage = ms.ToArray();
            return sourseImage;
        }
    }
}