using BusinessObjects.Entity;
using BusinessObjects.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    public class ThreadManager
    {
        
        private readonly IWebManager webManager;
        public ThreadManager(IWebManager webManager)
        {
            this.webManager = webManager;
        }

        public ThreadManager() : this(new WebManager()) { }

        public async Task<ForumCollectionEntity> GetBookmarks(ForumEntity forumCategory)
        {
            List<ForumEntity> forumSubcategoryList = new List<ForumEntity>();
            List<ForumThreadEntity> forumThreadList = new List<ForumThreadEntity>();
            String url = forumCategory.Location;
            if (forumCategory.CurrentPage > 0)
            {
                url = forumCategory.Location + string.Format(Constants.PAGE_NUMBER, forumCategory.CurrentPage);
            }

            HtmlDocument doc = (await webManager.DownloadHtml(url)).Document;
            HtmlNode forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("threadlist"));


            foreach (HtmlNode threadNode in forumNode.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals("thread")))
            {
                ForumThreadEntity threadEntity = new ForumThreadEntity();
                threadEntity.Parse(threadNode);
                forumThreadList.Add(threadEntity);
            }
            return new ForumCollectionEntity(WebUtility.HtmlDecode(forumNode.InnerText), forumThreadList, forumSubcategoryList);
        }

        public async Task<ForumCollectionEntity> GetForumThreadsAndSubforums(ForumEntity forumCategory)
        {
            List<ForumEntity> forumSubcategoryList = new List<ForumEntity>();
            List<ForumThreadEntity> forumThreadList = new List<ForumThreadEntity>();
            String url = forumCategory.Location;
            if (forumCategory.CurrentPage > 0)
            {
                url = forumCategory.Location + string.Format(Constants.PAGE_NUMBER,forumCategory.CurrentPage);
            }

            HtmlDocument doc = (await webManager.DownloadHtml(url)).Document;
            HtmlNode pageNode = doc.DocumentNode.Descendants("select").FirstOrDefault();

            forumCategory.TotalPages = Convert.ToInt32(pageNode.Descendants("option").LastOrDefault().GetAttributeValue("value", ""));

            HtmlNode forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("threadlist"));

            foreach (HtmlNode threadNode in forumNode.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals("thread")))
            {
                ForumThreadEntity threadEntity = new ForumThreadEntity();
                threadEntity.Parse(threadNode);
                forumThreadList.Add(threadEntity);
            }

            if (doc.DocumentNode.Descendants().Any(node => node.GetAttributeValue("id", "").Contains("subforums")))
            {
                forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("id", "").Contains("subforums"));
                foreach (HtmlNode subforumNode in forumNode.Descendants("tr"))
                {
                    if (subforumNode.Descendants("a").Any())
                    {
                        ForumEntity forumSubCategory = new ForumEntity(WebUtility.HtmlDecode(subforumNode.Descendants("a").FirstOrDefault().InnerText), subforumNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", ""), "");
                        forumSubcategoryList.Add(forumSubCategory);
                    }
                }
            }
            forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("bclast"));
            return new ForumCollectionEntity(WebUtility.HtmlDecode(forumNode.InnerText), forumThreadList, forumSubcategoryList);
        }

    }
}
