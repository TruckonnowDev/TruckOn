﻿using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Enum;

namespace WebDispacher.ViewModels.Company
{
    public class CreateCompanyViewModel
        {
            [Required(ErrorMessage = "NameRequired")]
            public string Name { get; set; }
            
            [Required(ErrorMessage ="EmailRequired")]
            public string Email { get; set; }
        }
}
