using DaoModels.DAO.Enum;

namespace DaoModels.DAO.Models
{
    public class VehicleDetails
    {
        public int Id { get; set; }
        public int? VehicleModelId { get; set; }
        public VehicleModel VehicleModel { get; set; }
        public string Color { get; set; }
        public string Plate { get; set; }
        public string VIN { get; set; }
        public string Lot { get; set; }
        public string AdditionalInfo { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
