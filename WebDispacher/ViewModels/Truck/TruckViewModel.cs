using System;
using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Interface;

namespace WebDispacher.ViewModels.Truck
{
    public class TruckViewModel : ITr
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        //[Required(ErrorMessage = "NameRequired")]
        [MaxLength(25)]
        [Display(Name = "Name")]
        public string NameTruck { get; set; }

        [MaxLength(4)]
        [Display(Name = "Year")]
        public string Year { get; set; }

        [Display(Name = "Make")]
        [MaxLength(15)]
        public string Make { get; set; }

        [MaxLength(20)]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Exp")]
        public DateTime? Exp { get; set; }

        [MaxLength(17)]
        public string Vin { get; set; }

        [MaxLength(25)]
        [Display(Name = "Owner")]
        public string Owner { get; set; }
        
        //[Required(ErrorMessage = "PlateRequired")]
        [MaxLength(10)]
        [Display(Name = "Plate")]
        public string PlateTruck { get; set; }

        [MaxLength(15)]
        [Display(Name = "Color")]
        public string ColorTruck { get; set; }
        
        //[Required(ErrorMessage = "TypeRequired")]
        [Display(Name = "Type")]
        public string Type { get; set; }

        public DateTime DateTimeRegistration { get; set; }
        public DateTime DateTimeLastUpload { get; set; }
    }
}