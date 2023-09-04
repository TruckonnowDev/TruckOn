using DaoModels.DAO.Models;
using System.Collections.Generic;

namespace WebDispacher.ViewModels.Vehicles
{
    public class VehicleModelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int VehicleBrandId { get; set; }
        public int VehicleBodyId { get; set; }
    }
}
