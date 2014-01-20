using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    public class FrontPageWebArticleEntity
    {
        public string ArticleHtml { get; set; }

        public int TotalPages { get; set; }

        public void MapTo(string html, int totalPages)
        {
            ArticleHtml = html;
            TotalPages = totalPages;
        }
    }
}
