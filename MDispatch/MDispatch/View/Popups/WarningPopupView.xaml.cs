using MDispatch.ViewModels.Popups;
using Rg.Plugins.Popup.Pages;

namespace MDispatch.View.Popups
{
    public partial class WarningPopupView : PopupPage
    {
		public WarningPopupView()
		{
			InitializeComponent();
            BindingContext = new WarningPopupPageViewModel(Navigation);
        }
    }
}