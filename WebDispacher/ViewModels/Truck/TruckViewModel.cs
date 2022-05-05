using DaoModels.DAO.Interface;

namespace WebDispacher.ViewModels.Truck
{
    public class TruckViewModel : ITr
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string NameTruck { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string State { get; set; }
        public string Exp { get; set; }
        public string Vin { get; set; }
        public string Owner { get; set; }
        public string PlateTruck { get; set; }
        public string ColorTruck { get; set; }
        public string Type { get; set; }
    }
}