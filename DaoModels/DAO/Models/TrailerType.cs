namespace DaoModels.DAO.Models
{
    public class TrailerType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int VehicleCategoryId { get; set; }
        public VehicleCategory VehicleCategory { get; set; }
    }
}