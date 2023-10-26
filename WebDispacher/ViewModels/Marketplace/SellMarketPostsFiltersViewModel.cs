using DaoModels.DAO.Enum;
using System;

namespace WebDispacher.ViewModels.Marketplace
{
    public class SellMarketPostsFiltersViewModel
    {
        public bool IsOnlyMyPosts { get; set; } = false;

        public string KeyWords { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int CountPages { get; set; } = 1;
        public ConditionItem ConditionItem { get; set; } = ConditionItem.NotSelected;
        public double FirstPrice { get; set; }
        public double LastPrice { get; set; }
        public SortType SortType { get; set; } = SortType.Base;
        public ConditionPost ConditionPost { get; set; } = ConditionPost.Published;
    }
}
