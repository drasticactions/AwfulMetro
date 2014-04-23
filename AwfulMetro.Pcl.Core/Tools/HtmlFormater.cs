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
        public static async Task<string> FormatThreadHtml(ObservableCollection<ForumPostEntity> postEntities )
        {

            string html = await PathIO.ReadTextAsync("ms-appx:///Assets/thread.html");

            var doc2 = new HtmlDocument();

            doc2.LoadHtml(html);

            HtmlNode bodyNode = doc2.DocumentNode.Descendants("body").FirstOrDefault();

            string threadHtml = string.Empty;

            foreach (ForumPostEntity post in postEntities)
            {
                string userAvatar = string.Empty;
                if(!string.IsNullOrEmpty(post.User.AvatarLink))
                userAvatar = string.Concat("<img src=\"", post.User.AvatarLink, "\" alt=\"\" class=\"av\" border=\"0\"");
                string username = string.Format("<h2 class=\"text article-title win-type-ellipsis\"><span class=\"author\">{0}</span><h2>", post.User.Username);
                string postData = string.Format("<h4 class=\"text article-title win-type-ellipsis\"><span class=\"registered\">{0}</span><h4>", post.PostDate);
                string postBody = string.Format("<div class=\"postbody\">{0}</div>", post.PostHtml);
                string userInfo = string.Format("<div class=\"userinfo\">{0}{1}</div>", username, postData);
                string postButtons = CreateButtons(post);

                string footer = string.Format("<tr class=\"postbar\"><td class=\"postlinks\">{0}</td></tr>", postButtons);
                threadHtml += string.Format("<div id=\"threadView\"><article><header>{0}{1}</header><div class=\"article-content\">{2}</div><footer>{3}</footer></article></div>", userAvatar, userInfo, postBody, footer);
            }

            bodyNode.InnerHtml = threadHtml;
            return WebUtility.HtmlDecode(WebUtility.HtmlDecode(doc2.DocumentNode.OuterHtml));
        }

        private static string CreateButtons(ForumPostEntity post)
        {
            var clickHandler = string.Format("window.ForumCommand('quote', '{0}')", post.PostId);

            string quoteButton = HtmlButtonBuilder.CreateSubmitButton("Quote", clickHandler);

            clickHandler = string.Format("window.ForumCommand('edit', '{0}')", post.PostId);

            string editButton = HtmlButtonBuilder.CreateSubmitButton("Edit", clickHandler);

            clickHandler = string.Format("window.ForumCommand('markAsLastRead', '{0}')", string.Empty);

            string markAsLastReadButton = HtmlButtonBuilder.CreateSubmitButton("Last Read", clickHandler);

            return post.User.IsCurrentUserPost
                    ? string.Concat("<ul class=\"profilelinks\">",
                        quoteButton, markAsLastReadButton, editButton, "</ul>")
                    : string.Concat("<ul class=\"profilelinks\">",
                        quoteButton, markAsLastReadButton, "</ul>");
        }
    }
}
