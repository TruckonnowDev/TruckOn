using System;
using System.Threading.Tasks;
using MDispatch.NewElement.FeedBackHaptik;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MDispatch.Service.HelpersViews
{
    public class OpacityTouchView
    {
        public static async Task TouchFeedBack(Xamarin.Forms.View view)
        {
            await view.FadeTo(0.65, 300, Easing.SpringIn);
            switch (Device.RuntimePlatform)
            {
                case Device.iOS: DependencyService.Get<IHaptikFeedBack>().SelectionFeedbackGenerator();  break;
                case Device.Android: HapticFeedback.Perform(HapticFeedbackType.Click); break;
            }
            await view.FadeTo(1, 300, Easing.SpringOut);
        }
    }
}