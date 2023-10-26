using System.Collections.Generic;

namespace WebDispacher.ViewModels.Marketplace
{
    public class BuyItemMarketPostShortVmList
    {
        public List<ItemMarketPostShortViewModel> Items { get; set; }
        public BuyMarketPostsFiltersViewModel Filters { get; set; }
        public int CountPage { get; set; }
    }
}
