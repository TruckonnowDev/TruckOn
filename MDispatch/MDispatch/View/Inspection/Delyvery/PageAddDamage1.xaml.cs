﻿using MDispatch.NewElement.ResIzeImage;
using MDispatch.View.PageApp;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection.PickedUp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageAddDamage1 : ContentPage
	{
        private FullPagePhotoDelyveryMV fullPagePhotoDelyveryMV = null;
        public int stateSelect = 0;
        public int indexSelectDamage = 0;
        public string nameDamage = null;
        public string prefNameDamage = null;
        private FullPagePhotoDelyvery fullPagePhotoDelyvery = null;

        public PageAddDamage1(FullPagePhotoDelyveryMV fullPagePhotoDelyveryMV, FullPagePhotoDelyvery fullPagePhotoDelyvery, List<Xamarin.Forms.View> views = null)
        {
            this.fullPagePhotoDelyvery = fullPagePhotoDelyvery;
            this.fullPagePhotoDelyveryMV = fullPagePhotoDelyveryMV;
            InitializeComponent();
            touchImage.Source = this.fullPagePhotoDelyveryMV.SourseImage;
            NavigationPage.SetHasNavigationBar(this, false);
            if (views != null)
            {
                foreach (var view in views)
                {
                    absla.Children.Add(view);
                    ((ImgResize)view).OneTabAction += AddControlPanelDmg;
                }
            }
        }

        Image dmgSelected = null;
        private void AddControlPanelDmg(object sender)
        {
            controlDmg.IsEnabled = true;
            controlDmg.IsVisible = true;
            dmgSelected = (Image)sender;
            scrolSizeDmg.PropertyChanged += ScrolSizeDmg_PropertyChanged;
            deletBtnDmg.Clicked += DeleteDamage;
        }

        private void DeleteDamage(object sender, EventArgs e)
        {
            ((ImgResize)dmgSelected).OneTabAction -= AddControlPanelDmg;
            controlDmg.IsEnabled = false;
            controlDmg.IsVisible = false;
            absla.Children.Remove(dmgSelected);
            fullPagePhotoDelyveryMV.RemmoveDamage(dmgSelected);
        }

        private void RemoveSelectedDmg()
        {
            scrolSizeDmg.PropertyChanged -= ScrolSizeDmg_PropertyChanged;
            deletBtnDmg.Clicked -= DeleteDamage;
            controlDmg.IsEnabled = false;
            controlDmg.IsVisible = false;
            dmgSelected = null;
        }

        [Obsolete]
        private async void TouchImage_TouchAction(object sender, NewElement.TouchCordinate.TouchActionEventArgs e)
        {
            stateSelect = 1;
            await PopupNavigation.PushAsync(new DamageSelecter1(null, this), true);
            await WaiteSelectDamage();
            await PopupNavigation.PopAsync(true);
            if (stateSelect == 0)
            {
                stateSelect = 1;
                await Navigation.PushAsync(new CameraPagePhoto1(null, fullPagePhotoDelyvery, this));
                await WaiteSelectDamage();
                ImgResize image = new ImgResize()
                {
                    Source = $"DamageD{indexSelectDamage}.png",
                    WidthRequest = 30,
                    HeightRequest = 30,
                };
                image.OneTabAction += AddControlPanelDmg;
                AbsoluteLayout.SetLayoutBounds(image, new Rectangle(e.XInterest * 0.0001, e.YInterest * 0.0001, 30, 30));
                AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.PositionProportional);
                absla.Children.Add(image);
                await Task.Run(() =>
                {
                    fullPagePhotoDelyveryMV.SetDamage(nameDamage, indexSelectDamage, prefNameDamage, e.XInterest * 0.0001, e.YInterest * 0.0001, 30, 30, image, fullPagePhotoDelyveryMV.AllSourseImage.Last());
                });
            }
            else
            {
                stateSelect = 0;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            List<Xamarin.Forms.View> views = absla.Children.ToList().GetRange(3, absla.Children.ToList().Count - 3);
            foreach (var view in views)
            {
                try
                {
                    ((ImgResize)view).OneTabAction -= AddControlPanelDmg;
                    fullPagePhotoDelyvery.AddDamagCurrentLayut(view);
                }
                catch
                { }
            }
            return base.OnBackButtonPressed();
        }

        private async Task WaiteSelectDamage()
        {
            await Task.Run(() =>
            {
                while(stateSelect == 1)
                { }
            });
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
                        fullPagePhotoDelyvery.AddDamagCurrentLayut(view);
                    }
                    catch
                    { }
                }
            }
            await Navigation.PopAsync();
        }

        private void ScrolSizeDmg_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                ImgResize rezizeImgnew = (ImgResize)dmgSelected;
                Rectangle rectangle = AbsoluteLayout.GetLayoutBounds(rezizeImgnew);
                rectangle.Height = scrolSizeDmg.Value;
                rectangle.Width = scrolSizeDmg.Value;
                    AbsoluteLayout.SetLayoutBounds(rezizeImgnew, rectangle);
                    Task.Run(() =>
                    {
                        fullPagePhotoDelyveryMV.ReSetDamage(dmgSelected, (int)rectangle.Width, (int)rectangle.Height);
                    });
            }
        }
    }
}