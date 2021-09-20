

using System;
using System.ComponentModel;
using CoreGraphics;
using MDispatch.iOS.NewRender.Frame;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MDispatch.NewElement.FrameCusstomise.ShadowFrame), typeof(ShadowFrame))]
namespace MDispatch.iOS.NewRender.Frame
{
    public class ShadowFrame : FrameRenderer
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            NewElement.FrameCusstomise.ShadowFrame shadowFrame = Element as NewElement.FrameCusstomise.ShadowFrame;
            if (shadowFrame.HasShadow)
            {
                Layer.ShadowColor = shadowFrame.ColorShadow.ToCGColor();
                Layer.ShadowOffset = new CGSize(-3, 3);
                Layer.ShadowOpacity = shadowFrame.OpacityShadow;
                Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
                Layer.MasksToBounds = false;
                Layer.ShadowRadius = shadowFrame.ShadowRadius;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            NewElement.FrameCusstomise.ShadowFrame shadowFrame = Element as NewElement.FrameCusstomise.ShadowFrame;
            if (shadowFrame.HasShadow)
            {
                Layer.ShadowColor = shadowFrame.ColorShadow.ToCGColor();
                Layer.ShadowOffset = new CGSize(-3, 3);
                Layer.ShadowOpacity = shadowFrame.OpacityShadow;
                Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
                Layer.MasksToBounds = false;
                Layer.ShadowRadius = shadowFrame.ShadowRadius;
            }
        }
    }
}
