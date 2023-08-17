using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;

namespace WebDispacher.ViewModels.Company
{
    public class CreateCompanyViewModel
    {
        [Required(ErrorMessage = "NameRequired")]
        public string Name { get; set; }
            
        [Required(ErrorMessage ="EmailRequired")]
        public string Email { get; set; }

        [Required(ErrorMessage ="PasswordRequired")]
        public string Password { get; set; }

        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        [Required(ErrorMessage = "CompanyTypeRequired")]
        public CompanyType CompanyType { get; set; }
    }
}
