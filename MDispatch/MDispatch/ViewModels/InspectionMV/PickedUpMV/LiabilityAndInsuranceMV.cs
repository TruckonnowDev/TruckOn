using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service;
using MDispatch.Service.Helpers;
using MDispatch.Service.Net;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using MDispatch.View.Inspection.PickedUp;
using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob;

namespace MDispatch.ViewModels.InspectionMV.PickedUpMV
{
    public class LiabilityAndInsuranceMV : BindableBase
    {
        public ManagerDispatchMob managerDispatchMob = null;
        public INavigation Navigation { get; set; }
        public DelegateCommand GoToFeedBackCommand { get; set; }
        public InitDasbordDelegate initDasbordDelegate = null;
        public LiabilityAndInsurance liabilityAndInsurance = null;

        public LiabilityAndInsuranceMV(ManagerDispatchMob managerDispatchMob, string idVech, string idShip, INavigation navigation, InitDasbordDelegate initDasbordDelegate,
            LiabilityAndInsurance liabilityAndInsurance)
        {
            this.managerDispatchMob = managerDispatchMob;
            Navigation = navigation;
            IdShip = idShip;
            IdVech = idVech;
            this.liabilityAndInsurance = liabilityAndInsurance;
            GoToFeedBackCommand = new DelegateCommand(GoToFeedBack);
            this.initDasbordDelegate = initDasbordDelegate;
            InitShipping();
        }

        private string email = null;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string IdShip { get; set; }

        public string IdVech { get; set; }

        public int StataLoadShip { get; set; }

        private bool isLoader = false;
        public bool Isloader
        {
            get => isLoader;
            set => SetProperty(ref isLoader, value);
        }

        private Shipping shipping = null;
        public Shipping Shipping
        {
            get => shipping;
            set => SetProperty(ref shipping, value);
        }

        private Photo sigPhoto = null;
        public Photo SigPhoto
        {
            get => sigPhoto;
            set => SetProperty(ref sigPhoto, value);
        }

        private Video videoRecount = null;
        public Video VideoRecount
        {
            get => videoRecount;
            set => SetProperty(ref videoRecount, value);
        }

        public string What_form_of_payment_are_you_using_to_pay_for_transportation { set; get; }
        public string CountPay { set; get; }

        [System.Obsolete]
        private async void InitShipping()
        {
            Isloader = true;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            Shipping shipping1 = null;
            //await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.GetShipping(token, IdShip, ref description, ref shipping1);
                });
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        await Navigation.PopAsync();
                    }
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                    HelpersView.CallError(description);
                }
                else if (state == 3)
                {
                    Shipping = shipping1;
                }
                else if (state == 4)
                {
                    if (Navigation.NavigationStack.Count > 1)
                    {
                        await Navigation.PopAsync();
                    }
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    HelpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
                StataLoadShip = 1;
            }
            Isloader = false;
        }

        [System.Obsolete]
        public async void SaveSigAndMethodPay()
        {
            Isloader = true;
            await PopupNavigation.PushAsync(new LoadPage());
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                //Task.Run(async () => await SaveRecountVideo());
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("AskPikedUpSig", token, IdShip, SigPhoto, ref description);
                    state = managerDispatchMob.SaveMethodPay(token, IdShip, What_form_of_payment_are_you_using_to_pay_for_transportation, CountPay, ref description);
                    initDasbordDelegate.Invoke();
                });
                await PopupNavigation.PopAsync();
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, Navigation));
                }
                else if (state == 3)
                {
                    if (What_form_of_payment_are_you_using_to_pay_for_transportation == "Cash")
                    {
                        await Navigation.PushAsync(new VideoCameraPage(this, ""));
                    }
                    else if (What_form_of_payment_are_you_using_to_pay_for_transportation == "Check")
                    {
                        await Navigation.PushAsync(new CameraPaymmant(this, "", "CheckPaymment.png"));
                    }
                    else
                    {
                        var isProblem = await CheckProplem();
                        await Navigation.PushAsync(new Ask2Page(this.managerDispatchMob, this.IdVech, this.IdShip, this.initDasbordDelegate, isProblem));
                    }
                    if (Navigation.NavigationStack.Count > 2)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[1]);
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.PaymmantMethodSaved);
                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, Navigation));
                }
            }
            else
            {
                //await PopupNavigation.PopAsync();
            }
            Isloader = true;
        }

        [System.Obsolete]
        public async void AddPhoto(byte[] photoResult)
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            Photo photo = new Photo();
            photo.Base64 = Convert.ToBase64String(photoResult);
            photo.path = $"../Photo/{IdVech}/Pay/DelyverySig.jpg";
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.SavePay("SaveSig", token, IdShip, 1, photo, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, Navigation));
                }
                else if (state == 3)
                {
                    if (Navigation.NavigationStack.Count > 2)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[1]);
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.PaymmantMethodSaved);
                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, Navigation));
                }
            }
        }

        [System.Obsolete]
        public async void SaveRecountVideo()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await PopupNavigation.PushAsync(new LoadPage());
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                if (videoRecount != null)
                {
                    await Task.Run(() => 
                    {
                        state = managerDispatchMob.SavePay("SaveRecount", token, IdShip, 1, VideoRecount, ref description);
                    });
                    //state = 3;
                    //TaskManager.CommandToDo("SaveRecount", 1, token, IdShip, 1, VideoRecount);
                }
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, Navigation));
                }
                else if (state == 3)
                {
                    if (Navigation.NavigationStack.Count > 2)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[1]);
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.VideoSavedSuccessfully);

                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, Navigation));
                }
                await PopupNavigation.PopAsync();
            }
            else
            {

            }
        }

        [System.Obsolete]
        public async Task SendLiabilityAndInsuranceEmaile()
        {
            GoEvaluationAndSurvey();
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("SendBolMail", token, IdShip, Email, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage($"{LanguageHelper.BOLIsSent} {Email}");
                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
        }

        [System.Obsolete]
        public async void GoEvaluationAndSurvey()
        {
            if (PopupNavigation.PopupStack.Count != 0)
            {
                await PopupNavigation.PopAsync(true);
            }
            await PopupNavigation.PushAsync(new EvaluationAndSurveyDialog(this, Navigation));
        }

        public async Task<bool> CheckProplem()
        {
            bool isProplem = false;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.CheckProblem(token, IdShip, ref isProplem);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {

                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
            return isProplem;
        }

        [System.Obsolete]
        private async void GoToFeedBack()
        {
            await PopupNavigation.PopAllAsync(true);
            await Navigation.PushAsync(new View.Inspection.Feedback(managerDispatchMob, Shipping.VehiclwInformations.FirstOrDefault(v => v.Id == IdVech), this));
        }

        public async void GoToContinue()
        {
            var isProblem = await CheckProplem();
            await Navigation.PushAsync(new Ask2Page(managerDispatchMob, IdVech, IdShip, initDasbordDelegate, isProblem));
            if (Navigation.NavigationStack.Count > 1)
            {
                Navigation.RemovePage(Navigation.NavigationStack[1]);
            }
        }

        public async void SetProblem()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.SetProblem(token, IdShip);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.FutureDispatcherProblem);
                }
                else if (state == 4)
                {
                    await PopupNavigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
        }
    }
}