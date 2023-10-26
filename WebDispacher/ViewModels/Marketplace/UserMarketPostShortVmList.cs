using System.Collections.Generic;

namespace WebDispacher.ViewModels.Marketplace
{
    public class UserMarketPostShortVmList
    {
        public List<ItemMarketPostShortViewModel> Items { get; set; }
        public UserMarketPostsFiltersViewModel Filters { get; set; }
        public int CountPage { get; set; }
    }
}
