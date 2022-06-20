using MDispatch.Helpers;
using MDispatch.Models;
using MDispatch.NewElement.ToastNotify;
using MDispatch.Service;
using MDispatch.Service.GlobalHelper;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.Service.Utils;
using MDispatch.View;
using MDispatch.View.GlobalDialogView;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using Plugin.Settings;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.InspectionMV
{
    public class FeedBackMV : BaseViewModel
    {
        public readonly IManagerDispatchMobService managerDispatchMob;
        private object paymmpayMVInspactionant = null;
        private readonly IGlobalHelperService _globalHelper;
        private readonly IHelperViewService _helperView;

        public FeedBackMV(
            IManagerDispatchMobService managerDispatchMob, 
            VehiclwInformation vehiclwInformation, 
            INavigation navigation, object paymmpayMVInspactionant)
            :base(navigation)
        {
            _globalHelper = DependencyService.Get<IGlobalHelperService>();
            _helperView = DependencyService.Get<IHelperViewService>();
            this.paymmpayMVInspactionant = paymmpayMVInspactionant;
            this.managerDispatchMob = managerDispatchMob;
            VehiclwInformation = vehiclwInformation;
        }

        private MDispatch.Models.Feedback feedback = null;
        public MDispatch.Models.Feedback Feedback
        {
            get => feedback;
            set => SetProperty(ref feedback, value);
        }

        private VehiclwInformation vehiclwInformation = null;
        public VehiclwInformation VehiclwInformation
        {
            get => vehiclwInformation;
            set => SetProperty(ref vehiclwInformation, value);
        }

        private string email = null;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public async void SaveAsk()
        {
            await _popupNavigation.PushAsync(new LoadPage(), true);
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => _utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    managerDispatchMob.AskWork("SendCouponMail", token, null, Email, ref description);
                    state = managerDispatchMob.AskWork("FeedBack", token, null, Feedback, ref description);
                });
                await _popupNavigation.PopAsync(true);
                if (state == 1)
                {
                    _globalHelper.OutAccount();
                    await _popupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                    _helperView.CallError(description);
                }
                else if (state == 3)
                {
                    DependencyService.Get<IToast>().ShowMessage(LanguageHelper.FeedbackSaved);
                    if (paymmpayMVInspactionant is AskForUsersDelyveryMW)
                    {
                        await Navigation.PopAsync(true);
                    }
                    else
                    {
                        //if (((LiabilityAndInsuranceMV)paymmpayMVInspactionant).What_form_of_payment_are_you_using_to_pay_for_transportation == "Cash")
                        //{
                        //    await Navigation.PushAsync(new VideoCameraPage(((LiabilityAndInsuranceMV)paymmpayMVInspactionant), ""));
                        //}
                        //else if (((LiabilityAndInsuranceMV)paymmpayMVInspactionant).What_form_of_payment_are_you_using_to_pay_for_transportation == "Check")
                        //{
                        //    await Navigation.PushAsync(new CameraPaymmant(((LiabilityAndInsuranceMV)paymmpayMVInspactionant), "", "CheckPaymment.png"));
                        //}
                        //else
                        //{
                        //    await Navigation.PushAsync(new Ask2Page(((LiabilityAndInsuranceMV)paymmpayMVInspactionant).managerDispatchMob, ((LiabilityAndInsuranceMV)paymmpayMVInspactionant).IdVech, ((LiabilityAndInsuranceMV)paymmpayMVInspactionant).IdShip, ((LiabilityAndInsuranceMV)paymmpayMVInspactionant).initDasbordDelegate));
                        //}
                        await Navigation.PopAsync(true);
                    }
                }
            }
            else if (state == 4)
            {
                //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                _helperView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
            }
        }
    }
}
