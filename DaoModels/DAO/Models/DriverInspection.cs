using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class DriverInspection
    {
        public int Id { get; set; }
        public DateTime DateTimeInspection { get; set; }
        public int? DriverId { get; set; }
        public Driver Driver { get; set; }
        public int? TruckId { get; set; }
        public Truck Truck { get; set; }
        public int? TrailerId { get; set; }
        public Trailer Trailer { get; set; }
        public ICollection<PhotoDriverInspection> PhotosDriverInspection { get; set; }
    }
}
