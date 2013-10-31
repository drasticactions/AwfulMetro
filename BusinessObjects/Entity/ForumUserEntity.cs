using BusinessObjects.Tools;

using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace BusinessObjects.Entity
{
    public class ForumUserEntity
    {
        public string Username { get; private set; }

        public string AvatarLink { get; private set; }

        public string AvatarTitle { get; private set; }

        public string UserDateJoined { get; private set; }

        public string UserProfileLink { get; private set; }

        public string UserPrivateMessageLink { get; private set; }

        public string UserPostHistoryLink { get; private set; }

        public string UserRapSheetLink { get; private set; }

        public bool CanSendPrivateMessage { get; private set; }

        public string ICQContactString { get; private set; }

        public string AIMContactString { get; private set; }

        public string YahooContactString { get; private set; }

        public string HomePageString { get; private set; }

        public int PostCount { get; private set; }

        public string LastPostDate { get; private set; }

        public string UserLocation { get; private set; }

        public string AboutUser { get; private set; }

        public bool IsMod { get; private set; }

        public long Id { get; private set; }

        public string PostRate { get; private set; }

        public void ParseFromPost(HtmlNode postNode)
        {
            this.Username = WebUtility.HtmlDecode(postNode.Descendants("dt").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("author")).InnerHtml);
            this.UserDateJoined = postNode.Descendants("dd").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("registered")).InnerHtml;
            if (postNode.Descendants("dd").FirstOrDefault(node => node.GetAttributeValue("class", "").Equals("title")) != null)
            {
                this.AvatarTitle = WebUtility.HtmlDecode(postNode.Descendants("dd").FirstOrDefault(node => node.GetAttributeValue("class", "").Equals("title")).InnerText.WithoutNewLines());
            }
            if (postNode.Descendants("dd").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("title")).Descendants("img").FirstOrDefault() != null)
            {
                this.AvatarLink = FixPostHtmlImage(postNode.Descendants("dd").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("title")).Descendants("img").FirstOrDefault().OuterHtml);
            }
            this.Id = Convert.ToInt64(postNode.DescendantsAndSelf("td").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("userinfo")).GetAttributeValue("class", "").Split('-')[1]);
        }

        public void ParseFromUserProfile(HtmlNode profileNode)
        {
            this.AboutUser = string.Empty;
            foreach (HtmlNode aboutParagraph in profileNode.Descendants("p"))
            {
                this.AboutUser += WebUtility.HtmlDecode(aboutParagraph.InnerText.WithoutNewLines().Trim()) + Environment.NewLine + Environment.NewLine;
                //this.AboutUser += this.RemoveNewLine(aboutParagraph.InnerText);
            }
            HtmlNode additionalNode = profileNode.Descendants("dl").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("additional"));
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
        private static string FixPostHtmlImage(String postHtml)
        {
            return "<!DOCTYPE html><html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/ui-light.css\"></head><body style=\"background-color: white;\"></head><body>" + postHtml + "</body></html>";
        }
    }
}
