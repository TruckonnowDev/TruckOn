using DaoModels.DAO.Models;
using System.Collections.Generic;
using System.Text;
using WebDispacher.ViewModels.Pagination;
using WebDispacher.ViewModels.Truck.Enum;

namespace WebDispacher.ViewModels.Truck
{
    public class TruckFiltersViewModel : PaginationSort
    {
        public int GroupId { get; set; }
        public List<TruckFilterViewModel> TrucksFiltersViewModels { get; set; } = new List<TruckFilterViewModel>();
        public List<TruckGroup> AvailableGroups { get; set; } = new List<TruckGroup>();

        public override string ToUrl()
        {
            var sb = new StringBuilder();
            for(var i = 0; i < TrucksFiltersViewModels.Count; i++)
            {
                var sbAddString = TrucksFiltersViewModels[i].SortField != TruckSortField.None && TrucksFiltersViewModels[i].SortType != TruckSortType.None ? $"&sorttype[{i}]={TrucksFiltersViewModels[i].SortType}&sortfield[{i}]={TrucksFiltersViewModels[i].SortField}&groupid={TrucksFiltersViewModels[i].GroupId}" : string.Empty;
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
