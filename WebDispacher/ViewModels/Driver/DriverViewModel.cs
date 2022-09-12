using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Driver
{
    public class DriverViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [Required]
        public string EmailAddress { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }
        
        [Required]
        public string TrailerCapacity { get; set; }
        
        [Required]
        public string DriversLicenseNumber { get; set; }
        public string DateRegistration { get; set; }
        public int CompanyId { get; set; }
    }
}