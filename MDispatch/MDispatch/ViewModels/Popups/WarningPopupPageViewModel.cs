using MDispatch.Service.Utils;
using MvvmHelpers.Commands;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MDispatch.ViewModels.Popups
{
    public class WarningPopupPageViewModel : BasePopupViewModel
    {
        private readonly IUtilsService _utilsService;
        public WarningPopupPageViewModel(
            INavigation navigation)
            : base(navigation)
        {
            _utilsService = DependencyService.Get<IUtilsService>();
        }

        public ICommand OkCommand => new AsyncCommand(OnOkCommand);

        private async Task OnOkCommand()
        {
            await OnGoBackPopupCommand();
            await _utilsService.StartListening(true);
        }
    }
}
