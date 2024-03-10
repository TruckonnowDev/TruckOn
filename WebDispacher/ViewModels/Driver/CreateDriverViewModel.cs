using DaoModels.DAO.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using WebDispacher.Attributes;

namespace WebDispacher.ViewModels.Driver
{
    public class CreateDriverViewModel
    {
        //[Required(ErrorMessage = "FirstNameRequired")]
        public string FirstName { get; set; }
        
        //[Required(ErrorMessage = "LastNameRequired")]
        public string LastName { get; set; }
        
        //[Required(ErrorMessage = "EmailRequired")]
        public string Email { get; set; }

        public string OwnerOperator { get; set; }
        public string CompanyName { get; set; }
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

        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        //[Required(ErrorMessage = "DriverLicenseNumberRequired")]
        public string DriverLicenseNumber { get; set; }

        //[Required(ErrorMessage = "DriverLicenseStateRequired")]
        public string DriverLicenseState { get; set; }

        //[Required(ErrorMessage = "DateDriverLicenseExpirationRequired")]
        [BindNever]
        public DateTime? DateDriverLicenseExpiration { get; set; }
        [BindNever]
        public DateTime? DateDriverLicenseIssued { get; set; }
        [BindNever]
        //[Required(ErrorMessage = "DateMedicalCardExpirationRequired")]
        public DateTime? DateMedicalCardExpiration { get; set; }
        [BindNever]
        //[Required(ErrorMessage = "DateOfBirthRequired")]
        public DateTime DateOfBirth { get; set; }
        public DateTime DateRegistration { get; set; }
        public int DriverControlId { get; set; }
        public ShortDriverControlViewModel DriverControl { get; set; }
        public int CompanyId { get; set; }
    }
}