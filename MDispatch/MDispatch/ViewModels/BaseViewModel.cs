using MDispatch.Service.GlobalHelper;
using MDispatch.Service.Utils;
using Prism.Mvvm;
using Rg.Plugins.Popup.Contracts;
using Xamarin.Forms;

namespace MDispatch.ViewModels
{
    public class BaseViewModel : BindableBase
    {
        internal readonly IUtilsService _utils;
        internal readonly IGlobalHelperService _globalHelperService;
        internal readonly INavigation _navigation;

        public BaseViewModel(
            INavigation navigation)
        {
            _utils = DependencyService.Get<IUtilsService>();
            _globalHelperService = DependencyService.Get<IGlobalHelperService>();
            _navigation = navigation;
        }
    }
}
