namespace WebDispacher.ViewModels.Pagination
{
    public class PaginationSort
    {
        public int Page { get; set; } = 1;
        public int CountPages { get; set; } = 1;

        public virtual string ToUrl(int page)
        {
            return $"?page={page}";
        }

        public virtual string ToUrl()
        {
            return string.Empty;
        }
    }
}