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
    public class PopularThreadsEntity
    {
        public String Tag { get; private set; }

        public String Title { get; private set; }

        public long ThreadId { get; private set; }

        public void Parse(HtmlNode popularThreadsNode)
        {
            foreach(HtmlNode popularThread in popularThreadsNode.Descendants("li"))
            {
                this.Tag = popularThread.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
                this.Title = popularThread.Descendants("a").FirstOrDefault().InnerText;
                this.ThreadId = Convert.ToInt64(popularThread.Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Split('=')[1]);
            }
        }

    }
}
