
using Xamarin.Forms;

namespace MDispatch.NewElement.Effect
{
    public class CornerRadius : RoutingEffect
    {
        public CornerRadius()
      : base($"MDispatch.{nameof(CornerRadius)}")
        {
        }

        public float Radius { get; set; }
    }
}
