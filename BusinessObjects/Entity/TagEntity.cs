using HtmlAgilityPack;
using System.Linq;

namespace AwfulMetro.Core.Entity
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
