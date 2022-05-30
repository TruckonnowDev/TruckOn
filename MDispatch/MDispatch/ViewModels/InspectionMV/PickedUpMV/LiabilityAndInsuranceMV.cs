using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.View.Inspection;
using MDispatch.View.Inspection.PickedUp;
using Plugin.Settings;
using Prism.Commands;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MDispatch.Service.ManagerDispatchMob.ManagerDispatchMobService;

namespace MDispatch.ViewModels.InspectionMV.PickedUpMV
{
    public class LiabilityAndInsuranceMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        public DelegateCommand GoToFeedBackCommand { get; set; }
        public InitDasbordDelegate initDasbordDelegate = null;
        public LiabilityAndInsurance liabilityAndInsurance = null;
        private readonly IHelperViewService _helperView;

        public LiabilityAndInsuranceMV(
            IManagerDispatchMobService managerDispatchMob, 
            string idVech, string idShip, 
            INavigation navigation, 
            InitDasbordDelegate initDasbordDelegate,
            LiabilityAndInsurance liabilityAndInsurance)
            : base(navigation)
        {
            _helperView = DependencyService.Get<IHelperViewService>();
            this.managerDispatchMob = managerDispatchMob;
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
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    if (_navigation.NavigationStack.Count > 1)
                    {
                        await _navigation.PopAsync();
                    }
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    Shipping = shipping1;
                }
                else if (state == 4)
                {
                    if (_navigation.NavigationStack.Count > 1)
                    {
                        await _navigation.PopAsync();
                    }
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                    _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
                }
                StataLoadShip = 1;
            }
            Isloader = false;
        }

        public async void SaveSigAndMethodPay()
        {
            Isloader = true;
            await _navigation.PushAsync(new LoadPage());
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                //Task.Run(async () => await SaveRecountVideo());
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("AskPikedUpSig", token, IdShip, SigPhoto, ref description);
                    state = managerDispatchMob.SaveMethodPay(token, IdShip, What_form_of_payment_are_you_using_to_pay_for_transportation, CountPay, ref description);
                    initDasbordDelegate.Invoke();
                });
                await _navigation.PopAsync();
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await _navigation.PushAsync(new Alert(description, _navigation));
                }
                else if (state == 3)
                {
                    if (What_form_of_payment_are_you_using_to_pay_for_transportation == "Cash")
                    {
                        await _navigation.PushAsync(new VideoCameraPage(this, ""));
                    }
                    else if (What_form_of_payment_are_you_using_to_pay_for_transportation == "Check")
                    {
                        await _navigation.PushAsync(new CameraPaymmant(this, "", "CheckPaymment.png"));
                    }
                    else
                    {
                        await _navigation.PushAsync(new Ask2Page(this.managerDispatchMob, this.IdVech, this.IdShip, this.initDasbordDelegate));
                    }
                    if (_navigation.NavigationStack.Count > 2)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[1]);
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.PaymmantMethodSaved);
                }
                else if (state == 4)
                {
                    await _navigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, _navigation));
                }
            }
            else
            {
                //await PopupNavigation.PopAsync();
            }
            Isloader = true;
        }

        public async void AddPhoto(byte[] photoResult)
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            Photo photo = new Photo();
            photo.Base64 = Convert.ToBase64String(photoResult);
            photo.path = $"../Photo/{IdVech}/Pay/DelyverySig.jpg";
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.SavePay("SaveSig", token, IdShip, 1, photo, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await _navigation.PushAsync(new Alert(description, _navigation));
                }
                else if (state == 3)
                {
                    if (_navigation.NavigationStack.Count > 2)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[1]);
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.PaymmantMethodSaved);
                }
                else if (state == 4)
                {
                    await _navigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, _navigation));
                }
            }
        }

        public async void SaveRecountVideo()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await _navigation.PushAsync(new LoadPage());
            await Task.Run(() => _utils.CheckNet());
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
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await _navigation.PushAsync(new Alert(description, _navigation));
                }
                else if (state == 3)
                {
                    if (_navigation.NavigationStack.Count > 2)
                    {
                        _navigation.RemovePage(_navigation.NavigationStack[1]);
                    }
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.VideoSavedSuccessfully);

                }
                else if (state == 4)
                {
                    await _navigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, _navigation));
                }
                await _navigation.PopAsync();
            }
            else
            {

            }
        }

        public async Task SendLiabilityAndInsuranceEmaile()
        {
            GoEvaluationAndSurvey();
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.AskWork("SendBolMail", token, IdShip, Email, ref description);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await _navigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage($"{LanguageHelper.BOLIsSent} {Email}");
                }
                else if (state == 4)
                {
                    await _navigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
        }

        public async void GoEvaluationAndSurvey()
        {
            if (_navigation.NavigationStack.Count != 0)
            {
                await _navigation.PopAsync(true);
            }
            await _navigation.PushAsync(new EvaluationAndSurveyDialog(this, _navigation));
        }

        public async Task<bool> CheckProplem()
        {
            bool isProplem = false;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.CheckProblem(token, IdShip, ref isProplem);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await _navigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {

                }
                else if (state == 4)
                {
                    await _navigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
            return isProplem;
        }

        private async void GoToFeedBack()
        {
            await _navigation.PopToRootAsync(true);
            await _navigation.PushAsync(new View.Inspection.Feedback(managerDispatchMob, Shipping.VehiclwInformations.FirstOrDefault(v => v.Id == IdVech), this));
        }

        public async void GoToContinue()
        {
            await _navigation.PushAsync(new Ask2Page(managerDispatchMob, IdVech, IdShip, initDasbordDelegate));
            if (_navigation.NavigationStack.Count > 1)
            {
                _navigation.RemovePage(_navigation.NavigationStack[1]);
            }
        }

        public async void SetProblem()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerDispatchMob.SetProblem(token, IdShip);
                    initDasbordDelegate.Invoke();
                });
                if (state == 1)
                {
                    _globalHelperService.OutAccount();
                    await _navigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    await _navigation.PushAsync(new Alert(description, null));
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.FutureDispatcherProblem);
                }
                else if (state == 4)
                {
                    await _navigation.PushAsync(new Alert(LanguageHelper.TechnicalWorkServiceAlert, null));
                }
            }
        }
    }
}