using DaoModels.DAO.Enum;
using System;
using System.Text;
using WebDispacher.ViewModels.Pagination;

namespace WebDispacher.ViewModels.Marketplace
{
    public class BuyMarketPostsFiltersViewModel : PaginationSort
    {
        public bool IsOnlyMyPosts { get; set; } = false;

        public string KeyWords { get; set; } = string.Empty;
        public SortType SortType { get; set; } = SortType.Base;
        public ConditionPost ConditionPost { get; set; } = ConditionPost.Published;
        public override string ToUrl()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(KeyWords))
            {
                sb.Append($"&keywords={KeyWords}");
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

            if (!string.IsNullOrEmpty(KeyWords))
            {
                sb.Append($"&keywords={KeyWords}");
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
