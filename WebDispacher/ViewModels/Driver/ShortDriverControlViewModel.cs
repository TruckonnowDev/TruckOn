using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Driver
{
    public class ShortDriverControlViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        public string Password { get; set; }

        [Required(ErrorMessage = "TrailerCapacityRequired")]
        public string TrailerCapacity { get; set; }
    }
}
