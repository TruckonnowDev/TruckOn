using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class VehicleBrand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; }
        public ICollection<VehicleModel> VehicleModels { get; set; }
    }
}
