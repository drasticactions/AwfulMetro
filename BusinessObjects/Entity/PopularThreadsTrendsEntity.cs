using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;

namespace AwfulMetro.Core.Entity
{
    public class PopularThreadsTrendsEntity
    {
        public string Tag { get; private set; }

        public string Title { get; private set; }

        public long Id { get; private set; }

        public string LocationUrl { get; private set; }

        public void ParseThread(HtmlNode popularThreadsNode)
        {
            this.Tag = popularThreadsNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
            this.Title = WebUtility.HtmlDecode(popularThreadsNode.Descendants("a").FirstOrDefault().InnerText);
            this.Id = Convert.ToInt64(popularThreadsNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty).Split('=')[1]);
        }

        public void ParseTrend(HtmlNode popularTrendsNode)
        {
            this.Title = WebUtility.HtmlDecode(popularTrendsNode.Descendants("a").FirstOrDefault().InnerText);
            this.LocationUrl = Constants.FRONT_PAGE + popularTrendsNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty);
        }

    }
}
