using System;
using MDispatch.NewElement;
using MDispatch.Service.HelpersViews;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace MDispatch.View.Inspection
{
    public partial class ViewPhoto : PopupPage
    {
        public ViewPhoto(ImageSource sourse)
        {
            InitializeComponent();
            img.Source = sourse;
        }

        protected override async void OnAppearing()
        {
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            DependencyService.Get<IOrientationHandler>().ForcePortrait();
            base.OnDisappearing();
        }

        private async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            await OpacityTouchView.TouchFeedBack((Xamarin.Forms.View)sender);
            await PopupNavigation.PopAsync();
        }
    }
}
