using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.RA.Carrier
{
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "PasswordRequired")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "ConfirmPasswordRequired")]
        [Compare("Password", ErrorMessage = "ConfirmPasswordNotCompare")]
        public string ConfirmPassword { get; set; }
    }
}
