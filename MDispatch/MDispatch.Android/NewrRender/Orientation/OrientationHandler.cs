using Android.App;
using Android.Content.PM;
using MDispatch.Droid.NewrRender.Orientation;
using MDispatch.NewElement;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(OrientationHandler))]
namespace MDispatch.Droid.NewrRender.Orientation
{
    public class OrientationHandler : IOrientationHandler
    {
        [System.Obsolete]
        public void ForceLandscape()
        {
            ((Activity)Forms.Context).RequestedOrientation = ScreenOrientation.Landscape;
        }

        [System.Obsolete]
        public bool IsForceLandscape()
        {
            return ((Activity)Forms.Context).RequestedOrientation == ScreenOrientation.Landscape;
        }

        [System.Obsolete]
        public void ForceSensor()
        {
            ((Activity)Forms.Context).RequestedOrientation = ScreenOrientation.FullSensor;
        }

        [System.Obsolete]
        public void ForcePortrait()
        {
            ((Activity)Forms.Context).RequestedOrientation = ScreenOrientation.Portrait;
        }
    }
}