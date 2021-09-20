using System;
using System.Collections.Generic;
using CoreGraphics;
using FormsControls.Base;
using MDispatch.iOS.NewRender.ContextPage;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(AnimationNavigationPage), typeof(RenderPageNavigation))]
namespace MDispatch.iOS.NewRender.ContextPage
{
    public class RenderPageNavigation : NavigationRenderer
    {
        public RenderPageNavigation()
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var nb = (AnimationNavigationPage)Element;
            if (InteractivePopGestureRecognizer != null)
            {
                InteractivePopGestureRecognizer.Enabled = false;
                if (nb != null)
                {
                    nb.BarBackgroundColor = Color.FromHex("#ffff");
                    nb.BarTextColor = Color.FromHex("#000000");
                }

            }
        }
    }
}