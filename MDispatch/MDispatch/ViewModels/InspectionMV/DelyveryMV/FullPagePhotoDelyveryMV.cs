using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Models.Enum;
using MDispatch.Models.ModelDataBase;
using MDispatch.NewElement;
using MDispatch.NewElement.Directory;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.Service.Net;
using MDispatch.Service.RequestQueue;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using MDispatch.View.PageApp;
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
        public IVehicle Car = null;
        private InitDasbordDelegate initDasbordDelegate = null;
        private GetVechicleDelegate getVechicleDelegate = null;
        private FullPagePhotoDelyvery fullPagePhoto = null;

        public FullPagePhotoDelyveryMV(ManagerDispatchMob managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, string typeCar,
            int inderxPhotoInspektion, INavigation navigation, InitDasbordDelegate initDasbordDelegate, GetVechicleDelegate getVechicleDelegate,
            string onDeliveryToCarrier, string totalPaymentToCarrier, FullPagePhotoDelyvery fullPagePhoto)
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
            this.fullPagePhoto = fullPagePhoto;
        }

        private async void Init()
        {
            await Car.OrintableScreen(InderxPhotoInspektion + 1);
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
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            //bool isNavigationMany = false;
            //string description = null;
            //int state = 0;
            if (InderxPhotoInspektion >= Car.CountCarImg)
            {
                await CheckVechicleAndGoToResultPage();
            }
            else
            {
                Car.OrintableScreen(InderxPhotoInspektion);
                FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(managerDispatchMob, VehiclwInformation, IdShip, $"{Car.TypeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", Car.TypeIndex.Replace(" ", ""), InderxPhotoInspektion + 1, initDasbordDelegate, getVechicleDelegate, Car.GetNameLayout(InderxPhotoInspektion + 1), OnDeliveryToCarrier, TotalPaymentToCarrier);
                await Navigation.PushAsync(fullPagePhotoDelyvery);
                await Navigation.PushAsync(new CameraPagePhoto1($"{Car.TypeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", fullPagePhotoDelyvery, "PhotoIspection"));
            }
            await Task.Run(() => Utils.CheckNet(true, true));
            if (App.isNetwork)
            {
                if(isNavigWthDamag)
                {
                    Navigation.RemovePage(Navigation.NavigationStack[2]);
                }
                if (Navigation.NavigationStack.Count > 1)
                {
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                }
                await Task.Run(() =>
                {
                    ManagerQueue.AddReqvest("SavePhoto", token, VehiclwInformation.Id, PhotoInspection);
                    initDasbordDelegate.Invoke();
                });
            }
            else
            {
                //HelpersView.ReSet();
                //HelpersView.CallError(LanguageHelper.NotNetworkAlert);
                ////await PopupNavigation.PushAsync(new Errror("Not Network", null));
                //BackToRootPage();
                SelectMethodAction();
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
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    initDasbordDelegate.Invoke();
                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
            else
            {
                HelpersView.CallError(LanguageHelper.NotNetworkAlert);
                await PopupNavigation.PushAsync(new Alert(LanguageHelper.NotNetworkAlert, null));
                BackToRootPage();
            }
        }

        [System.Obsolete]
        private async Task CheckVechicleAndGoToResultPage()
        {
            List<VehiclwInformation> vehiclwInformation1s = getVechicleDelegate.Invoke();
            int indexCurrentVechecle = vehiclwInformation1s.FindIndex(v => v == VehiclwInformation);
            IVehicle Car = GetTypeCar(vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
            if (vehiclwInformation1s.Count - 1 == indexCurrentVechecle)
            {
                DependencyService.Get<IOrientationHandler>().ForceSensor();
                Continue();
                await Navigation.PopToRootAsync();
            }
            else
            {
                await PopupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.ContinuingInspectionDelivery, vehiclwInformation1s[indexCurrentVechecle + 1]));
                FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(managerDispatchMob, VehiclwInformation, IdShip, $"{Car.TypeIndex.Replace(" ", "")}{1}.png", Car.TypeIndex.Replace(" ", ""), 1, initDasbordDelegate, getVechicleDelegate, Car.GetNameLayout(1), OnDeliveryToCarrier, TotalPaymentToCarrier);
                await Navigation.PushAsync(fullPagePhotoDelyvery);
                await Navigation.PushAsync(new CameraPagePhoto1($"{Car.TypeIndex.Replace(" ", "")}{1}.png", fullPagePhotoDelyvery, "PhotoIspection"));
            }
        }

        private int GetIndexPhoto(VehiclwInformation vehiclwInformation, IVehicle car)
        {
            int indexPhoto = vehiclwInformation.PhotoInspections == null || vehiclwInformation.PhotoInspections.Count == 0 ? 1
                : vehiclwInformation.PhotoInspections.Count >= 11 ? (vehiclwInformation.PhotoInspections.Count - car.CountCarImg) : 1;
            return indexPhoto;
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

        public async void BackToRootPage()
        {
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            await Navigation.PopToRootAsync();
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
                HelpersView.ReSet();
                HelpersView.CallError(LanguageHelper.NotNetworkAlert);
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
            await Navigation.PushAsync(fullPagePhoto);
            await Navigation.PushAsync(new CameraPagePhoto($"{Car.TypeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", fullPagePhoto, "PhotoIspection"));
            if (Navigation.NavigationStack.Count > 1)
            {
                Navigation.RemovePage(Navigation.NavigationStack[1]);
            }
        }

        private async Task SaveAllPhotoInspactionToGalery()
        {
            List<byte[]> imgBytes = PhotoInspection.Photos.Select(p => Convert.FromBase64String(p.Base64)).ToList();
            bool isSaveAllPhoto = await SaveAllPhoto(imgBytes);
        }


        private async Task SaveAllPhotoInspactionToOflineFolder()
        {
            await managerDispatchMob.dataBaseContext.AddPhotoInspection(new FolderOffline()
            {
                IdShiping = IdShip,
                IdVech = VehiclwInformation.Id,
                Index = InderxPhotoInspektion,
                FolderOflineType = FolderOflineType.PhotoInspaction,
                Json = JsonConvert.SerializeObject(PhotoInspection),
                InspactionType = InspactionType.Delivery,
            });
        }

        private async Task<bool> SaveAllPhoto(List<byte[]> imgBytes)
        {
            int i = 0;
            foreach (byte[] imgByte in imgBytes)
            {
                bool isSaveAllPhoto = await DependencyService.Get<IMediaService>().SaveImageFromByte(imgByte, $"{VehiclwInformation.Id}-PikedUp-PhotoInspection-{i}");
                if (!isSaveAllPhoto)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task ClosePageToFirstPageInspction()
        {
            for (int i = 0; Navigation.NavigationStack.Count >= 3; i++)
            {
                await Navigation.PopAsync();
            }
        }
    }
}