using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;

namespace AwfulMetro.Core.Manager
{
    public class ForumManager
    {
        private readonly IWebManager _webManager;
        public ForumManager(IWebManager webManager)
        {
            this._webManager = webManager;
        }

        public ForumManager() : this(new WebManager()) { }

        public async Task<List<ForumCategoryEntity>> GetForumCategoryMainPage()
        {
            var forumGroupList = new List<ForumCategoryEntity>();

            var doc = (await _webManager.DownloadHtml(Constants.FORUM_LIST_PAGE)).Document;
            
            HtmlNode forumNode = doc.DocumentNode.Descendants("select").FirstOrDefault(node => node.GetAttributeValue("name", string.Empty).Equals("forumid"));
            if (forumNode != null)
            {
                var forumNodes = forumNode.Descendants("option");

                foreach (var node in forumNodes)
                {
                    var value = node.Attributes["value"].Value;
                    int id;
                    if (int.TryParse(value, out id) && id > -1)
                    {
                        if (node.NextSibling.InnerText.Contains("--"))
                        {
                            string forumName = WebUtility.HtmlDecode(node.NextSibling.InnerText.Replace("-", string.Empty));
                            var forumSubCategory = new ForumEntity(forumName, string.Format(Constants.FORUM_PAGE, value), string.Empty);
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
            }
            return forumGroupList;
        }

        public async Task<List<ForumCategoryEntity>> GetForumCategory()
        {
            var forumGroupList = new List<ForumCategoryEntity>();
            HtmlDocument doc = await GetForumFrontPageHtml();
            HtmlNode forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("forums"));

            foreach (HtmlNode link in forumNode.Descendants("tr"))
            {
                if (link.Descendants("th").Any())
                {
                    var forumGroup = new ForumCategoryEntity(WebUtility.HtmlDecode(link.Descendants("a").FirstOrDefault().InnerText), link.Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty));
                    forumGroupList.Add(forumGroup);
                }
                else if (link.Descendants("td").Any())
                {
                    var forumSubCategory = new ForumEntity(WebUtility.HtmlDecode(link.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("forum")).InnerText), link.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("forum")).GetAttributeValue("href", string.Empty), link.Descendants("a").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Equals("forum")).GetAttributeValue("title", string.Empty));
                    forumGroupList.LastOrDefault().ForumList.Add(forumSubCategory);
                }
            }

#if DEBUG
            forumGroupList[3].ForumList.Add(AddDebugForum());
#endif

            return forumGroupList;
        }

        public async Task<HtmlDocument> GetForumFrontPageHtml()
        {
            StorageFile file;
            string forumHtml = string.Format(Constants.HTML_FILE, "forum");
            bool htmlFileExists = await WebViewHelper.HtmlExists(forumHtml);
            if(htmlFileExists)
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(forumHtml);
                string html = await FileIO.ReadTextAsync(file);
                var document = new HtmlDocument();
                document.LoadHtml(html);
                return document;
            }

            file = await ApplicationData.Current.LocalFolder.CreateFileAsync(forumHtml, CreationCollisionOption.ReplaceExisting);
            
            HtmlDocument doc = (await _webManager.DownloadHtml(Constants.BASE_URL)).Document;
            using (var memoryStream = new MemoryStream())
            {
                doc.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                await FileIO.WriteBytesAsync(file, memoryStream.ToArray());
                    
            }
            return doc;
        }

        public async Task<List<ForumEntity>> GetSubForums(ForumEntity forumCategory)
        {
            var subforumList = new List<ForumEntity>();
            var url = forumCategory.Location;
            var doc = (await _webManager.DownloadHtml(url)).Document;
            if (doc.DocumentNode.Descendants().Any(node => node.GetAttributeValue("id", string.Empty).Contains("subforums")))
            {
                var forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("subforums"));
                subforumList.AddRange(from subforumNode in forumNode.Descendants("tr") where subforumNode.Descendants("a").Any() select new ForumEntity(WebUtility.HtmlDecode(subforumNode.Descendants("a").FirstOrDefault().InnerText), subforumNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty), string.Empty));
            }
            return subforumList;
        }

        private ForumEntity AddDebugForum()
        {
            return new ForumEntity("Apps In Developmental States", "forumdisplay.php?forumid=261", "Debug Forum");
        }
    }
}
