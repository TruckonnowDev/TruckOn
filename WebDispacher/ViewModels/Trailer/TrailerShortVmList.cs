using DaoModels.DAO.Models;
using System.Collections.Generic;

namespace WebDispacher.ViewModels.Trailer
{
    public class TrailerShortVmList
    {
        public Dictionary<TrailerGroup, List<DaoModels.DAO.Models.Trailer>> Items { get; set; }
        public List<TrailerStatusWidget> Widgets { get; set; }
        public TrailerFiltersViewModel Filters { get; set; }
        public int CountPage { get; set; }
    }
}