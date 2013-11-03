using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;

namespace AwfulMetro.Core.Entity
{
    /// <summary>
    /// Represents a thread in a forum.
    /// </summary>
    public class ForumThreadEntity
    {
        public string Name { get; private set; }

        public string Location { get; set; }

        public string ImageIconLocation { get; private set; }

        public string Author { get; private set; }

        public int ReplyCount { get; private set; }

        public int ViewCount { get; private set; }

        public int Rating { get; private set; }

        public string KilledBy { get; private set; }

        public bool IsSticky { get; private set; }

        public bool IsLocked { get; private set; }

        public bool HasBeenViewed { get; private set; }

        public bool CanMarkAsUnread { get; private set; }

        public int RepliesSinceLastOpened { get; private set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public int ScrollToPost { get; set; }

        public long ThreadId { get; private set; }

        /// <summary>
        /// Parses a thread HTML node to extract the information from it.
        /// </summary>
        /// <param name="threadNode">The thread HTML node.</param>
        public void Parse(HtmlNode threadNode)
        {
            this.Name = WebUtility.HtmlDecode(threadNode.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("thread_title")).InnerText);

            this.KilledBy = threadNode.Descendants("a").LastOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("author")).InnerText;
            this.IsSticky = threadNode.Descendants("td").Any(node => node.GetAttributeValue("class", string.Empty).Contains("title_sticky"));
            this.IsLocked = threadNode.GetAttributeValue("class", string.Empty).Contains("closed");
            this.CanMarkAsUnread = threadNode.Descendants("a").Any(node => node.GetAttributeValue("class", string.Empty).Equals("x"));
            this.HasBeenViewed = this.CanMarkAsUnread;
            this.Author = threadNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("author")).InnerText;
            if (threadNode.Descendants("a").Any(node => node.GetAttributeValue("class", string.Empty).Equals("count")))
            {
                this.RepliesSinceLastOpened = Convert.ToInt32(threadNode.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("count")).InnerText);
            }
            if (threadNode.Descendants("td").Any(node => node.GetAttributeValue("class", string.Empty).Contains("replies")))
            {
                this.ReplyCount = Convert.ToInt32(threadNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("replies")).InnerText);
            }
            else
            {
                this.ReplyCount = 1;
            }

            // Isn't this user configurable?
            this.TotalPages = (this.ReplyCount / 40) + 1;
            this.Location = Constants.BASE_URL + threadNode.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("thread_title")).GetAttributeValue("href",string.Empty);
            this.ThreadId = Convert.ToInt64(this.Location.Split('=')[1]);
            this.ImageIconLocation = threadNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("icon")).Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
        }

        public void ParseFromPopularThread(string name, long threadId)
        {
            this.Name = name;
            this.ThreadId = threadId;
            this.Location = string.Format(Constants.THREAD_PAGE, threadId);
            this.CurrentPage = 0;
        }
    }
}
