using System;
using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using WebDispacher.Attributes;
using WebDispacher.ViewModels.Truck;

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
        [Required(ErrorMessage = "NameRequired")]
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
        [Display(Name = "AnnualIns")]
        [DisplayFormat(DataFormatString = "{0:MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime? AnnualIns { get; set; }

        [Display(Name = "TrailerGroup")]
        [Required(ErrorMessage = "TrailerGroupRequired")]
        public int? TrailerGroupId { get; set; }
        public TrailerGroup TrailerGroup { get; set; }

        [Display(Name = "TrailerStatus")]
        [Required(ErrorMessage = "TrailerStatusRequired")]
        public int? TrailerStatusId { get; set; }

        public TrailerStatus TrailerStatus { get; set; }

        //[Required(ErrorMessage = "TypeRequired")]
        [History]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "TrailerType")]
        public int? TrailerTypeId { get; set; }
        public TrailerTypeViewModel TrailerTypeViewModel { get; set; }

        [Display(Name = "TrailerLocationType")]
        [Required(ErrorMessage = "TrailerLocationTypeRequired")]
        public LocationType LocationType { get; set; }

        [Display(Name = "TrailerLocation")]
        [Required(ErrorMessage = "TrailerLocationRequired")]
        public string LocationAddress { get; set; }

        [Display(Name = "VehicleCategory")]
        public int? VehicleCategoryId { get; set; }

        public DateTime DateTimeRegistration { get; set; }
        public DateTime DateTimeLastUpdate { get; set; }
    }
}