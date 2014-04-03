using System.Linq;
using System.Net;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    public class PrivateMessageEntity
    {
        public string Status { get; set; }

        public string Icon { get; set; }

        public string Title { get; set; }

        public string Sender { get; set; }

        public string Date { get; set; }

        public string MessageUrl { get; set; }

        public void Parse(HtmlNode rowNode)
        {
            Status =
                rowNode.Descendants("td")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("status"))
                    .Descendants("img")
                    .FirstOrDefault()
                    .GetAttributeValue("src", string.Empty);

            var icon = rowNode.Descendants("td")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("icon"))
                    .Descendants("img")
                    .FirstOrDefault();
            
            if (icon != null)
            {
                Icon = icon.GetAttributeValue("src", string.Empty);
            }

            var titleNode = rowNode.Descendants("td")
                .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("title"));

            Title =
               titleNode
                    .InnerText.Replace("\n", string.Empty);

            string titleHref = titleNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty).Replace("&amp;", "&");

            MessageUrl = Constants.BASE_URL + titleHref;

            Sender = rowNode.Descendants("td")
                .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("sender"))
                .InnerText;
            Date = rowNode.Descendants("td")
                .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("date"))
                .InnerText;
        }
    }
}