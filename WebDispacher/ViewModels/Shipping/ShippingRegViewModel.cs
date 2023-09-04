using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Shipping
{
    public class ShippingRegViewModel
    {
        [Required(ErrorMessage = "FirstNameRequired")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastNameRequired")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "CompanyNameRequired")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "EmailRequired")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneNumberRequired")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "CarsPerMonthRequired")]
        public string CarsPerMonth { get; set; }

        public string HowFoundAboutUs { get; set; }
    }
}
