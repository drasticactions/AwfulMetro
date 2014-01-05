using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Manager
{
    public class ThreadManager
    {
        private readonly IWebManager _webManager;
        public ThreadManager(IWebManager webManager)
        {
            this._webManager = webManager;
        }

        public ThreadManager() : this(new WebManager()) { }

        public async Task<List<ForumThreadEntity>> GetBookmarks(ForumEntity forumCategory, int page)
        {
            var forumSubcategoryList = new List<ForumEntity>();
            var forumThreadList = new List<ForumThreadEntity>();
            String url = Constants.BOOKMARKS_URL;
            if (forumCategory.CurrentPage > 0)
            {
                url = Constants.BOOKMARKS_URL + string.Format(Constants.PAGE_NUMBER, page);
            }

            HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;
            HtmlNode forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("threadlist"));


            foreach (HtmlNode threadNode in forumNode.Descendants("tr").Where(node => node.GetAttributeValue("class", string.Empty).StartsWith("thread")))
            {
                var threadEntity = new ForumThreadEntity();
                threadEntity.Parse(threadNode);
                forumThreadList.Add(threadEntity);
            }
            return forumThreadList;
        }

        public async Task<bool> AddBookmarks(List<long> threadIdList)
        {
            foreach (long threadId in threadIdList)
            {
                await this._webManager.PostData(
                Constants.BOOKMARK, string.Format(
                    Constants.ADD_BOOKMARK, threadId
                   ));
            }
            return true;
        }

        public async Task<bool> RemoveBookmarks(List<long> threadIdList)
        {
            foreach (long threadId in threadIdList)
            {
                await this._webManager.PostData(
                Constants.BOOKMARK, string.Format(
                    Constants.REMOVE_BOOKMARK, threadId
                   ));
            }
            return true;
        }

        public async Task<bool> MarkThreadUnread(List<long> threadIdList)
        {
            foreach (long threadId in threadIdList)
            {
                await this._webManager.PostData(
                Constants.RESET_BASE, string.Format(
                    Constants.RESET_SEEN, threadId
                   ));
            }
            return true;
        }

        public async Task<List<ForumThreadEntity>> GetForumThreads(ForumEntity forumCategory, int page)
        {
            // TODO: Remove parsing logic from managers. I don't think they have a place here...
            var url = forumCategory.Location + string.Format(Constants.PAGE_NUMBER, page);

            HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;

            HtmlNode forumNode = doc.DocumentNode.Descendants().FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("threadlist"));
            var forumThreadList = new List<ForumThreadEntity>();
            foreach (HtmlNode threadNode in forumNode.Descendants("tr").Where(node => node.GetAttributeValue("class", string.Empty).StartsWith("thread")))
            {
                var threadEntity = new ForumThreadEntity();
                threadEntity.Parse(threadNode);
                forumThreadList.Add(threadEntity);
            }
            return forumThreadList;
        }

    }
}
