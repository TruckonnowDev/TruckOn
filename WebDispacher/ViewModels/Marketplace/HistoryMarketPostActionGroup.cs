using System;
using System.Collections.Generic;

namespace WebDispacher.ViewModels.Marketplace
{
    public class HistoryMarketPostActionGroup
    {
        public DateTime GroupAction { get; set; }
        public List<HistoryMarketPostActionViewModel> Groups { get; set; }
    }
}