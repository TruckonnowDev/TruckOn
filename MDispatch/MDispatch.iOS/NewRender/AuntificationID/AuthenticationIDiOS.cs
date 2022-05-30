using Foundation;
using LocalAuthentication;
using MDispatch.iOS.NewRender.AuntificationID;
using MDispatch.NewElement.AuntificationID;
using MDispatch.Service.GlobalHelper;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AuthenticationIDiOS))]
namespace MDispatch.iOS.NewRender.AuntificationID
{
    public class AuthenticationIDiOS : IAuntificationID
    {
        public string BiometryType { get; set; }
        public bool IsCanceleUI { get; set; }

        public void GetAuthenticationMethod()
        {
            var context = new LAContext();

            if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out var authError1))
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    context.LocalizedReason = "Authorize for access to secrets"; // iOS 11
                    BiometryType = context.BiometryType == LABiometryType.TouchId ? "Touch ID" : "Face ID";
                }
            }

            // Is pin login possible?
            else if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out var authError2))
            {
                BiometryType = "Device PIN";
            }

            // Local authentication not possible
            else
            {
                BiometryType = "none";
            }
        }


        public void Authentication()
        {
            LAContext context = new LAContext();
            NSError AuthError;
            NSString localizedReason = new NSString("To access secrets");

            // Because LocalAuthentication APIs have been extended over time,
            // you must check iOS version before setting some properties

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                BiometryType = context.BiometryType == LABiometryType.TouchId ? "TouchID" : "FaceID";
            }
            //
            if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out AuthError))
            {
                LAContextReplyHandler replyHandler = new LAContextReplyHandler((success, error) =>
                {
                    IsCanceleUI = false;
                    if (!success && error.Code == -2)
                    {
                        IGlobalHelperService globalHelperService = DependencyService.Get<IGlobalHelperService>();
                        Device.BeginInvokeOnMainThread(() => { globalHelperService.OutAccount(); });
                    }
                    else if (!success && error.Code == -4)
                    {
                        IsCanceleUI = true;
                    }
                    else if (!success)
                    {
                        CallPINPanel(localizedReason, context);
                    }
                });
                context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, localizedReason, replyHandler);
            }
            else if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, out AuthError))
            {
                CallPINPanel(localizedReason, context);
            }
        }

        public void CallPINPanel(NSString localizedReason, LAContext context)
        {
            LAContextReplyHandler replyHandler = new LAContextReplyHandler((success, error) =>
            {
                IsCanceleUI = false;
                if (!success && error.Code == -4)
                {
                    IsCanceleUI = true;
                }
                else if(!success && error.Code != -4)
                {
                    IGlobalHelperService globalHelperService = DependencyService.Get<IGlobalHelperService>();
                    Device.BeginInvokeOnMainThread(() => { globalHelperService.OutAccount(); });
                }
            });
            context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthentication, localizedReason, replyHandler);
        }
    }
}