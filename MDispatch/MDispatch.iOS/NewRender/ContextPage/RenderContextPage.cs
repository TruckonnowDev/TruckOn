using System;
using FormsControls.Base;
using MDispatch.iOS.NewRender.ContextPage;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(AnimationNavigationPage), typeof(RenderContextPage))]
namespace MDispatch.iOS.NewRender.ContextPage
{
    public class RenderContextPage : NavigationRenderer
    {
        public RenderContextPage()
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
                    nb.BarBackgroundColor = Color.FromHex("#4fd2c2");
                    nb.BarTextColor = Color.FromHex("#7f2ed2");
                }

            }
        }
    }
}