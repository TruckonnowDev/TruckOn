using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int VehicleBrandId { get; set; }
        public VehicleBrand VehicleBrand { get; set; }
        public int VehicleBodyId { get; set; }
        public VehicleBody VehicleBody { get; set; }
        public ICollection<VehicleDetails> Vehicles { get; set; }
        public ICollection<Truck> Trucks { get; set; }
        public ICollection<Trailer> Trailers { get; set; }
    }
}
