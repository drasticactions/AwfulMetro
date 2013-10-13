using BusinessObjects.Entity;
using BusinessObjects.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    public class FrontPageManager
    {
        public static List<PopularThreadsTrendsEntity> GetPopularThreads(HtmlDocument doc)
        {
            List<PopularThreadsTrendsEntity> popularThreadsList = new List<PopularThreadsTrendsEntity>();
            HtmlNode popularThreadNodeList = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("popular_threads")).FirstOrDefault();
            foreach(HtmlNode popularThreadNode in popularThreadNodeList.Descendants("li"))
            {
                PopularThreadsTrendsEntity popularThread = new PopularThreadsTrendsEntity();
                popularThread.ParseThread(popularThreadNode);
                popularThreadsList.Add(popularThread);
            }
            return popularThreadsList;
        }

        public static List<PopularThreadsTrendsEntity> GetPopularTrends(HtmlDocument doc)
        {
            List<PopularThreadsTrendsEntity> popularTrendsList = new List<PopularThreadsTrendsEntity>();
            HtmlNode popularTrendsNodeList = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("organ whatshot")).FirstOrDefault();
            foreach (HtmlNode popularThreadNode in popularTrendsNodeList.Descendants("li"))
            {
                PopularThreadsTrendsEntity popularTrend = new PopularThreadsTrendsEntity();
                popularTrend.ParseTrend(popularThreadNode);
                popularTrendsList.Add(popularTrend);
            }
            return popularTrendsList;
        }

        public static List<FrontPageArticleEntity> GetFrontPageArticles(HtmlDocument doc)
        {
            List<FrontPageArticleEntity> frontPageArticleList = new List<FrontPageArticleEntity>();
            HtmlNode frontPageNode = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("main_article")).FirstOrDefault();
            FrontPageArticleEntity mainArticle = new FrontPageArticleEntity();
            mainArticle.ParseMainArticle(frontPageNode);
            frontPageArticleList.Add(mainArticle);
            frontPageNode = doc.DocumentNode.Descendants("ul").Where(node => node.GetAttributeValue("class", "").Contains("news")).FirstOrDefault();
            foreach(HtmlNode frontPageNewsArticle in frontPageNode.Descendants("li"))
            {
                FrontPageArticleEntity article = new FrontPageArticleEntity();
                article.ParseFrontPageArticle(frontPageNewsArticle);
                frontPageArticleList.Add(article);
            }
            return frontPageArticleList;
        }

        public static List<FrontPageArticleEntity> GetFeatures(HtmlDocument doc)
        {
            List<FrontPageArticleEntity> frontPageFeatureList = new List<FrontPageArticleEntity>();
            HtmlNode frontPageNode = doc.DocumentNode.Descendants("ul").Where(node => node.GetAttributeValue("class", "").Contains("featured")).FirstOrDefault();
            foreach (HtmlNode frontPageFeature in frontPageNode.Descendants("li"))
            {
                FrontPageArticleEntity article = new FrontPageArticleEntity();
                article.ParseFeatured(frontPageFeature);
                frontPageFeatureList.Add(article);
            }
            return frontPageFeatureList;
        }

        public static async Task<HtmlDocument> GetFrontPage()
        {
            HttpWebRequest request = await AuthManager.CreateGetRequest(Constants.FRONT_PAGE);
            return await WebManager.DownloadHtml(request);
        }
    }
}
