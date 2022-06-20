using Xamarin.Forms;

namespace MDispatch.Service.HelperView
{
    public interface IHelperViewService
    {
        void CallError(string info);
        void Hidden();
        void InitAlert(FlexLayout flexLayout);
        void InitAlert(StackLayout stackLayout);
        void ReSet();
    }
}
