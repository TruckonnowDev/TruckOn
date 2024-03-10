using System.Collections.Generic;

namespace WebDispacher.ViewModels.Driver
{
    public class DriverVmList
    {
        public List<DaoModels.DAO.Models.Driver> Items { get; set; }
        public DriverFiltersViewModel Filters { get; set; }
    }
}