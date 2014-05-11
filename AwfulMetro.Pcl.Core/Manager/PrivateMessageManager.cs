using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Entity;
using AwfulMetro.Pcl.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class PrivateMessageManager
    {
          private readonly IWebManager _webManager;

        public PrivateMessageManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public PrivateMessageManager()
            : this(new WebManager())
        {
        }

        public async Task<bool> SendPrivateMessage(NewPrivateMessageEntity newPrivateMessageEntity)
        {
            if (newPrivateMessageEntity == null)
                return false;
            var form = new MultipartFormDataContent
            {
                {new StringContent("dosend"), "action"},
                {new StringContent(newPrivateMessageEntity.Receiver), "touser"},
                {new StringContent(newPrivateMessageEntity.Icon.Id.ToString(CultureInfo.InvariantCulture)), "iconid"},
                {new StringContent(HtmlEncode(newPrivateMessageEntity.Title)), "title"},
                {new StringContent(HtmlEncode(newPrivateMessageEntity.Body)), "message"},
                {new StringContent("yes"), "parseurl"},
                {new StringContent("yes"), "parseurl"},
                {new StringContent("Send Message"), "submit"}
            };
            HttpResponseMessage response = await _webManager.PostFormData(Constants.NEW_PRIVATE_MESSAGE_BASE, form);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PrivateMessageEntity>> GetPrivateMessages(int page)
        {
            var privateMessageEntities = new List<PrivateMessageEntity>();
            var url = Constants.PRIVATE_MESSAGES;
            if (page > 0)
            {
                url = Constants.PRIVATE_MESSAGES + string.Format(Constants.PAGE_NUMBER, page);
            }

            HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;

            HtmlNode forumNode =
                doc.DocumentNode.Descendants("tbody").FirstOrDefault();


            foreach (
                HtmlNode threadNode in
                    forumNode.Descendants("tr"))
            {
                var threadEntity = new PrivateMessageEntity();
                threadEntity.Parse(threadNode);
                privateMessageEntities.Add(threadEntity);
            }
            return privateMessageEntities;
        }

        public async Task<string> GetPrivateMessageHtml(string url)
        {

            string html = await PathIO.ReadTextAsync("ms-appx:///Assets/thread.html");

            var doc2 = new HtmlDocument();

            doc2.LoadHtml(html);

            HtmlNode bodyNode = doc2.DocumentNode.Descendants("body").FirstOrDefault();

            HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;

            HtmlNode[] replyNodes = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("id", "").Equals("thread")).ToArray();

            HtmlNode threadNode = replyNodes.FirstOrDefault(node => node.GetAttributeValue("id", "").Equals("thread"));

            int threadId = ParseInt(threadNode.GetAttributeValue("class", string.Empty));

            IEnumerable<HtmlNode> postNodes =
                threadNode.Descendants("table")
                    .Where(node => node.GetAttributeValue("class", string.Empty).Contains("post"));
          ObservableCollection<ForumPostEntity> postList = new ObservableCollection<ForumPostEntity>();
            foreach (
     HtmlNode postNode in
         postNodes)
            {
                var post = new ForumPostEntity();
                post.Parse(postNode);
                postList.Add(post);
            }

            return await HtmlFormater.FormatThreadHtml(postList);
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

        private static string HtmlEncode(string text)
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
