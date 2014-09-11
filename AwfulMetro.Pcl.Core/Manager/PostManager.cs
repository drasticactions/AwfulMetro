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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Entity;
using AwfulMetro.Pcl.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Pcl.Core.Manager
{
    public class PostManager
    {
        private readonly IWebManager _webManager;

        public PostManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public PostManager() : this(new WebManager())
        {
        }

        public async Task<string> GetPost(int postId)
        {
            try
            {
                string url = string.Format(Constants.SHOW_POST, postId);
                WebManager.Result result = await _webManager.GetData(url);
                HtmlDocument doc = result.Document;
                HtmlNode threadNode =
                    doc.DocumentNode.Descendants("div")
                        .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("thread"));
                HtmlNode postNode =
                    threadNode.Descendants("table")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("post"));
                var post = new ForumPostEntity();
                post.Parse(postNode);
                return await HtmlFormater.FormatPostHtml(post);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ForumThreadEntity> GetThreadPosts(ForumThreadEntity forumThread)
        {
            string url = forumThread.Location;

            if (forumThread.CurrentPage > 0)
            {
                url = forumThread.Location + string.Format(Constants.PAGE_NUMBER, forumThread.CurrentPage);
            }
            else if (forumThread.HasBeenViewed)
            {
                url = forumThread.Location + Constants.GOTO_NEW_POST;
            }

            var forumThreadPosts = new ObservableCollection<ForumPostEntity>();

            //TEMP CODE
            var threadManager = new ThreadManager();
            var doc = await threadManager.GetThread(forumThread, url);

            HtmlNode threadNode =
                doc.DocumentNode.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("thread"));

            foreach (
                HtmlNode postNode in
                    threadNode.Descendants("table")
                        .Where(node => node.GetAttributeValue("class", string.Empty).Contains("post")))
            {
                var post = new ForumPostEntity();
                post.Parse(postNode);
                forumThreadPosts.Add(post);
            }

            threadNode =
                doc.DocumentNode.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("pages top"));
            if (threadNode != null)
            {
                threadNode =
                    threadNode.Descendants("option")
                        .FirstOrDefault(node => node.GetAttributeValue("selected", string.Empty).Contains("selected"));

                if (forumThread.CurrentPage <= 0)
                {
                    forumThread.CurrentPage = GetPageNumber(threadNode);
                }
            }
            forumThread.ForumPosts = forumThreadPosts;
            return forumThread;
        }

        private static int GetPageNumber(HtmlNode threadNode)
        {
            if (threadNode != null && !string.IsNullOrEmpty(threadNode.GetAttributeValue("value", string.Empty)))
            {
                return Convert.ToInt32(threadNode.GetAttributeValue("value", string.Empty));
            }
            return 1;
        }
    }
}