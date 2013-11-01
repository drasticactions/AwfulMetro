using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Entity
{
    public class FrontPageArticleEntity
    {
        public String Title { get; private set; }

        public String Author { get; private set; }

        public String Date { get; private set; }

        public String FeatureTitle { get; private set; }

        public String FeatureLink { get; private set; }

        public String ArticleText { get; private set; }

        public String ArticleLink { get; private set; }

        public String ArticleImage { get; private set; }

        public String AuthorLink { get; private set; }

        public void ParseFeatured(HtmlNode featureNode)
        {
            this.ArticleImage = featureNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
            this.Title = WebUtility.HtmlDecode(featureNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            this.ArticleLink = featureNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
            this.ArticleText = WebUtility.HtmlDecode(featureNode.Descendants("p").FirstOrDefault().InnerText);
            this.FeatureTitle = WebUtility.HtmlDecode(featureNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("c_name")).FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            this.FeatureLink = featureNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("c_name")).FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
        }

        public void ParseMainArticle(HtmlNode articleNode)
        {
            this.ArticleImage = articleNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
            this.Title = WebUtility.HtmlDecode(articleNode.Descendants("h2").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            this.ArticleLink = articleNode.Descendants("h2").FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
            this.Date = articleNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("date")).FirstOrDefault().InnerHtml;
            this.Author = WebUtility.HtmlDecode(articleNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("author")).FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            this.AuthorLink = articleNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("author")).FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
            this.ArticleText = WebUtility.HtmlDecode(articleNode.Descendants("p").FirstOrDefault().InnerText);
        }

        public void ParseFrontPageArticle(HtmlNode articleNode)
        {
            this.ArticleImage = articleNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
            this.Title = WebUtility.HtmlDecode(articleNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            this.ArticleLink = articleNode.Descendants("h3").FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
            this.Date = articleNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("date")).FirstOrDefault().InnerHtml;
            this.Author = WebUtility.HtmlDecode(articleNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("author")).FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
            this.AuthorLink = articleNode.Descendants("span").Where(node => node.GetAttributeValue("class", "").Contains("author")).FirstOrDefault().Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
            this.ArticleText = WebUtility.HtmlDecode(articleNode.Descendants("p").FirstOrDefault().InnerText);
        }

    }
}
