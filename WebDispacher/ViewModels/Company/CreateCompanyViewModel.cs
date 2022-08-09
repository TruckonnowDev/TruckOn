using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Enum;

namespace WebDispacher.ViewModels.Company
{
    public class CreateCompanyViewModel
        {
            [Required]
            public string Name { get; set; }
            
            [Required]
            public string Email { get; set; }
        }
}
