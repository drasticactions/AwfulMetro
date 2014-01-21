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
        public string Name { get; set; }

        public string Location { get; set; }

        public string ImageIconLocation { get; set; }

        public string Author { get; set; }

        public int ReplyCount { get; set; }

        public int ViewCount { get; set; }

        public int Rating { get; set; }

        public string RatingImage { get; set; }

        public string KilledBy { get; set; }

        public bool IsSticky { get; set; }

        public bool IsLocked { get; set; }

        public bool HasBeenViewed { get; set; }

        public bool CanMarkAsUnread { get; set; }

        public int RepliesSinceLastOpened { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public int ScrollToPost { get; set; }

        public string ScrollToPostString { get; set; }

        public long ThreadId { get; set; }

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
            this.ReplyCount = threadNode.Descendants("td").Any(node => node.GetAttributeValue("class", string.Empty).Contains("replies")) ? Convert.ToInt32(threadNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("replies")).InnerText) : 1;
            this.ViewCount = threadNode.Descendants("td").Any(node => node.GetAttributeValue("class", string.Empty).Contains("views")) ? Convert.ToInt32(threadNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("views")).InnerText) : 1;
            this.RatingImage = threadNode.Descendants("td").Any(node => node.GetAttributeValue("class", string.Empty).Contains("rating")) && threadNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("rating")).Descendants("img").Any() ? threadNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("rating")).Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty) : null;
            // Isn't this user configurable?
            //this.TotalPages = (this.ReplyCount / 40) + 1;
            this.Location = Constants.BASE_URL + threadNode.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("thread_title")).GetAttributeValue("href",string.Empty) + Constants.PER_PAGE;
            this.ThreadId = Convert.ToInt64(threadNode.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("thread_title")).GetAttributeValue("href",string.Empty).Split('=')[1]);
            HtmlNode first = threadNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("icon"));
            if(first != null)
            this.ImageIconLocation = first.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
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
