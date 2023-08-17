using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class CompanyUser
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
