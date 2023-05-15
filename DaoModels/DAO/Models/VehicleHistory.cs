using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class VehicleHistory
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Plate { get; set; }
        public string VIN { get; set; }
        public string Lot { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
