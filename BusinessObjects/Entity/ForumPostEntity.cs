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
            this.User = new ForumUserEntity();
            this.User.ParseFromPost(postNode);
            HtmlNode postDateNode = postNode.Descendants()
                          .Where(node => node.GetAttributeValue("class", "").Equals("postdate"))
                          .FirstOrDefault();
            string postDateString = postDateNode == null ? string.Empty : postDateNode.InnerText;
            if(postDateString != null)
            {
                this.PostDate = this.RemoveNewLine(postDateString).Trim();
            }
            this.PostId = Int64.Parse(postNode.GetAttributeValue("id", "").Replace("post", "").Replace("#", ""));
            this.PostFormatted = this.RemoveNewLine(WebUtility.HtmlDecode(postNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Equals("postbody")).FirstOrDefault().InnerHtml));
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

        private String RemoveNewLine(String text)
        {

            var sb = new StringBuilder(text.Length);
            foreach (char i in text)
            {
                if (i != '\n' && i != '\r' && i != '\t' && i != '#' && i != '?')
                {
                    sb.Append(i);
                }
                else if (i == '\n')
                {
                    sb.Append(' ');
                }
            }
            return sb.ToString();
        }
    }
}
