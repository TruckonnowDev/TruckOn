using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using Plugin.Settings;
using Prism.Commands;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.PageAppMV
{
    public class EditPaymentInfoMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public DelegateCommand SavePaymentUpCommand { get; set; }

        public EditPaymentInfoMV(
            IManagerDispatchMobService managerDispatchMob, 
            Shipping shipping,
            INavigation navigation)
            :base(navigation)
        {
            this.managerDispatchMob = managerDispatchMob;
            Shipping = shipping;
            SavePaymentUpCommand = new DelegateCommand(SavePayments);
            SorseDropDown = new string[]
            {
                "COD",
                "COP",
                "2 days",
                "5 days",
                "7 days",
                "10 days",
                "15 days",
                "20 days",
                "30 days",
                "45 days",
            };
        }

        private string[] sorseDropDown = null;
        public string[] SorseDropDown
        {
            get => sorseDropDown;
            set => SetProperty(ref sorseDropDown, value);
        }

        private Shipping shipping = null;
        public Shipping Shipping
        {
            get => shipping;
            set => SetProperty(ref shipping, value);
        }

        private async void SavePayments()
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
                    state = managerDispatchMob.OrderOneWork("Save", Shipping.Id, token, "Payment", Shipping.PriceListed, Shipping.TotalPaymentToCarrier, ref description);
                });
                await _popupNavigation.PopAllAsync(true);
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                {
                    await _popupNavigation.PushAsync(new Alert(description, Navigation));
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.InformationPaymentSaved);
                }
                else if (state == 4)
                {
                    await _popupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, Navigation));
                }
            }
            else
            {
                await _popupNavigation.PopAsync(true);
            }
        }
    }
}