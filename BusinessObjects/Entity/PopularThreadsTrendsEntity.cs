using BusinessObjects.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entity
{
    public class PopularThreadsTrendsEntity
    {
        public String Tag { get; private set; }

        public String Title { get; private set; }

        public long Id { get; private set; }

        public string LocationUrl { get; private set; }

        public void ParseThread(HtmlNode popularThreadsNode)
        {
            this.Tag = popularThreadsNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
            this.Title = WebUtility.HtmlDecode(popularThreadsNode.Descendants("a").FirstOrDefault().InnerText);
            this.Id = Convert.ToInt64(popularThreadsNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Split('=')[1]);
        }

        public void ParseTrend(HtmlNode popularTrendsNode)
        {
            this.Title = WebUtility.HtmlDecode(popularTrendsNode.Descendants("a").FirstOrDefault().InnerText);
            this.LocationUrl = Constants.BASE_URL + popularTrendsNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
        }

    }
}
