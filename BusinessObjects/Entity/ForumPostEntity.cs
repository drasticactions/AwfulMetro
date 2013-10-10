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
    /// Represents an individual post in a thread.
    /// </summary>
    public class ForumPostEntity
    {

        public ForumUserEntity User { get; private set; }
        public String PostDate { get; private set; }

        public String PostReportLink { get; private set; }

        public String PostQuoteLink { get; private set; }

        public String PostLink { get; private set; }

        public String PostFormatted { get; private set; }

        public String PostHTML { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool IsMod { get; private set; }

        /// <summary>
        /// Parses a forum post in a thread.
        /// </summary>
        /// <param name="postNode">The post HTML node.</param>
        public void Parse(HtmlNode postNode)
        {
            this.User = new ForumUserEntity();
            this.User.ParseFromPost(postNode);
            this.PostHTML = this.FixPostHtml(WebUtility.HtmlDecode(postNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Equals("postbody")).FirstOrDefault().InnerHtml));
        }

        /// <summary>
        /// Fixes the missing tags in an pulled post HTML node.
        /// </summary>
        /// <param name="postHtml"></param>
        /// <returns></returns>
        private String FixPostHtml(String postHtml)
        {
            return "<!DOCTYPE html><html>" + Constants.HTML_HEADER + "<body></head><body>" + postHtml + "</body></html>";
        }
    }
}
