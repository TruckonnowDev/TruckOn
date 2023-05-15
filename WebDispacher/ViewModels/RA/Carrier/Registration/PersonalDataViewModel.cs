using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.RA.Carrier.Registration
{
    public class PersonalDataViewModel
    {
        [Required(ErrorMessage = "EmailRequired")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FirstNameRequired")]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastNameRequired")]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "USDOTNumberRequired")]
        [Display(Name = "USDOTNumber")]
        public string USDOTNumber { get; set; }

        [Required(ErrorMessage = "CompanyNameRequired")]
        [Display(Name = "CompanyName")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "PhoneNumberRequired")]
        [MinLength(4, ErrorMessage = "MinLengthPhoneNumber")]
        [MaxLength(12, ErrorMessage = "MaxLengthPhoneNumber")]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
