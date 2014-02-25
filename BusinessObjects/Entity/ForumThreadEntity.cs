﻿using System;
using System.Linq;
using System.Net;
using Windows.Data.Html;
using Windows.UI.Xaml.Controls;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    /// <summary>
    ///     Represents a thread in a forum.
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

        public int ForumId { get; set; }

        /// <summary>
        ///     Parses a thread HTML node to extract the information from it.
        /// </summary>
        /// <param name="threadNode">The thread HTML node.</param>
        public void Parse(HtmlNode threadNode)
        {
            Name =
                WebUtility.HtmlDecode(
                    threadNode.Descendants("a")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("thread_title"))
                        .InnerText);

            KilledBy =
                threadNode.Descendants("a")
                    .LastOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("author"))
                    .InnerText;
            IsSticky =
                threadNode.Descendants("td")
                    .Any(node => node.GetAttributeValue("class", string.Empty).Contains("title_sticky"));
            IsLocked = threadNode.GetAttributeValue("class", string.Empty).Contains("closed");
            CanMarkAsUnread =
                threadNode.Descendants("a").Any(node => node.GetAttributeValue("class", string.Empty).Equals("x"));
            HasBeenViewed = CanMarkAsUnread;
            Author =
                threadNode.Descendants("td")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("author"))
                    .InnerText;
            if (threadNode.Descendants("a").Any(node => node.GetAttributeValue("class", string.Empty).Equals("count")))
            {
                RepliesSinceLastOpened =
                    Convert.ToInt32(
                        threadNode.Descendants("a")
                            .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("count"))
                            .InnerText);
            }
            ReplyCount =
                threadNode.Descendants("td")
                    .Any(node => node.GetAttributeValue("class", string.Empty).Contains("replies"))
                    ? Convert.ToInt32(
                        threadNode.Descendants("td")
                            .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("replies"))
                            .InnerText)
                    : 1;
            ViewCount =
                threadNode.Descendants("td")
                    .Any(node => node.GetAttributeValue("class", string.Empty).Contains("views"))
                    ? Convert.ToInt32(
                        threadNode.Descendants("td")
                            .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("views"))
                            .InnerText)
                    : 1;
            RatingImage =
                threadNode.Descendants("td")
                    .Any(node => node.GetAttributeValue("class", string.Empty).Contains("rating")) &&
                threadNode.Descendants("td")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("rating"))
                    .Descendants("img")
                    .Any()
                    ? threadNode.Descendants("td")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("rating"))
                        .Descendants("img")
                        .FirstOrDefault()
                        .GetAttributeValue("src", string.Empty)
                    : null;
            // Isn't this user configurable?
            TotalPages = (ReplyCount/40) + 1;
            Location = Constants.BASE_URL +
                       threadNode.Descendants("a")
                           .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("thread_title"))
                           .GetAttributeValue("href", string.Empty) + Constants.PER_PAGE;
            ThreadId =
                Convert.ToInt64(
                    threadNode.Descendants("a")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("thread_title"))
                        .GetAttributeValue("href", string.Empty)
                        .Split('=')[1]);
            HtmlNode first =
                threadNode.Descendants("td")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("icon"));
            if (first != null)
                ImageIconLocation = first.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
        }

        /// <summary>
        /// If we are coming to this thread from another thread, we need to parse the thread information differently.
        /// </summary>
        /// <param name="threadDocument">The thread HTML Document.</param>
        public void ParseFromThread(HtmlDocument threadDocument)
        {
            var title = threadDocument.DocumentNode.Descendants("title").FirstOrDefault();

            if (title != null)
            {
                Name = title.InnerText;
            }

            var threadIdNode = threadDocument.DocumentNode.Descendants("body").First();
            ThreadId = Convert.ToInt64(threadIdNode.GetAttributeValue("data-thread", string.Empty));

            Location = string.Format(Constants.THREAD_PAGE, this.ThreadId);
            var pageNavigationNode = threadDocument.DocumentNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("pages top"));
            if (string.IsNullOrWhiteSpace(pageNavigationNode.InnerHtml))
            {
                TotalPages = 1;
                CurrentPage = 1;
            }
            else
            {
                var lastPageNode = pageNavigationNode.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("title", string.Empty).Equals("Last page"));
                if (lastPageNode != null)
                {
                    string urlHref = lastPageNode.GetAttributeValue("href", string.Empty);
                    var query = Extensions.ParseQueryString(urlHref);
                    TotalPages = Convert.ToInt32(query["pagenumber"]);
                }

                var pageSelector = pageNavigationNode.Descendants("select").FirstOrDefault();

                var selectedPage = pageSelector.Descendants("option")
                    .FirstOrDefault(node => node.GetAttributeValue("selected", string.Empty).Equals("selected"));

                CurrentPage = Convert.ToInt32(selectedPage.GetAttributeValue("value", string.Empty));
            }
        }

        public void ParseFromPopularThread(string name, long threadId)
        {
            Name = name;
            ThreadId = threadId;
            Location = string.Format(Constants.THREAD_PAGE, threadId);
            CurrentPage = 0;
        }
    }
}