using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Settings
{
    public class SettingsUserViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        [Required(ErrorMessage = "LoginRequired")]
        [Display(Name = "Login")]
        public string Login { get; set; }
        
        [Required(ErrorMessage = "PasswordRequired")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [Display(Name = "Date")]
        public string Date { get; set; }
        public string KeyAuthorized { get; set; }
    }
}