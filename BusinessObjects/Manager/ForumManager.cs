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

namespace BusinessObjects.Manager
{
    public class ForumManager
    {
        public static async Task<List<ForumCategoryEntity>> GetForumCategory()
        {
            List<ForumCategoryEntity> forumGroupList = new List<ForumCategoryEntity>();
            List<ForumEntity> forumSubcategoryList = new List<ForumEntity>();
            HttpWebRequest request = await AuthManager.CreateGetRequest(Constants.BASE_URL);
            HtmlDocument doc = await WebManager.DownloadHtml(request);
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

        private static ForumEntity AddDebugForum()
        {
            return new ForumEntity("Apps In Developmental States", "forumdisplay.php?forumid=261", "Debug Forum");
        }
    }
}
