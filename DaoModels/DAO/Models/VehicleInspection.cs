using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class VehicleInspection
    {
        public int Id { get; set; }
        public DateTime DateTimeInspection { get; set; }
        public int? VehicleId { get; set; }
        public VehicleDetails Vehicle { get; set; }
        public ICollection<PhotoVehicleInspection> PhotosVehicleInspection { get; set; }
    }
}
