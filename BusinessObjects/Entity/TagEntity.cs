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
    public class TagEntity
    {
        public string Title { get; private set; }

        public string ImageUrl { get; private set; }

        public void Parse(HtmlNode tagNode)
        {
            this.Title = tagNode.Descendants("img").FirstOrDefault().GetAttributeValue("alt", "");
            this.ImageUrl = tagNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
        }
    }
}
