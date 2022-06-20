using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement;
using MDispatch.NewElement.Directory;
using MDispatch.NewElement.ResIzeImage;
using MDispatch.Service.ManagerDispatchMob;
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
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.View.PageApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullPagePhoto : ContentPage
    {
        public FullPagePhotoMV fullPagePhotoMV = null;
        private string pngPaternPhoto = null;

        public FullPagePhoto(IManagerDispatchMobService managerDispatchMob, VehiclwInformation vehiclwInformation, string idShip, string pngPaternPhoto, string typeCar, int photoIndex, InitDasbordDelegate initDasbordDelegate,
            GetVechicleDelegate getVechicleDelegate, string nameLayoute, string onDeliveryToCarrier, string totalPaymentToCarrier)
        {
            this.pngPaternPhoto = pngPaternPhoto;
            fullPagePhotoMV = new FullPagePhotoMV(managerDispatchMob, vehiclwInformation, idShip, typeCar, photoIndex, Navigation, initDasbordDelegate, getVechicleDelegate, onDeliveryToCarrier, totalPaymentToCarrier, this);
            InitializeComponent();
            BindingContext = fullPagePhotoMV;
            paternPhoto.Source = pngPaternPhoto;
            dmla.IsVisible = false;
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            if (fullPagePhotoMV.Car.TypeIndex != null && fullPagePhotoMV.Car.TypeIndex != "")
            {
                NameSelectPhoto.Text = $"{nameLayoute} - {photoIndex}/{fullPagePhotoMV.Car.CountCarImg}";
            }
            else
            {
                NameSelectPhoto.Text = "--------------------";
            }
            
        }

        public async Task SetbtnVisable(bool isLoadFolderOffline = false)
        {
            if (fullPagePhotoMV.AllSourseImage != null && fullPagePhotoMV.AllSourseImage.Count != 0)
            {
                if (isLoadFolderOffline)
                {
                    Photos.SelectedItem = fullPagePhotoMV.AllSourseImage[0];
                }
                else
                {
                    fullPagePhotoMV.SourseImage = fullPagePhotoMV.AllSourseImage[0];
                }
                btnNext.IsVisible = true;
                btnNext.HorizontalOptions = LayoutOptions.End;
                btnAddPhoto.IsVisible = false;
                btnDamage.IsVisible = true;
                btnRetake.IsVisible = true;
            }
        }

        internal void AddDamagCurrentLayut(Xamarin.Forms.View view, double? xInterest = null, double? yInterest = null, int? width = null, int? height = null)
        {
            if (xInterest != null && yInterest != null)
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
            ImageSource imageSource = fullPagePhotoMV.SelectPhotoForDamage((Image)sender);
            if (imageSource != null)
            {
                Photos.SelectedItem = imageSource;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var actionSheet = await DisplayActionSheet(LanguageHelper.TitelSelectPickPhoto, LanguageHelper.CancelBtnText, null, LanguageHelper.SelectGalery, LanguageHelper.SelectPhoto);
            if (actionSheet == LanguageHelper.SelectGalery)
            {
                await Navigation.PushAsync(new CameraPagePhoto(pngPaternPhoto, this, "PhotoIspection"));
            }
            else if (actionSheet == LanguageHelper.SelectPhoto)
            {
                Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
                if (stream != null)
                {
                    MemoryStream ms = new MemoryStream();
                    stream.CopyTo(ms);
                    fullPagePhotoMV.AddNewFotoSourse(ms.ToArray());
                    await fullPagePhotoMV.SetPhoto(ms.ToArray(), 0, 0);
                    await SetbtnVisable();
                }
            }

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

        [Obsolete]
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if(fullPagePhotoMV.AllSourseImage != null && fullPagePhotoMV.AllSourseImage.Count != 0)
            {
                fullPagePhotoMV.SavePhoto();
            }
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            if (fullPagePhotoMV.PhotoInspection != null)
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
            var actionSheet = await DisplayActionSheet(LanguageHelper.TitelSelectPickPhoto, LanguageHelper.CancelBtnText, null, LanguageHelper.SelectGalery, LanguageHelper.SelectPhoto);
            if (actionSheet == LanguageHelper.SelectGalery)
            {
                RetakeFullPagePickedUp retakeFullPagePickedUp = null;
                StreamImageSource streamImageSource = (StreamImageSource)fullPagePhotoMV.SourseImage;
                Task<Stream> task = streamImageSource.Stream(System.Threading.CancellationToken.None);
                Stream stream = task.Result;
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                byte[] bytes = ms.ToArray();
                retakeFullPagePickedUp = new RetakeFullPagePickedUp(fullPagePhotoMV, bytes);
                await Navigation.PushAsync(new RetakePage(retakeFullPagePickedUp));
            }
            else if (actionSheet == LanguageHelper.SelectPhoto)
            {
                Stream streamNewPhoto = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
                if (streamNewPhoto != null)
                {
                    MemoryStream msNewPhoto = new MemoryStream();
                    streamNewPhoto.CopyTo(msNewPhoto);
                    StreamImageSource streamImageSource = (StreamImageSource)fullPagePhotoMV.SourseImage;
                    Task<Stream> task = streamImageSource.Stream(System.Threading.CancellationToken.None);
                    Stream streamOldPhpto = task.Result;
                    MemoryStream msOldPhoto = new MemoryStream();
                    streamOldPhpto.CopyTo(msOldPhoto);
                    fullPagePhotoMV.ReSetPhoto(msNewPhoto.ToArray(), msOldPhoto.ToArray());

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