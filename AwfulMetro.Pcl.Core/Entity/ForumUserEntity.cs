﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    // Food for thought: This might be better served as a ForumUser entity, with a ForumUserProfile subclass.
    public class ForumUserEntity
    {
        private ForumUserEntity()
        {
        }

        public string Username { get; private set; }

        public string AvatarLink { get; private set; }

        public string AvatarTitle { get; private set; }

        public DateTime DateJoined { get; private set; }

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

        public DateTime LastPostDate { get; private set; }

        public string Location { get; private set; }

        public string AboutUser { get; private set; }

        public bool IsMod { get; private set; }

        public long Id { get; private set; }

        public string PostRate { get; private set; }

        public string SellerRating { get; private set; }

        public static ForumUserEntity FromPost(HtmlNode postNode)
        {
            var user = new ForumUserEntity
            {
                Username =
                    WebUtility.HtmlDecode(
                        postNode.Descendants("dt")
                            .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("author"))
                            .InnerHtml),
                DateJoined =
                    DateTime.Parse(postNode.Descendants("dd")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("registered"))
                        .InnerHtml)
            };
            HtmlNode avatarTitle =
                postNode.Descendants("dd")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("title"));
            HtmlNode avatarImage =
                postNode.Descendants("dd")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("title"))
                    .Descendants("img")
                    .FirstOrDefault();

            if (avatarTitle != null)
            {
                user.AvatarTitle = WebUtility.HtmlDecode(avatarTitle.InnerText).WithoutNewLines().Trim();
            }
            if (avatarImage != null)
            {
                user.AvatarLink = FixPostHtmlImage(avatarImage.OuterHtml);
            }
            user.Id =
                Convert.ToInt64(
                    postNode.DescendantsAndSelf("td")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("userinfo"))
                        .GetAttributeValue("class", string.Empty)
                        .Split('-')[1]);
            return user;
        }

        /// <summary>
        ///     Gets avatar information from the user login page.
        ///     Note: This should not be done in this way. It needs to be refactored
        /// </summary>
        /// <param name="threadNode">The "thread" class node on the user profile page.</param>
        /// <param name="user">The user entity, parsed from FromUserProfile</param>
        public void FromUserProfileAvatarInformation(HtmlNode threadNode)
        {
            HtmlNode avatarTitle =
                threadNode.Descendants("dd")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("title"));
            HtmlNode avatarImage =
                threadNode.Descendants("dd")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("title"))
                    .Descendants("img")
                    .Any()
                    ? threadNode.Descendants("dd")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("title"))
                        .Descendants("img")
                        .FirstOrDefault()
                    : null;

            if (avatarTitle != null)
            {
                AvatarTitle = WebUtility.HtmlDecode(avatarTitle.InnerText).WithoutNewLines().Trim();
            }
            if (avatarImage != null)
            {
                AvatarLink = FixPostHtmlImage(avatarImage.OuterHtml);
            }
        }

        public static ForumUserEntity FromUserProfile(HtmlNode profileNode, HtmlNode authorNode)
        {
            HtmlNode additionalNode =
                profileNode.Descendants("dl")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("additional"));
            Dictionary<string, string> additionalProfileAttributes = ParseAdditionalProfileAttributes(additionalNode);

            var user = new ForumUserEntity
            {
                Username =
                    authorNode.Descendants("dt")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("author"))
                        .InnerText,
                AboutUser = string.Empty,
                DateJoined = DateTime.Parse(additionalProfileAttributes["Member Since"]),
                PostCount = int.Parse(additionalProfileAttributes["Post Count"]),
                PostRate = additionalProfileAttributes["Post Rate"],
                LastPostDate = DateTime.Parse(additionalProfileAttributes["Last Post"])
            };

            foreach (HtmlNode aboutParagraph in profileNode.Descendants("p"))
            {
                user.AboutUser += WebUtility.HtmlDecode(aboutParagraph.InnerText.WithoutNewLines().Trim()) +
                                  Environment.NewLine + Environment.NewLine;
            }
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
            IEnumerable<HtmlNode> dts = additionalNode.Descendants("dt");
            IEnumerable<HtmlNode> dds = additionalNode.Descendants("dd");
            Dictionary<string, string> result =
                dts.Zip(dds, (first, second) => new Tuple<string, string>(first.InnerText, second.InnerText))
                    .ToDictionary(k => k.Item1, v => v.Item2);
            // Clean up malformed HTML that results in the "last post" value being all screwy
            string lastPostValue = result["Last Post"];
            int removalStartIndex = lastPostValue.IndexOf('\n');
            int lengthToRemove = lastPostValue.Length - removalStartIndex;
            result["Last Post"] = lastPostValue.Remove(removalStartIndex, lengthToRemove);
            return result;
        }

        /// <summary>
        ///     Fixes the missing tags in an users avatar HTML node.
        /// </summary>
        /// <param name="postHtml"></param>
        /// <returns></returns>
        private static string FixPostHtmlImage(string postHtml)
        {
            return
                "<!DOCTYPE html><html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/ui-light.css\"></head><body style=\"background-color: white;\"></head><body>" +
                postHtml + "</body></html>";
        }
    }
}