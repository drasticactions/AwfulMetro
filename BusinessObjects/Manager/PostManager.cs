using BusinessObjects.Entity;
using BusinessObjects.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    public class PostManager
    {
        private readonly IWebManager webManager;
        public PostManager(IWebManager webManager)
        {
            this.webManager = webManager;
        }

        public PostManager() : this(new WebManager()) { }

        public async Task<List<ForumPostEntity>> GetThreadPosts(ForumThreadEntity forumThread)
        {
            string url = forumThread.Location;

            if (forumThread.CurrentPage > 0)
            {
                url = forumThread.Location + string.Format(Constants.PAGE_NUMBER, forumThread.CurrentPage);
            }

            List<ForumPostEntity> forumThreadPosts = new List<ForumPostEntity>();
            
            //TEMP CODE
            //Inject
            
            var result = await webManager.DownloadHtml(url);
            HtmlDocument doc = result.Document;
            string responseUri = result.AbsoluteUri;
            
            string[] test = responseUri.Split('#');
            if (test.Length > 1 && test.Contains("pti"))
            {
                forumThread.ScrollToPost = int.Parse(Regex.Match(responseUri.Split('#')[1], @"\d+").Value);
            }
            //HtmlDocument doc = await WebManager.DownloadHtml(request);
            HtmlNode threadNode = doc.DocumentNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("id", "").Contains("thread"));

            foreach (HtmlNode postNode in threadNode.Descendants("table").Where(node => node.GetAttributeValue("class", "").Contains("post")))
            {
                var post = new ForumPostEntity();
                post.Parse(postNode);
                forumThreadPosts.Add(post);
            }

            threadNode = doc.DocumentNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("pages top"));
            threadNode = threadNode.Descendants("option").FirstOrDefault(node => node.GetAttributeValue("selected", "").Contains("selected"));

            if (forumThread.CurrentPage <= 0)
            {
                forumThread.CurrentPage = PostManager.GetPageNumber(threadNode);
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
            return 1;
        }
    }
}
