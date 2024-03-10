using System;
using System.Collections.Generic;

namespace DaoModels.DAO.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string OwnerOperator { get; set; }
        public string Language { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Login { get; set; }
        public string SocialSecurity { get; set; }
        public string Restrictions { get; set; }
        public string Endorsements { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactRelationship { get; set; }
        public int? EmergencyContactPhoneNumberId { get; set; }
        public PhoneNumber EmergencyContactPhoneNumber { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string DriverLicenseState { get; set; }
        public DateTime? DateDriverLicenseExpiration { get; set; }
        public DateTime? DateMedicalCardExpiration { get; set; }
        public DateTime? DateDriverLicenseIssued { get; set; }
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public string Email { get; set; }
        public int DriverControlId { get; set; }
        public DriverControl DriverControl { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public int? GeolocationId { get; set; }
        public Geolocation Geolocation { get; set; }
        public int? DriverReportId { get; set; }
        public DriverReport DriverReport { get; set; }
        public DateTime DateRegistration { get; set; }
        public DateTime DateLastUpdate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<DocumentDriver> Documents { get; set; }
        public ICollection<DriverInspection> Inspections { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
