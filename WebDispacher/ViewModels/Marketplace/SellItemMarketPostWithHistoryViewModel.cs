using System.Collections.Generic;
using WebDispacher.ViewModels.History;

namespace WebDispacher.ViewModels.Marketplace
{
    public class SellItemMarketPostWithHistoryViewModel
    {
        public SellItemMarketPostViewModel SellItemMarketPost { get; set; }
        public List<GroupsHistoriesListShortViewModel> Actions { get; set; }
    }
}