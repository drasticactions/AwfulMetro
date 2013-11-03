using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace AwfulMetro.Core.Entity
{
    public class ForumSearchEntity
    {
        public string Tag { get; private set; }

        public string ThreadTitle { get; private set; }

        public string PostContent { get; private set; }

        public long PostId { get; private set; }

        public long ThreadId { get; private set; }

        public string ForumTitle { get; private set; }

        public long ForumId { get; private set; }

        public string Author { get; private set; }

        public long AuthorId { get; private set; }

        public int ReplyCount { get; private set; }

        public int ThreadViewCount { get; private set; }

        public string Date { get; private set; }

        public void Parse(HtmlNode searchRow)
        {
            if (!searchRow.Descendants("td").FirstOrDefault().InnerText.Contains("no permission"))
            {
                this.Tag = searchRow.Descendants("td").FirstOrDefault().Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
                searchRow.Descendants("td").FirstOrDefault().Remove();
                this.ThreadTitle = WebUtility.HtmlDecode(searchRow.Descendants("td").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
                this.ThreadId = Int64.Parse(Regex.Match(searchRow.Descendants("td").FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Split('=')[1], @"\d+").Value);
                this.PostContent = WebUtility.HtmlDecode(searchRow.Descendants("td").FirstOrDefault().Descendants("div").FirstOrDefault().InnerText);
                this.PostId = Int64.Parse(Regex.Match(searchRow.Descendants("td").FirstOrDefault().Descendants("a").LastOrDefault().GetAttributeValue("href", "").Split('=')[3], @"\d+").Value);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                this.ForumTitle = WebUtility.HtmlDecode(searchRow.Descendants("td").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
                this.ForumId = Int64.Parse(Regex.Match(searchRow.Descendants("td").FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Split('=')[1], @"\d+").Value);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                this.Author = WebUtility.HtmlDecode(searchRow.Descendants("td").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
                this.AuthorId = Convert.ToInt64(searchRow.Descendants("td").FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Split('=')[2]);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                this.ReplyCount = Convert.ToInt32(searchRow.Descendants("td").FirstOrDefault().InnerText);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                this.ThreadViewCount = Convert.ToInt32(searchRow.Descendants("td").FirstOrDefault().InnerText);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                this.Date = searchRow.Descendants("td").FirstOrDefault().Descendants("span").FirstOrDefault().InnerText;
                searchRow.Descendants("td").FirstOrDefault().Remove();
            }
            
        }
    }
}
