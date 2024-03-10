using DaoModels.DAO.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebDispacher.ViewModels.Widget.Enum;

namespace WebDispacher.ViewModels.Widget
{
    public class CreateWidgetVm
    {
        [Display(Name = "Status")]
        [Required(ErrorMessage = "SelectStatusRequired")]
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public TypeWidget TypeWidget { get; set; }
    }
}