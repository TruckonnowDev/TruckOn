using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.NewElement.Directory;
using MDispatch.NewElement.ResIzeImage;
using MDispatch.Service;
using MDispatch.View.Inspection;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
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
    public partial class FullPagePhotoDelyvery : ContentPage
    {
        public FullPagePhotoDelyveryMV fullPagePhotoDelyveryMV = null;
        private string pngPaternPhoto = null;

        public FullPagePhotoDelyvery(ManagerDispatchMob managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, string pngPaternPhoto,
            string typeCar, int photoIndex, InitDasbordDelegate initDasbordDelegate, GetVechicleDelegate getVechicleDelegate, string nameLayoute, 
            string onDeliveryToCarrier, string totalPaymentToCarrier)
        {
            this.pngPaternPhoto = pngPaternPhoto;
            fullPagePhotoDelyveryMV = new FullPagePhotoDelyveryMV(managerDispatchMob, vehiclwInformation, idShip, typeCar, photoIndex, Navigation, initDasbordDelegate, getVechicleDelegate, onDeliveryToCarrier, totalPaymentToCarrier, this);
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);  
            BindingContext = fullPagePhotoDelyveryMV;
            paternPhoto.Source = pngPaternPhoto;
            dmla.IsVisible = false;
            if (fullPagePhotoDelyveryMV.Car.TypeIndex != null && fullPagePhotoDelyveryMV.Car.TypeIndex != "")
            {
                NameSelectPhoto.Text = $"{nameLayoute} - {photoIndex}/{fullPagePhotoDelyveryMV.Car.CountCarImg}";
            }
            else
            {
                NameSelectPhoto.Text = "--------------------";
            }
        }

        public void SetbtnVisable(bool isLoadFolderOffline = false)
        {
            if (fullPagePhotoDelyveryMV.AllSourseImage != null && fullPagePhotoDelyveryMV.AllSourseImage.Count != 0)
            {
                if(isLoadFolderOffline)
                {
                    Photos.SelectedItem = fullPagePhotoDelyveryMV.AllSourseImage[0];
                }
                else
                {
                    fullPagePhotoDelyveryMV.SourseImage = fullPagePhotoDelyveryMV.AllSourseImage[0];
                }
                btnNext.IsVisible = true;
                btnNext.HorizontalOptions = LayoutOptions.End;
                btnAddPhoto.IsVisible = false;
                btnDamage.IsVisible = true;
                btnRetake.IsVisible = true;
            }
        }

        public void SetbtnVisableButton()
        {
            btnNext.IsVisible = true;
            btnNext.HorizontalOptions = LayoutOptions.End;
            btnAddPhoto.IsVisible = false;
            btnDamage.IsVisible = true;
            btnRetake.IsVisible = true;
        }
        

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var actionSheet = await DisplayActionSheet(LanguageHelper.TitelSelectPickPhoto, LanguageHelper.CancelBtnText, null, LanguageHelper.SelectGalery, LanguageHelper.SelectPhoto);
            if (actionSheet == LanguageHelper.SelectGalery)
            {
                await Navigation.PushAsync(new CameraPagePhoto1(pngPaternPhoto, this, "PhotoIspection"));
            }
            else if (actionSheet == LanguageHelper.SelectPhoto)
            {
                Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
                if (stream != null)
                {
                    MemoryStream ms = new MemoryStream();
                    stream.CopyTo(ms);
                    fullPagePhotoDelyveryMV.AddNewFotoSourse(ms.ToArray());
                    fullPagePhotoDelyveryMV.SetPhoto(ms.ToArray());
                    SetbtnVisable();
                }
            }
        }

        private async void MessagesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            fullPagePhotoDelyveryMV.SourseImage = fullPagePhotoDelyveryMV.ConvertBase64ToImageSource(fullPagePhotoDelyveryMV.PhotoInspection.Photos[e.SelectedItemIndex].Base64);
            if((ImageSource)Photos.SelectedItem != fullPagePhotoDelyveryMV.AllSourseImage[0])
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
            if(fullPagePhotoDelyveryMV.AllSourseImage != null && fullPagePhotoDelyveryMV.AllSourseImage.Count != 0)
            {
                fullPagePhotoDelyveryMV.SavePhoto();
            }
            else
            {
               // await PopupNavigation.
            }
        }

        internal void AddDamagCurrentLayut(Xamarin.Forms.View view, double? xInterest = null, double? yInterest = null, int? width = null, int? height = null)
        {
            if(xInterest != null && yInterest != null)
            {
                double x = (double)xInterest;
                double y = (double)yInterest;
                AbsoluteLayout.SetLayoutBounds(view, new Rectangle((double)xInterest, (double)yInterest, 30, 30));
                AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.PositionProportional);
            }
            ((ImgResize)view).OneTabAction += SelectImageSourse;
            dmla.Children.Add(view);
        }

        private void SelectImageSourse(object sender)
        {
            ImageSource imageSource = fullPagePhotoDelyveryMV.SelectPhotoForDamage((Image)sender);
            if (imageSource != null)
            {
                Photos.SelectedItem = imageSource;
            }
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            if (fullPagePhotoDelyveryMV.PhotoInspection != null)
            {
                await Navigation.PushAsync(new Inspection.PickedUp.PageAddDamage1(fullPagePhotoDelyveryMV, this, dmla.Children.ToList()));
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
            var actionSheet = await DisplayActionSheet(LanguageHelper.TitelSelectPickPhoto, LanguageHelper.CancelBtnText, null, LanguageHelper.SelectGalery, LanguageHelper.SelectPhoto);
            if (actionSheet == LanguageHelper.SelectGalery)
            {
                RetakeFullPageDelivery retakeFullPageDelivery = null;
                StreamImageSource streamImageSource = (StreamImageSource)fullPagePhotoDelyveryMV.SourseImage;
                Task<Stream> task = streamImageSource.Stream(System.Threading.CancellationToken.None);
                Stream stream = task.Result;
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                byte[] bytes = ms.ToArray();
                retakeFullPageDelivery = new RetakeFullPageDelivery(fullPagePhotoDelyveryMV, bytes);
                await Navigation.PushAsync(new RetakePage(retakeFullPageDelivery));
            }
            else if (actionSheet == LanguageHelper.SelectPhoto)
            {

                Stream streamNewPhoto = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
                if (streamNewPhoto != null)
                {
                    MemoryStream msNewPhoto = new MemoryStream();
                    streamNewPhoto.CopyTo(msNewPhoto);
                    StreamImageSource streamImageSource = (StreamImageSource)fullPagePhotoDelyveryMV.SourseImage;
                    Task<Stream> task = streamImageSource.Stream(System.Threading.CancellationToken.None);
                    Stream streamOldPhpto = task.Result;
                    MemoryStream msOldPhoto = new MemoryStream();
                    streamOldPhpto.CopyTo(msOldPhoto);
                    fullPagePhotoDelyveryMV.ReSetPhoto(msNewPhoto.ToArray(), msOldPhoto.ToArray());
                }
            }
        }

        public async Task CreateActionSheet(Action<string> calbackResultAction, params string[] paramsAction)
        {
            var actionSheet = await DisplayActionSheet(LanguageHelper.TitelSelectPickPhoto, LanguageHelper.CancelBtnText, null, paramsAction);
            calbackResultAction(actionSheet);
        }
    }
}