﻿namespace DaoModels.DAO.Models
{
    public class Contact
    {
        public int ID { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
    }
}