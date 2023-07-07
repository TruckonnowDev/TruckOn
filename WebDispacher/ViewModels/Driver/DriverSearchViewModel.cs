using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;

namespace WebDispacher.ViewModels.Driver
{
    public class DriverSearchViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "FirstNameRequired")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastNameRequired")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "LicenseNumberRequired")]
        public string LicenseNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}
