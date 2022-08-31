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
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using Plugin.Settings;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.InspectionMV
{
    public class FeedBackMV : BindableBase
    {
        public ManagerDispatchMob managerDispatchMob = null;
        public INavigation Navigation { get; set; }
        private object paymmpayMVInspactionant = null;

        public FeedBackMV(ManagerDispatchMob managerDispatchMob, 
            VehiclwInformation vehiclwInformation, 
            INavigation navigation, 
            object paymmpayMVInspactionant,
            Models.Feedback feedback = null)
        {
            if (feedback != null &&
                feedback.id != 0)
            {
                Feedback = new Models.Feedback
                {
                    How_Are_You_Satisfied_With_Service = feedback.How_Are_You_Satisfied_With_Service,
                    How_did_the_driver_perform = feedback.How_did_the_driver_perform,
                    id = feedback.id,
                    Would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion = feedback.Would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion,
                    Would_You_Use_Our_Company_Again = feedback.Would_You_Use_Our_Company_Again,
                };
                HasFeedback = true;
            }
            this.paymmpayMVInspactionant = paymmpayMVInspactionant;
            this.managerDispatchMob = managerDispatchMob;
            Navigation = navigation;
            VehiclwInformation = vehiclwInformation;
        }

        private MDispatch.Models.Feedback feedback = null;
        public MDispatch.Models.Feedback Feedback
        {
            get => feedback;
            set => SetProperty(ref feedback, value);
        }

        private bool _hasFeedback;
        public bool HasFeedback
        {
            get => _hasFeedback;
            set => SetProperty(ref _hasFeedback, value);
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

        [System.Obsolete]
        public async void SaveAsk()
        {
            await PopupNavigation.PushAsync(new LoadPage(), true);
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    if (paymmpayMVInspactionant is LiabilityAndInsuranceMV)
                    {
                        managerDispatchMob.AskWork("SendCouponMail", token, null, Email, ref description);
                        state = managerDispatchMob.AskWork("FeedBack", token, null, Feedback, ref description, (paymmpayMVInspactionant as LiabilityAndInsuranceMV).IdShip);
                    }
                    else if(paymmpayMVInspactionant is AskForUsersDelyveryMW)
                    {
                        managerDispatchMob.AskWork("SendCouponMail", token, null, Email, ref description);
                        state = managerDispatchMob.AskWork("FeedBack", token, null, Feedback, ref description, (paymmpayMVInspactionant as AskForUsersDelyveryMW).IdShip);
                    }
                    else
                    {
                        throw new System.Exception();
                    }
                });
                await PopupNavigation.PopAsync(true);
                if (state == 1)
                {
                    GlobalHelper.OutAccount();
                    await PopupNavigation.PushAsync(new Alert(description, null));
                }
                if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                    HelpersView.CallError(description);
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
                HelpersView.CallError(LanguageHelper.TechnicalWorkServiceAlert);
            }
        }
    }
}
