using System;
using System.Collections.Generic;

namespace WebDispacher.ViewModels.Order
{
    public class HistoryOrderGroup
    {
        public DateTime GroupAction { get; set; }
        public List<HistoryOrderActionWithVehicleViewModel> Groups { get; set; }
    }
}
