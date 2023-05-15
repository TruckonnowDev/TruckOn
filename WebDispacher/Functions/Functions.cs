using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebDispacher.Functions
{
    public class Functions : RazorPage<dynamic>
    {
        public override Task ExecuteAsync()
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<int, string> GetPages(int selectPage, int countPages)
        {
            var delta = 2;
            int left = selectPage - delta;
            int right = selectPage + delta + 1;
            var pagesList = new List<int>();
            for (var i = 1; i <= countPages; i++)
            {
                if (i == 1 || i == countPages || i >= left && i < right)
                {
                    pagesList.Add(i);
                }
            }
            int ig = 0;
            var pagesListWithPoints = new Dictionary<int, string>();
            foreach (var item in pagesList)
            {

                if (ig != 0)
                {
                    if (item - ig == 2)
                    {
                        pagesListWithPoints.Add(ig + 1, (ig + 1).ToString());
                    }
                    else if (item - ig != 1)
                    {
                        var srednee = (item + ig) / 2;
                        pagesListWithPoints.Add(srednee, "...");
                    }
                }
                pagesListWithPoints.Add(item, item.ToString());
                ig = item;
            }

            return pagesListWithPoints;
        }
    }
}
