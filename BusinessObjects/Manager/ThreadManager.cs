using BusinessObjects.Entity;
using BusinessObjects.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    public class ThreadManager
    {

        public static async Task<ForumCollectionEntity> GetForumThreadsAndSubforums(ForumEntity forumCategory)
        {
            List<ForumEntity> forumSubcategoryList = new List<ForumEntity>();
            List<ForumThreadEntity> forumThreadList = new List<ForumThreadEntity>();
            String url = forumCategory.Location;
            if (forumCategory.CurrentPage > 0)
            {
                url = forumCategory.Location + Constants.PAGE_NUMBER + forumCategory.CurrentPage;
            }

            HttpWebRequest request = await AuthManager.CreateGetRequest(url);
            HtmlDocument doc = await WebManager.DownloadHtml(request);
            HtmlNode pageNode = doc.DocumentNode.Descendants("select").FirstOrDefault();

            forumCategory.TotalPages = Convert.ToInt32(pageNode.Descendants("option").LastOrDefault().GetAttributeValue("value", ""));

            HtmlNode forumNode = doc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("class", "").Contains("threadlist")).FirstOrDefault();

            foreach (HtmlNode threadNode in forumNode.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals("thread")))
            {
                ForumThreadEntity threadEntity = new ForumThreadEntity();
                threadEntity.Parse(threadNode);
                forumThreadList.Add(threadEntity);
            }

            if (doc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("id", "").Contains("subforums")).Any())
            {
                forumNode = doc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("id", "").Contains("subforums")).FirstOrDefault();
                foreach (HtmlNode subforumNode in forumNode.Descendants("tr"))
                {
                    if (subforumNode.Descendants("a").Any())
                    {
                        ForumEntity forumSubCategory = new ForumEntity(WebUtility.HtmlDecode(subforumNode.Descendants("a").FirstOrDefault().InnerText), subforumNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", ""), "");
                        forumSubcategoryList.Add(forumSubCategory);
                    }
                }
            }
            forumNode = doc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("class", "").Contains("bclast")).FirstOrDefault();
            return new ForumCollectionEntity(WebUtility.HtmlDecode(forumNode.InnerText), forumThreadList, forumSubcategoryList);
        }

    }
}
