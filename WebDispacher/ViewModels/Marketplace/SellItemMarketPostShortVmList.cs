using System.Collections.Generic;

namespace WebDispacher.ViewModels.Marketplace
{
    public class SellItemMarketPostShortVmList
    {
        public List<ItemMarketPostShortViewModel> Items { get; set; }
        public SellMarketPostsFiltersViewModel Filters { get; set; }
        public int CountPage { get; set; }
    }
}
