using DaoModels.DAO.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Driver
{
    public class CreateDriverViewModel
    {
        [Required(ErrorMessage = "FirstNameRequired")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "LastNameRequired")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "EmailRequired")]
        public string Email { get; set; }
        
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        [Required(ErrorMessage = "DriverLicenseNumberRequired")]
        public string DriverLicenseNumber { get; set; }

        [Required(ErrorMessage = "DateOfBirthRequired")]
        public DateTime DateOfBirth { get; set; }
        public DateTime DateRegistration { get; set; }
        public int DriverControlId { get; set; }
        public ShortDriverControlViewModel DriverControl { get; set; }
        public int CompanyId { get; set; }
    }
}