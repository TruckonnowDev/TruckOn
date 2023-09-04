using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaoModels.DAO.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<CompanyUser> CompanyUsers { get; set; }
        public DateTime DateTimeRegistration { get; set; }
        public DateTime? DateTimeLastUpdate { get; set; }
        public DateTime? DateTimeLastLogin { get; set; }
    }
}
