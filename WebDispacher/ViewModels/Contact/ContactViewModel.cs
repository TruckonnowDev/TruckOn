using System.ComponentModel.DataAnnotations;
using WebDispacher.ViewModels.Order;

namespace WebDispacher.ViewModels.Contact
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailRequired")]
        public string Email { get; set; }

        public int? PhoneNumberId { get; set; }
        public EditPhoneNumberViewModel PhoneNumber { get; set; }

        [Display(Name = "Position")]
        public string Position { get; set; }

        [Display(Name = "Ext")]
        [Required(ErrorMessage = "ExtRequired")]
        public string Ext { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "NameRequired")]
        public string Name { get; set; }
    }
}