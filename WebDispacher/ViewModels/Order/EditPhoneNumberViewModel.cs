using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WebDispacher.Attributes;

namespace WebDispacher.ViewModels.Order
{
    public class EditPhoneNumberViewModel
    {
        public int Id { get; set; }
        public string Iso2 { get; set; }

        [History]
        public int DialCode { get; set; }

        [History]
        public string Name { get; set; }
        
        [History]
        [Display(Name = "Number")]
        public string Number { get; set; }


    }
}
