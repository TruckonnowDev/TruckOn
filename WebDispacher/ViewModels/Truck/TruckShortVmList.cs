using DaoModels.DAO.Models;
using System.Collections.Generic;
using WebDispacher.ViewModels.Marketplace;

namespace WebDispacher.ViewModels.Truck
{
    public class TruckShortVmList
    {
        public Dictionary<TruckGroup, List<DaoModels.DAO.Models.Truck>> Items { get; set; }
        public List<TruckStatusWidget> Widgets { get; set; }
        public TruckFiltersViewModel Filters { get; set; }
        public int CountPage { get; set; }
    }
}