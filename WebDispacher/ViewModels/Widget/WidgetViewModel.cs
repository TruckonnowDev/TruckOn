using System.ComponentModel.DataAnnotations;
using WebDispacher.ViewModels.Widget.Enum;

namespace WebDispacher.ViewModels.Widget
{
    public class WidgetViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "StatusRequired")]
        public int StatusId { get; set; }

        public TypeWidget TypeWidget { get; set; }
    }
}