using HtmlAgilityPack;

using System.Linq;

namespace AwfulMetro.Core.Entity
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
