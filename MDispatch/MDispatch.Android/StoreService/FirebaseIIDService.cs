using Android.App;
using Android.Content;
using Firebase.Iid;
using MDispatch.Droid.StoreService;
using MDispatch.StoreNotify;

[assembly: Xamarin.Forms.Dependency(typeof(FirebaseIIDService))]
namespace MDispatch.Droid.StoreService
{
    [Service]
    //[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService, IStore
    {
        public override void OnTokenRefresh()
        {
            ManagerStore managerStore = new ManagerStore();
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            managerStore.SendTokenStore(refreshedToken);
        }
    }
}