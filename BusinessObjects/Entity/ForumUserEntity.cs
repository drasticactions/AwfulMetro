using System.Collections.Generic;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;

namespace AwfulMetro.Core.Entity
{
    // Food for thought: This might be better served as a ForumUser entity, with a ForumUserProfile subclass.
    public class ForumUserEntity
    {
        private ForumUserEntity() { }

        public string Username { get; private set; }

        public string AvatarLink { get; private set; }

        public string AvatarTitle { get; private set; }

        //This should really be a datetime
        public string DateJoined { get; private set; }

        public string ProfileLink { get; private set; }

        public string PrivateMessageLink { get; private set; }

        public string PostHistoryLink { get; private set; }

        public string RapSheetLink { get; private set; }

        public bool CanSendPrivateMessage { get; private set; }

        public string ICQContactString { get; private set; }

        public string AIMContactString { get; private set; }

        public string YahooContactString { get; private set; }

        public string HomePageString { get; private set; }

        public int PostCount { get; private set; }

        //Should be datetime
        public string LastPostDate { get; private set; }

        public string Location { get; private set; }

        public string AboutUser { get; private set; }

        public bool IsMod { get; private set; }

        public long Id { get; private set; }

        public string PostRate { get; private set; }

        public string SellerRating { get; private set; }

        public static ForumUserEntity FromPost(HtmlNode postNode)
        {
            var user = new ForumUserEntity();
            user.Username = WebUtility.HtmlDecode(postNode.Descendants("dt").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author")).InnerHtml);
            user.DateJoined = postNode.Descendants("dd").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("registered")).InnerHtml;
            var avatarTitle = postNode.Descendants("dd").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("title"));
            var avatarImage = postNode.Descendants("dd").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("title")).Descendants("img").FirstOrDefault();

            if (avatarTitle != null)
            {
                user.AvatarTitle = WebUtility.HtmlDecode(avatarTitle.InnerText).WithoutNewLines().Trim();
            }
            if (avatarImage != null)
            {
                user.AvatarLink = FixPostHtmlImage(avatarImage.OuterHtml);
            }
            user.Id = Convert.ToInt64(postNode.DescendantsAndSelf("td").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("userinfo")).GetAttributeValue("class", string.Empty).Split('-')[1]);
            return user;
        }

        public static ForumUserEntity FromUserProfile(HtmlNode profileNode)
        {
            var user = new ForumUserEntity();

            user.AboutUser = string.Empty;
            foreach (var aboutParagraph in profileNode.Descendants("p"))
            {
                user.AboutUser += WebUtility.HtmlDecode(aboutParagraph.InnerText.WithoutNewLines().Trim()) + Environment.NewLine + Environment.NewLine;
            }

            user.Username = profileNode.Descendants("dt").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author")).InnerText;
            var additionalNode = profileNode.Descendants("dl").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("additional"));

            var additionalProfileAttributes = ParseAdditionalProfileAttributes(additionalNode);

            user.DateJoined = additionalProfileAttributes["Member Since"];
            user.PostCount = int.Parse(additionalProfileAttributes["Post Count"]);
            user.PostRate = additionalProfileAttributes["Post Rate"];
            user.LastPostDate = additionalProfileAttributes["Last Post"];
            if (additionalProfileAttributes.ContainsKey("Seller Rating"))
            {
                user.SellerRating = additionalProfileAttributes["Seller Rating"];
            }
            if (additionalProfileAttributes.ContainsKey("Location"))
            {
                user.Location = additionalProfileAttributes["Location"];
            }
            return user;
        }

        private static Dictionary<string, string> ParseAdditionalProfileAttributes(HtmlNode additionalNode)
        {
            var dts = additionalNode.Descendants("dt");
            var dds = additionalNode.Descendants("dd");
            var result = dts.Zip(dds, (first, second) => new Tuple<string, string>(first.InnerText, second.InnerText)).ToDictionary(k=>k.Item1, v=>v.Item2);
            // Clean up malformed HTML that results in the "last post" value being all screwy
            var lastPostValue = result["Last Post"];
            var removalStartIndex = lastPostValue.IndexOf('\n');
            var lengthToRemove = lastPostValue.Length - removalStartIndex;
            result["Last Post"] = lastPostValue.Remove(removalStartIndex, lengthToRemove);
            return result;
        }

        /// <summary>
        /// Fixes the missing tags in an users avatar HTML node.
        /// </summary>
        /// <param name="postHtml"></param>
        /// <returns></returns>
        private static string FixPostHtmlImage(string postHtml)
        {
            return "<!DOCTYPE html><html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/ui-light.css\"></head><body style=\"background-color: white;\"></head><body>" + postHtml + "</body></html>";
        }
    }
}
