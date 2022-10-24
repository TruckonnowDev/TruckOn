using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Interface;

namespace WebDispacher.ViewModels.Trailer
{
    public class TrailerViewModel : ITr
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        
        [Required(ErrorMessage = "NameRequired")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        
        [Display(Name= "Year")]
        public string Year { get; set; }

        [Display(Name = "Make")]
        public string Make { get; set; }

        [Display(Name = "HowLong")]
        public string HowLong { get; set; }

        public string Vin { get; set; }

        [Display(Name = "Owner")]
        public string Owner { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }
        
        [Required(ErrorMessage = "PlateRequired")]
        [Display(Name = "Plate")]
        public string Plate { get; set; }
        public string Exp { get; set; }

        [Display(Name = "AnnualIns")]
        public string AnnualIns { get; set; }
        
        [Required(ErrorMessage = "TypeRequired")]
        [Display(Name = "Type")]
        public string Type { get; set; }
    }
}