using System;

namespace DaoModels.DAO.Models
{
    public class DocumentDriver
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DocPath { get; set; }
        public int DriverId { get; set; }
        public Driver Driver { get; set; }
        public DateTime DateTimeUpload { get; set; }
    }
}
