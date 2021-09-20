using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("MDispatch")]
[assembly: ExportEffect(typeof(MDispatch.iOS.NewRender.Effect.CornerRadius), nameof(MDispatch.iOS.NewRender.Effect.CornerRadius))]
namespace MDispatch.iOS.NewRender.Effect
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
                    Container.Layer.CornerRadius = effect.Radius;
                    Container.Layer.MasksToBounds = true;
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
    }
}
