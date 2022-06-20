using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service.GlobalHelper;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.Service.Utils;
using MDispatch.View;
using MDispatch.View.AskPhoto;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using MDispatch.View.Inspection.Delyvery;
using MDispatch.View.Inspection.PickedUp;
using MDispatch.View.PageApp;
using MDispatch.View.PageApp.DialogPage;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using MDispatch.ViewModels.InspectionMV.Servise.Models;
using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.PageAppMV
{
    public class InfoOrderMV : BaseViewModel
    {
        public DelegateCommand ToInstructionComand { get; set; }
        public DelegateCommand ToEditPikedUpCommand { get; set; }
        public DelegateCommand ToEditDeliveryCommand { get; set; }
        public DelegateCommand ToPaymentCommand { get; set; }
        private InitDasbordDelegate initDasbordDelegate = null;
        private GetVechicleDelegate getVechicleDelegate = null;
        private GetShiping GetShiping = null;
        private Action callBackVehiclwInformation = null;
        private readonly IManagerDispatchMobService _managerDispatchMob;
        private readonly IHelperViewService _helpersView;
        private readonly IGlobalHelperService _globalHelper;

        public InfoOrderMV(
            IManagerDispatchMobService managerDispatchMob, 
            InitDasbordDelegate initDasbordDelegate, 
            string statusInspection, string idShipping, 
            Action callBackVehiclwInformation,
            INavigation navigation)
            :base(navigation)
        {
            _helpersView = DependencyService.Get<IHelperViewService>();
            _globalHelper = DependencyService.Get<IGlobalHelperService>();
            this.initDasbordDelegate = initDasbordDelegate;
            this._managerDispatchMob = managerDispatchMob;
            this.callBackVehiclwInformation = callBackVehiclwInformation;
            Shipping = shipping;
            getVechicleDelegate = GetVehiclwInformations;
            GetShiping = GetShipings;
            StatusInspection = statusInspection;
            IdShipping = idShipping;
            ToInstructionComand = new DelegateCommand(ToInstruction);
            ToEditPikedUpCommand = new DelegateCommand(ToEditPikedUp);
            ToEditDeliveryCommand = new DelegateCommand(ToEditDelivery);
            ToPaymentCommand = new DelegateCommand(ToEditPayment);
            Init();
        }

        private Shipping shipping = null;
        public Shipping Shipping
        {
            get => shipping;
            set => SetProperty(ref shipping, value);
        }

        private string idShipping = "";
        public string IdShipping
        {
            get => idShipping;
            set => SetProperty(ref idShipping, value);
        }

        private int count = 0;
        public int Count
        {
            get => Shipping.VehiclwInformations.Count;
            set => SetProperty(ref count, value);
        }

        private Shipping GetShipings()
        {
            return Shipping;
        }

        private string status = "";
        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        private string statusInspection = "";
        public string StatusInspection
        {
            get => statusInspection;
            set => SetProperty(ref statusInspection, value);
        }

        private string statusInspectionView = "";
        public string StatusInspectionView
        {
            get => statusInspectionView;
            set => SetProperty(ref statusInspectionView, value);
        }

        private bool isInspection = false;
        public bool IsInspection
        {
            get => isInspection;
            set => SetProperty(ref isInspection, value);
        }

        private bool isInstructinRead = false;
        public bool IsInstructinRead
        {
            get
            {
                return isInstructinRead;
            }
            set
            {
                SetProperty(ref isInstructinRead, value);
            } 
        }

        private List<VehiclwInformation> GetVehiclwInformations()
        {
            return Shipping.VehiclwInformations;
        }

        private async void ToInstruction()
        {
            if (Shipping != null)
            {
                Navigation.PushAsync(new Instruction(this));
            }
            else
            {
                _helpersView.CallError(LanguageHelper.NoDataAlert);
            }
        }

        private async void ToEditPikedUp()
        {
            if (Shipping != null)
            {
                await Navigation.PushAsync(new EditPicupInfo(_managerDispatchMob, Shipping), true);
            }
            else
            {
                _helpersView.CallError(LanguageHelper.NoDataAlert);
            }
        }

        private async void ToEditDelivery()
        {
            if (Shipping != null)
            {
                await Navigation.PushAsync(new EditDeliveryInformation(_managerDispatchMob, Shipping), true);
            }
            else
            {
                _helpersView.CallError(LanguageHelper.NoDataAlert);
            }
        }

        [System.Obsolete]
        private async void ToEditPayment()
        {
            await _popupNavigation.PushAsync(new EditPayment(_managerDispatchMob, Shipping), true);
        }

        public async void ToVehicleDetails(VehiclwInformation vehiclwInformation)
        {
            await Navigation.PushAsync(new VechicleDetails(vehiclwInformation, _managerDispatchMob));
        }

        public async void Init()
        {
            string description = null;
            int state = 0;
            Shipping shipping = null;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            if (StatusInspection == "Assigned")
            {
                IsInspection = true;
                StatusInspectionView = LanguageHelper.VehicleInspectionPikedUp;
            }
            else if (StatusInspection == "Picked up")
            {
                IsInspection = true;
                StatusInspectionView = LanguageHelper.VehicleInspectionDelivery;
            }
            else
            {
                IsInspection = false;
                StatusInspection = "Delivered";
            }
            await _popupNavigation.PushAsync(new LoadPage());
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                { 
                    state = _managerDispatchMob.InspectionStatus(token, IdShipping, StatusInspection, ref description, ref shipping);
                });
                await Task.Delay(100);
                if (state == 1)
                {
                    _globalHelper.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    _helpersView.CallError(description);
                }
                else if (state == 3)
                {
                    Shipping = shipping;
                    callBackVehiclwInformation();
                    if (Shipping.VehiclwInformations == null || Shipping.VehiclwInformations.Count == 0)
                    {
                        IsInspection = true;
                        StatusInspectionView = LanguageHelper.ThereAreNoVehiclesInThisOrder;
                    }
                    IsInstructinRead = !Shipping.IsInstructinRead;
                    if(StatusInspection != "Delivered")
                    {
                        IsInspection = Shipping.IsInstructinRead;
                    }
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    _helpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            await _popupNavigation.PopAsync();
        }

        [System.Obsolete]
        public async void ToStartInspection()
        {
            if(Shipping == null || Shipping.VehiclwInformations == null || Shipping.VehiclwInformations.Count == 0)
            {
                await PopupNavigation.PushAsync(new Alert(LanguageHelper.NoVehiclesAlert, null), true);
                return;
            }
            Shipping.VehiclwInformations.Sort((a, b) => a.Id.CompareTo(b.Id));
            VehiclwInformation vehiclwInformation1 = Shipping.VehiclwInformations[0];
            foreach (var vehiclwInformation in Shipping.VehiclwInformations)
            {
                IVehicle car = null;
                if (vehiclwInformation.Ask != null && vehiclwInformation.Ask.TypeVehicle != null)
                {
                    car = GetTypeCar(vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
                }
                List <PhotoInspection> photoInspections = null;
                if (vehiclwInformation.PhotoInspections != null)
                {
                    photoInspections = vehiclwInformation.PhotoInspections.FindAll(p => p.CurrentStatusPhoto == "PikedUp");
                }
                if (vehiclwInformation.Ask == null)
                {
                    await PopupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.StartInspectionPickedUp, vehiclwInformation));
                    await Navigation.PushAsync(new AskPage(_managerDispatchMob, vehiclwInformation, Shipping.Id, initDasbordDelegate, getVechicleDelegate, Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier), true);
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    return;
                }
                else if (photoInspections == null || photoInspections.Count == 0)
                {
                    await PopupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.ContinuingInspectionPickedUp, vehiclwInformation));
                    FullPagePhoto fullPagePhoto = new FullPagePhoto(_managerDispatchMob, vehiclwInformation, Shipping.Id, $"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""), 1, initDasbordDelegate, getVechicleDelegate, car.GetNameLayout(1), Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier);
                    await Navigation.PushAsync(fullPagePhoto, true);
                    //await Navigation.PushAsync(new CameraPagePhoto($"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", fullPagePhoto, "PhotoIspection"));
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    return;
                }
                else if (photoInspections.Find(p => p.IndexPhoto == car.CountCarImg) == null)
                {
                    await PopupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.ContinuingInspectionPickedUp, vehiclwInformation));
                    int lastIndexPhoto = photoInspections[vehiclwInformation.PhotoInspections.Count - 1].IndexPhoto + 1;
                    FullPagePhoto fullPagePhoto = new FullPagePhoto(_managerDispatchMob, vehiclwInformation, Shipping.Id, $"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}{lastIndexPhoto}.png", vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""), lastIndexPhoto, initDasbordDelegate, getVechicleDelegate, car.GetNameLayout(lastIndexPhoto), Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier);
                    await Navigation.PushAsync(fullPagePhoto, true);
                    //await Navigation.PushAsync(new CameraPagePhoto($"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}{lastIndexPhoto}.png", fullPagePhoto, "PhotoIspection"));
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    return;
                }
                else if (vehiclwInformation.Ask1 == null)
                {
                    await PopupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.ContinuingInspectionPickedUp, vehiclwInformation));
                    await Navigation.PushAsync(new Ask1Page(_managerDispatchMob, vehiclwInformation, Shipping.Id, initDasbordDelegate, getVechicleDelegate, vehiclwInformation.Ask.TypeVehicle, Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier), true);
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    return;
                }
                else if(vehiclwInformation.Ask1.App_will_force_driver_to_take_pictures_of_each_strap == null || (vehiclwInformation.Ask1.App_will_force_driver_to_take_pictures_of_each_strap != null && vehiclwInformation.Ask1.App_will_force_driver_to_take_pictures_of_each_strap.Count == 0))
                {
                    await Navigation.PushAsync(new CameraStrapAndTrack(_managerDispatchMob, vehiclwInformation, Shipping.Id, initDasbordDelegate, getVechicleDelegate, Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier, vehiclwInformation.Ask.TypeVehicle), true);
                    return;
                }
            }
            if (Shipping.AskFromUser == null)
            {
                vehiclwInformation1 = Shipping.VehiclwInformations[0];
                await Navigation.PushAsync(new View.Inspection.PickedUp.ClientStart(_managerDispatchMob, vehiclwInformation1, Shipping.Id, initDasbordDelegate, Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier), true);
                await PopupNavigation.PushAsync(new Alert(LanguageHelper.PassTheDeviceAlert, null));
                Navigation.RemovePage(Navigation.NavigationStack[1]);
                return;
            }
            else if (Shipping.AskFromUser.App_will_ask_for_signature_of_the_client_signature == null || Shipping.AskFromUser.What_form_of_payment_are_you_using_to_pay_for_transportation == null
                || (Shipping.TotalPaymentToCarrier == "COP" && Shipping.AskFromUser.CountPay == null))
            {
                await Navigation.PushAsync(new LiabilityAndInsurance(_managerDispatchMob, vehiclwInformation1.Id, Shipping.Id, initDasbordDelegate, Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier, Shipping.IsProblem), true);
                Navigation.RemovePage(Navigation.NavigationStack[1]);
                return;
            }
            else if ((Shipping.AskFromUser.PhotoPay == null && Shipping.AskFromUser.VideoRecord == null) && Shipping.AskFromUser.What_form_of_payment_are_you_using_to_pay_for_transportation != "Biling" && Shipping.TotalPaymentToCarrier == "COP")
            {
                LiabilityAndInsuranceMV liabilityAndInsuranceMV = new LiabilityAndInsuranceMV(_managerDispatchMob, vehiclwInformation1.Id, Shipping.Id, Navigation, initDasbordDelegate, null);
                if (Shipping.AskFromUser.What_form_of_payment_are_you_using_to_pay_for_transportation == "Cash")
                {
                    await Navigation.PushAsync(new VideoCameraPage(liabilityAndInsuranceMV, ""));
                }
                else if (Shipping.AskFromUser.What_form_of_payment_are_you_using_to_pay_for_transportation == "Check")
                {
                    await Navigation.PushAsync(new CameraPaymmant(liabilityAndInsuranceMV, "", "CheckPaymment.png"));
                }
                else
                {
                    await Navigation.PushAsync(new Ask2Page(liabilityAndInsuranceMV.managerDispatchMob, liabilityAndInsuranceMV.IdVech, liabilityAndInsuranceMV.IdShip, liabilityAndInsuranceMV.initDasbordDelegate));
                }
                Navigation.RemovePage(Navigation.NavigationStack[1]);
            }
            else if(Shipping.Ask2 == null)
            {
                Ask2Page ask2Page = new Ask2Page(_managerDispatchMob, vehiclwInformation1.Id, shipping.Id, initDasbordDelegate);
                await Navigation.PushAsync(ask2Page);
                Navigation.RemovePage(Navigation.NavigationStack[1]);
            }
            else
            {
                ContinuePick();
            }
        }

        [System.Obsolete]
        public async void ToStartInspectionDelyvery()
        {
            if (Shipping == null || Shipping.VehiclwInformations == null || Shipping.VehiclwInformations.Count == 0)
            {
                await PopupNavigation.PushAsync(new Alert(LanguageHelper.NoVehiclesAlert, null), true);
                return;
            }
            Shipping.VehiclwInformations.Sort((a, b) => a.Id.CompareTo(b.Id));
            foreach (var vehiclwInformation in Shipping.VehiclwInformations)
            {
                if (vehiclwInformation.AskDelyvery == null)
                {
                    await Navigation.PushAsync(new AskPageDelyvery(_managerDispatchMob, vehiclwInformation, Shipping.Id, initDasbordDelegate, getVechicleDelegate, Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier, GetShiping), true);
                    await PopupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.StartInspectionDelivery, vehiclwInformation));
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    return;
                }
            }
            if (Shipping.askForUserDelyveryM == null)
            {
                await Navigation.PushAsync(new View.Inspection.Delyvery.ClientStart(_managerDispatchMob, Shipping.Id, initDasbordDelegate, Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier, Shipping.VehiclwInformations[0], GetShiping, getVechicleDelegate, shipping.IsProblem), true);
                await PopupNavigation.PushAsync(new Alert(LanguageHelper.PassTheDeviceAlert, null));
                Navigation.RemovePage(Navigation.NavigationStack[1]);
                return;
            }
            else if ((Shipping.askForUserDelyveryM.VideoRecord == null && Shipping.askForUserDelyveryM.PhotoPay == null) && Shipping.askForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation != "Biling" && Shipping.TotalPaymentToCarrier == "COD")
            {
                AskForUsersDelyveryMW askForUsersDelyveryMW = new AskForUsersDelyveryMW(_managerDispatchMob, Shipping.Id, Navigation, GetShiping, initDasbordDelegate, getVechicleDelegate, Shipping.VehiclwInformations[0], Shipping.TotalPaymentToCarrier, Shipping.askForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation);
                if (Shipping.askForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation == "Cash")
                {
                    await Navigation.PushAsync(new VideoCameraPage(this, ""));
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    return;
                }
                else if (Shipping.askForUserDelyveryM.What_form_of_payment_are_you_using_to_pay_for_transportation == "Check")
                {
                    await Navigation.PushAsync(new CameraPaymmant(this, "", "CheckPaymment.png"));
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    return;
                }
            }
            foreach (var vehiclwInformation in Shipping.VehiclwInformations)
            {
                IVehicle car = GetTypeCar(vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
                List<PhotoInspection> photoInspections = null;
                if (vehiclwInformation.PhotoInspections != null)
                {
                    photoInspections = vehiclwInformation.PhotoInspections.FindAll(p => p.CurrentStatusPhoto == "Delyvery");
                }
                if (photoInspections != null && photoInspections.Count == 0)
                {
                    await PopupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.ContinuingInspectionDelivery, vehiclwInformation));
                    FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(_managerDispatchMob, vehiclwInformation, Shipping.Id, $"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""), photoInspections.Count + 1, initDasbordDelegate, getVechicleDelegate, car.GetNameLayout(1), Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier);
                    await Navigation.PushAsync(fullPagePhotoDelyvery, true);
                    //await Navigation.PushAsync(new CameraPagePhoto1($"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}1.png", fullPagePhotoDelyvery, "PhotoIspection"));
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    return;
                }
                else if (photoInspections.Find(p => p.IndexPhoto == car.CountCarImg) == null)
                {
                    await PopupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.ContinuingInspectionDelivery, vehiclwInformation));
                    int photoInspection = photoInspections[photoInspections.Count - 1].IndexPhoto + 1;
                    FullPagePhotoDelyvery fullPagePhotoDelyvery = new FullPagePhotoDelyvery(_managerDispatchMob, vehiclwInformation, Shipping.Id, $"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}{photoInspection}.png", vehiclwInformation.Ask.TypeVehicle.Replace(" ", ""), photoInspections.Count + 1, initDasbordDelegate, getVechicleDelegate, car.GetNameLayout(photoInspection), Shipping.OnDeliveryToCarrier, Shipping.TotalPaymentToCarrier);
                    await Navigation.PushAsync(fullPagePhotoDelyvery);
                    //await Navigation.PushAsync(new CameraPagePhoto1($"{vehiclwInformation.Ask.TypeVehicle.Replace(" ", "")}{photoInspection}.png", fullPagePhotoDelyvery, "PhotoIspection"));
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    return;
                }
            }
            ContinueDely();
        }

        [System.Obsolete]
        public async void ContinuePick()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await PopupNavigation.PushAsync(new LoadPage());
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = _managerDispatchMob.Recurent(token, Shipping.Id, "Picked up", ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    initDasbordDelegate.Invoke();
                    await Navigation.PopToRootAsync(true);
                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
            await PopupNavigation.PopAsync();
        }


        [System.Obsolete]
        public async void ContinueDely()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await PopupNavigation.PushAsync(new LoadPage());
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    string status = null;
                    if (Shipping.TotalPaymentToCarrier == "COD" || Shipping.TotalPaymentToCarrier == "COP")
                    {
                        status = "Delivered,Paid";
                    }
                    else
                    {
                        status = "Delivered,Billed";
                    }
                    state = _managerDispatchMob.Recurent(token, Shipping.Id, status, ref description);
                });
                if (state == 1)
                {
                    _globalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    await PopupNavigation.PopAsync();
                    initDasbordDelegate.Invoke();
                    await Navigation.PopToRootAsync(true);
                }
                else if (state == 4)
                {
                    await PopupNavigation.PopAsync();
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
        }

        [System.Obsolete]
        public async void SetIstractions()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = _managerDispatchMob.SetInstaraction(token, Shipping.Id, ref description);
                });
                if (state == 1)
                {
                    _globalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    IsInstructinRead = false;
                    if (Shipping.VehiclwInformations != null && Shipping.VehiclwInformations.Count != 0 && Shipping.CurrentStatus != "Delivered")
                    {
                        IsInspection = true;
                    }
                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert("Technical work on the service", null));
                }
            }
        }

        private IVehicle GetTypeCar(string typeCar)
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
    }
}