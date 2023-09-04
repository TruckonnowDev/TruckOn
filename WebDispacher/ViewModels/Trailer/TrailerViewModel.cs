using System;
using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Interface;

namespace WebDispacher.ViewModels.Trailer
{
    public class TrailerViewModel : ITr
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        //[Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        [MaxLength(25)]
        public string Name { get; set; }
        
        [Display(Name= "Year")]
        [MaxLength(4)]
        public string Year { get; set; }

        [Display(Name = "Make")]
        [MaxLength(15)]
        public string Make { get; set; }
        
        [Display(Name = "Model")]
        [MaxLength(20)]
        public string Model { get; set; }

        [Display(Name = "HowLong")]
        [MaxLength(5)]
        public string HowLong { get; set; }

        [MaxLength(17)]
        public string Vin { get; set; }

        [Display(Name = "Owner")]
        [MaxLength(25)]
        public string Owner { get; set; }

        [Display(Name = "Color")]
        [MaxLength(15)]
        public string Color { get; set; }
        
        //[Required(ErrorMessage = "PlateRequired")]
        [Display(Name = "Plate")]
        [MaxLength(10)]
        public string Plate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime? Exp { get; set; }

        [Display(Name = "AnnualIns")]
        [MaxLength(12)]
        public string AnnualIns { get; set; }
        
        //[Required(ErrorMessage = "TypeRequired")]
        [Display(Name = "Type")]
        public string Type { get; set; }

        public DateTime DateTimeRegistration { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
    }
}