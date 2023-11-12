using System;
using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Interface;
using WebDispacher.Attributes;

namespace WebDispacher.ViewModels.Trailer
{
    public class TrailerViewModel : ITr
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }

        //[Required(ErrorMessage = "NameRequired")]
        [History]
        [MaxLength(25)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [History]
        [MaxLength(4)]
        [Display(Name= "Year")]
        public string Year { get; set; }

        [History]
        [MaxLength(15)]
        [Display(Name = "Make")]
        public string Make { get; set; }

        [History]
        [MaxLength(20)]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [History]
        [MaxLength(5)]
        [Display(Name = "HowLong")]
        public string HowLong { get; set; }

        [History]
        [MaxLength(17)]
        public string Vin { get; set; }

        [History]
        [MaxLength(25)]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [History]
        [MaxLength(15)]
        [Display(Name = "Color")]
        public string Color { get; set; }

        //[Required(ErrorMessage = "PlateRequired")]

        [History]
        [MaxLength(10)]
        [Display(Name = "Plate")]
        public string Plate { get; set; }

        [History]
        [DisplayFormat(DataFormatString = "{0:MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime? Exp { get; set; }

        [History]
        [MaxLength(12)]
        [Display(Name = "AnnualIns")]
        public string AnnualIns { get; set; }

        //[Required(ErrorMessage = "TypeRequired")]
        [History]
        [Display(Name = "Type")]
        public string Type { get; set; }

        public DateTime DateTimeRegistration { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
    }
}