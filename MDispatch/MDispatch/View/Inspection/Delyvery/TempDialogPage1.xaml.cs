using MDispatch.View.Inspection.PickedUp;
using MDispatch.ViewModels.InspectionMV.DelyveryMV;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection.Delyvery
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TempDialogPage1 : PopupPage
    {
        private AskForUsersDelyveryMW askForUsersDelyveryMW = null;

        public TempDialogPage1(AskForUsersDelyveryMW askForUsersDelyveryMW)
        {
            this.askForUsersDelyveryMW = askForUsersDelyveryMW;
            InitializeComponent();
        }
        
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
            if (askForUsersDelyveryMW.Payment == "COD" || askForUsersDelyveryMW.Payment == "COP" || askForUsersDelyveryMW.Payment == "Biling")
            {
                //askForUsersDelyveryMW.Continue();
                //await askForUsersDelyveryMW.Navigation.PopToRootAsync();
            }
            else
            {
                //await askForUsersDelyveryMW.Navigation.PushAsync(new CameraPaymmant(askForUsersDelyveryMW, ""));
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
            await Navigation.PushAsync(new EvaluationAndSurveyDialog1(askForUsersDelyveryMW));
        }
    }
}