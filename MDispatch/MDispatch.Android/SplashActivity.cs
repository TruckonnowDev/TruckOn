using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;

namespace MDispatch.Droid
{
    [Activity(Label = "Truckonnow",
              Icon = "@mipmap/icon_truck",
              Theme = "@style/SplashTheme",
              ScreenOrientation = ScreenOrientation.SensorPortrait,
              MainLauncher = true)]
    public class SplashActivity: AppCompatActivity
    {
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();
        }
    }
}
