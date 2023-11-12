using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class TruckType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int VehicleCategoryId { get; set; }
        public VehicleCategory VehicleCategory { get; set; }
    }
}