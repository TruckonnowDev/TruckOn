﻿using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.Service.Net;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.PageAppMV
{
    public class EditPickedupMV : BindableBase
    {
        public ManagerDispatchMob managerDispatchMob = null;
        public DelegateCommand SavePikedUpCommand { get; set; }
        public INavigation Navigationn { get; set; }

        public EditPickedupMV(ManagerDispatchMob managerDispatchMob, Shipping shipping)
        {
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

        [System.Obsolete]
        private async void SavePikedUp()
        {
            await PopupNavigation.PushAsync(new LoadPage(), true);
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.OrderOneWork("Save", Shipping.Id, token, Shipping.idOrder, Shipping.NameP, Shipping.ContactNameP, Shipping.AddresP,
                    Shipping.CityP, Shipping.StateP, Shipping.ZipP, Shipping.PhoneP, Shipping.EmailP, "PikedUp", ref description);
                });
                await PopupNavigation.PopAsync(true);
                await Navigationn.PopAsync(true);
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, Navigationn));
                    HelpersView.CallError(description);
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.InformationPikedUpSaved);
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", Navigationn));
                    HelpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
            }
            else
            {
                await PopupNavigation.PopAsync(true);
            }
        }
    }
}