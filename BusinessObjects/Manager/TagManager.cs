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
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    public class TagManager
    {

        public static async Task<List<TagEntity>> GetTagList(long forumId)
        {
            List<TagEntity> tagList = new List<TagEntity>();
            HttpWebRequest request = await AuthManager.CreateGetRequest(string.Format(Constants.NEW_THREAD, forumId));
            HtmlDocument doc = await WebManager.DownloadHtml(request);
            var iconArray = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("posticon"))
                .ToArray();
            foreach(var icon in iconArray)
            {
                TagEntity tag = new TagEntity();
                tag.Parse(icon);
                tagList.Add(tag);
            }
            return tagList;
        }

    }
}
