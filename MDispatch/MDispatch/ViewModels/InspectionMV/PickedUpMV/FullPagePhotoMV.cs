﻿using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Models.Enum;
using MDispatch.Models.ModelDataBase;
using MDispatch.NewElement;
using MDispatch.NewElement.Directory;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.Service.RequestQueue;
using MDispatch.View.Inspection;
using MDispatch.View.PageApp;
using MDispatch.ViewModels.InspectionMV.Servise.Models;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.InspectionMV.PickedUpMV
{
    public class FullPagePhotoMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public IVehicle Car = null;
        private InitDasbordDelegate initDasbordDelegate = null;
        private GetVechicleDelegate getVechicleDelegate = null;
        private FullPagePhoto fullPagePhoto = null;
        private readonly IManagerQueueService _managerQueueService;
        private readonly IHelperViewService _helperView;

        public FullPagePhotoMV(
            IManagerDispatchMobService managerDispatchMob, 
            VehiclwInformation vehiclwInformation, 
            string idShip, string typeCar, 
            int inderxPhotoInspektion, INavigation navigation, 
            InitDasbordDelegate initDasbordDelegate, 
            GetVechicleDelegate getVechicleDelegate, 
            string onDeliveryToCarrier, string totalPaymentToCarrier, 
            FullPagePhoto fullPagePhoto)
            : base(navigation)
        {
            _managerQueueService = DependencyService.Get<IManagerQueueService>();
            _helperView = DependencyService.Get<IHelperViewService>();
            this.getVechicleDelegate = getVechicleDelegate;
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
            InderxPhotoInspektion = inderxPhotoInspektion;
            Car = GetTypeCar(typeCar.Replace(" ", ""));
            Init();
            IdShip = idShip;
            OnDeliveryToCarrier = onDeliveryToCarrier;
            TotalPaymentToCarrier = totalPaymentToCarrier;
            this.fullPagePhoto = fullPagePhoto;
        }

        private async void Init()
        {
            await Car.OrintableScreen(inderxPhotoInspektion);
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
        
        private PhotoInspection photoInspection = null;
        public PhotoInspection PhotoInspection
        {
            get => photoInspection;
            set => SetProperty(ref photoInspection, value);
        }

        private IVehicle GetTypeCar(string typeCar)
        {
            IVehicle car = null;
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
                case "Sportbike":
                    {
                        car = new MotorcycleSport();
                        break;
                    }
                case "Touringmotorcycle":
                    {
                        car = new MotorcycleTouring();
                        break;
                    }
                case "Cruisemotorcycle":
                    {
                        car = new MotorcycleСruising();
                        break;
                    }
                case "Tricycle":
                    {
                        car = new MotorcycleTricycle();
                        break;
                    }
            }
            return car;
        }

        public async void RemmoveDamage(Image image)
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

        public async void SetDamage(string nameDamage, int indexDamage, string prefNameDamage, double xInterest, double yInterest, int widthDamage, int heightDamage,  Image image, ImageSource imageSource1)
        {
            Damage damage = new Damage();
            damage.FullNameDamage = $"{prefNameDamage} - {nameDamage}";
            damage.IndexImageVech = InderxPhotoInspektion.ToString();
            damage.TypeDamage = nameDamage;
            damage.TypePrefDamage = prefNameDamage;
            damage.IndexDamage = indexDamage;
            damage.XInterest = xInterest;
            damage.YInterest = yInterest;
            damage.Image = image;
            damage.WidthDamage = widthDamage;
            damage.HeightDamage = heightDamage;
            damage.TypeCurrentStatus = "P";
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

        public void ReSetPhoto(byte[] newPhoto, byte[] oldPhoto)
        {
            List<ImageSource> imageSources1 = new List<ImageSource>(AllSourseImage);
            Photo photo = new Photo();
            int Index = PhotoInspection.Photos.FindIndex(p => p.Base64 == Convert.ToBase64String(oldPhoto));
            Photo photoOld = PhotoInspection.Photos.FirstOrDefault(p => p.Base64 == Convert.ToBase64String(oldPhoto));
            photo.path = photoOld.path;
            photoOld = null;
            photo.Base64 = Convert.ToBase64String(newPhoto);
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


        public async Task AddNewFotoSourse(byte[] imageSorseByte)
        {
            if (AllSourseImage == null)
            {
                AllSourseImage = new List<ImageSource>();
            }
            List<ImageSource> imageSources1 = new List<ImageSource>(AllSourseImage);
            imageSources1.Add(ImageSource.FromStream(() => new MemoryStream(imageSorseByte)));
            AllSourseImage = imageSources1;
        }

        public async Task SetPhoto(byte[] PhotoInArrayByte, double width = 0, double height = 0)
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
            PhotoInspection.CurrentStatusPhoto = "PikedUp";
            Photo photo = new Photo();
            string pathIndePhoto = PhotoInspection.Photos.Count == 0 ? PhotoInspection.IndexPhoto.ToString() : $"{PhotoInspection.IndexPhoto}.{PhotoInspection.Photos.Count}";
            PhotoInspection.CurrentStatusPhoto = "PikedUp";
            photo.Width = width;
            photo.Height = height;
            photo.Base64 = Convert.ToBase64String(PhotoInArrayByte);
            photo.path = $"../Photo/{VehiclwInformation.Id}/PikedUp/PhotoInspection/{pathIndePhoto}.jpg";
            PhotoInspection.Photos.Add(photo);
        }

        public async void SavePhoto(bool isNavigWthDamag = false)
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            if (InderxPhotoInspektion < Car.CountCarImg)
            {
                Car.OrintableScreen(InderxPhotoInspektion);
                FullPagePhoto fullPagePhoto = new FullPagePhoto(managerDispatchMob, VehiclwInformation, IdShip, $"{Car.TypeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", Car.TypeIndex.Replace(" ", ""), InderxPhotoInspektion + 1, initDasbordDelegate, getVechicleDelegate, Car.GetNameLayout(InderxPhotoInspektion + 1), OnDeliveryToCarrier, TotalPaymentToCarrier);
                await _navigation.PushAsync(fullPagePhoto);
                await _navigation.PushAsync(new CameraPagePhoto($"{Car.TypeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", fullPagePhoto, "PhotoIspection"));
            }
            else
            {
                DependencyService.Get<IOrientationHandler>().ForceSensor();
                await _navigation.PushAsync(new Ask1Page(managerDispatchMob, VehiclwInformation, IdShip, initDasbordDelegate, getVechicleDelegate, Car.TypeIndex.Replace(" ", ""), OnDeliveryToCarrier, TotalPaymentToCarrier), true);
            }
            await Task.Run(() => _utils.CheckNet(true, true));
            if (App.isNetwork)
            {
                if (isNavigWthDamag)
                {
                    _navigation.RemovePage(_navigation.NavigationStack[2]);
                }
                if (_navigation.NavigationStack.Count > 1)
                {
                    _navigation.RemovePage(_navigation.NavigationStack[1]);
                }
                await Task.Run(() =>
                {
                    _managerQueueService.AddRequest("SavePhoto", token, VehiclwInformation.Id, PhotoInspection);
                    initDasbordDelegate.Invoke();
                });
            }
            else
            {
                SelectMethodAction();
            }
        }

        private async void SelectMethodAction()
        {
            await ClosePageToFirstPageInspction();
            await fullPagePhoto.CreateActionSheet(SelectMethodAction, LanguageHelper.SelectBackToRootBage, LanguageHelper.SelectLoadGalery, LanguageHelper.SelectLoadFolderOffline, LanguageHelper.SelectLoadFolderOfflineAndGalery);
        }

        private async void SelectMethodAction(string actionSheet)
        {
            if (actionSheet == LanguageHelper.SelectBackToRootBage)
            {
                _helperView.ReSet();
                _helperView.CallError(LanguageHelper.NotNetworkAlert);
                BackToRootPage();
            }
            else if (actionSheet == LanguageHelper.SelectLoadGalery)
            {
                await SaveAllPhotoInspactionToGalery();
                await NextInspectionPhotoPage();
            }
            else if (actionSheet == LanguageHelper.SelectLoadFolderOffline)
            {
                await SaveAllPhotoInspactionToOflineFolder();
                await NextInspectionPhotoPage();
            }
            else if (actionSheet == LanguageHelper.SelectLoadFolderOfflineAndGalery)
            {
                await SaveAllPhotoInspactionToGalery();
                await SaveAllPhotoInspactionToOflineFolder();
                await NextInspectionPhotoPage();
            }
        }

        private async Task NextInspectionPhotoPage()
        {
            FullPagePhoto fullPagePhoto = new FullPagePhoto(managerDispatchMob, VehiclwInformation, IdShip, $"{Car.TypeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", Car.TypeIndex.Replace(" ", ""), InderxPhotoInspektion + 1, initDasbordDelegate, getVechicleDelegate, Car.GetNameLayout(InderxPhotoInspektion + 1), OnDeliveryToCarrier, TotalPaymentToCarrier);
            await _navigation.PushAsync(fullPagePhoto);
            await _navigation.PushAsync(new CameraPagePhoto($"{Car.TypeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", fullPagePhoto, "PhotoIspection"));
            if (_navigation.NavigationStack.Count > 1)
            {
                _navigation.RemovePage(_navigation.NavigationStack[1]);
            }
        }

        private async Task SaveAllPhotoInspactionToGalery()
        {
            List<byte[]> imgBytes = PhotoInspection.Photos.Select(p => Convert.FromBase64String(p.Base64)).ToList();
            bool isSaveAllPhoto = await SaveAllPhoto(imgBytes);
        }


        private async Task SaveAllPhotoInspactionToOflineFolder()
        {
            await managerDispatchMob.AddPhotoInspection(new FolderOffline()
            {
                IdShiping = IdShip,
                IdVech = VehiclwInformation.Id,
                Index = InderxPhotoInspektion,
                FolderOflineType = FolderOflineType.PhotoInspaction,
                Json = JsonConvert.SerializeObject(PhotoInspection),
                InspactionType = InspactionType.PikedUp,
            });
        }

        private async Task<bool> SaveAllPhoto(List<byte[]> imgBytes)
        {
            int i = 0;
            foreach (byte[] imgByte in imgBytes)
            {
                bool isSaveAllPhoto = await DependencyService.Get<IMediaService>().SaveImageFromByte(imgByte, $"{VehiclwInformation.Id}-PikedUp-PhotoInspection-{i}");
                if(!isSaveAllPhoto)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task ClosePageToFirstPageInspction()
        {
            for(int i = 0; _navigation.NavigationStack.Count >= 3; i++)
            {
                await _navigation.PopAsync();
            }
        }

        public async void BackToRootPage()
        {
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            await _navigation.PopToRootAsync();
        }
    }
}