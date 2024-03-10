using DaoModels.DAO.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Truck
{
    public class CreateTruckStatusVm
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "NameRequired")]
        public string Name { get; set; }

        [Display(Name = "SelectTheme")]
        [Required(ErrorMessage = "SelectThemeRequired")]
        public int? SelectedThemeId { get; set; }

        public List<TruckStatusTheme> TruckStatusThemes { get; set; } = new List<TruckStatusTheme>();
    }
}