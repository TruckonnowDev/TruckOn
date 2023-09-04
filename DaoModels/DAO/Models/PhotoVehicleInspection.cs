using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class PhotoVehicleInspection
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
        public int VehicleInspectionId { get; set; }
        public VehicleInspection VehicleInspection { get; set; }
        public int IndexPhoto { get; set; }
        public ICollection<Damage> Damages { get; set; }
    }
}
