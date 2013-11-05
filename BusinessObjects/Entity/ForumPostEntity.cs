using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;

namespace AwfulMetro.Core.Entity 
{
    /// <summary>
    /// Represents an individual post in a thread.
    /// </summary>
    public class ForumPostEntity
    {
        public ForumUserEntity User { get; private set; }

        public string PostDate { get; private set; }

        public string PostReportLink { get; private set; }

        public string PostQuoteLink { get; private set; }

        public string PostLink { get; private set; }

        public string PostFormatted { get; private set; }

        public string PostHtml { get; private set; }

        public int PostHeight { get; private set; }

        public long PostId { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool IsMod { get; private set; }

        public bool IsQuoting { get; set; }

        /// <summary>
        /// Parses a forum post in a thread.
        /// </summary>
        /// <param name="postNode">The post HTML node.</param>
        public void Parse(HtmlNode postNode)
        {
            this.User = ForumUserEntity.FromPost(postNode);
            
            HtmlNode postDateNode = postNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postdate"));
            string postDateString = postDateNode == null ? string.Empty : postDateNode.InnerText;
            if(postDateString != null)
            {
                this.PostDate = postDateString.WithoutNewLines().Trim();
            }
            this.PostId = Int64.Parse(postNode.GetAttributeValue("id", string.Empty).Replace("post", string.Empty).Replace("#", string.Empty));
            this.PostFormatted = WebUtility.HtmlDecode(postNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postbody")).InnerHtml).WithoutNewLines();
            this.PostHtml = FixPostHtml(WebUtility.HtmlDecode(postNode.Descendants("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postbody")).InnerHtml));

            /*
             * Attempts to find the height of the post by counting the new lines and 
             * multiplying by twenty. Maybe a good way to get more posts on the screen?
             */
            this.PostHeight = PostHtml.Count(p => p == '\n') * 20;
        }

        /// <summary>
        /// Fixes the missing tags in an pulled post HTML node.
        /// </summary>
        /// <param name="postHtml"></param>
        /// <returns></returns>
        private static string FixPostHtml(String postHtml)
        {
            return "<!DOCTYPE html><html>" + Constants.HTML_HEADER + "<body></head><body>" + postHtml + "</body></html>";
        }
    }
}
