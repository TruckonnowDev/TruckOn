using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Interface;

namespace WebDispacher.ViewModels.Truck
{
    public class TruckViewModel : ITr
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        //[Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        [MaxLength(25)]
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
        public string Exp { get; set; }

        [MaxLength(17)]
        public string Vin { get; set; }

        [Display(Name = "Owner")]
        [MaxLength(25)]
        public string Owner { get; set; }
        
        //[Required(ErrorMessage = "PlateRequired")]
        [Display(Name = "Plate")]
        [MaxLength(10)]
        public string PlateTruck { get; set; }

        [Display(Name = "Color")]
        [MaxLength(15)]
        public string ColorTruck { get; set; }
        
        //[Required(ErrorMessage = "TypeRequired")]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}