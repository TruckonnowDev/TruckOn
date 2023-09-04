using DaoModels.DAO.Models;
using System.Collections.Generic;

namespace WebDispacher.ViewModels.Vehicles
{
    public class VehicleBrandViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int VehicleTypeId { get; set; }
    }
}
