using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Interface;

namespace WebDispacher.ViewModels.Truck
{
    public class TruckViewModel : ITr
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        [Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        public string NameTruck { get; set; }

        [Display(Name = "Year")]
        public string Year { get; set; }

        [Display(Name = "Make")]
        public string Make { get; set; }

        [Display(Name = "Model")]
        public string Model { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }
        public string Exp { get; set; }
        public string Vin { get; set; }

        [Display(Name = "Owner")]
        public string Owner { get; set; }
        
        [Required(ErrorMessage = "PlateRequired")]
        [Display(Name = "Plate")]
        public string PlateTruck { get; set; }

        [Display(Name = "Color")]

        public string ColorTruck { get; set; }
        
        [Required(ErrorMessage = "TypeRequired")]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}