using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebDispacher.TagHelpers
{
    [HtmlTargetElement("pagination-list")]
    public class PaginationTagHelper : TagHelper
    {
        public Dictionary<int, string> ActualPages { get; set; }
        public int SelectedPage { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "section";
            var sb = new StringBuilder();
            if (ActualPages.Count > 1)
            {
                sb.Append(
                    "<div class=\"mt-5 mb-5\">" +
                    "<div class=\"order-pagination\">" +
                    "<div class=\"pages-pagination-radio\">");
                if(SelectedPage != 1)
                {
                    sb.Append($"<a href=\"?page={SelectedPage-1}\">❮</a>");
                }
                else
                {
                    sb.Append("<a class=\"without-href\">❮</a>"); 
                }
                sb.Append("</div>");
                sb.Append("<div class=\"pages-pagination\">");

                foreach(var item in ActualPages )
                {
                    if(item.Key == SelectedPage)
                    {
                        sb.Append($"<a href class=\"active-page\">{item.Value}</a>");
                    }
                    else
                    {
                        sb.Append($"<a href=\"?page={item.Key}\">{item.Value}</a>");
                    }
                }
                sb.Append("</div>");

                sb.Append("<div class=\"pages-pagination-radio\">");
                if (SelectedPage != ActualPages.Count)
                {
                    sb.Append($"<a href=\"?page={SelectedPage + 1}\">❯</a>");
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
