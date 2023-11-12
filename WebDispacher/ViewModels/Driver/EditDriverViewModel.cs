using DaoModels.DAO.Models;
using System;
using System.ComponentModel.DataAnnotations;
using WebDispacher.Attributes;

namespace WebDispacher.ViewModels.Driver
{
    public class EditDriverViewModel
    {
        public int Id { get; set; }

        [History]
        [Required(ErrorMessage = "FirstNameRequired")]
        public string FirstName { get; set; }

        [History]
        [Required(ErrorMessage = "LastNameRequired")]
        public string LastName { get; set; }

        [History]
        [Required(ErrorMessage = "EmailRequired")]
        public string Email { get; set; }

        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public int DriverControlId { get; set; }
        public ShortDriverControlViewModel DriverControl { get; set; }

        [History]
        [Required(ErrorMessage = "DriverLicenseNumberRequired")]
        public string DriverLicenseNumber { get; set; }

        [History]
        public DateTime DateOfBirth { get; set; }
    }
}
