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
        public static async Task<List<PopularThreadsEntity>> GetPopularThreads()
        {
            List<PopularThreadsEntity> popularThreadsList = new List<PopularThreadsEntity>();
            HttpWebRequest request = await AuthManager.CreateGetRequest(Constants.FRONT_PAGE);
            HtmlDocument doc = await WebManager.DownloadHtml(request);
            HtmlNode popularThreadNodeList = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("popular_threads")).FirstOrDefault();
            foreach(HtmlNode popularThreadNode in popularThreadNodeList.Descendants("li"))
            {
                PopularThreadsEntity popularThread = new PopularThreadsEntity();
                popularThread.Parse(popularThreadNode);
                popularThreadsList.Add(popularThread);
            }
            return popularThreadsList;
        }
    }
}
