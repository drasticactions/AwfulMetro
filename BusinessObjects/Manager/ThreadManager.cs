using System.Text.RegularExpressions;
using Windows.UI.Xaml.Controls;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Manager
{
    public class ThreadManager
    {
        private readonly IWebManager _webManager;
        public ThreadManager(IWebManager webManager)
        {
            this._webManager = webManager;
        }

        public ThreadManager() : this(new WebManager()) { }

        public async Task<string> GetThread(ForumThreadEntity forumThread)
        {

            string url = string.Format(Constants.REPLY_BASE, forumThread.ThreadId);
            var result = await _webManager.DownloadHtml(url);
            HtmlDocument doc = result.Document;
            return await GetThreadHtml(doc);
        }

        public async Task<string> GetThreadHtml(HtmlDocument doc)
        {
            var html = await Windows.Storage.PathIO.ReadTextAsync("ms-appx:///Assets/thread.html");

            var doc2 = new HtmlDocument();

            doc2.LoadHtml(html);

            var bodyNode = doc2.DocumentNode.Descendants("body").FirstOrDefault();

            var replyNodes = doc.DocumentNode.Descendants("div").ToArray();

            var threadNode = replyNodes.FirstOrDefault(node => node.GetAttributeValue("id", "").Equals("thread"));

            var threadId = ParseInt(threadNode.GetAttributeValue("class", string.Empty));

            var postNodes =
                threadNode.Descendants("table")
                    .Where(node => node.GetAttributeValue("class", string.Empty).Equals("post "));

            foreach (var post in postNodes)
            {
                var userInfoNode = post.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("userinfo"));

                var userId = ParseInt(userInfoNode.GetAttributeValue("class", string.Empty));

                var postId = ParseInt(post.GetAttributeValue("id", string.Empty));

                

                var profileLinksNode =
                    post.Descendants("td")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postlinks"));
                profileLinksNode.InnerHtml = string.Empty;
                
                var profileButton = WebUtility.HtmlDecode(string.Format("<li><button onclick=\"window.ForumCommand('profile', '{0}');\">Profile</button></li>", userId));

                var postHistoryButton = WebUtility.HtmlDecode(string.Format("<li><button onclick=\"window.ForumCommand('post_history', '{0}');\">Post History</button></li>", userId));

                var rapSheetButton = WebUtility.HtmlDecode(string.Format("<li><button onclick=\"window.ForumCommand('rap_sheet', '{0}');\">Rap Sheet</button></li>", userId));

                var quoteButton = WebUtility.HtmlDecode(string.Format("<li><button onclick=\"window.ForumCommand('quote', '{0}');\">Quote</button></li>", postId));
                profileLinksNode.InnerHtml = string.Concat("<ul class=\"profilelinks\">", profileButton, postHistoryButton, rapSheetButton, quoteButton, "</ul>");

               // var postLinksNode = post.Descendants("ul")
                      //  .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postbuttons"));

             //   postLinksNode.InnerHtml = string.Empty;

                var postDateNode = post.Descendants("td")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postdate"));
                string postDate = postDateNode.InnerText;

                var toPostButton = WebUtility.HtmlDecode(string.Format("<button style=\"min-width:20px;display: inline-block;\" onclick=\"window.TopPost(#{0});\">#</button>", postId));

                var usersPostsInThreadButton = WebUtility.HtmlDecode(string.Format("<button style=\"min-width:20px;display: inline-block;\" onclick=\"window.ForumCommand('users_posts_in_thread', '{0},{1}');\">?</button>", userId, threadId));

               // postDateNode.InnerHtml = string.Concat("<div style=\"display: inline-block;\">", toPostButton, usersPostsInThreadButton, "</div>", postDate);
            }

            bodyNode.InnerHtml = threadNode.OuterHtml;

            return WebUtility.HtmlDecode(doc2.DocumentNode.OuterHtml);
        }

        private int ParseInt(string postClass)
        {
            string re1 = ".*?";	// Non-greedy match on filler
            string re2 = "(\\d+)";	// Integer Number 1

            Regex r = new Regex(re1 + re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(postClass);
            if (!m.Success) return 0;
            String int1 = m.Groups[1].ToString();
            return Convert.ToInt32(int1);
        }

        public async Task<List<ForumThreadEntity>> GetBookmarks(ForumEntity forumCategory, int page)
        {
            var forumSubcategoryList = new List<ForumEntity>();
            var forumThreadList = new List<ForumThreadEntity>();
            String url = Constants.BOOKMARKS_URL;
            if (forumCategory.CurrentPage > 0)
            {
                url = Constants.BOOKMARKS_URL + string.Format(Constants.PAGE_NUMBER, page);
            }

            HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;
            HtmlNode forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("threadlist"));


            foreach (HtmlNode threadNode in forumNode.Descendants("tr").Where(node => node.GetAttributeValue("class", string.Empty).StartsWith("thread")))
            {
                var threadEntity = new ForumThreadEntity();
                threadEntity.Parse(threadNode);
                forumThreadList.Add(threadEntity);
            }
            return forumThreadList;
        }

        public async Task<bool> AddBookmarks(List<long> threadIdList)
        {
            foreach (long threadId in threadIdList)
            {
                await this._webManager.PostData(
                Constants.BOOKMARK, string.Format(
                    Constants.ADD_BOOKMARK, threadId
                   ));
            }
            return true;
        }

        public async Task<bool> RemoveBookmarks(List<long> threadIdList)
        {
            foreach (long threadId in threadIdList)
            {
                await this._webManager.PostData(
                Constants.BOOKMARK, string.Format(
                    Constants.REMOVE_BOOKMARK, threadId
                   ));
            }
            return true;
        }

        public async Task<bool> MarkThreadUnread(List<long> threadIdList)
        {
            foreach (long threadId in threadIdList)
            {
                await this._webManager.PostData(
                Constants.RESET_BASE, string.Format(
                    Constants.RESET_SEEN, threadId
                   ));
            }
            return true;
        }

        public async Task<List<ForumThreadEntity>> GetForumThreads(ForumEntity forumCategory, int page)
        {
            // TODO: Remove parsing logic from managers. I don't think they have a place here...
            var url = forumCategory.Location + string.Format(Constants.PAGE_NUMBER, page);

            HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;

            HtmlNode forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("threadlist"));
            var forumThreadList = new List<ForumThreadEntity>();
            foreach (HtmlNode threadNode in forumNode.Descendants("tr").Where(node => node.GetAttributeValue("class", string.Empty).StartsWith("thread")))
            {
                var threadEntity = new ForumThreadEntity();
                threadEntity.Parse(threadNode);
                forumThreadList.Add(threadEntity);
            }
            return forumThreadList;
        }

    }
}
