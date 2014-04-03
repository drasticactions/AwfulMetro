using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    public class PostIconEntity
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public void Parse(HtmlNode node)
        {
            try
            {
                Id = Convert.ToInt32(node.Descendants("input").First().GetAttributeValue("value", string.Empty));
                ImageUrl = node.Descendants("img").First().GetAttributeValue("src", string.Empty);
                Title = node.Descendants("img").First().GetAttributeValue("alt", string.Empty);
            }
            catch (Exception)
            {
                // If, for some reason, it fails to get an icon, ignore the error.
                // The list view won't show it.
            }
        }
    }
}
