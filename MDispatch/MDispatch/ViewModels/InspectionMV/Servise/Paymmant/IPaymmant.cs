using MDispatch.Models;
using MDispatch.View.Inspection.Delyvery;
using MDispatch.View.Inspection.PickedUp;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MDispatch.ViewModels.InspectionMV.Servise.Paymmant
{
    public interface IPaymmant
    {
        AskForUserDelyvery AskForUserDelyvery { get; set; }
        LiabilityAndInsurance LiabilityAndInsurance { get; set; }
        bool IsAskPaymmant { get; set; }
        StackLayout GetStackLayout();
        Task Pay();
    }
}
