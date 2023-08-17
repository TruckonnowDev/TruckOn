using System;

namespace DaoModels.DAO.Models
{
    public class DocumentTrailer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DocPath { get; set; }
        public int TrailerId { get; set; }
        public Trailer Trailer { get; set; }
        public DateTime DateTimeUpload { get; set; }
    }
}
