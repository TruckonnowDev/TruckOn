using WebDispacher.ViewModels.Truck.Enum;

namespace WebDispacher.ViewModels.Truck
{
    public class TruckFilterViewModel
    {
        public TruckSortField SortField { get; set; } = TruckSortField.None;
        public TruckSortType SortType { get; set; } = TruckSortType.None;
        public int GroupId { get; set; }
    }
}