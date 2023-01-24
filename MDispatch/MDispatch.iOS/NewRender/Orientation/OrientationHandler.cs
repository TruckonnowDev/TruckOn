using System;
using Foundation;
using MDispatch.iOS.NewRender.Orientation;
using MDispatch.NewElement;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(OrientationHandler))]    
namespace MDispatch.iOS.NewRender.Orientation
{
    public class OrientationHandler : IOrientationHandler
    {
        public void ForceLandscape()
        {
            ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentOrientation = UIInterfaceOrientationMask.LandscapeRight;
            UIDevice.CurrentDevice.SetValueForKey(NSNumber.FromNInt((int)(UIInterfaceOrientation.LandscapeRight)), new NSString("orientation"));
        }

        public bool IsForceLandscape()
        {
            return ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentOrientation == UIInterfaceOrientationMask.LandscapeRight;
        }

        public void ForcePortrait()
        {
            ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentOrientation = UIInterfaceOrientationMask.Portrait;
            UIDevice.CurrentDevice.SetValueForKey(NSNumber.FromNInt((int)(UIInterfaceOrientation.Portrait)), new NSString("orientation")); 
        }

        public void ForceSensor()
        {
            ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentOrientation = UIInterfaceOrientationMask.All;
            UIDevice.CurrentDevice.SetValueForKey(NSNumber.FromNInt((int)(UIInterfaceOrientation.Portrait)), new NSString("orientation"));
        }
    }
}
