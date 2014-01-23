using System.Linq;
using System.Net;
using HtmlAgilityPack;

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
            ArticleImage = featureNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
            Title =
                WebUtility.HtmlDecode(
                    featureNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            ArticleLink =
                featureNode.Descendants("h3")
                    .FirstOrDefault()
                    .Descendants("a")
                    .FirstOrDefault()
                    .GetAttributeValue("href", string.Empty);
            ArticleText = WebUtility.HtmlDecode(featureNode.Descendants("p").FirstOrDefault().InnerText);
            FeatureTitle =
                WebUtility.HtmlDecode(
                    featureNode.Descendants("div")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("c_name"))
                        .Descendants("a")
                        .FirstOrDefault()
                        .InnerText);
            FeatureLink =
                featureNode.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("c_name"))
                    .Descendants("a")
                    .FirstOrDefault()
                    .GetAttributeValue("href", string.Empty);
        }

        public void ParseMainArticle(HtmlNode articleNode)
        {
            ArticleImage = articleNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
            Title =
                WebUtility.HtmlDecode(
                    articleNode.Descendants("h2").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            ArticleLink =
                articleNode.Descendants("h2")
                    .FirstOrDefault()
                    .Descendants("a")
                    .FirstOrDefault()
                    .GetAttributeValue("href", string.Empty);
            Date =
                articleNode.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("date"))
                    .InnerHtml;
            Author =
                WebUtility.HtmlDecode(
                    articleNode.Descendants("span")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author"))
                        .Descendants("a")
                        .FirstOrDefault()
                        .InnerText);
            AuthorLink =
                articleNode.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author"))
                    .Descendants("a")
                    .FirstOrDefault()
                    .GetAttributeValue("href", string.Empty);
            ArticleText = WebUtility.HtmlDecode(articleNode.Descendants("p").FirstOrDefault().InnerText);
        }

        public void ParseFrontPageArticle(HtmlNode articleNode)
        {
            ArticleImage = articleNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
            Title =
                WebUtility.HtmlDecode(
                    articleNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            ArticleLink =
                articleNode.Descendants("h3")
                    .FirstOrDefault()
                    .Descendants("a")
                    .FirstOrDefault()
                    .GetAttributeValue("href", string.Empty);
            Date =
                articleNode.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("date"))
                    .InnerHtml;
            Author =
                WebUtility.HtmlDecode(
                    articleNode.Descendants("span")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author"))
                        .Descendants("a")
                        .FirstOrDefault()
                        .InnerText);
            AuthorLink =
                articleNode.Descendants("span")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author"))
                    .Descendants("a")
                    .FirstOrDefault()
                    .GetAttributeValue("href", string.Empty);
            ArticleText = WebUtility.HtmlDecode(articleNode.Descendants("p").FirstOrDefault().InnerText);
        }
    }
}