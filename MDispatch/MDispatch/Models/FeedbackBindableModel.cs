using Prism.Mvvm;

namespace MDispatch.Models
{
    public class FeedbackBindableModel : BindableBase
    {
        private int _id;

        public int id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private int _how_Are_You_Satisfied_With_Service;
        public int How_Are_You_Satisfied_With_Service
        {
            get => _how_Are_You_Satisfied_With_Service;
            set => SetProperty(ref _how_Are_You_Satisfied_With_Service, value);
        }

        public string _would_You_Use_Our_Company_Again;
        public string Would_You_Use_Our_Company_Again
        {
            get => _would_You_Use_Our_Company_Again;
            set => SetProperty(ref _would_You_Use_Our_Company_Again, value);
        }

        public string _would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion;

        public string Would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion
        {
            get => _would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion;
            set => SetProperty(ref _would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion, value);
        }

        private int _how_did_the_driver_perform;
        public int How_did_the_driver_perform
        {
            get => _how_did_the_driver_perform;
            set => SetProperty(ref _how_did_the_driver_perform, value);
        }
    }
}
