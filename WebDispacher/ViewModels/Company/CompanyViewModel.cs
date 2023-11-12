using DaoModels.DAO.Enum;
using DaoModels.DAO.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebDispacher.ViewModels.Order;

namespace WebDispacher.ViewModels.Company
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "NameRequired")]
        public string Name { get;set; }

        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailRequired")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        //[Required(ErrorMessage = "PasswordRequired")]
        public string Password { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessage = "CompanyTypeRequired")]
        public CompanyType CompanyType { get; set; }

        public CompanyStatus CompanyStatus { get; set; }

        public DateTime DateTimeRegistration { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
    }
}
