using System;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using FormsControls.Base;
using MDispatch.Droid.NewrRender.PageRenders;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(AnimationNavigationPage), typeof(NavigationPageRenderer))]
namespace MDispatch.Droid.NewrRender.PageRenders
{
    [Obsolete]
    public class NavigationPageRenderer: Xamarin.Forms.Platform.Android.AppCompat.NavigationPageRenderer
    {
        private Android.Support.V7.Widget.Toolbar toolbar;

        public override void OnViewAdded(Android.Views.View child)
        {
            var context = (Activity)Xamarin.Forms.Forms.Context;
            base.OnViewAdded(child);
            if (child.GetType() == typeof(Android.Support.V7.Widget.Toolbar))
            {
                toolbar = (Android.Support.V7.Widget.Toolbar)child;
                toolbar.ChildViewAdded += Toolbar_ChildViewAdded;
            }
        }

        private void Toolbar_ChildViewAdded(object sender, ChildViewAddedEventArgs e)
        {
            var view = e.Child.GetType();
            if (e.Child.GetType() == typeof(AndroidX.AppCompat.Widget.AppCompatTextView))
            {
                var textView = (Android.Widget.TextView)e.Child;
                textView.SetPadding(30, 0, 0, 0);
                textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 20);
                textView.SetTextColor(Android.Graphics.Color.Black);
                var spaceFont = Typeface.CreateFromAsset(Xamarin.Forms.Forms.Context.ApplicationContext.Assets, "OpenSans-Bold.ttf");
                textView.Typeface = spaceFont;
                toolbar.ChildViewAdded -= Toolbar_ChildViewAdded;
            }
        }
    }
}
