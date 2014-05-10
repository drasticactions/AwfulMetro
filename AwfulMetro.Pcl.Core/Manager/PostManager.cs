using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Entity;
using HtmlAgilityPack;

namespace AwfulMetro.Pcl.Core.Manager
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

        public async Task<ForumThreadEntity> GetThreadPosts(ForumThreadEntity forumThread)
        {
            try
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

                var forumThreadPosts = new ObservableCollection<ForumPostEntity>();

                //TEMP CODE
                var threadManager = new ThreadManager();
                var doc = await threadManager.GetThread(forumThread, url);

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
                if (threadNode != null)
                {
                    threadNode =
        threadNode.Descendants("option")
            .FirstOrDefault(node => node.GetAttributeValue("selected", string.Empty).Contains("selected"));

                    if (forumThread.CurrentPage <= 0)
                    {
                        forumThread.CurrentPage = GetPageNumber(threadNode);
                    }
                }
                forumThread.ForumPosts = forumThreadPosts;
                return forumThread;
            }
            catch (Exception)
            {
                return null;
            }
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