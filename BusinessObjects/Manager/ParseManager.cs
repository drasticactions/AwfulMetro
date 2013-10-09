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
    public class ParseManager
    {

        const string PAGE_NUMBER = "&pagenumber=";

        public static async Task<HtmlDocument> DownloadHtml(HttpWebRequest request)
        {
            var response = await request.GetResponseAsync();
            var stream = response.GetResponseStream();
            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                return doc;
            }
        }
        public static async Task<List<ForumCategoryEntity>> GetForumCategory()
        {
            List<ForumCategoryEntity> forumGroupList = new List<ForumCategoryEntity>();
            List<ForumEntity> forumSubcategoryList = new List<ForumEntity>();
            HttpWebRequest request = await AuthManager.CreateGetRequest(Constants.BASE_URL);
            HtmlDocument doc = await ParseManager.DownloadHtml(request);
            HtmlNode forumNode = doc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("id", "").Contains("forums")).FirstOrDefault();
            
            foreach (HtmlNode link in forumNode.Descendants("tr"))
            {
                if (link.Descendants("th").Any())
                {
                    ForumCategoryEntity forumGroup = new ForumCategoryEntity(WebUtility.HtmlDecode(link.Descendants("a").FirstOrDefault().InnerText), link.Descendants("a").FirstOrDefault().GetAttributeValue("href", ""));
                    forumGroupList.Add(forumGroup);
                }
                else if (link.Descendants("td").Any())
                {
                    ForumEntity forumSubCategory = new ForumEntity(WebUtility.HtmlDecode(link.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("forum")).FirstOrDefault().InnerText), link.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("forum")).FirstOrDefault().GetAttributeValue("href", ""), link.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("forum")).FirstOrDefault().GetAttributeValue("title", ""));
                    forumGroupList.LastOrDefault().ForumList.Add(forumSubCategory);
                }
            }
            return forumGroupList;
        }

        public static async Task<ForumCollectionEntity> GetForumThreadsAndSubforums(ForumEntity forumCategory)
        {
            List<ForumEntity> forumSubcategoryList = new List<ForumEntity>();
            List<ForumThreadEntity> forumThreadList = new List<ForumThreadEntity>();
            String url = forumCategory.Location;
            if (forumCategory.CurrentPage > 0)
            {
                url = forumCategory.Location + PAGE_NUMBER + forumCategory.CurrentPage;
            }
            
            HttpWebRequest request = await AuthManager.CreateGetRequest(url);
            HtmlDocument doc = await ParseManager.DownloadHtml(request);
            HtmlNode pageNode = doc.DocumentNode.Descendants("select").FirstOrDefault();
            
            forumCategory.TotalPages = Convert.ToInt32(pageNode.Descendants("option").LastOrDefault().GetAttributeValue("value", ""));
            
            HtmlNode forumNode = doc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("class", "").Contains("threadlist")).FirstOrDefault();
           
            foreach(HtmlNode threadNode in forumNode.Descendants("tr").Where(node => node.GetAttributeValue("class","").Equals("thread")))
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

        public static async Task<List<ForumPostEntity>> GetThreadPosts(ForumThreadEntity forumThread)
        {
            String url = forumThread.Location;
            
            if(forumThread.CurrentPage > 0)
            {
                url = forumThread.Location + PAGE_NUMBER + forumThread.CurrentPage;
            }
            
            List<ForumPostEntity> forumThreadPosts = new List<ForumPostEntity>();
            HttpWebRequest request = await AuthManager.CreateGetRequest(url);
            
            HtmlDocument doc = await ParseManager.DownloadHtml(request);
            HtmlNode threadNode = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("id", "").Contains("thread")).FirstOrDefault();
            
            foreach (HtmlNode postNode in threadNode.Descendants("table").Where(node => node.GetAttributeValue("class", "").Contains("post")))
            {
                ForumPostEntity post = new ForumPostEntity();
                post.Parse(postNode);
                forumThreadPosts.Add(post);
            }
            
            threadNode = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("pages top")).FirstOrDefault();
            threadNode = threadNode.Descendants("option").Where(node => node.GetAttributeValue("selected", "").Contains("selected")).FirstOrDefault();
            
            if(forumThread.CurrentPage <= 0)
            {
                forumThread.CurrentPage = ParseManager.GetPageNumber(threadNode);
            }

            HtmlNode pageNode = doc.DocumentNode.Descendants("select").FirstOrDefault();
            forumThread.TotalPages = Convert.ToInt32(pageNode.Descendants("option").LastOrDefault().GetAttributeValue("value", ""));

            return forumThreadPosts;
        }

        private static int GetPageNumber(HtmlNode threadNode)
        {
            if (threadNode != null && !string.IsNullOrEmpty(threadNode.GetAttributeValue("value", "")))
            {

                return Convert.ToInt32(threadNode.GetAttributeValue("value", ""));
            }
            else
            {
                return 1;
            }
        }
    }
}
