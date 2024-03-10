using DaoModels.DAO.Enum;
using System;
using System.Text;
using WebDispacher.ViewModels.Pagination;

namespace WebDispacher.ViewModels.Marketplace
{
    public class SellMarketPostsFiltersViewModel : PaginationSort
    {
        public bool IsOnlyMyPosts { get; set; } = false;

        public string KeyWords { get; set; } = string.Empty;
        public ConditionItem ConditionItem { get; set; } = ConditionItem.NotSelected;
        public double FirstPrice { get; set; }
        public double LastPrice { get; set; }
        public SortType SortType { get; set; } = SortType.Base;
        public ConditionPost ConditionPost { get; set; } = ConditionPost.Published;

        public override string ToUrl()
        {
            var sb = new StringBuilder();

            sb.Append($"&isonlymyposts={IsOnlyMyPosts}");

            if (!string.IsNullOrEmpty(KeyWords))
            {
                sb.Append($"&keywords={KeyWords}");
            }

            if (ConditionItem != ConditionItem.NotSelected)
            {
                sb.Append($"&conditionitem={ConditionItem}");
            }

            if (FirstPrice != 0)
            {
                sb.Append($"&firstprice={FirstPrice}");
            }
            
            if (LastPrice != 0)
            {
                sb.Append($"&lastprice={LastPrice}");
            }
            
            if (SortType != SortType.Base)
            {
                sb.Append($"&sorttype={SortType}");
            }

            if (ConditionPost != ConditionPost.NotSelected)
            {
                sb.Append($"&conditionpost={ConditionPost}");
            }

            return string.IsNullOrEmpty(sb.ToString()) ? string.Empty : sb.ToString();
        }

        public override string ToUrl(int page)
        {
            var sb = new StringBuilder($"?page={page}");

            sb.Append($"&isonlymyposts={IsOnlyMyPosts}");

            if (!string.IsNullOrEmpty(KeyWords))
            {
                sb.Append($"&keywords={KeyWords}");
            }

            if (ConditionItem != ConditionItem.NotSelected)
            {
                sb.Append($"&conditionitem={ConditionItem}");
            }

            if (FirstPrice != 0)
            {
                sb.Append($"&firstprice={FirstPrice}");
            }

            if (LastPrice != 0)
            {
                sb.Append($"&lastprice={LastPrice}");
            }

            if (SortType != SortType.Base)
            {
                sb.Append($"&sorttype={SortType}");
            }

            if (ConditionPost != ConditionPost.NotSelected)
            {
                sb.Append($"&conditionpost={ConditionPost}");
            }

            return string.IsNullOrEmpty(sb.ToString()) ? base.ToUrl(page) : sb.ToString();
        }
    }
}
