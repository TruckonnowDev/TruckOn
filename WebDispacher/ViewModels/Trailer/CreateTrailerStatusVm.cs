using DaoModels.DAO.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.Trailer
{
    public class CreateTrailerStatusVm
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "NameRequired")]
        public string Name { get; set; }

        [Display(Name = "SelectTheme")]
        [Required(ErrorMessage = "SelectThemeRequired")]
        public int? SelectedThemeId { get; set; }

        public List<TrailerStatusTheme> TrailerStatusThemes { get; set; } = new List<TrailerStatusTheme>();
    }
}