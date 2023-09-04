namespace DaoModels.DAO.Models
{
    public class Damage
    {
        public int Id { get; set; }
        public int PhotoVehicleInspectionId { get; set; }
        public PhotoVehicleInspection PhotoVehicleInspection { get; set; }
        public string IndexImageVech { get; set; }
        public string TypePrefDamage { get; set; }
        public string TypeDamage { get; set; }
        public string TypeCurrentStatus { get; set; }
        public int IndexDamage { get; set; }
        public string FullNameDamage { get; set; }
        public double XInterest { get; set; }
        public double YInterest { get; set; }
        public int HeightDamage { get; set; }
        public int WidthDamage { get; set; }
    }
}
