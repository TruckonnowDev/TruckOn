using MDispatch.Helpers;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.Service.Utils;
using MDispatch.View.A_R;
using MDispatch.View.GlobalDialogView;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.Service.GlobalHelper
{
    public class GlobalHelperService : IGlobalHelperService
    {
        private readonly IUtilsService _utils;
        private readonly IManagerDispatchMobService _managerDispatchMobService;

        public GlobalHelperService()
        {
            _utils = DependencyService.Get<IUtilsService>();
            _managerDispatchMobService = DependencyService.Get<IManagerDispatchMobService>();
        }

        public async void OutAccount()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            if (PopupNavigation.Instance.PopupStack.Count != 0)
            {
                await PopupNavigation.Instance.PopAllAsync();
            }
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = _managerDispatchMobService.A_RWork("Clear", null, null, ref description, ref token);
                });
                if (state == 2)
                {
                    await PopupNavigation.Instance.PushAsync(new Alert("Error", null));
                }
                else if (state == 3)
                {
                    CrossSettings.Current.Remove("Token");
                    App.isAvtorization = false;
                    Application.Current.MainPage = new NavigationPage(new Avtorization());
                }
                else if (state == 4)
                {
                    await PopupNavigation.Instance.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
        }
    }
}
