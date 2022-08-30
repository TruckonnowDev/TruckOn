namespace DaoModels.DAO.Models
{
    public class Feedback
    {
        public int id { get; set; }
        public string ShippingId { get; set; }
        public int DriverId { get; set; }
        public string How_Are_You_Satisfied_With_Service { get; set; }
        public string Would_You_Use_Our_Company_Again  { get; set; }
        public string Would_You_Like_To_Get_An_notification_If_We_Have_Any_Promotion { get; set; }
        public string How_did_the_driver_perform { get; set; }
    }
}