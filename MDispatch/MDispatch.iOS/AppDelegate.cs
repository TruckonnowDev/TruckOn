using System;
using System.Threading.Tasks;
using Firebase.CloudMessaging;
using Foundation;
using MDispatch.iOS;
using MDispatch.iOS.StoreService1;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xfx;

[assembly: Xamarin.Forms.Dependency(typeof(AppDelegate))]
namespace MDispatch.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        [System.Obsolete]
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();
            XfxControls.Init();
            Forms.Init();
            FormsControls.Touch.Main.Init();
            UIApplication.SharedApplication.StatusBarHidden = true;
            UITabBar.Appearance.BarTintColor = Color.FromHex("fff").ToUIColor();
            UITabBar.Appearance.TintColor = Color.FromHex("A1A1A1").ToUIColor();
            UITabBar.Appearance.SelectedImageTintColor = Color.FromHex("2C5DEB").ToUIColor();
            UITabBarItem.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("OpenSans-SemiBold", 12)
            }, UIControlState.Normal);
            UITabBarItem.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("OpenSans-Bold", 12),
                TextColor = UIColor.Black,
            }, UIControlState.Selected);


            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("OpenSans-Bold", 20),
            });

            Firebase.Core.App.Configure();
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound | UNAuthorizationOptions.CriticalAlert;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                    
                });
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
            Messaging.SharedInstance.Delegate = this;
            Messaging.SharedInstance.AutoInitEnabled = true;
            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }

        public UIInterfaceOrientationMask CurrentOrientation = UIInterfaceOrientationMask.All;

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            return CurrentOrientation;
        }
            
        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            Task.Run(() =>
            {
                FirebaseIIDService firebaseIIDService = new FirebaseIIDService();
                firebaseIIDService.OnTokenRefresh();
            });
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            UIApplicationState state = UIApplication.SharedApplication.ApplicationState;
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Alert);
        }
    }
}