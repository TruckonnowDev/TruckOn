using MDispatch.Helpers;
using MDispatch.NewElement.Directory;
using MDispatch.NewElement.ResIzeImage;
using MDispatch.View.PageApp;
using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection.PickedUp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageAddDamage : ContentPage
	{
        private FullPagePhotoMV FullPagePhotoMV = null;
        public int stateSelect = 0;
        public int indexSelectDamage = 0;
        public string nameDamage = null;
        public string prefNameDamage = null;
        private FullPagePhoto fullPagePhoto = null;

        public PageAddDamage(FullPagePhotoMV fullPagePhotoMV, FullPagePhoto fullPagePhoto, List<Xamarin.Forms.View> views = null)
        {
            this.fullPagePhoto = fullPagePhoto;
            FullPagePhotoMV = fullPagePhotoMV;
            InitializeComponent();
            touchImage.Source = FullPagePhotoMV.SourseImage;
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            On<iOS>().SetPrefersStatusBarHidden(StatusBarHiddenMode.True)
                .SetPreferredStatusBarUpdateAnimation(UIStatusBarAnimation.Fade);
            if (views != null)
            {
                foreach (var view in views)
                {
                    view.Opacity = 0.5;
                    absla.Children.Add(view);
                    ((ImgResize)view).OneTabAction += AddControlPanelDmg;
                }
            }
        }

        private async void TouchImage_TouchAction(object sender, NewElement.TouchCordinate.TouchActionEventArgs e)
        {
            RemoveSelectedDmg();
            stateSelect = 1;
            await Navigation.PushAsync(new DamageSelecter(FullPagePhotoMV, this), true);
            await WaiteSelectDamage();
            await Navigation.PopAsync(true);
            if (stateSelect == 0)
            {
                stateSelect = 1;

                var actionSheet = await DisplayActionSheet(LanguageHelper.TitelSelectPickPhoto, LanguageHelper.CancelBtnText, null, LanguageHelper.SelectGalery, LanguageHelper.SelectPhoto);
                if (actionSheet == LanguageHelper.SelectGalery)
                {
                    await Navigation.PushAsync(new CameraPagePhoto(null, fullPagePhoto, null, this));
                }
                else if (actionSheet == LanguageHelper.SelectPhoto)
                {
                    Stream streamNewPhoto = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
                    if (streamNewPhoto != null)
                    {
                        MemoryStream msNewPhoto = new MemoryStream();
                        streamNewPhoto.CopyTo(msNewPhoto);
                        FullPagePhotoMV.AddNewFotoSourse(msNewPhoto.ToArray());
                        await FullPagePhotoMV.SetPhoto(msNewPhoto.ToArray());
                        stateSelect = 0;
                    }
                }
                await WaiteSelectDamage();
                ImgResize image = new ImgResize()
                {
                    Source = $"DamageP{indexSelectDamage}.png",
                    WidthRequest = 30,
                    HeightRequest = 30,
                };
                image.Opacity = 0.5;
                image.OneTabAction += AddControlPanelDmg;
                AbsoluteLayout.SetLayoutBounds(image, new Rectangle(e.XInterest * 0.0001, e.YInterest * 0.0001, 30, 30));
                AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.PositionProportional);
                absla.Children.Add(image);
                await Task.Run(() =>
                {
                    FullPagePhotoMV.SetDamage(nameDamage, indexSelectDamage, prefNameDamage, e.XInterest * 0.0001, e.YInterest * 0.0001, 30, 30, image, FullPagePhotoMV.AllSourseImage.Last());
                });
            }
            else
            {
                stateSelect = 0;
            }
        }

        Image dmgSelected = null;
        private void AddControlPanelDmg(object sender)
        {
            controlDmg.IsEnabled = true;
            controlDmg.IsVisible = true;
            dmgSelected = (Image)sender;
            scrolSizeDmg.ValueChanged += ScrolSizeDmg_ValueChanged;
            deletBtnDmg.Clicked += DeleteDamage;
        }

        private void DeleteDamage(object sender, EventArgs e)
        {
            ((ImgResize)dmgSelected).OneTabAction -= AddControlPanelDmg;
            controlDmg.IsEnabled = false;
            controlDmg.IsVisible = false;
            absla.Children.Remove(dmgSelected);
            FullPagePhotoMV.RemmoveDamage(dmgSelected);
        }

        private void RemoveSelectedDmg()
        {
            scrolSizeDmg.ValueChanged -= ScrolSizeDmg_ValueChanged;
            controlDmg.IsEnabled = false;
            controlDmg.IsVisible = false;
            dmgSelected = null;
        }

        protected override bool OnBackButtonPressed()
        {
            if (absla.Children.ToList().Count - 1 > 2)
            {
                List<Xamarin.Forms.View> views = absla.Children.ToList().GetRange(3, absla.Children.ToList().Count - 3);
                foreach (var view in views)
                {
                    try
                    {
                        ((ImgResize)view).OneTabAction -= AddControlPanelDmg;
                        fullPagePhoto.AddDamagCurrentLayut(view);
                    }
                    catch
                    { }
                }
            }
            return base.OnBackButtonPressed();
        }

        private async Task WaiteSelectDamage()
        {
            await Task.Run(() => { while(stateSelect == 1) { } });
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            if (absla.Children.ToList().Count - 1 > 2)
            {
                List<Xamarin.Forms.View> views = absla.Children.ToList().GetRange(3, absla.Children.ToList().Count - 3);
                foreach (var view in views)
                {
                    try
                    {
                        ((ImgResize)view).OneTabAction -= AddControlPanelDmg;
                        fullPagePhoto.AddDamagCurrentLayut(view);
                    }
                    catch
                    { }
                }
            }
            await Navigation.PopAsync();
        }

        [Obsolete]
        private void Button_Clicked(object sender, EventArgs e)
        {
            FullPagePhotoMV.SavePhoto(true);
        }

        private void ScrolSizeDmg_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            //Vibration.Vibrate(7);
            ImgResize rezizeImgnew = (ImgResize)dmgSelected;
            Rectangle rectangle = AbsoluteLayout.GetLayoutBounds(rezizeImgnew);
            rectangle.Height = scrolSizeDmg.Value;
            rectangle.Width = scrolSizeDmg.Value;
            AbsoluteLayout.SetLayoutBounds(rezizeImgnew, rectangle);
            Task.Run(() =>
            {
                FullPagePhotoMV.ReSetDamage(dmgSelected, (int)rectangle.Width, (int)rectangle.Height);
            });
            //Vibration.Vibrate(7);
        }

        private void ScrolSizeDmg_SizeChanged(object sender, EventArgs e)
        {
            double width = App.Current.MainPage.Width;
           ((Xamarin.Forms.Slider)sender).WidthRequest = width * 0.80;
        }
    }
}