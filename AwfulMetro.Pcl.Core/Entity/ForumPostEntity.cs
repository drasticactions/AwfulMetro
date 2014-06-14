using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Pcl.Core.Entity
{
    /// <summary>
    ///     Represents an individual post in a thread.
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

        public long PostIndex { get; private set; }

        public string PostDivString { get; private set; }

        public bool HasSeen { get; private set; }

        public bool IsQuoting { get; set; }

        /// <summary>
        ///     Parses a forum post in a thread.
        /// </summary>
        /// <param name="postNode">The post HTML node.</param>
        public void Parse(HtmlNode postNode)
        {
            User = ForumUserEntity.FromPost(postNode);

            HtmlNode postDateNode =
                postNode.Descendants()
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postdate"));
            string postDateString = postDateNode == null ? string.Empty : postDateNode.InnerText;
            if (postDateString != null)
            {
                PostDate = postDateString.WithoutNewLines().Trim();
            }

            PostIndex = ParseInt(postNode.GetAttributeValue("data-idx", string.Empty));

            var postId = postNode.GetAttributeValue("id", string.Empty);
            if (!string.IsNullOrEmpty(postId) && postId.Contains("#"))
            {
                PostId =
                    Int64.Parse(postNode.GetAttributeValue("id", string.Empty)
                        .Replace("post", string.Empty)
                        .Replace("#", string.Empty));
            }
            else if (!string.IsNullOrEmpty(postId) && postId.Contains("post"))
            {
                PostId =
                    Int64.Parse(postNode.GetAttributeValue("id", string.Empty)
                        .Replace("post", string.Empty));
            }
            else
            {
                PostId = 0;
            }
         
            var postBodyNode = postNode.Descendants("td")
                .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postbody"));
            this.FixQuotes(postBodyNode);
            PostHtml = postBodyNode.InnerHtml;
            HtmlNode profileLinksNode =
                    postNode.Descendants("td")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("postlinks"));
            HtmlNode postRow =
                postNode.Descendants("tr").FirstOrDefault();

            if (postRow != null)
            {
                HasSeen = postRow.GetAttributeValue("class", string.Empty).Contains("seen");
            }

            User.IsCurrentUserPost =
                profileLinksNode.Descendants("img")
                    .FirstOrDefault(node => node.GetAttributeValue("alt", string.Empty).Equals("Edit")) != null;
        }

        private void FixQuotes(HtmlNode postNode)
        {
            var quoteList =
                postNode.Descendants("a")
                    .Where(node => node.GetAttributeValue("class", string.Empty).Contains("quote_link"));
            foreach (var quote in quoteList)
            {
                int postId = ParseInt(quote.GetAttributeValue("href", string.Empty));
                quote.Attributes.Remove("href");
                quote.Attributes.Append("href", "javascript:void(0)");
                string postIdFormat = string.Concat("'#postId", postId, "'");
                quote.Attributes.Add("onclick", string.Format("window.ForumCommand('scrollToPost', '{0}')", postId));
            }
        }

        /// <summary>
        ///     Fixes the missing tags in an pulled post HTML node.
        /// </summary>
        /// <param name="postHtml"></param>
        /// <returns></returns>
        private static string FixPostHtml(String postHtml)
        {
            return "<!DOCTYPE html><html>" + Constants.HTML_HEADER + "<body>" + postHtml + "</body></html>";
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
    }
}