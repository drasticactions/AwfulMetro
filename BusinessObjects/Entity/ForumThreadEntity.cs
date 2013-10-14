using BusinessObjects.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entity
{
    /// <summary>
    /// Represents a thread in a forum.
    /// </summary>
    public class ForumThreadEntity
    {
        public String Name { get; private set; }

        public String Location { get; set; }

        public String ImageIconLocation { get; private set; }

        public String Author { get; private set; }

        public int ReplyCount { get; private set; }

        public int ViewCount { get; private set; }

        public int Rating { get; private set; }

        public String KilledBy { get; private set; }

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
            this.Name = WebUtility.HtmlDecode(threadNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("thread_title")).FirstOrDefault().InnerText);
            this.KilledBy = threadNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("author")).LastOrDefault().InnerText;
            this.IsSticky = threadNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Contains("title_sticky")).Any();
            this.IsLocked = threadNode.GetAttributeValue("class", "").Contains("closed");
            this.CanMarkAsUnread = threadNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("x")).Any();
            this.HasBeenViewed = this.CanMarkAsUnread;
            this.Author = threadNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("author")).FirstOrDefault().InnerText;
            if (threadNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("count")).Any())
            {
                this.RepliesSinceLastOpened = Convert.ToInt32(threadNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("count")).FirstOrDefault().InnerText);
            }
            if (threadNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Contains("replies")).Any())
            {
                this.ReplyCount = Convert.ToInt32(threadNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Contains("replies")).FirstOrDefault().InnerText);
            }
            else
            {
                this.ReplyCount = 1;
            }
            this.TotalPages = (this.ReplyCount / 40) + 1;
            this.Location = Constants.BASE_URL + threadNode.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("thread_title")).FirstOrDefault().GetAttributeValue("href","");
            this.ThreadId = Convert.ToInt64(this.Location.Split('=')[1]);
            this.ImageIconLocation = threadNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Equals("icon")).FirstOrDefault().Descendants("img").FirstOrDefault().GetAttributeValue("src", "");
        }

        public void ParseFromPopularThread(String name, long threadId)
        {
            this.Name = name;
            this.ThreadId = threadId;
            this.Location = string.Format(Constants.THREAD_PAGE, threadId);
            this.CurrentPage = 0;
        }
    }
}
