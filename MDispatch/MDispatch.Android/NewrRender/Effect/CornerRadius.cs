using System.Linq;
using Android.Graphics;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("MDispatch")]
[assembly: ExportEffect(typeof(MDispatch.Droid.NewrRender.Effect.CornerRadius), nameof(MDispatch.Droid.NewrRender.Effect.CornerRadius))]
namespace MDispatch.Droid.NewrRender.Effect
{
    public class CornerRadius : PlatformEffect
    {
        public CornerRadius()
        {
        }

        protected override void OnAttached()
        {
            try
            {
                var effect = (NewElement.Effect.CornerRadius)Element.Effects.FirstOrDefault(e => e is NewElement.Effect.CornerRadius);
                if (effect != null)
                {
                    Container.ClipToOutline = true;
                    Container.OutlineProvider = new RoundedOutlineProvider(effect.Radius);
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Cannot set property on attached control. Error: {ex.Message}");
            }
        }

        protected override void OnDetached()
        {

        }

        private class RoundedOutlineProvider : ViewOutlineProvider
        {
            private readonly float radius;
            public RoundedOutlineProvider(float radius)
            {
                this.radius = radius;
            }
            public override void GetOutline(Android.Views.View view, Outline outline)
            {
                outline?.SetRoundRect(0, 0, view.Width, view.Height, radius*2);
            }
        }
    }
}
