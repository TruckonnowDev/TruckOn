using WebDispacher.ViewModels.Trailer.Enum;

namespace WebDispacher.ViewModels.Trailer
{
    public class TrailerFilterViewModel
    {
        public TrailerSortField SortField { get; set; } = TrailerSortField.None;
        public TrailerSortType SortType { get; set; } = TrailerSortType.None;
        public int GroupId { get; set; }
    }
}