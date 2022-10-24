using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Contact
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        [Required(ErrorMessage = "EmailRequired")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PhoneRequired")]
        [MinLength(4, ErrorMessage = "MinLengthPhone")]
        [MaxLength(12, ErrorMessage = "MaxLengthPhone")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}