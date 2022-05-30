using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.Service.RequestQueue;
using MDispatch.Service.Utils;
using MDispatch.Vidget.View;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Popups;
using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.Vidget.VM
{
    public class FullPhotoTruckVM : BindableBase
    {
        public INavigation Navigation = null;
        public DelegateCommand NextCommand { get; set; }
        private InitDasbordDelegate initDasbordDelegate = null;
        private readonly IManagerQueueService _managerQueueService;
        private readonly IUtilsService _utils;
        private readonly IHelperViewService _helperView;
        private readonly IManagerDispatchMobService managerDispatchMob;

        [System.Obsolete]
        public FullPhotoTruckVM(IManagerDispatchMobService managerDispatchMob, string idDriver, int indexCurent, INavigation navigation, TruckCar truckCar, InitDasbordDelegate initDasbordDelegate = null)
        {
            _managerQueueService = DependencyService.Get<IManagerQueueService>();
            _helperView = DependencyService.Get<IHelperViewService>();
            _utils = DependencyService.Get<IUtilsService>();
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            this.Navigation = navigation;
            this.truckCar = truckCar;
            IdDriver = idDriver;
            IndexCurent = indexCurent;
            //NextCommand = new DelegateCommand(NextPage);
            //truckCar.GetModalAlert(IndexCurent);
            Init();
        }


        [Obsolete]
        private async void Init()
        {
            if (IndexCurent == 1)
            {
                CheckPlate();
            }
            if (truckCar != null && truckCar.IsNextInspection && truckCar.CountPhoto + 1 == IndexCurent)
            {
                CheckPlate();
            }
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
            if (truckCar != null)
            {
                if (IndexCurent <= truckCar.CountPhoto)
                {
                    NameLayute = truckCar.NamePatern[IndexCurent - 1];
                    PhotoLayute = $"{truckCar.Type}{truckCar.Layouts[IndexCurent - 1]}.png";
                }
            }
        }

        public string IdDriver { get; set; }

        public int IndexCurent { get; set; }


        private TruckCar truckCar = null;
        public TruckCar TruckCar
        {
            get => truckCar;
            set => SetProperty(ref truckCar, value);
        }


        private string plateTruck = "";
        public string PlateTruck
        {
            get => plateTruck;
            set => SetProperty(ref plateTruck, value);
        }

        private string plateTrailer = "";
        public string PlateTrailer
        {
            get => plateTrailer;
            set => SetProperty(ref plateTrailer, value);
        }

        private bool isCorectPlate = false;
        public bool IsCorectPlate
        {
            get => isCorectPlate;
            set => SetProperty(ref isCorectPlate, value);
        }

        private string nameLayute = null;
        public string NameLayute
        {
            get => nameLayute;
            set => SetProperty(ref nameLayute, value);
        }

        private string photoLayute = null;
        public string PhotoLayute
        {
            get => photoLayute;
            set => SetProperty(ref photoLayute, value);
        }

        private ImageSource imageSourceTake = null;
        public ImageSource ImageSourceTake
        {
            get => imageSourceTake;
            set => SetProperty(ref imageSourceTake, value);
        }
         
        public Photo Photo { get; set; }

        [System.Obsolete]
        public async Task AddPhoto(byte[] photoRes)
        {
            Photo photo = new Photo();
            photo.Base64 = Convert.ToBase64String(photoRes);
            photo.path = $"../Photo/Driver/{IdDriver}/{IndexCurent}.jpg";
            Photo = photo;
            MemoryStream stream = new MemoryStream(photoRes);
            stream.Position = 0;
            var byteArray = stream.ToArray();
            ImageSourceTake = ImageSource.FromStream(() => new MemoryStream(byteArray));
            await NextPage();
        }



        [System.Obsolete]
        private async Task NextPage()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            bool isEndInspection = false;
            string description = null;
            int state = 0;
            if (truckCar.CountPhoto > IndexCurent)
            {
                isEndInspection = true;
                await Navigation.PushAsync(new View.CameraPage(managerDispatchMob, IdDriver, IndexCurent + 1, initDasbordDelegate, truckCar));
            }
            else 
            {
                if(truckCar.IsNextInspection)
                {
                    await Navigation.PushAsync(new View.CameraPage(managerDispatchMob, IdDriver, IndexCurent + 1, initDasbordDelegate, truckCar));
                    isEndInspection = true;
                }
                else
                {
                    DependencyService.Get<IOrientationHandler>().ForceSensor();
                    UpdateInspectionDriver();
                }
            }
            await Task.Run(() => _utils.CheckNet(false, true));
            if (App.isNetwork)
            {
                Navigation.RemovePage(Navigation.NavigationStack[1]);
                Task.Run(() => _managerQueueService.AddRequest("SaveInspactionDriver", token, IdDriver, Photo, IndexCurent, truckCar.TypeTransportVehicle));
            }
            else
            {
                _helperView.CallError(LanguageHelper.NotNetworkAlert);
                await Navigation.PopToRootAsync(true);
            }
        }

        [System.Obsolete]
        private async void UpdateInspectionDriver()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.DriverWork("UpdateInspectionDriver", token, ref description, IdDriver);
                });
                if (state == 1)
                {
                    await Navigation.PushAsync(new Alert(LanguageHelper.NotNetworkAlert, null));
                }
                else if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    initDasbordDelegate.Invoke();
                    await Navigation.PopToRootAsync();
                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
            else
            {
                _helperView.CallError(LanguageHelper.NotNetworkAlert);
                BackToRootPage();
            }
        }

        [System.Obsolete]
        internal async Task SetPlate(string nowCheck)
        {
            await PopupNavigation.PushAsync(new LoadPage());
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            bool isPlate = false;
            TruckCar truckCar = null;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.SetPlate(token, PlateTruck, PlateTrailer, nowCheck, ref description, ref isPlate, ref truckCar);
                });
                if (state == 1)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.NotNetworkAlert, null));
                }
                else if (state == 2)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    TruckCar = truckCar;
                    IsCorectPlate = !isPlate;
                    if(isPlate)
                    {
                        await PopupNavigation.PopAllAsync();
                    }
                    else
                    {
                        await PopupNavigation.PopAsync();
                    }
                    if (TruckCar != null)
                    {
                        IndexCurent = 1;
                        if (IndexCurent <= TruckCar.CountPhoto)
                        {
                            NameLayute = TruckCar.NamePatern[IndexCurent - 1];
                            PhotoLayute = $"{truckCar.Type}{truckCar.Layouts[IndexCurent - 1]}.png";
                        }
                    }
                }
                else if (state == 4)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
            else
            {
                _helperView.CallError(LanguageHelper.NotNetworkAlert);
                BackToRootPage();
            }
        }

        [System.Obsolete]
        internal async void CheckPlate()
        {
            await PopupNavigation.PushAsync(new LoadPage());
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            string plateTruckAndTrailer = null;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.CheckPlate(token, ref description, ref plateTruckAndTrailer);
                });
                if (state == 1)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.NotNetworkAlert, null));
                }
                else if (state == 2)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3) 
                {
                    await PopupNavigation.PopAsync();
                    string plateTruck = plateTruckAndTrailer.Split(',')[0];
                    string plateTrailer = plateTruckAndTrailer.Split(',')[1];
                    PlateTruck = plateTruck;
                    PlateTrailer = plateTrailer;
                    if(IndexCurent == 1)
                    {
                        if(TruckCar == null)
                        {
                            await PopupNavigation.PushAsync(new PlateTruckWrite(this));
                        }
                        else if (TruckCar.TypeTransportVehicle == "Truck")
                        {
                            await PopupNavigation.PushAsync(new PlateTruckWrite(this));
                        }
                        else if (TruckCar.TypeTransportVehicle == "Trailer")
                        {
                            await PopupNavigation.PushAsync(new PlateTrailerWritePopupView(this));
                        }
                    }
                    else if (TruckCar.IsNextInspection && TruckCar.CountPhoto + 1 == IndexCurent)
                    {
                        await PopupNavigation.PushAsync(new PlateTrailerWritePopupView(this));
                    }
                }
                else if (state == 4)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
            else
            {
                _helperView.CallError(LanguageHelper.NotNetworkAlert);
                BackToRootPage();
            }
        }

        [System.Obsolete]
        private async void SetInspectionDriver()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() =>
            {
                //state = managerDispatchMob.DriverWork("SetInspectionDriver", token, ref description, IdDriver, InspectionDriver);
            });
            await PopupNavigation.PopAsync(true);
            if (state == 1)
            {
                await PopupNavigation.PushAsync(new Alert(LanguageHelper.NotNetworkAlert, null));
            }
            else if (state == 2)
            {
                await PopupNavigation.PushAsync(new Alert(description, null));
            }
            else if (state == 3)
            {
                
            }
            else if (state == 4)
            {
                await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
            }
            else
            {
                _helperView.CallError(LanguageHelper.NotNetworkAlert);
                BackToRootPage();
            }
        }

        [Obsolete]
        internal async void DetectText(byte[] result, string type)
        {
            //await PopupNavigation.PushAsync(new LoadPage());
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            int state = 0;
            string plate = null;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                state = managerDispatchMob.DetectPlate(token, Convert.ToBase64String(result), IdDriver, type, plate);

                if (state == 1)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.NotNetworkAlert, null));
                }
                else if (state == 3)
                {
                    await PopupNavigation.PopAsync();
                    if (type == "truck")
                    {
                        await PopupNavigation.PushAsync(new PlateTruckWrite(this));
                        PlateTruck = plate;
                    }
                    else if(type == "trailer")
                    {
                        await PopupNavigation.PushAsync(new PlateTrailerWritePopupView(this));
                        PlateTrailer = plate;
                    }
                }
                else if (state == 4)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
            else
            {
                _helperView.CallError(LanguageHelper.NotNetworkAlert);
                BackToRootPage();
            }
        }

        public async void BackToRootPage()
        {
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            await Navigation.PopToRootAsync();
            
        }
    }
}