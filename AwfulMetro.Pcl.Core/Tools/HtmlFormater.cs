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
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Entity;
using HtmlAgilityPack;

namespace AwfulMetro.Pcl.Core.Tools
{
    public static class HtmlFormater
    {
        public static async Task<string> FormatThreadHtml(ForumThreadEntity forumThreadEntity )
        {
            ObservableCollection<ForumPostEntity> postEntities = forumThreadEntity.ForumPosts;
            string html = await PathIO.ReadTextAsync("ms-appx:///Assets/thread.html");

            var doc2 = new HtmlDocument();

            doc2.LoadHtml(html);

            
            HtmlNode head = doc2.DocumentNode.Descendants("head").FirstOrDefault();

            switch (forumThreadEntity.ForumId)
            {
                case 219:
                    head.InnerHtml += "<link href=\"ms-appx-web:///Assets/219.css\" type=\"text/css\" media=\"all\" rel=\"stylesheet\">";
                    break;
                case 26:
                    break;

            }

            HtmlNode bodyNode = doc2.DocumentNode.Descendants("body").FirstOrDefault();

            string threadHtml = string.Empty;
            int seenCount = 1;
            if (postEntities == null) return WebUtility.HtmlDecode(WebUtility.HtmlDecode(doc2.DocumentNode.OuterHtml)) ;
            for (int index = 0; index < postEntities.Count; index++)
            {
                ForumPostEntity post = postEntities[index];
                if (seenCount > 2)
                    seenCount = 1;
                string hasSeen = post.HasSeen ? string.Concat("seen", seenCount) : string.Concat("postCount", seenCount);
                seenCount++;
                string userAvatar = string.Empty;
                if (!string.IsNullOrEmpty(post.User.AvatarLink))
                    userAvatar = string.Concat("<img src=\"", post.User.AvatarLink,
                        "\" alt=\"\" class=\"av\" border=\"0\">");
                string username =
                    string.Format(
                        "<h2 class=\"text article-title win-type-ellipsis\"><span class=\"author\">{0}</span><h2>",
                        post.User.Username);
                string postData =
                    string.Format(
                        "<h4 class=\"text article-title win-type-ellipsis\"><span class=\"registered\">{0}</span><h4>",
                        post.PostDate);
                string postBody = string.Format("<div class=\"postbody\">{0}</div>", post.PostHtml);
                string userInfo = string.Format("<div class=\"userinfo\">{0}{1}</div>", username, postData);
                string postButtons = CreateButtons(post);

                string footer = string.Format("<tr class=\"postbar\"><td class=\"postlinks\">{0}</td></tr>", postButtons);
                threadHtml +=
                    string.Format(
                        "<div class={6} id={4}><div id={5}><div id=\"threadView\"><header>{0}{1}</header><article><div class=\"article-content\">{2}</div></article><footer>{3}</footer></div></div></div>",
                        userAvatar, userInfo, postBody, footer, string.Concat("\"pti", index + 1, "\""), string.Concat("\"postId", post.PostId, "\""), string.Concat("\"", hasSeen, "\""));
            }

            bodyNode.InnerHtml = threadHtml;
            return doc2.DocumentNode.OuterHtml;
        }

        private static string CreateButtons(ForumPostEntity post)
        {
            var clickHandler = string.Format("window.ForumCommand('quote', '{0}')", post.PostId);

            string quoteButton = HtmlButtonBuilder.CreateSubmitButton("Quote", clickHandler);

            clickHandler = string.Format("window.ForumCommand('edit', '{0}')", post.PostId);

            string editButton = HtmlButtonBuilder.CreateSubmitButton("Edit", clickHandler);

            clickHandler = string.Format("window.ForumCommand('markAsLastRead', '{0}')", post.PostIndex);

            string markAsLastReadButton = HtmlButtonBuilder.CreateSubmitButton("Last Read", clickHandler);

            return post.User.IsCurrentUserPost
                    ? string.Concat("<ul class=\"profilelinks\">",
                        quoteButton, markAsLastReadButton, editButton, "</ul>")
                    : string.Concat("<ul class=\"profilelinks\">",
                        quoteButton, markAsLastReadButton, "</ul>");
        }
    }
}
