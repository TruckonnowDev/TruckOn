using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class VehicleType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<VehicleBrand> VehicleBrands { get; set; }
    }
}
