using MvvmHelpers.Commands;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MDispatch.ViewModels.Popups
{
    public class BasePopupViewModel : BaseViewModel
    {
        public BasePopupViewModel(
            INavigation navigation)
            : base(navigation)
        {
        }

        public ICommand GoBackPopupCommand => new AsyncCommand(OnGoBackPopupCommand);

        internal virtual async Task OnGoBackPopupCommand()
        {
            await _popupNavigation.PopAsync();
        }
    }
}
