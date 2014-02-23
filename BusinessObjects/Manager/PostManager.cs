using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class PostManager
    {
        private readonly IWebManager _webManager;

        public PostManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public PostManager() : this(new WebManager())
        {
        }

        public async Task<string> GetThreadPostInformation(ForumThreadEntity forumThread)
        {
            string url = forumThread.Location;

            if (forumThread.CurrentPage > 0)
            {
                url = forumThread.Location + string.Format(Constants.PAGE_NUMBER, forumThread.CurrentPage);
            }
            else if (forumThread.HasBeenViewed)
            {
                url = forumThread.Location + Constants.GOTO_NEW_POST;
            }




            WebManager.Result result = await _webManager.DownloadHtml(url);
            HtmlDocument doc = result.Document;
            string responseUri = result.AbsoluteUri;

            /* TODO: The following checks the thread URL for "pti" (which indicated which post to scroll to)
           * Having it in the post manager though, is wrong. This needs to be refactored and a better method of 
           * getting this needs to be put in.
           */
            string[] test = responseUri.Split('#');
            if (test.Length > 1 && test[1].Contains("pti"))
            {
                forumThread.ScrollToPost = Int32.Parse(Regex.Match(responseUri.Split('#')[1], @"\d+").Value) - 1;
                forumThread.ScrollToPostString = "#" + test[1];
            }

            HtmlNode threadNode =
                doc.DocumentNode.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("pages top"));
            threadNode =
                threadNode.Descendants("option")
                    .FirstOrDefault(node => node.GetAttributeValue("selected", string.Empty).Contains("selected"));
            if (forumThread.CurrentPage <= 0)
            {
                forumThread.CurrentPage = GetPageNumber(threadNode);
            }

            HtmlNode[] pageNodes = doc.DocumentNode.Descendants("div").ToArray();
            HtmlNode pageNode =
                pageNodes.FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("pages top"));
            pageNode = pageNode.Descendants("option").LastOrDefault();
            forumThread.TotalPages = pageNode == null
                ? 1
                : Convert.ToInt32(pageNode.GetAttributeValue("value", string.Empty));

            var threadManager = new ThreadManager();
            return await threadManager.GetThreadHtml(doc);
        }

        public async Task<List<ForumPostEntity>> GetThreadPosts(ForumThreadEntity forumThread)
        {
            //TODO: No longer being used, needs to be removed.
            string url = forumThread.Location;

            if (forumThread.CurrentPage > 0)
            {
                url = forumThread.Location + string.Format(Constants.PAGE_NUMBER, forumThread.CurrentPage);
            }
            else if (forumThread.HasBeenViewed)
            {
                url = forumThread.Location + Constants.GOTO_NEW_POST;
            }

            var forumThreadPosts = new List<ForumPostEntity>();

            //TEMP CODE
            WebManager.Result result = await _webManager.DownloadHtml(url);
            HtmlDocument doc = result.Document;
            string responseUri = result.AbsoluteUri;

            /* TODO: The following checks the thread URL for "pti" (which indicated which post to scroll to)
             * Having it in the post manager though, is wrong. This needs to be refactored and a better method of 
             * getting this needs to be put in.
             */
            string[] test = responseUri.Split('#');
            if (test.Length > 1 && test[1].Contains("pti"))
            {
                forumThread.ScrollToPost = Int32.Parse(Regex.Match(responseUri.Split('#')[1], @"\d+").Value) - 1;
            }
            HtmlNode threadNode =
                doc.DocumentNode.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("thread"));

            foreach (
                HtmlNode postNode in
                    threadNode.Descendants("table")
                        .Where(node => node.GetAttributeValue("class", string.Empty).Contains("post")))
            {
                var post = new ForumPostEntity();
                post.Parse(postNode);
                forumThreadPosts.Add(post);
            }

            threadNode =
                doc.DocumentNode.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("pages top"));
            threadNode =
                threadNode.Descendants("option")
                    .FirstOrDefault(node => node.GetAttributeValue("selected", string.Empty).Contains("selected"));

            if (forumThread.CurrentPage <= 0)
            {
                forumThread.CurrentPage = GetPageNumber(threadNode);
            }

            HtmlNode pageNode = doc.DocumentNode.Descendants("select").FirstOrDefault();
            forumThread.TotalPages = forumThread.CurrentPage <= 1
                ? 1
                : Convert.ToInt32(pageNode.Descendants("option")
                    .LastOrDefault()
                    .GetAttributeValue("value", string.Empty));

            return forumThreadPosts;
        }

        private static int GetPageNumber(HtmlNode threadNode)
        {
            if (threadNode != null && !string.IsNullOrEmpty(threadNode.GetAttributeValue("value", string.Empty)))
            {
                return Convert.ToInt32(threadNode.GetAttributeValue("value", string.Empty));
            }
            return 1;
        }
    }
}