using System.Linq;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    public class TagEntity
    {
        public string Title { get; private set; }

        public string ImageUrl { get; private set; }

        public void Parse(HtmlNode tagNode)
        {
            Title = tagNode.Descendants("img").FirstOrDefault().GetAttributeValue("alt", string.Empty);
            ImageUrl = tagNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
        }
    }
}