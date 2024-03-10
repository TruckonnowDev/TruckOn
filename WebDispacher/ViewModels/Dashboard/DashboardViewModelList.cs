using DaoModels.DAO.Models;
using System.Collections.Generic;
using WebDispacher.ViewModels.Truck;

namespace WebDispacher.ViewModels.Dashboard
{
    public class DashboardViewModelList
    {
        public List<TruckStatusWidget> TruckWidgets { get; set; }
        public List<TrailerStatusWidget> TrailerWidgets { get; set; }
        public List<OrderStatusWidget> OrderWidgets { get; set; }
        public List<DaoModels.DAO.Models.Driver> DriversExpirationMedicalCard { get; set; }
        public List<DaoModels.DAO.Models.Driver> DriversExpirationDriverLicense { get; set; }
        public List<DaoModels.DAO.Models.Truck> TrucksExpirationPlate { get; set; }
        public List<DaoModels.DAO.Models.Truck> TrucksExpirationLastInspection { get; set; }
        public List<DaoModels.DAO.Models.Trailer> TrailersExpirationPlate { get; set; }
        public List<DaoModels.DAO.Models.Trailer> TrailersExpirationLastInspection { get; set; }
    }
}