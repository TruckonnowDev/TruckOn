using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Company
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        public string Name { get;set; }

        [Required(ErrorMessage = "PhoneRequired")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "EmailRequired")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Type")]
        public string CompanyType { get; set; }
    }
}
