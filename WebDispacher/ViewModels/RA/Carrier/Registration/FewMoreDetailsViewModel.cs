using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebDispacher.ViewModels.RA.Carrier.Registration
{
    public class FewMoreDetailsViewModel
    {
        [Required(ErrorMessage = "UnitsRequired")]
        [Display(Name = "Units")]
        public int Units { get; set; }

        [Display(Name = "HowYouFindUs")]
        public string HowYouFindUs { get; set; }

        [Display(Name = "Promo")]
        public string Promo { get; set; }

        public PersonalDataViewModel PersonalData { get; set; }
    }
}