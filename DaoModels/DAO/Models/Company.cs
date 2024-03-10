using DaoModels.DAO.Enum;
using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class Company
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public int USDOT { get; set; }
        public CompanyType CompanyType { get; set; }
        public CompanyStatus CompanyStatus { get; set; }
        public DateTime DateTimeRegistration { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
        public ICollection<DocumentCompany> Documents { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Driver> Drivers { get; set; }
        public ICollection<Trailer> Trailers { get; set; }
        public ICollection<TruckGroup> TruckGroups { get; set; }
        public ICollection<PaymentMethod> PaymentMethods { get; set; }
        public ICollection<CustomerST> CustomerSTs { get; set; }
        public ICollection<CompanyUser> CompanyUsers { get; set; }
    }
}
