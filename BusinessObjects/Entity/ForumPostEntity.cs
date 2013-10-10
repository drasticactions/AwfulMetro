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
        public String Username { get; private set; }

        public String AvatarLink { get; private set; }

        public String AvatarTitle { get; private set; }

        public String UserDateJoined { get; private set; }

        public String PostDate { get; private set; }

        public String UserProfileLink { get; private set; }

        public String UserPrivateMessageLink { get; private set; }

        public String UserPostHistoryLink { get; private set; }

        public String UserRapSheetLink { get; private set; }

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
            this.Username = WebUtility.HtmlDecode(postNode.Descendants("dt").Where(node => node.GetAttributeValue("class", "").Contains("author")).FirstOrDefault().InnerHtml);
            this.UserDateJoined = postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Contains("registered")).FirstOrDefault().InnerHtml;
            if (postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Equals("title")).FirstOrDefault() != null)
            {
                this.AvatarTitle = WebUtility.HtmlDecode(this.RemoveNewLine(postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Equals("title")).FirstOrDefault().InnerText));
            }
            if (postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Contains("title")).FirstOrDefault().Descendants("img").FirstOrDefault() != null)
            {
                this.AvatarLink = this.FixPostHtmlImage(postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Contains("title")).FirstOrDefault().Descendants("img").FirstOrDefault().OuterHtml);
            }
            this.PostHTML = this.FixPostHtml(WebUtility.HtmlDecode(postNode.Descendants("td").Where(node => node.GetAttributeValue("class", "").Equals("postbody")).FirstOrDefault().InnerHtml));
        }

        /// <summary>
        /// Fixes the missing tags in an pulled post HTML node.
        /// </summary>
        /// <param name="postHtml"></param>
        /// <returns></returns>
        private String FixPostHtml(String postHtml)
        {
            return "<!DOCTYPE html><html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/ui-light.css\"></head><body></head><body>" + postHtml + "</body></html>";
        }

        /// <summary>
        /// Fixes the missing tags in an users avatar HTML node.
        /// </summary>
        /// <param name="postHtml"></param>
        /// <returns></returns>
        private String FixPostHtmlImage(String postHtml)
        {
            return "<!DOCTYPE html><html>" + Constants.HTML_HEADER + "<body style=\"background-color: rgb(29, 29, 29);\"></head><body>" + postHtml + "</body></html>";
        }

        private String RemoveNewLine (String text)
        {
            
            var sb = new StringBuilder(text.Length);
            foreach (char i in text)
                if (i != '\n' && i != '\r' && i != '\t')
                    sb.Append(i);
            return sb.ToString();
        }
    }
}
