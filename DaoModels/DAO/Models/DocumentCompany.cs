using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class DocumentCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DocPath { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime DateTimeUpload { get; set; }
    }
}
