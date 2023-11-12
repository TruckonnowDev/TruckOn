using System.Collections.Generic;
using WebDispacher.ViewModels.History;

namespace WebDispacher.ViewModels.Marketplace
{
    public class BuyItemMarketPostWithHistoryViewModel
    {
        public BuyItemMarketPostViewModel BuyItemMarketPost { get; set; }
        public List<GroupsHistoriesListShortViewModel> Actions { get; set; }
    }
}
