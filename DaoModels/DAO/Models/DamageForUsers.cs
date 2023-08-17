namespace DaoModels.DAO.Models
{
    public class DamageForUser
    {
        public int Id { get; set; }
        public string TypePrefDamage { get; set; }
        public string TypeDamage { get; set; }
        public string TypeCurrentStatus { get; set; }
        public int IndexDamage { get; set; }
        public string FullNameDamage { get; set; }
        public double XInterest { get; set; }
        public double YInterest { get; set; }
        public int HeightDamage { get; set; }
        public int WidthDamage { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
