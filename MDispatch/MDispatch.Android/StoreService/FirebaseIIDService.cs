using Android.App;
using Android.Content;
using Firebase.Iid;
using MDispatch.Droid.StoreService;
using MDispatch.Service.ManagerStore;
using MDispatch.Service.StoreNotify;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(FirebaseIIDService))]
namespace MDispatch.Droid.StoreService
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService, IStore
    {
        private readonly IManagerStore _managerStore;
        public FirebaseIIDService()
        {
            _managerStore = DependencyService.Get<IManagerStore>();
        }
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            _managerStore.SendTokenStore(refreshedToken);
        }
    }
}