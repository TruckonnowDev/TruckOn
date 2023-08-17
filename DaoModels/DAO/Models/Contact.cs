using System;

namespace DaoModels.DAO.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public string Position { get; set; }
        public string Ext { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
    }
}
