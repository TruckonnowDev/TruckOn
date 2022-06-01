using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.ViewModels.TAbbMV.DialogAsk
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AskHint : PopupPage
    {
        private ActiveMV activeMV = null;

        public AskHint(ActiveMV activeMV)
        {
            this.activeMV = activeMV;
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            activeMV.GoToInspectionDrive();
            await PopupNavigation.Instance.PopAsync();
        }
    }
}