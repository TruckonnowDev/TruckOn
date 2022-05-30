using System.Threading.Tasks;

namespace MDispatch.Service.OpacityTouchView
{
    public interface IOpacityTouchViewService
    {
        Task TouchFeedBack(Xamarin.Forms.View view);
    }
}
