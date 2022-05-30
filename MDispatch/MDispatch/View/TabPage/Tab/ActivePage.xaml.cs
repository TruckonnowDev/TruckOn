using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View.PageApp;
using MDispatch.View.PageApp.Settings;
using MDispatch.ViewModels.TAbbMV;
using MDispatch.ViewModels.TAbbMV.DialogAsk;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.View.TabPage.Tab
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ActivePage : ContentPage
    {
        public ActiveMV activeMV = null;
        private StackLayout SelectStackLayout = null;
        private InitDasbordDelegate initDasbordDelegate = null;
        private readonly IHelperViewService _helperView;

        public ActivePage (IManagerDispatchMobService managerDispatchMob, INavigation navigation)
		{
            this.activeMV = new ActiveMV(managerDispatchMob, navigation);
            _helperView = DependencyService.Get<IHelperViewService>();
            InitializeComponent ();
            BindingContext = this.activeMV;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

            StackLayout stackLayout = ((StackLayout)sender).FindByName<StackLayout>("st");

            if (SelectStackLayout != null)
            {
                SelectStackLayout.BackgroundColor = Color.White;
            }
            SelectStackLayout = stackLayout;
            SelectStackLayout.BackgroundColor = Color.FromHex("#f5c8c8");
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            if (!activeMV.UnTimeOfInspection.ISMaybiInspection)
            {
                await Navigation.PushAsync(new AskHint(activeMV));
            }
            else
            {
                if (SelectStackLayout != null)
                {
                    SelectStackLayout.BackgroundColor = Color.White;
                    SelectStackLayout = null;
                }
                string idOrder = null;
                FlexLayout stackLayout = (FlexLayout)sender;
                Label idorderL = stackLayout.FindByName<Label>("idOrder");
                if (idorderL != null)
                {
                    idOrder = idorderL.Text;
                }
                else
                {
                    idOrder = stackLayout.Parent.Parent.FindByName<Label>("idOrder").Text;
                }
                await activeMV._navigation.PushAsync(new InfoOrder(activeMV.managerDispatchMob, activeMV.initDasbordDelegate,
                    activeMV.Shippings.Find(s => s.Id == idOrder).CurrentStatus, activeMV.Shippings.Find(s => s.Id == idOrder).Id));
            }
        }

        private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            TapGestureRecognizer_Tapped(sender, e);
        }

        private void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            TapGestureRecognizer_Tapped_1(sender, e);
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            this.activeMV._globalHelperService.OutAccount();
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            _helperView.InitAlert(body);
            activeMV.Init();
        }

        [Obsolete]
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _helperView.Hidden();
        }

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri($"http://truckonnow.com/Doc/{CrossSettings.Current.GetValueOrDefault("IdDriver", "0")}"));
        }

        private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Settings(activeMV.managerDispatchMob));
        }
    }
}