using System.Collections.Generic;
using System.Text;
using WebDispacher.ViewModels.Pagination;

namespace WebDispacher.ViewModels.Driver
{
    public class DriverFiltersViewModel : PaginationSort
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char? FirstLetter { get; set; }

        public List<char> AvailableFirstLetters { get; set; }

        public override string ToUrl()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(FirstName))
            {
                sb.Append($"&firstname={FirstName}");
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                sb.Append($"&lastname={LastName}");
            }

            if(FirstLetter.HasValue)
            {
                sb.Append($"&firstletter={FirstLetter.Value}");
            }

            return string.IsNullOrEmpty(sb.ToString()) ? string.Empty : sb.ToString();
        }

        public override string ToUrl(int page)
        {
            var sb = new StringBuilder($"?page={page}");

            if (!string.IsNullOrEmpty(FirstName))
            {
                sb.Append($"&firstname={FirstName}");
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                sb.Append($"&lastname={LastName}");
            }

            if (FirstLetter.HasValue)
            {
                sb.Append($"&firstletter={FirstLetter.Value}");
            }

            return string.IsNullOrEmpty(sb.ToString()) ? base.ToUrl(page) : sb.ToString();
        }
    }
}