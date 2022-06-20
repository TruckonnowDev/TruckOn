using MDispatch.Vidget.View;
using MDispatch.Vidget.VM;
using MvvmHelpers.Commands;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MDispatch.ViewModels.Popups
{
    public class PlateTrailerWritePopupPageViewModel : BasePopupViewModel
    {
        private readonly FullPhotoTruckVM _fullPhotoTruckVM;
        public PlateTrailerWritePopupPageViewModel(
            FullPhotoTruckVM fullPhotoTruckVM,
            INavigation navigation)
            : base(navigation)
        {
            _fullPhotoTruckVM = fullPhotoTruckVM;
        }

        private string _plateTrailer;
        public string PlateTrailer
        {
            get => _plateTrailer;
            set => SetProperty(ref _plateTrailer, value);
        }

        public ICommand SendCommand => new AsyncCommand(OnSendCommand);
        public ICommand ScanCommand => new AsyncCommand(OnScanCommand);

        internal override async Task OnGoBackPopupCommand()
        {
            await base.OnGoBackPopupCommand();
            _fullPhotoTruckVM.BackToRootPage();
        }

        private async Task OnSendCommand()
        {
            await _fullPhotoTruckVM.SetPlate("Trailer");
        }

        private async Task OnScanCommand()
        {
            await _popupNavigation.PopAsync();
            await _fullPhotoTruckVM.Navigation.PushAsync(new ScanCamera(_fullPhotoTruckVM, "trailer"));
        }
    }
}
