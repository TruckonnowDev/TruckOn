using MDispatch.ViewModels.InspectionMV.PickedUpMV;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection.PickedUp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EvaluationAndSurveyDialog : PopupPage
    {
        LiabilityAndInsuranceMV liabilityAndInsuranceMV = null;
        INavigation Navigation1 = null;

        public EvaluationAndSurveyDialog (LiabilityAndInsuranceMV liabilityAndInsuranceMV, INavigation navigation)
		{
            Navigation1 = navigation;
            this.liabilityAndInsuranceMV = liabilityAndInsuranceMV;
			InitializeComponent ();
            BindingContext = this.liabilityAndInsuranceMV;
		}

        private async void Button_Clicked_1(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync(true);
        }
    }
}