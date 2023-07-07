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
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Position")]
        public string Position { get; set; }

        [Required(ErrorMessage = "ExtRequired")]
        [Display(Name = "Ext")]
        public string Ext { get; set; }

        [Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}