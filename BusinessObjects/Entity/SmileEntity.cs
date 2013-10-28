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
    public class SmileEntity
    {
        public string Title { get; private set; }

        public string ImageUrl { get; private set; }

        public void Parse(HtmlNode smileNode)
        {
            this.Title = smileNode.Descendants("div").FirstOrDefault().InnerText;
            this.ImageUrl = smileNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
        }
    }
}
