using MDispatch.Models.Enum;
using MDispatch.NewElement;
using MDispatch.Service;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.ViewModels.PageAppMV.Settings;
using Plugin.LatestVersion;
using Plugin.Settings;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.PageApp.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        private SettingsMV settingsMV = null;
        private readonly IHelperViewService _helperView;

        [Obsolete]
        public Settings(IManagerDispatchMobService managerDispatchMob)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            settingsMV = new SettingsMV(managerDispatchMob, Navigation);
            InitializeComponent();
            On<iOS>().SetUseSafeArea(true);
            BindingContext = settingsMV;
            SetLan(CrossSettings.Current.GetValueOrDefault("Language", (int)LanguageType.English));
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        [Obsolete]
        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri($"{Config.BaseSiteUrl}/Doc/{CrossSettings.Current.GetValueOrDefault("IdDriver", "0")}"));
        }

        [Obsolete]
        private void Button_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri($"{Config.BaseSiteUrl}/Doc?truckPlate={settingsMV.PlateTruck1}&trailerPlate={settingsMV.PlateTrailer1}"));
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            settingsMV.OutAccount();
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            await CrossLatestVersion.Current.OpenAppInStore();
        }

        private async void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
        {
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
            await Navigation.PushModalAsync(new ScanPlateSettings(settingsMV, "truck"));
        }

        private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
            await Navigation.PushModalAsync(new ScanPlateSettings(settingsMV, "trailer"));
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _helperView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _helperView.Hidden();
        }

        private async void TapGestureRecognizer_Tapped_4(System.Object sender, System.EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new SelectLanguage(SetLan));
        }

        private void SetLan(int lanIndex)
        {
            imgLan.Source = lanIndex == (int)LanguageType.English ? "EnglishLanIcon.png" : lanIndex == (int)LanguageType.Russian ? "RussianLanIcon.png" : "SpanishLanIcon.png";
            piLang.Text = lanIndex == (int)LanguageType.English ? "English" : lanIndex == (int)LanguageType.Russian ? "Russian" : "Spanish";
        }
    }
}
