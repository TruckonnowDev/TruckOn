using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models.Settings
{
    public class TransportVehicle
    {
        public int Id { get; set; }
        public int CountPhoto { get; set; }
        public string Type { get; set; }
        public bool IsNextInspection { get; set; }
        public string TypeTransportVehicle { get; set; }
        public List<Layouts> Layouts { get; set; }
        public string PlateTruck { get; set; }
        public string PlateTraler { get; set; }
    }
}
