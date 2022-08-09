﻿using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Contact
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        [MinLength(4)]
        [MaxLength(12)]
        public string Phone { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}