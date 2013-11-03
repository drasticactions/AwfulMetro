using HtmlAgilityPack;
using System.Linq;
using System.Net;

namespace AwfulMetro.Core.Entity
{
    public class FrontPageArticleEntity
    {
        public string Title { get; private set; }

        public string Author { get; private set; }

        public string Date { get; private set; }

        public string FeatureTitle { get; private set; }

        public string FeatureLink { get; private set; }

        public string ArticleText { get; private set; }

        public string ArticleLink { get; private set; }

        public string ArticleImage { get; private set; }

        public string AuthorLink { get; private set; }

        public void ParseFeatured(HtmlNode featureNode)
        {
            this.ArticleImage = featureNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
            this.Title = WebUtility.HtmlDecode(featureNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            this.ArticleLink = featureNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty);
            this.ArticleText = WebUtility.HtmlDecode(featureNode.Descendants("p").FirstOrDefault().InnerText);
            this.FeatureTitle = WebUtility.HtmlDecode(featureNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("c_name")).Descendants("a").FirstOrDefault().InnerText);
            this.FeatureLink = featureNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("c_name")).Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty);
        }

        public void ParseMainArticle(HtmlNode articleNode)
        {
            this.ArticleImage = articleNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
            this.Title = WebUtility.HtmlDecode(articleNode.Descendants("h2").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            this.ArticleLink = articleNode.Descendants("h2").FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty);
            this.Date = articleNode.Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("date")).InnerHtml;
            this.Author = WebUtility.HtmlDecode(articleNode.Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author")).Descendants("a").FirstOrDefault().InnerText);
            this.AuthorLink = articleNode.Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author")).Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty);
            this.ArticleText = WebUtility.HtmlDecode(articleNode.Descendants("p").FirstOrDefault().InnerText);
        }

        public void ParseFrontPageArticle(HtmlNode articleNode)
        {
            this.ArticleImage = articleNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
            this.Title = WebUtility.HtmlDecode(articleNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            this.ArticleLink = articleNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty);
            this.Date = articleNode.Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("date")).InnerHtml;
            this.Author = WebUtility.HtmlDecode(articleNode.Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author")).Descendants("a").FirstOrDefault().InnerText);
            this.AuthorLink = articleNode.Descendants("span").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author")).Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty);
            this.ArticleText = WebUtility.HtmlDecode(articleNode.Descendants("p").FirstOrDefault().InnerText);
        }
    }
}
