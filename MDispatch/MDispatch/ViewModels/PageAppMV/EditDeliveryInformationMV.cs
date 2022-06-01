using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using Plugin.Settings;
using Prism.Commands;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.PageAppMV
{
    public class EditDeliveryInformationMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public DelegateCommand SavePikedUpCommand { get; set; }
        private readonly IHelperViewService _helperView;

        public EditDeliveryInformationMV(
            IManagerDispatchMobService managerDispatchMob, 
            Shipping shipping,
            INavigation navigation)
            : base(navigation)
        {
           _helperView = DependencyService.Get<IHelperViewService>();
            this.managerDispatchMob = managerDispatchMob;
            Shipping = shipping;
            SavePikedUpCommand = new DelegateCommand(SaveDelivery);
        }

        private Shipping shipping = null;
        public Shipping Shipping
        {
            get => shipping;
            set => SetProperty(ref shipping, value);
        }

        private async void SaveDelivery()
        {
            await _popupNavigation.PushAsync(new LoadPage(), true);
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.OrderOneWork("Save", Shipping.Id, token, Shipping.idOrder, Shipping.NameD, Shipping.ContactNameD, Shipping.AddresD,
                    Shipping.CityD, Shipping.StateD, Shipping.ZipD, Shipping.PhoneD, Shipping.EmailD, "Delivery", ref description);
                });
                await _popupNavigation.PopAsync(true);
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, Navigationn));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.InformationDeliverySaved);
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigationn));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            else
            {
                await _popupNavigation.PopAsync(true);
            }
        }
    }
}