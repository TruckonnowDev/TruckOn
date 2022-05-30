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
    public class EditPickedupMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public DelegateCommand SavePikedUpCommand { get; set; }
        private readonly IHelperViewService _helperView;

        public EditPickedupMV(
            IManagerDispatchMobService managerDispatchMob, 
            Shipping shipping,
            INavigation navigation)
            : base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            this.managerDispatchMob = managerDispatchMob;
            Shipping = shipping;
            SavePikedUpCommand = new DelegateCommand(SavePikedUp);
        }

        private Shipping shipping = null;
        public Shipping Shipping
        {
            get => shipping;
            set => SetProperty(ref shipping, value);
        }

        private async void SavePikedUp()
        {
            await _navigation.PushAsync(new LoadPage(), true);
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.OrderOneWork("Save", Shipping.Id, token, Shipping.idOrder, Shipping.NameP, Shipping.ContactNameP, Shipping.AddresP,
                    Shipping.CityP, Shipping.StateP, Shipping.ZipP, Shipping.PhoneP, Shipping.EmailP, "PikedUp", ref description);
                });
                await _navigation.PopAsync(true);
                await _navigation.PopAsync(true);
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, Navigationn));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.InformationPikedUpSaved);
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigationn));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            else
            {
                await _navigation.PopAsync(true);
            }
        }
    }
}