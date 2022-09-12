using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Payment
{
    public class CardViewModel
    {
        [Required]
        [MaxLength(19)]
        [MinLength(13)]
        public string Number { get; set; }
        
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        
        [Required]
        public string Expiry { get; set; }
        
        [Required]
        [MaxLength(4)]
        [MinLength(3)]
        public string Cvc { get; set; }
    }
}