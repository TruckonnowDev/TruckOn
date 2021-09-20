using System;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text;
using MDispatch.Droid.NewrRender.TextInputRender;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(TextInputeRender))]
namespace MDispatch.Droid.NewrRender.TextInputRender
{
    [Obsolete]
    public class TextInputeRender: EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            Control?.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
    }
}
