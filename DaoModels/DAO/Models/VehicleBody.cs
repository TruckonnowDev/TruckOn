using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class VehicleBody
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<VehicleModel> VehicleModels { get; set; }
    }
}
