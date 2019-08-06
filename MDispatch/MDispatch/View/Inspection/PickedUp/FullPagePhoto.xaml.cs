﻿using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.NewElement.ResIzeImage;
using MDispatch.Service;
using MDispatch.View.Inspection;
using MDispatch.View.Inspection.PickedUp;
using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using MDispatch.ViewModels.InspectionMV.Servise.Retake;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.View.PageApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullPagePhoto : ContentPage
    {
        public FullPagePhotoMV fullPagePhotoMV = null;
        private string pngPaternPhoto = null;

        public FullPagePhoto(ManagerDispatchMob managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, string pngPaternPhoto, string typeCar, int photoIndex, InitDasbordDelegate initDasbordDelegate, 
            GetVechicleDelegate getVechicleDelegate, string nameLayoute, string onDeliveryToCarrier, string totalPaymentToCarrier)
        {
            this.pngPaternPhoto = pngPaternPhoto;
            fullPagePhotoMV = new FullPagePhotoMV(managerDispatchMob, vehiclwInformation, idShip, typeCar, photoIndex, Navigation, initDasbordDelegate, getVechicleDelegate, onDeliveryToCarrier, totalPaymentToCarrier);
            InitializeComponent();
            BindingContext = fullPagePhotoMV;
            paternPhoto.Source = pngPaternPhoto;
            dmla.IsVisible = false;
            if (fullPagePhotoMV.Car.typeIndex != null && fullPagePhotoMV.Car.typeIndex != "")
            {
                NameSelectPhoto.Text = $"{nameLayoute} - {photoIndex}/{fullPagePhotoMV.Car.CountCarImg}";
            }
            else
            {
                NameSelectPhoto.Text = "--------------------";
            }
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async Task SetbtnVisable()
        {
            if (fullPagePhotoMV.AllSourseImage != null && fullPagePhotoMV.AllSourseImage.Count != 0)
            {
                Photos.SelectedItem = fullPagePhotoMV.AllSourseImage[0];
                btnNext.IsVisible = true;
                btnNext.HorizontalOptions = LayoutOptions.End;
                btnAddPhoto.IsVisible = false;
                btnDamage.IsVisible = true;
                btnRetake.IsVisible = true;
            }
        }

        public void AddDamagCurrentLayut(Xamarin.Forms.View view)
        {
            ((ImgResize)view).OneTabAction += SelectImageSourse;
            dmla.Children.Add(view);
        }

        private void SelectImageSourse(object sender)
        {
            ImageSource imageSource = fullPagePhotoMV.SelectPhotoForDamage((Image)sender);
            if (imageSource != null)
            {
                Photos.SelectedItem = imageSource;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraPagePhoto(pngPaternPhoto, this));
        }

        private async void MessagesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            fullPagePhotoMV.SourseImage = (ImageSource)e.SelectedItem;
            if((ImageSource)Photos.SelectedItem != fullPagePhotoMV.AllSourseImage[0])
            {
                dmla.IsVisible = false;
                paternPhoto.Source = "";
                btnNext.HorizontalOptions = LayoutOptions.EndAndExpand;
                btnDamage.IsVisible = false;
            }
            else
            {
                btnNext.HorizontalOptions = LayoutOptions.End;
                btnDamage.IsVisible = true;
                dmla.IsVisible = true;
                paternPhoto.Source = pngPaternPhoto;
            }
        }
        
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if(fullPagePhotoMV.AllSourseImage != null && fullPagePhotoMV.AllSourseImage.Count != 0)
            {
                fullPagePhotoMV.SavePhoto();
            }
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            if (fullPagePhotoMV.PhotoInspection != null && fullPagePhotoMV.AllSourseImage.FindIndex(a => a == fullPagePhotoMV.SourseImage) == 0)
            {
                await Navigation.PushAsync(new PageAddDamage(fullPagePhotoMV, this, dmla.Children.ToList()));
            }
        }

        protected override bool OnBackButtonPressed()
        {
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            return base.OnBackButtonPressed();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            DependencyService.Get<IOrientationHandler>().ForceSensor();
            await Navigation.PopAsync();
        }

        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            RetakeFullPagePickedUp retakeFullPagePickedUp = null;
            StreamImageSource streamImageSource = (StreamImageSource)fullPagePhotoMV.SourseImage;
            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
            Task<Stream> task = streamImageSource.Stream(cancellationToken);
            Stream stream = task.Result;
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            byte[] bytes = ms.ToArray();
            retakeFullPagePickedUp = new RetakeFullPagePickedUp(fullPagePhotoMV, bytes);
            await Navigation.PushAsync(new RetakePage(retakeFullPagePickedUp));
        }
    }
}