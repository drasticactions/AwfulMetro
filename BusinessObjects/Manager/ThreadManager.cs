using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class ThreadManager
    {
        private readonly IWebManager _webManager;

        public ThreadManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public ThreadManager() : this(new WebManager())
        {
        }

        public async Task<string> GetThread(ForumThreadEntity forumThread)
        {
            string url = string.Format(Constants.REPLY_BASE, forumThread.ThreadId);
            WebManager.Result result = await _webManager.DownloadHtml(url);
            HtmlDocument doc = result.Document;
            return await GetThreadHtml(doc);
        }

        private string GetForumThreadCss(int forumId)
        {
            switch (forumId)
            {
                case 219:
                    return "219.css";
                case 26:
                    return "26.css";
                default:
                    return "default-forum.css";
            }
        }

        public async Task<string> GetThreadHtml(HtmlDocument doc)
        {
            string html = await PathIO.ReadTextAsync("ms-appx:///Assets/thread.html");

            var doc2 = new HtmlDocument();

            doc2.LoadHtml(html);

            HtmlNode bodyNode = doc2.DocumentNode.Descendants("body").FirstOrDefault();

            // Get thread body.
            var forumBodyNode = doc.DocumentNode.Descendants("body").FirstOrDefault();



            if (forumBodyNode != null)
            {
                var forumIdNode = forumBodyNode.GetAttributeValue("data-forum", string.Empty);
                if (!string.IsNullOrEmpty(forumIdNode))
                {
                    var forumId = Convert.ToInt32(forumIdNode);
                    var headNode = doc2.DocumentNode.Descendants("head").FirstOrDefault();
                    var cssNode = doc2.CreateElement("link");
                    headNode.AppendChild(cssNode);
                    cssNode.SetAttributeValue("href",
                        string.Format("ms-appx-web:///Assets/{0}", GetForumThreadCss(forumId)));
                    cssNode.SetAttributeValue("media", "all");
                    cssNode.SetAttributeValue("rel", "stylesheet");
                    cssNode.SetAttributeValue("type", "text/css");
                }
            }

            HtmlNode[] replyNodes = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("id", "").Equals("thread")).ToArray();

            HtmlNode threadNode = replyNodes.FirstOrDefault(node => node.GetAttributeValue("id", "").Equals("thread"));

            int threadId = ParseInt(threadNode.GetAttributeValue("class", string.Empty));

            IEnumerable<HtmlNode> postNodes =
                threadNode.Descendants("table")
                    .Where(node => node.GetAttributeValue("class", string.Empty).Contains("post"));

            // Some thread pages have malformed HTML, which causes HTML Agility pack to get extra nodes we don't want
            // So instead of directly editing the node, we'll get each post and create a new Html string.
            string postHtml = string.Empty;

            foreach (HtmlNode post in postNodes)
            {
                HtmlNode userInfoNode =
                    post.Descendants("td")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("userinfo")) ??
                    post.Descendants("dl")
                            .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("userinfo"));

                int userId = ParseInt(userInfoNode.GetAttributeValue("class", string.Empty));

                int postId = ParseInt(post.GetAttributeValue("id", string.Empty));

                //TODO: Create HTML Render engine to handle this, rather than hard coding. That way it can be used for the front page or other web views.

                HtmlNode profileLinksNode =
                    post.Descendants("td")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postlinks"));
                bool isCurrentUserPost =
                    profileLinksNode.Descendants("img")
                        .FirstOrDefault(node => node.GetAttributeValue("alt", string.Empty).Equals("Edit")) != null;
                profileLinksNode.InnerHtml = string.Empty;

                string clickHandler = string.Format("window.ForumCommand('profile', '{0}')", userId);

                string profileButton = HtmlButtonBuilder.CreateSubmitButton("Profile", clickHandler);

                clickHandler = string.Format("window.ForumCommand('rap_sheet', '{0}')", userId);

                string rapSheetButton = HtmlButtonBuilder.CreateSubmitButton("Rap Sheet", clickHandler);

                clickHandler = string.Format("window.ForumCommand('quote', '{0}')", postId);

                string quoteButton = HtmlButtonBuilder.CreateSubmitButton("Quote", clickHandler);

                clickHandler = string.Format("window.ForumCommand('edit', '{0}')", postId);

                string editButton = HtmlButtonBuilder.CreateSubmitButton("Edit", clickHandler);

                clickHandler = string.Format("window.ForumCommand('post_history', '{0}')", userId);

                string postHistoryButton = HtmlButtonBuilder.CreateSubmitButton("Post History", clickHandler);

                profileLinksNode.InnerHtml = isCurrentUserPost
                    ? string.Concat("<ul class=\"profilelinks\">", profileButton, postHistoryButton, rapSheetButton,
                        quoteButton, editButton, "</ul>")
                    : string.Concat("<ul class=\"profilelinks\">", profileButton, postHistoryButton, rapSheetButton,
                        quoteButton, "</ul>");

                HtmlNode postDateNode = post.Descendants("td")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postdate"));
                string postDate = postDateNode.InnerText;

                string toPostButton =
                    WebUtility.HtmlDecode(
                        string.Format(
                            "<button style=\"min-width:20px;display: inline-block;\" onclick=\"window.TopPost(#{0});\">#</button>",
                            postId));

                string usersPostsInThreadButton =
                    WebUtility.HtmlDecode(
                        string.Format(
                            "<button style=\"min-width:20px;display: inline-block;\" onclick=\"window.ForumCommand('users_posts_in_thread', '{0},{1}');\">?</button>",
                            userId, threadId));

                //postDateNode.InnerHtml = string.Concat("<div style=\"display: inline-block;\">", toPostButton, usersPostsInThreadButton, "</div>", postDate);
                postDateNode.InnerHtml = postDate;
                postHtml += post.OuterHtml;
            }

            bodyNode.InnerHtml = postHtml;

            return WebUtility.HtmlDecode(WebUtility.HtmlDecode(doc2.DocumentNode.OuterHtml));
        }

        private int ParseInt(string postClass)
        {
            string re1 = ".*?"; // Non-greedy match on filler
            string re2 = "(\\d+)"; // Integer Number 1

            var r = new Regex(re1 + re2, RegexOptions.IgnoreCase | RegexOptions.Singleline);
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
            HtmlNode forumNode =
                doc.DocumentNode.Descendants()
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("threadlist"));


            foreach (
                HtmlNode threadNode in
                    forumNode.Descendants("tr")
                        .Where(node => node.GetAttributeValue("class", string.Empty).StartsWith("thread")))
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
                await _webManager.PostData(
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
                await _webManager.PostData(
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
                await _webManager.PostData(
                    Constants.RESET_BASE, string.Format(
                        Constants.RESET_SEEN, threadId
                        ));
            }
            return true;
        }

        public async Task<List<ForumThreadEntity>> GetForumThreads(ForumEntity forumCategory, int page)
        {
            // TODO: Remove parsing logic from managers. I don't think they have a place here...
            string url = forumCategory.Location + string.Format(Constants.PAGE_NUMBER, page);

            HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;

            HtmlNode forumNode =
                doc.DocumentNode.Descendants()
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("threadlist"));
            var forumThreadList = new List<ForumThreadEntity>();
            foreach (
                HtmlNode threadNode in
                    forumNode.Descendants("tr")
                        .Where(node => node.GetAttributeValue("class", string.Empty).StartsWith("thread")))
            {
                var threadEntity = new ForumThreadEntity();
                threadEntity.Parse(threadNode);
                forumThreadList.Add(threadEntity);
            }
            return forumThreadList;
        }

        public async Task<NewThreadEntity> GetThreadCookies(long forumId)
        {
            try
            {
                string url = string.Format(Constants.NEW_THREAD, forumId);
                WebManager.Result result = await _webManager.DownloadHtml(url);
                HtmlDocument doc = result.Document;

                HtmlNode[] formNodes = doc.DocumentNode.Descendants("input").ToArray();

                HtmlNode formKeyNode =
                    formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("formkey"));

                HtmlNode formCookieNode =
                    formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("form_cookie"));

                var newForumEntity = new NewThreadEntity();
                try
                {
                    string formKey = formKeyNode.GetAttributeValue("value", "");
                    string formCookie = formCookieNode.GetAttributeValue("value", "");
                    newForumEntity.FormKey = formKey;
                    newForumEntity.FormCookie = formCookie;
                    return newForumEntity;
                }
                catch (Exception)
                {
                    throw new InvalidOperationException("Could not parse new thread form data.");
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CreateNewThread(NewThreadEntity newThreadEntity)
        {
            if (newThreadEntity == null)
                return false;
            var form = new MultipartFormDataContent
            {
                {new StringContent("postthread"), "action"},
                {new StringContent(newThreadEntity.Forum.ForumId.ToString(CultureInfo.InvariantCulture)), "forumid"},
                {new StringContent(newThreadEntity.FormKey), "formkey"},
                {new StringContent(newThreadEntity.FormCookie), "form_cookie"},
                {new StringContent(newThreadEntity.PostIcon.Id.ToString(CultureInfo.InvariantCulture)), "iconid"},
                {new StringContent(HtmlEncode(newThreadEntity.Subject)), "subject"},
                {new StringContent(HtmlEncode(newThreadEntity.Content)), "message"},
                {new StringContent(newThreadEntity.ParseUrl.ToString()), "parseurl"},
                {new StringContent("Submit Reply"), "submit"}
            };
            HttpResponseMessage response = await _webManager.PostFormData(Constants.NEW_THREAD_BASE, form);

            return response.IsSuccessStatusCode;
        }

        public async Task<string> CreateNewThreadPreview(NewThreadEntity newThreadEntity)
        {
            if (newThreadEntity == null)
                return string.Empty;
            var form = new MultipartFormDataContent
            {
                {new StringContent("postthread"), "action"},
                {new StringContent(newThreadEntity.Forum.ForumId.ToString(CultureInfo.InvariantCulture)), "forumid"},
                {new StringContent(newThreadEntity.FormKey), "formkey"},
                {new StringContent(newThreadEntity.FormCookie), "form_cookie"},
                {new StringContent(newThreadEntity.PostIcon.Id.ToString(CultureInfo.InvariantCulture)), "iconid"},
                {new StringContent(HtmlEncode(newThreadEntity.Subject)), "subject"},
                {new StringContent(HtmlEncode(newThreadEntity.Content)), "message"},
                {new StringContent(newThreadEntity.ParseUrl.ToString()), "parseurl"},
                {new StringContent("Submit Post"), "submit"},
                {new StringContent("Preview Post"), "preview"}
            };

            // We post to SA the same way we would for a normal reply, but instead of getting a redirect back to the
            // thread, we'll get redirected to back to the reply screen with the preview message on it.
            // From here we can parse that preview and return it to the user.

            HttpResponseMessage response = await _webManager.PostFormData(Constants.NEW_THREAD_BASE, form);
            Stream stream = await response.Content.ReadAsStreamAsync();
            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlNode[] replyNodes = doc.DocumentNode.Descendants("div").ToArray();

                HtmlNode previewNode =
                    replyNodes.FirstOrDefault(node => node.GetAttributeValue("class", "").Equals("inner postbody"));
                return previewNode == null ? string.Empty : FixPostHtml(previewNode.OuterHtml);
            }
        }

        private static string FixPostHtml(String postHtml)
        {
            return "<!DOCTYPE html><html>" + Constants.HTML_HEADER + "<body>" + postHtml + "</body></html>";
        }

        public static string HtmlEncode(string text)
        {
            // In order to get Unicode characters fully working, we need to first encode the entire post.
            // THEN we decode the bits we can safely pass in, like single/double quotes.
            // If we don't, the post format will be screwed up.
            char[] chars = WebUtility.HtmlEncode(text).ToCharArray();
            var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (char c in chars)
            {
                int value = Convert.ToInt32(c);
                if (value > 127)
                    result.AppendFormat("&#{0};", value);
                else
                    result.Append(c);
            }

            result.Replace("&quot;", "\"");
            result.Replace("&#39;", @"'");
            result.Replace("&lt;", @"<");
            result.Replace("&gt;", @">");
            return result.ToString();
        }
    }
}