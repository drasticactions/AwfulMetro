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
    public class ForumUserEntity
    {
        public String Username { get; private set; }

        public String AvatarLink { get; private set; }

        public String AvatarTitle { get; private set; }

        public String UserDateJoined { get; private set; }

        public String UserProfileLink { get; private set; }

        public String UserPrivateMessageLink { get; private set; }

        public String UserPostHistoryLink { get; private set; }

        public String UserRapSheetLink { get; private set; }

        public bool CanSendPrivateMessage { get; private set; }

        public String ICQContactString { get; private set; }

        public String AIMContactString { get; private set; }

        public String YahooContactString { get; private set; }

        public String HomePageString { get; private set; }

        public int PostCount { get; private set; }

        public String LastPostDate { get; private set; }

        public String UserLocation { get; private set; }

        public String AboutUser { get; private set; }

        public bool IsMod { get; private set; }

        public long id { get; private set; }

        public string PostRate { get; private set; }

        public void ParseFromPost(HtmlNode postNode)
        {
            this.Username = WebUtility.HtmlDecode(postNode.Descendants("dt").Where(node => node.GetAttributeValue("class", "").Contains("author")).FirstOrDefault().InnerHtml);
            this.UserDateJoined = postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Contains("registered")).FirstOrDefault().InnerHtml;
            if (postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Equals("title")).FirstOrDefault() != null)
            {
                this.AvatarTitle = WebUtility.HtmlDecode(this.RemoveNewLine(postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Equals("title")).FirstOrDefault().InnerText.Trim()));
            }
            if (postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Contains("title")).FirstOrDefault().Descendants("img").FirstOrDefault() != null)
            {
                this.AvatarLink = this.FixPostHtmlImage(postNode.Descendants("dd").Where(node => node.GetAttributeValue("class", "").Contains("title")).FirstOrDefault().Descendants("img").FirstOrDefault().OuterHtml);
            }
            this.id = Convert.ToInt64(postNode.DescendantsAndSelf("td").Where(node => node.GetAttributeValue("class", "").Contains("userinfo")).FirstOrDefault().GetAttributeValue("class", "").Split('-')[1]);
        }

        public void ParseFromUserProfile(HtmlNode profileNode)
        {
            this.AboutUser = string.Empty;
            foreach (HtmlNode aboutParagraph in profileNode.Descendants("p"))
            {
                this.AboutUser += WebUtility.HtmlDecode(this.RemoveNewLine(aboutParagraph.InnerText).Trim()) + System.Environment.NewLine + System.Environment.NewLine;
                //this.AboutUser += this.RemoveNewLine(aboutParagraph.InnerText);
            }
            HtmlNode additionalNode = profileNode.Descendants("dl").Where(node => node.GetAttributeValue("class", "").Contains("additional")).FirstOrDefault();
            this.UserDateJoined = additionalNode.Descendants("dd").FirstOrDefault().InnerText;
            additionalNode.Descendants("dd").FirstOrDefault().Remove();
            this.PostCount = Convert.ToInt32(additionalNode.Descendants("dd").FirstOrDefault().InnerText);
            additionalNode.Descendants("dd").FirstOrDefault().Remove();
            this.PostRate = additionalNode.Descendants("dd").FirstOrDefault().InnerText;
            additionalNode.Descendants("dd").FirstOrDefault().Remove();
            this.LastPostDate = additionalNode.Descendants("dd").FirstOrDefault().InnerText;
            additionalNode.Descendants("dd").FirstOrDefault().Remove();
            if (additionalNode.Descendants("dd").Any())
            {
                this.UserLocation = additionalNode.Descendants("dd").FirstOrDefault().InnerText;
                additionalNode.Descendants("dd").FirstOrDefault().Remove();
            }
        }

        /// <summary>
        /// Fixes the missing tags in an users avatar HTML node.
        /// </summary>
        /// <param name="postHtml"></param>
        /// <returns></returns>
        private String FixPostHtmlImage(String postHtml)
        {
            return "<!DOCTYPE html><html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/ui-light.css\"></head><body style=\"background-color: white;\"></head><body>" + postHtml + "</body></html>";
        }

        private String RemoveNewLine(String text)
        {

            var sb = new StringBuilder(text.Length);
            foreach (char i in text)
            { 
                if (i != '\n' && i != '\r' && i != '\t')
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
