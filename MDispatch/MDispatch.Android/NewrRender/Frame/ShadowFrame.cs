using System;
using Android.Content;
using Android.Graphics;
using AndroidX.CardView.Widget;
using MDispatch.Droid.NewrRender.Frame;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MDispatch.NewElement.FrameCusstomise.ShadowFrame), typeof(ShadowFrame))]
namespace MDispatch.Droid.NewrRender.Frame
{
    [Obsolete]
    public class ShadowFrame: Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer
    {
        public ShadowFrame(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);
            NewElement.FrameCusstomise.ShadowFrame shadowFrame = Element as NewElement.FrameCusstomise.ShadowFrame;
            if (shadowFrame == null)
            {
                return;
            }
            if (shadowFrame.HasShadow)
            {
                SetLayerType(Android.Views.LayerType.Software, null);
                Elevation = shadowFrame.CornerRadius * 3;
                CardElevation = shadowFrame.CornerRadius * 3;
                OffsetLeftAndRight(3);
                OffsetTopAndBottom(-3);
                SetOutlineSpotShadowColor(Android.Graphics.Color.ParseColor("#7291ff"));
            }
        }
    }
}
