using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Payment
{
    public class CardViewModel
    {
        [Required(ErrorMessage = "NumberRequired")]
        [MaxLength(19, ErrorMessage = "MaxLengthNumber")]
        [MinLength(13, ErrorMessage = "MinLengthNumber")]
        [Display(Name = "Number")]
        public string Number { get; set; }
        
        [Required(ErrorMessage = "NameRequired")]
        [MaxLength(25, ErrorMessage = "MaxLengthName")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "ExpiryRequired")]
        [Display(Name = "Expiry")]
        public string Expiry { get; set; }
        
        [Required(ErrorMessage = "CVVRequired")]
        [MaxLength(4, ErrorMessage = "MaxLengthCVV")]
        [MinLength(3, ErrorMessage = "MinLengthCVV")]
        public string Cvc { get; set; }
    }
}