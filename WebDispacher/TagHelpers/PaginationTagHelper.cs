using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using WebDispacher.ViewModels.Pagination;
using WebDispacher.ViewModels.Truck.Enum;

namespace WebDispacher.TagHelpers
{
    [HtmlTargetElement("pagination-list")]
    public class PaginationTagHelper : TagHelper
    {
        public Dictionary<int, string> ActualPages { get; set; }
        public PaginationSort Sorts { get; set; } = new PaginationSort();

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "section";

            var sb = new StringBuilder();

            if (ActualPages.Count > 1)
            {
                var sortString = Sorts.ToUrl();

                sb.Append(
                    "<div class=\"mt-5 mb-5\">" +
                    "<div class=\"order-pagination\">" +
                    "<div class=\"pages-pagination-radio\">");

                if(Sorts.Page != 1)
                {
                    sb.Append($"<a href=\"{Sorts.ToUrl(Sorts.Page-1)}\">❮</a>");
                }
                else
                {
                    sb.Append("<a class=\"without-href\">❮</a>"); 
                }
                sb.Append("</div>");
                sb.Append("<div class=\"pages-pagination\">");

                foreach(var item in ActualPages )
                {
                    if(item.Key == Sorts.Page)
                    {
                        sb.Append($"<a href class=\"active-page\">{item.Value}</a>");
                    }
                    else
                    {
                        sb.Append($"<a href=\"?page={item.Key}{sortString}\">{item.Value}</a>");
                    }
                }
                sb.Append("</div>");

                sb.Append("<div class=\"pages-pagination-radio\">");
                if (Sorts.Page != ActualPages.Count)
                {
                    sb.Append($"<a href=\"{Sorts.ToUrl(Sorts.Page +1)}\">❯</a>");
                }
                else
                {
                    sb.Append("<a class=\"without-href\">❯</a>");
                }
                sb.Append("</div></div></div>");
            }
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}