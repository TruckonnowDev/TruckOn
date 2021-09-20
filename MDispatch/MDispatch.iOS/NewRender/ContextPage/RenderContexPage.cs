using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using MDispatch.iOS.NewRender.ContextPage;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(ContentPage), typeof(RenderContexPage))]
namespace MDispatch.iOS.NewRender.ContextPage
{
    public class RenderContexPage : PageRenderer
    {
        public RenderContexPage()
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (NavigationController != null)
            {
                UIViewController vc = NavigationController.TopViewController;
                List<UIBarButtonItem> items = new List<UIBarButtonItem>();
                foreach (UIBarButtonItem item in vc.NavigationItem.RightBarButtonItems)
                {
                    if (item.Image != null)
                    {
                        UIButton btn = new UIButton(UIButtonType.Custom);
                        btn.SetImage(item.Image, UIControlState.Normal);

                        btn.Frame = new CGRect(0, 0, 23, 23);
                        btn.TouchUpInside += (sender, args) =>
                        {
                            item.Target.PerformSelector(item.Action, null, 0.0);
                        };

                        UIBarButtonItem customItem = new UIBarButtonItem(btn);
                        customItem.CustomView.WidthAnchor.ConstraintEqualTo(23).Active = true;
                        customItem.CustomView.HeightAnchor.ConstraintEqualTo(23).Active = true;
                        items.Add(customItem);
                    }
                    else
                    {
                        item.SetTitleTextAttributes(new UITextAttributes()
                        {
                            Font = UIFont.FromName("OpenSans-SemiBold", 16),
                        }, UIControlState.Normal);
                    }
                }
                //if(vc.NavigationItem.BackBarButtonItem != null)
                //{
                //    vc.NavigationItem.BackBarButtonItem.SetTitleTextAttributes(new UITextAttributes()
                //    {
                //        Font = UIFont.FromName("OpenSans-SemiBold", 16),
                //    }, UIControlState.Normal);
                //}
                if (items.Count != 0)
                {
                    vc.NavigationItem.RightBarButtonItems = items.ToArray();
                }

            }
            View.Frame = UIScreen.MainScreen.Bounds;
        }
    }
}
