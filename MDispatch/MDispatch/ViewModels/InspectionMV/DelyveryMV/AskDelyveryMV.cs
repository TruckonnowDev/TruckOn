using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using MDispatch.View.Inspection.Delyvery;
using MDispatch.ViewModels.InspectionMV.Servise.Models;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.InspectionMV.DelyveryMV
{
    public class AskDelyveryMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public InitDasbordDelegate initDasbordDelegate = null;
        private GetVechicleDelegate getVechicleDelegate = null;
        private GetShiping getShiping = null;
        private readonly IHelperViewService _helperView;
        public AskDelyveryMV(IManagerDispatchMobService managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, INavigation navigation, GetShiping getShiping,
            InitDasbordDelegate initDasbordDelegate, GetVechicleDelegate getVechicleDelegate, string onDeliveryToCarrier, string totalPaymentToCarrier)
            :base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            this.getVechicleDelegate = getVechicleDelegate;
            this.getShiping = getShiping;
            this.initDasbordDelegate = initDasbordDelegate;
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
            IdShip = idShip;
            OnDeliveryToCarrier = onDeliveryToCarrier;
            TotalPaymentToCarrier = totalPaymentToCarrier;
        }

        public string IdShip { get; set; }
        private string OnDeliveryToCarrier { get; set; }
        private string TotalPaymentToCarrier { get; set; }

        private VehiclwInformation vehiclwInformation = null;
        public VehiclwInformation VehiclwInformation
        {
            get => vehiclwInformation;
            set => SetProperty(ref vehiclwInformation, value);
        }

        private AskDelyvery askDelyvery = null;
        public AskDelyvery AskDelyvery
        {
            get => askDelyvery;
            set => SetProperty(ref askDelyvery, value);
        }

        public async void SaveAsk()
        {
            bool isNavigationMany = false;
            if (Navigation.NavigationStack.Count > 2)
            {
                await _popupNavigation.PushAsync(new LoadPage());
                isNavigationMany = true;
            }
            IVehicle car = GetTypeCar(VehiclwInformation.Ask.TypeVehicle.Replace(" ", ""));
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await CheckVechicleAndGoToResultPage();
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("AskDelyvery", token, VehiclwInformation.Id, askDelyvery, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        await Navigation.PopAsync();
                    }
                    //await PopupNavigation.PushAsync(new Errror(description, Navigation));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[1]);
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.AnswersSaved);
                }
                else if (state == 4)
                {
                    if (isNavigationMany)
                    {
                        await _popupNavigation.RemovePageAsync(_popupNavigation.PopupStack[0]);
                        isNavigationMany = false;
                    }
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        await Navigation.PopAsync();
                    }
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigation));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
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

        private async Task CheckVechicleAndGoToResultPage()
        {
            List<VehiclwInformation> vehiclwInformation1s = getVechicleDelegate.Invoke();
            int indexCurrentVechecle = vehiclwInformation1s.FindIndex(v => v == VehiclwInformation);
            if (vehiclwInformation1s.Count - 1 == indexCurrentVechecle)
            {
                await _popupNavigation.PushAsync(new Alert(LanguageHelper.PassTheDeviceAlert, null));
                await Navigation.PushAsync(new ClientStart(managerDispatchMob, IdShip, initDasbordDelegate, OnDeliveryToCarrier, TotalPaymentToCarrier, vehiclwInformation1s[0], getShiping, getVechicleDelegate, false));
            }
            else
            {
                await _popupNavigation.PushAsync(new HintPageVechicle(LanguageHelper.ContinuingInspectionDelivery, vehiclwInformation1s[indexCurrentVechecle + 1]));
                await Navigation.PushAsync(new AskPageDelyvery(managerDispatchMob, vehiclwInformation1s[indexCurrentVechecle + 1], IdShip, initDasbordDelegate, getVechicleDelegate, OnDeliveryToCarrier, TotalPaymentToCarrier, getShiping), true);
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
            }
            return car;
        }
    }
}