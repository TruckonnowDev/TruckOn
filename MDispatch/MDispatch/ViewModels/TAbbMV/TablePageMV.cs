using MDispatch.Service;
using MDispatch.Service.ManagerDispatchMob;
using Xamarin.Forms;

namespace MDispatch.ViewModels.TAbbPage
{
    public class TablePageMV : BaseViewModel
    {
        private readonly IManagerDispatchMobService managerDispatchMob;

        public TablePageMV(
            IManagerDispatchMobService managerDispatchMob,
            INavigation navigation)
            :base(navigation)
        {
            this.managerDispatchMob = managerDispatchMob;
        }
    }
}