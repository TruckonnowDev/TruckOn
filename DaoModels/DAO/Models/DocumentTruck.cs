using System;

namespace DaoModels.DAO.Models
{
    public class DocumentTruck
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DocPath { get; set; }
        public int TruckId { get; set; }
        public Truck Truck { get; set; }
        public DateTime DateTimeUpload { get; set; }
    }
}
