using DaoModels.DAO.Models;
using System.Collections.Generic;
using System.Text;
using WebDispacher.ViewModels.Pagination;
using WebDispacher.ViewModels.Trailer.Enum;

namespace WebDispacher.ViewModels.Trailer
{
    public class TrailerFiltersViewModel : PaginationSort
    {
        public int GroupId { get; set; }
        public List<TrailerFilterViewModel> TrailerFiltersViewModels { get; set; } = new List<TrailerFilterViewModel>();
        public List<TrailerGroup> AvailableGroups { get; set; } = new List<TrailerGroup>();

        public override string ToUrl()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < TrailerFiltersViewModels.Count; i++)
            {
                var sbAddString = TrailerFiltersViewModels[i].SortField != TrailerSortField.None && TrailerFiltersViewModels[i].SortType != TrailerSortType.None ? $"&sorttype[{i}]={TrailerFiltersViewModels[i].SortType}&sortfield[{i}]={TrailerFiltersViewModels[i].SortField}&groupid={TrailerFiltersViewModels[i].GroupId}" : string.Empty;
                sb.Append(sbAddString);
            }

            return sb.ToString();
        }

        public override string ToUrl(int page)
        {
            return base.ToUrl(page);
        }
    }
}