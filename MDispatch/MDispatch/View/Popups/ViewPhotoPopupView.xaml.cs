using System;
using MDispatch.NewElement;
using MDispatch.Service.OpacityTouchView;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace MDispatch.View.Popups
{
    public partial class ViewPhotoPopupView : PopupPage
    {
        private readonly IOpacityTouchViewService _opacityTouchViewService;
        public ViewPhotoPopupView(ImageSource sourse)
        {
            InitializeComponent();
            _opacityTouchViewService = DependencyService.Get<IOpacityTouchViewService>();
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
            await _opacityTouchViewService.TouchFeedBack((Xamarin.Forms.View)sender);
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
