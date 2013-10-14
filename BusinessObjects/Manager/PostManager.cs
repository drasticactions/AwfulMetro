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
    public class PostManager
    {
        public static async Task<List<ForumPostEntity>> GetThreadPosts(ForumThreadEntity forumThread)
        {
            String url = forumThread.Location;

            if (forumThread.CurrentPage > 0)
            {
                url = forumThread.Location + string.Format(Constants.PAGE_NUMBER, forumThread.CurrentPage);
            }

            List<ForumPostEntity> forumThreadPosts = new List<ForumPostEntity>();
            HttpWebRequest request = await AuthManager.CreateGetRequest(url);

            //TEMP CODE
            HtmlDocument doc;
            var response = await request.GetResponseAsync();
            var stream = response.GetResponseStream();
            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                doc = new HtmlDocument();
                doc.LoadHtml(html);
            }
            string[] test = response.ResponseUri.AbsoluteUri.Split('#');
            if (test.Length > 1)
            {
              forumThread.ScrollToPost = Int32.Parse(Regex.Match(response.ResponseUri.AbsoluteUri.Split('#')[1], @"\d+").Value);
            }
            //HtmlDocument doc = await WebManager.DownloadHtml(request);
            HtmlNode threadNode = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("id", "").Contains("thread")).FirstOrDefault();

            foreach (HtmlNode postNode in threadNode.Descendants("table").Where(node => node.GetAttributeValue("class", "").Contains("post")))
            {
                ForumPostEntity post = new ForumPostEntity();
                post.Parse(postNode);
                forumThreadPosts.Add(post);
            }

            threadNode = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("pages top")).FirstOrDefault();
            threadNode = threadNode.Descendants("option").Where(node => node.GetAttributeValue("selected", "").Contains("selected")).FirstOrDefault();

            if (forumThread.CurrentPage <= 0)
            {
                forumThread.CurrentPage = PostManager.GetPageNumber(threadNode);
            }

            HtmlNode pageNode = doc.DocumentNode.Descendants("select").FirstOrDefault();
            forumThread.TotalPages = Convert.ToInt32(pageNode.Descendants("option").LastOrDefault().GetAttributeValue("value", ""));

            return forumThreadPosts;
        }

        private static int GetPageNumber(HtmlNode threadNode)
        {
            if (threadNode != null && !string.IsNullOrEmpty(threadNode.GetAttributeValue("value", "")))
            {

                return Convert.ToInt32(threadNode.GetAttributeValue("value", ""));
            }
            else
            {
                return 1;
            }
        }
    }
}
