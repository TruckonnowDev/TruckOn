using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using MDispatch.View.PageApp;
using MDispatch.ViewModels.InspectionMV.Servise.Models;
using Plugin.Settings;
using Prism.Commands;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.InspectionMV.DelyveryMV
{
    public class AskForUsersDelyveryMW : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public InitDasbordDelegate initDasbordDelegate = null;
        private GetShiping getShiping = null; 
        public GetVechicleDelegate getVechicleDelegate = null;
        public DelegateCommand GoToFeedBackCommand { get; set; }
        private readonly IHelperViewService _helperView;
        public AskForUsersDelyveryMW(IManagerDispatchMobService managerDispatchMob, string idShip, INavigation navigation, GetShiping getShiping, InitDasbordDelegate initDasbordDelegate,
            GetVechicleDelegate getVechicleDelegate, VehiclwInformation vehiclwInformation, string totalPaymentToCarrier, string paymmant = null)
            :base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            this.initDasbordDelegate = initDasbordDelegate;
            this.getVechicleDelegate = getVechicleDelegate;
            this.getShiping = getShiping;
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
            IdShip = idShip;
            TotalPaymentToCarrier = totalPaymentToCarrier;
            GoToFeedBackCommand = new DelegateCommand(GoToFeedBack);
            if (paymmant != null)
            {
                Payment = paymmant;
            }
        }

        public string IdShip { get; set; }
        public string TotalPaymentToCarrier { get; set; }
        public string Payment { get; set; }

        public VehiclwInformation vehiclwInformation = null;

        internal async Task<bool> CheckProplem()
        {
            bool isProplem = false;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.CheckProblem(token, IdShip, ref isProplem);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {

                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            return isProplem;
        }

        public VehiclwInformation VehiclwInformation
        {
            get => vehiclwInformation;
            set => SetProperty(ref vehiclwInformation, value);
        }

        private AskForUserDelyveryM askForUserDelyveryM = null;
        public AskForUserDelyveryM AskForUserDelyveryM
        {
            get => askForUserDelyveryM;
            set => SetProperty(ref askForUserDelyveryM, value);
        }

        private string email = null;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        private int inderxPhotoInspektion = 0;
        public int InderxPhotoInspektion
        {
            get => inderxPhotoInspektion;
            set => SetProperty(ref inderxPhotoInspektion, value);
        }

        private Video videoRecount = null;
        public Video VideoRecount
        {
            get => videoRecount;
            set => SetProperty(ref videoRecount, value);
        }

        public List<DamageForUser> damageForUsers { get; set; }

        public async void SaveAsk(string paymmant)
        {
            bool isPaymantPhoto = false;
            bool isNavigationMany = false;
            if (_navigation.NavigationStack.Count > 2)
            {
                await _navigation.PushAsync(new LoadPage());
                isNavigationMany = true;
            }
            Payment = paymmant;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            //await PopupNavigation.PushAsync(new TempDialogPage1(this));
            if (Payment == "COD" || Payment == "COP" || Payment == "Biling")
            {
                IVehicle Car = GetTypeCar(vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
                FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(managerDispatchMob, VehiclwInformation, IdShip, $"{Car.TypeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", Car.TypeIndex.Replace(" ", ""), inderxPhotoInspektion + 1, initDasbordDelegate, getVechicleDelegate, Car.GetNameLayout(InderxPhotoInspektion + 1), Payment, TotalPaymentToCarrier);
                await _navigation.PushAsync(fullPagePhotoDelyvery);
                await _navigation.PushAsync(new CameraPagePhoto1($"{Car.TypeIndex.Replace(" ", "")}{InderxPhotoInspektion + 1}.png", fullPagePhotoDelyvery, "PhotoIspection"));
                isPaymantPhoto = true;
            }
            else
            {
                if (askForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation == "Cash")
                {
                    await _navigation.PushAsync(new VideoCameraPage(this, ""));
                }
                else if (askForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation == "Check")
                {
                    await _navigation.PushAsync(new CameraPaymmant(this, "", "CheckPaymment.png"));
                }
                else
                {
                    IVehicle Car = GetTypeCar(vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
                    FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(managerDispatchMob, vehiclwInformation, IdShip, $"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""),
                       InderxPhotoInspektion + 1, initDasbordDelegate, getVechicleDelegate, Car.GetNameLayout(1), Payment, TotalPaymentToCarrier);
                    await _navigation.PushAsync(fullPagePhotoDelyvery, true);
                    await _navigation.PushAsync(new CameraPagePhoto1($"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", fullPagePhotoDelyvery, "PhotoIspection"));
                }
            }
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
               // await Task.Run(() => SaveRecountVideo());
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("AskForUserDelyvery", token, IdShip, AskForUserDelyveryM, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    if (isNavigationMany)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[0]);
                        isNavigationMany = false;
                    }
                    if (_navigation.NavigationStack.Count > 1)
                    {
                        await _navigation.PopAsync();
                    }
                    //await PopupNavigation.PushAsync(new Errror(description, Navigation));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    if (isNavigationMany)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[0]);
                        isNavigationMany = false;
                    }
                    if (isPaymantPhoto)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[1]);
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.AnswersSaved);
                }
                else if (state == 4)
                {
                    if (isNavigationMany)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[0]);
                        isNavigationMany = false;
                    }
                    if (_navigation.NavigationStack.Count > 1)
                    {
                        await _navigation.PopAsync();
                    }
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigation));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
        }

        public IVehicle GetTypeCar(string typeCar)
        {
            IVehicle car = null;
            switch (typeCar)
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

        public async void SaveRecountVideo()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await _navigation.PushAsync(new LoadPage());
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                if (videoRecount != null)
                {
                    await Task.Run(() =>
                    {
                        state = managerDispatchMob.SavePay("SaveRecount", token, IdShip, 2, videoRecount, ref description);
                    });
                    //state = 3;
                    //TaskManager.CommandToDo("SaveRecount", 1, token, IdShip, 2, VideoRecount);
                }
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, Navigation));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    if (_navigation.NavigationStack.Count > 2)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[1]);
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.VideoSavedSuccessfully);
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigation));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
                await _navigation.PopAsync();
            }
        }

        public async void AddPhoto(byte[] photoResult)
        {
            await _navigation.PushAsync(new LoadPage());
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            Photo photo = new Photo();
            photo.Base64 = Convert.ToBase64String(photoResult);
            photo.path = $"../Photo/{VehiclwInformation.Id}/Pay/DelyverySig.jpg";
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.SavePay("SaveSig", token, IdShip, 2, photo, ref description);
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, Navigation));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.PaymentSaved);
                    if (_navigation.NavigationStack.Count > 2)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[1]);
                    }
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigation));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            await _navigation.PopAsync();
        }

        public void RemmoveDamage(Image image, StackLayout stackLayout)
        {
            if (image != null && damageForUsers != null && damageForUsers.FirstOrDefault(d => d.Image == image) != null)
            {
                List<ImageSource> AllSourseImage = new List<ImageSource>();
                stackLayout.Children.ToList().ForEach((imageV) => 
                {
                    Image tempImage = (Image)imageV;
                    AllSourseImage.Add(tempImage.Source);
                });
                List<ImageSource> imageSources2 = new List<ImageSource>(AllSourseImage);
                DamageForUser damageForUser = damageForUsers.FirstOrDefault(d => d.Image == image);
                imageSources2.Remove(imageSources2.FirstOrDefault(i => i == damageForUser.ImageSource));
                AllSourseImage = imageSources2;
                damageForUsers.Remove(damageForUser);
                stackLayout.Children.Remove(image);
            }
        }

        internal void SetDamage(string nameDamage, int indexDamage, string prefNameDamage, double xInterest, double yInterest, Image image, ImageSource imageSource1)
        {
            DamageForUser damageForUser = new DamageForUser();
            damageForUser.FullNameDamage = $"{prefNameDamage} - {nameDamage}";
            damageForUser.TypePrefDamage = prefNameDamage;
            damageForUser.IndexDamage = indexDamage;
            damageForUser.XInterest = xInterest;
            damageForUser.YInterest = yInterest;
            damageForUser.Image = image;
            damageForUser.TypeCurrentStatus = "D";
            damageForUser.ImageSource = imageSource1;
            if (damageForUsers == null)
            {
                damageForUsers = new List<DamageForUser>();
            }
            damageForUsers.Add(damageForUser);
        }

        

        public async void GoToFeedBack()
        {
            await _navigation.PopToRootAsync(true);
            await _navigation.PushAsync(new View.Inspection.Feedback(managerDispatchMob, VehiclwInformation, this));
        }

        internal async void SetProblem()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.SetProblem(token, IdShip);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.FutureDispatcherProblem);
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
        }

    }
}