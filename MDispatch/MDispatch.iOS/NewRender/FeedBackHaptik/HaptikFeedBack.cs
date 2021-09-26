using System.Threading.Tasks;
using MDispatch.iOS.NewRender.FeedBackHaptik;
using MDispatch.NewElement.FeedBackHaptik;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(HaptikFeedBack))]
namespace MDispatch.iOS.NewRender.FeedBackHaptik
{
    public class HaptikFeedBack : IHaptikFeedBack
    {
        public void SelectionFeedbackGenerator()
        {
            var selection = new UISelectionFeedbackGenerator();
            selection.Prepare();
            selection.SelectionChanged();
        }
    }
}
