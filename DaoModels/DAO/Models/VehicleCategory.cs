using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class VehicleCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TruckType> TruckTypes { get; set; }
    }
}