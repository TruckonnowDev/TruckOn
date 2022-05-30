using MDispatch.Models;
using MDispatch.Service.HelperView;
using MDispatch.Service.ManagerDispatchMob;
using MDispatch.ViewModels.InspectionMV;
using Plugin.InputKit.Shared.Controls;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MDispatch.View.Inspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Feedback : ContentPage
	{
        FeedBackMV feedBackMV = null;
        private readonly IHelperViewService _helperView;
        public Feedback (IManagerDispatchMobService managerDispatchMob, VehiclwInformation vehiclwInformation, object payMVInspaction)
		{
            _helperView = DependencyService.Get<IHelperViewService>();
            feedBackMV = new FeedBackMV(managerDispatchMob, vehiclwInformation, Navigation, payMVInspaction);
			InitializeComponent ();
            BindingContext = feedBackMV;
            Init();
		}

        private void Init()
        {
            feedBackMV.Feedback = new Models.Feedback();
            feedBackMV.Feedback.How_Are_You_Satisfied_With_Service = "0";
            feedBackMV.Feedback.Would_You_Use_Our_Company_Again = "No";
            feedBackMV.Feedback.Would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion = "No";
            feedBackMV.Feedback.How_did_the_driver_perform = "0";
        }

        private void AdvancedSlider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "value")
            {
                feedBackMV.Feedback.How_Are_You_Satisfied_With_Service = ((AdvancedSlider)sender).Value.ToString();
            }
        }
        
        Button button4 = null;
        private void Button_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#4fd2c2");
            feedBackMV.Feedback.Would_You_Use_Our_Company_Again = button.Text;
            if (button4 != null)
            {
                button4.TextColor = Color.Silver;
            }
            button4 = button;
        }

        Button button1 = null;
        private void Button_Clicked1(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            button.TextColor = Color.FromHex("#4fd2c2");
            feedBackMV.Feedback.Would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion = button.Text;
            if (button1 != null)
            {
                button1.TextColor = Color.Silver;
            }
            button1 = button;
        }

        private void AdvancedSlider_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "value")
            {
                feedBackMV.Feedback.How_did_the_driver_perform = ((AdvancedSlider)sender).Value.ToString();
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            feedBackMV.SaveAsk();
        }

        [Obsolete]
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _helperView.InitAlert(body);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _helperView.Hidden();
        }
    }
}