using MDispatch.Service.GlobalHelper;
using MDispatch.Service.Utils;
using Prism.Mvvm;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace MDispatch.ViewModels
{
    public class BaseViewModel : BindableBase
    {
        internal readonly IUtilsService _utils;
        internal readonly IGlobalHelperService _globalHelperService;
        public readonly INavigation Navigation;
        internal readonly IPopupNavigation _popupNavigation;

        public BaseViewModel(
            INavigation navigation)
        {
            _utils = DependencyService.Get<IUtilsService>();
            _globalHelperService = DependencyService.Get<IGlobalHelperService>();
            Navigation = navigation;
            _popupNavigation = PopupNavigation.Instance;
        }
    }
}
