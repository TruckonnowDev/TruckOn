using System;
using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Enum;
using DaoModels.DAO.Interface;
using DaoModels.DAO.Models;
using WebDispacher.Attributes;
using WebDispacher.Constants;

namespace WebDispacher.ViewModels.Truck
{
    public class TruckViewModel : ITr
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        //[Required(ErrorMessage = "NameRequired")]

        [History]
        [MaxLength(25)]
        [Display(Name = "Name")]
        public string NameTruck { get; set; }

        [History]
        [MaxLength(4)]
        [Display(Name = "Year")]
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
        [Display(Name = "State")]
        public string State { get; set; }

        [History]
        [Display(Name = "Exp")]
        public DateTime? Exp { get; set; }


        [History]
        [Display(Name = "AnnualIns")]
        [DisplayFormat(DataFormatString = "{0:MM/yy}", ApplyFormatInEditMode = true)]
        public DateTime? AnnualIns { get; set; }

        [History]
        [MaxLength(17)]
        public string Vin { get; set; }

        [History]
        [MaxLength(25)]
        [Display(Name = "Owner")]
        public string Owner { get; set; }

        //[Required(ErrorMessage = "PlateRequired")]
        [History]
        [MaxLength(10)]
        [Display(Name = "Plate")]
        public string PlateTruck { get; set; }

        [History]
        [MaxLength(15)]
        [Display(Name = "Color")]
        public string ColorTruck { get; set; }

        [Display(Name = "TruckStatus")]
        [Required(ErrorMessage = "TruckStatusRequired")]
        public int? TruckStatusId { get; set; }

        public TruckStatus TruckStatus { get; set; }

        [Display(Name = "TruckType")]
        public int? TruckTypeId { get; set; }
        public TruckTypeViewModel TruckTypeViewModel { get; set; }

        [Display(Name = "TruckGroup")]
        [Required(ErrorMessage = "TruckGroupRequired")]
        public int? TruckGroupId { get; set; }
        public TruckGroup TruckGroup { get; set; }

        [Display(Name = "TruckLocationType")]
        [Required(ErrorMessage = "TruckLocationTypeRequired")]
        public LocationType LocationType { get; set; }

        [Display(Name = "TruckLocation")]
        [Required(ErrorMessage = "TruckLocationRequired")]
        public string LocationAddress { get; set; }

        [Display(Name = "VehicleCategory")]
        public int? VehicleCategoryId { get; set; }

        public DateTime DateTimeRegistration { get; set; }
        public DateTime DateTimeLastUpload { get; set; }
        public string Type { get; set; } = TruckAndTrailerConstants.Truck;
    }
}