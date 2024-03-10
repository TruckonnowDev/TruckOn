namespace DaoModels.DAO.Models
{
    public class DriverControl
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string IssuingStateProvince { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string TokenShope { get; set; }
        public bool IsInspectionDriver { get; set; }
        public bool IsInspectionToDayDriver { get; set; }
        public string LastDateInspaction { get; set; }
    }
}
