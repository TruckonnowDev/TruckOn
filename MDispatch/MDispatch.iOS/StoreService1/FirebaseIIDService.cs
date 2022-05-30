using Firebase.CloudMessaging;
using MDispatch.iOS.StoreService1;
using MDispatch.Service.ManagerStore;
using MDispatch.Service.StoreNotify;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(FirebaseIIDService))]
namespace MDispatch.iOS.StoreService1
{
    public class FirebaseIIDService : IStore
    {
        private readonly IManagerStore _managerStore;
        public FirebaseIIDService()
        {
            _managerStore = DependencyService.Get<IManagerStore>();
        }

        public void OnTokenRefresh()
        {
            _managerStore.SendTokenStore(Messaging.SharedInstance.FcmToken);
        }
    }
}
