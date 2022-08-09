using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Settings
{
    public class SettingsUserViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        [Required]
        public string Login { get; set; }
        
        [Required]
        public string Password { get; set; }
        public string Date { get; set; }
        public string KeyAuthorized { get; set; }
    }
}