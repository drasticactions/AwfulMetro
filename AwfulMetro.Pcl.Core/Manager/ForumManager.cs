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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class ForumManager
    {
        private readonly IWebManager _webManager;

        public ForumManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public ForumManager() : this(new WebManager())
        {
        }

        public async Task<ObservableCollection<ForumCategoryEntity>> GetForumCategoryMainPage()
        {
            try
            {
                var forumGroupList = new ObservableCollection<ForumCategoryEntity>();
                var result = await _webManager.GetData(Constants.FORUM_LIST_PAGE);
                HtmlDocument doc = result.Document;

                HtmlNode forumNode =
                    doc.DocumentNode.Descendants("select")
                        .FirstOrDefault(node => node.GetAttributeValue("name", string.Empty).Equals("forumid"));
                if (forumNode != null)
                {
                    IEnumerable<HtmlNode> forumNodes = forumNode.Descendants("option");

                    foreach (HtmlNode node in forumNodes)
                    {
                        string value = node.Attributes["value"].Value;
                        int id;
                        if (!int.TryParse(value, out id) || id <= -1) continue;
                        if (node.NextSibling.InnerText.Contains("--"))
                        {
                            string forumName = WebUtility.HtmlDecode(node.NextSibling.InnerText.Replace("-", string.Empty));
                            bool isSubforum = node.NextSibling.InnerText.Count(c => c == '-') > 2;
                            var forumSubCategory = new ForumEntity(forumName, string.Format(Constants.FORUM_PAGE, value),
                                string.Empty, isSubforum);
                            forumGroupList.LastOrDefault().ForumList.Add(forumSubCategory);
                        }
                        else
                        {
                            string forumName = WebUtility.HtmlDecode(node.NextSibling.InnerText);
                            var forumGroup = new ForumCategoryEntity(forumName, string.Format(Constants.FORUM_PAGE, value));
                            forumGroupList.Add(forumGroup);
                        }
                    }
                }

#if DEBUG
                forumGroupList[3].ForumList.Add(AddDebugForum());
#endif

                return forumGroupList;
            }
                // TODO: Expand catch for specific errors.
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<HtmlDocument> GetForumFrontPageHtml()
        {
            StorageFile file;
            string forumHtml = string.Format(Constants.HTML_FILE, "forum");
            bool htmlFileExists = await WebViewHelper.HtmlExists(forumHtml);
            if (htmlFileExists)
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(forumHtml);
                string html = await FileIO.ReadTextAsync(file);
                var document = new HtmlDocument();
                document.LoadHtml(html);
                return document;
            }

            file =
                await
                    ApplicationData.Current.LocalFolder.CreateFileAsync(forumHtml,
                        CreationCollisionOption.ReplaceExisting);

            HtmlDocument doc = (await _webManager.GetData(Constants.BASE_URL)).Document;
            using (var memoryStream = new MemoryStream())
            {
                doc.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                await FileIO.WriteBytesAsync(file, memoryStream.ToArray());
            }
            return doc;
        }

        public async Task<ObservableCollection<ForumEntity>> GetSubForums(ForumEntity forumCategory)
        {
            var subforumList = new List<ForumEntity>();
            string url = forumCategory.Location;
            HtmlDocument doc = (await _webManager.GetData(url)).Document;
            if (
                !doc.DocumentNode.Descendants()
                    .Any(node => node.GetAttributeValue("id", string.Empty).Contains("subforums"))) return null;
            HtmlNode forumNode =
                doc.DocumentNode.Descendants()
                    .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("subforums"));
            subforumList.AddRange(from subforumNode in forumNode.Descendants("tr")
                where subforumNode.Descendants("a").Any()
                select
                    new ForumEntity(WebUtility.HtmlDecode(subforumNode.Descendants("a").FirstOrDefault().InnerText),
                        Constants.BASE_URL +
                        subforumNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty),
                        string.Empty, true));
            var obSubforumList = new ObservableCollection<ForumEntity>();
            foreach (var forum in subforumList)
            {
                obSubforumList.Add(forum);
            }
            return obSubforumList;
        }

        private ForumEntity AddDebugForum()
        {
            return new ForumEntity("Apps In Developmental States", Constants.BASE_URL + "forumdisplay.php?forumid=261",
                "Debug Forum", false);
        }
    }
}