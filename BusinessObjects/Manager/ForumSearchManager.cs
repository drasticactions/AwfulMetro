using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Entity;
using BusinessObjects.Tools;
using HtmlAgilityPack;

namespace BusinessObjects.Manager
{
    public class ForumSearchManager
    {
        public static async Task<List<ForumSearchEntity>> GetSearchResults(String url)
        {
            List<ForumSearchEntity> searchResults = new List<ForumSearchEntity>();
            HttpWebRequest request = await AuthManager.CreateGetRequest(url);
            HtmlDocument doc = await WebManager.DownloadHtml(request);
            HtmlNode searchNode = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("inner")).FirstOrDefault();
            url = Constants.BASE_URL + "search.php" + searchNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
            request = await AuthManager.CreateGetRequest(url);
            doc = await WebManager.DownloadHtml(request);
            searchResults = ForumSearchManager.ParseSearchRows(doc.DocumentNode.Descendants("table").Where(node => node.GetAttributeValue("id", "").Contains("main_full")).FirstOrDefault());
            return searchResults;
        }

        private static List<ForumSearchEntity> ParseSearchRows(HtmlNode searchNode)
        {
            List<ForumSearchEntity> searchRowEntityList = new List<ForumSearchEntity>();
            var searchRowNodes = searchNode.Descendants("tr");
            searchRowNodes.FirstOrDefault().Remove();
            foreach (HtmlNode searchRow in searchRowNodes)
            {
                ForumSearchEntity searchRowEntity = new ForumSearchEntity();
                searchRowEntity.Parse(searchRow);
                if(!string.IsNullOrEmpty(searchRowEntity.Author))
                    searchRowEntityList.Add(searchRowEntity);
            }
            return searchRowEntityList;
        }
    }
}
