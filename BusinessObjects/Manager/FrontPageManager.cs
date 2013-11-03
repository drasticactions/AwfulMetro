using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Manager
{
    public class FrontPageManager
    {
        private readonly IWebManager _webManager;
        public FrontPageManager(IWebManager webManager)
        {
            this._webManager = webManager;
        }

        public FrontPageManager() : this(new WebManager()) { }

        public List<PopularThreadsTrendsEntity> GetPopularThreads(HtmlDocument doc)
        {
            List<PopularThreadsTrendsEntity> popularThreadsList = new List<PopularThreadsTrendsEntity>();
            HtmlNode popularThreadNodeList = doc.DocumentNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("popular_threads"));
            foreach(HtmlNode popularThreadNode in popularThreadNodeList.Descendants("li"))
            {
                PopularThreadsTrendsEntity popularThread = new PopularThreadsTrendsEntity();
                popularThread.ParseThread(popularThreadNode);
                popularThreadsList.Add(popularThread);
            }
            return popularThreadsList;
        }

        public List<PopularThreadsTrendsEntity> GetPopularTrends(HtmlDocument doc)
        {
            List<PopularThreadsTrendsEntity> popularTrendsList = new List<PopularThreadsTrendsEntity>();
            HtmlNode popularTrendsNodeList = doc.DocumentNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("organ whatshot"));
            foreach (HtmlNode popularThreadNode in popularTrendsNodeList.Descendants("li"))
            {
                PopularThreadsTrendsEntity popularTrend = new PopularThreadsTrendsEntity();
                popularTrend.ParseTrend(popularThreadNode);
                popularTrendsList.Add(popularTrend);
            }
            return popularTrendsList;
        }

        public List<FrontPageArticleEntity> GetFrontPageArticles(HtmlDocument doc)
        {
            List<FrontPageArticleEntity> frontPageArticleList = new List<FrontPageArticleEntity>();
            HtmlNode frontPageNode = doc.DocumentNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("main_article"));
            FrontPageArticleEntity mainArticle = new FrontPageArticleEntity();
            mainArticle.ParseMainArticle(frontPageNode);
            frontPageArticleList.Add(mainArticle);
            frontPageNode = doc.DocumentNode.Descendants("ul").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("news"));
            foreach(HtmlNode frontPageNewsArticle in frontPageNode.Descendants("li"))
            {
                FrontPageArticleEntity article = new FrontPageArticleEntity();
                article.ParseFrontPageArticle(frontPageNewsArticle);
                frontPageArticleList.Add(article);
            }
            return frontPageArticleList;
        }

        public List<FrontPageArticleEntity> GetFeatures(HtmlDocument doc)
        {
            List<FrontPageArticleEntity> frontPageFeatureList = new List<FrontPageArticleEntity>();
            HtmlNode frontPageNode = doc.DocumentNode.Descendants("ul").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("featured"));
            foreach (HtmlNode frontPageFeature in frontPageNode.Descendants("li"))
            {
                FrontPageArticleEntity article = new FrontPageArticleEntity();
                article.ParseFeatured(frontPageFeature);
                frontPageFeatureList.Add(article);
            }
            return frontPageFeatureList;
        }

        public async Task<HtmlDocument> GetFrontPage()
        {
            return (await _webManager.DownloadHtml(Constants.FRONT_PAGE)).Document;
        }
    }
}
