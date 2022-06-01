using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.Service;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.PageApp;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.PageAppMV.VehicleDetals
{
    public class VehicleDetailsMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        private VechicleDetails vechicleDetails = null;

        public VehicleDetailsMV(
            IManagerDispatchMobService managerDispatchMob, 
            int idVech, VechicleDetails vechicleDetails,
            INavigation navigation)
            :base(navigation)
        {
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
            this.vechicleDetails = vechicleDetails;
            InitVehiclwInformation(idVech);
        }

        private VehiclwInformation vehiclwInformation = null;
        public VehiclwInformation VehiclwInformation
        {
            get => vehiclwInformation;
            set => SetProperty(ref vehiclwInformation, value);
        }

        private async void InitVehiclwInformation(int idVech)
        {
            await _popupNavigation.PushAsync(new LoadPage(), true);
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            VehiclwInformation vehiclwInformation1 = null;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.OrderWork("GetVechicleInffo", idVech, ref vehiclwInformation1, token, ref description);
                });
                await _popupNavigation.PopAsync(true);
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await Navigation.PopAsync(true);
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    VehiclwInformation = vehiclwInformation1;
                    await vechicleDetails.InitPhoto(VehiclwInformation);
                }
                else if (state == 4)
                {
                    await Navigation.PopAsync(true);
                    await _popupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
            else
            {
                await Navigation.PopAsync(true);
                await _popupNavigation.PopAsync(true);
            }
        }
    }
}