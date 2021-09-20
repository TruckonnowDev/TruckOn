using System;
using Xamarin.Forms;

namespace MDispatch.NewElement.FrameCusstomise
{
    public class ShadowFrame : Frame
    {
        public static readonly BindableProperty OpacityShadowProperty =
        BindableProperty.Create("OpacityShadow", typeof(float), typeof(ShadowFrame), defaultBindingMode: BindingMode.OneWay);
        public float OpacityShadow
        {
            get { return (float)GetValue(OpacityShadowProperty); }
            set { SetValue(OpacityShadowProperty, value); }
        }

        public static readonly BindableProperty ColorShadowProperty =
        BindableProperty.Create("ColorShadow", typeof(Color), typeof(ShadowFrame), defaultBindingMode: BindingMode.OneWay);
        public Color ColorShadow
        {
            get { return (Color)GetValue(ColorShadowProperty); }
            set { SetValue(ColorShadowProperty, value); }
        }

        public static readonly BindableProperty ShadowRadiusProperty =
        BindableProperty.Create("ShadowRadius", typeof(float), typeof(ShadowFrame), defaultBindingMode: BindingMode.OneWay);
        public float ShadowRadius
        {
            get { return (float)GetValue(ShadowRadiusProperty); }
            set { SetValue(ShadowRadiusProperty, value); }
        }
    }
}
