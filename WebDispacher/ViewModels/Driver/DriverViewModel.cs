using System;
using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Driver
{
    public class DriverViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "FullNameRequired")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "EmailRequired")]
        public string EmailAddress { get; set; }
        
        [Required(ErrorMessage = "PasswordRequired")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "PhoneRequired")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "TrailerCapacityRequired")]
        public string TrailerCapacity { get; set; }
        
        [Required(ErrorMessage = "DriversLicenseNumberRequired")]
        public string DriversLicenseNumber { get; set; }
        public string DateRegistration { get; set; }
        public int CompanyId { get; set; }
    }
}