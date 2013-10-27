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
        public string SmileText { get; private set; }

        public string SmileUrl { get; private set; }

        public void Parse(HtmlNode smileNode)
        {
            this.SmileText = smileNode.Descendants("div").FirstOrDefault().InnerText;
            this.SmileUrl = smileNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
        }
    }
}
