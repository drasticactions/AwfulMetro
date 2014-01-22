using System.Linq;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    public class SmileEntity
    {
        public string Title { get; private set; }

        public string ImageUrl { get; private set; }

        public void Parse(HtmlNode smileNode)
        {
            Title = smileNode.Descendants("div").FirstOrDefault().InnerText;
            ImageUrl = smileNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
        }
    }
}