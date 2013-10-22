using BusinessObjects.Entity;
using BusinessObjects.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BusinessObjects.Manager
{
    public class ForumManager
    {
        public static async Task<List<ForumCategoryEntity>> GetForumCategory()
        {
            List<ForumCategoryEntity> forumGroupList = new List<ForumCategoryEntity>();
            List<ForumEntity> forumSubcategoryList = new List<ForumEntity>();
            HtmlDocument doc = await ForumManager.GetForumFrontPageHtml();
            HtmlNode forumNode = doc.DocumentNode.Descendants().Where(node => node.GetAttributeValue("id", "").Contains("forums")).FirstOrDefault();

            foreach (HtmlNode link in forumNode.Descendants("tr"))
            {
                if (link.Descendants("th").Any())
                {
                    ForumCategoryEntity forumGroup = new ForumCategoryEntity(WebUtility.HtmlDecode(link.Descendants("a").FirstOrDefault().InnerText), link.Descendants("a").FirstOrDefault().GetAttributeValue("href", ""));
                    forumGroupList.Add(forumGroup);
                }
                else if (link.Descendants("td").Any())
                {
                    ForumEntity forumSubCategory = new ForumEntity(WebUtility.HtmlDecode(link.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("forum")).FirstOrDefault().InnerText), link.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("forum")).FirstOrDefault().GetAttributeValue("href", ""), link.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("forum")).FirstOrDefault().GetAttributeValue("title", ""));
                    forumGroupList.LastOrDefault().ForumList.Add(forumSubCategory);
                }
            }

#if DEBUG
            forumGroupList[3].ForumList.Add(ForumManager.AddDebugForum());
#endif

            return forumGroupList;
        }

        public static async Task<HtmlDocument> GetForumFrontPageHtml()
        {
            StorageFile file;
            HtmlDocument doc;
            string forumHtml = string.Format(Constants.HTML_FILE, "forum");
            bool htmlFileExists = await WebViewHelper.HtmlExists(forumHtml);
            if(htmlFileExists)
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(forumHtml);
                string html = await FileIO.ReadTextAsync(file);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                return document;
            }
            else
            {
                file = await ApplicationData.Current.LocalFolder.CreateFileAsync(forumHtml, CreationCollisionOption.ReplaceExisting);
                HttpWebRequest request = await AuthManager.CreateGetRequest(Constants.BASE_URL);
                doc = await WebManager.DownloadHtml(request);
                MemoryStream memoryStream = new MemoryStream();
                doc.Save(memoryStream);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                StreamReader streamReader = new StreamReader(memoryStream);
                await FileIO.WriteBytesAsync(file, memoryStream.ToArray());
            }
            return doc;
        }

        private static ForumEntity AddDebugForum()
        {
            return new ForumEntity("Apps In Developmental States", "forumdisplay.php?forumid=261", "Debug Forum");
        }
    }
}
