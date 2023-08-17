using System.Collections.Generic;

namespace WebDispacher.Models.Driver
{
    public class FullInformationInspection
    {
        public List<DaoModels.DAO.Models.Driver> Drivers { get; set; }
        public List<DaoModels.DAO.Models.Truck> Trucks { get; set; }
        public List<DaoModels.DAO.Models.Trailer> Trailers { get; set; }
        public List<InspectinView> Inspection { get; set; }
    }
}
