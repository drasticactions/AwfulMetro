#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Pcl.Core.Entity
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

        public bool IsCurrentUserPost { get; set; }

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
                            .InnerHtml)
            };
            var dateTimeNode = postNode.Descendants("dd")
                .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("registered"));
            if (dateTimeNode != null)
            {
                try
                {
                    user.DateJoined = DateTime.Parse(dateTimeNode.InnerHtml);
                }
                catch (Exception)
                {
                    // Parsing failed, so say they joined today.
                    // I blame SA for any parsing failures.
                    user.DateJoined = DateTime.UtcNow;
                }
                
            }
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
                user.AvatarLink = avatarImage.GetAttributeValue("src", string.Empty);
            }
            var userIdNode = postNode.DescendantsAndSelf("td")
                .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("userinfo")) ??
                             postNode.DescendantsAndSelf("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("userinfo"));
            if (userIdNode == null) return user;

            var splitString = userIdNode
                .GetAttributeValue("class", string.Empty)
                .Split('-');

            if (splitString.Length >= 2)
            {
                user.Id =
                    Convert.ToInt64(splitString[1]);
            }
            // Remove the UserInfo node after we are done with it, because
            // some forums (FYAD) use it in the body of posts. Why? Who knows!11!1
            userIdNode.Remove();
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
                AvatarLink = avatarImage.GetAttributeValue("src", string.Empty);
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