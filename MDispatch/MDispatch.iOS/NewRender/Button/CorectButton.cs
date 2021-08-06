using MDispatch.iOS.NewRender.Button;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(CorectButton))]
namespace MDispatch.iOS.NewRender.Button
{
    public class CorectButton : ButtonRenderer
    {
        public CorectButton()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.WordWrap;
            }
        }
    
    }
}