using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class ReplyManager
    {

        private readonly IWebManager _webManager;

        public ReplyManager() : this(new WebManager()) { }

        public ReplyManager(IWebManager webManager)
        {
            this._webManager = webManager;
        }

        public async Task<ForumReplyEntity> GetReplyCookies(ForumThreadEntity forumThread)
        {
            string url = string.Format(Constants.REPLY_BASE, forumThread.ThreadId);
            var result = await _webManager.DownloadHtml(url);
            HtmlDocument doc = result.Document;

            var formNodes = doc.DocumentNode.Descendants("input").ToArray();

            var formKeyNode = formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("formkey"));

            var formCookieNode = formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("form_cookie"));

            var bookmarkNode = formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("bookmark"));

            var textAreaNodes = doc.DocumentNode.Descendants("textarea").ToArray();

            var textNode = textAreaNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("message"));

            var threadIdNode = formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("threadid"));

            var threadManager = new ThreadManager();

            var htmlThread = await threadManager.GetThreadHtml(doc);

            var forumReplyEntity = new ForumReplyEntity();
            try
            {
                string formKey = formKeyNode.GetAttributeValue("value", "");
                string formCookie = formCookieNode.GetAttributeValue("value", "");
                string quote = WebUtility.HtmlDecode(textNode.InnerText);
                string threadId = threadIdNode.GetAttributeValue("value", "");
                //string bookmark = bookmarkNode.GetAttributeValue("value", "");
                forumReplyEntity.MapThreadInformation(formKey, formCookie, quote, threadId);
                forumReplyEntity.PreviousPostsRaw = htmlThread;
                return forumReplyEntity;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Could not parse newReply form data.");
            }
        }

        public async Task<ForumReplyEntity> GetReplyCookiesForEdit(long postId)
        {
            string url = string.Format(Constants.EDIT_BASE, postId);
            var result = await _webManager.DownloadHtml(url);
            HtmlDocument doc = result.Document;

            var formNodes = doc.DocumentNode.Descendants("input").ToArray();

            var bookmarkNode = formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("bookmark"));

            var textAreaNodes = doc.DocumentNode.Descendants("textarea").ToArray();

            var textNode = textAreaNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("message"));

            var threadManager = new ThreadManager();

            //Get previous posts from quote page.
            string url2 = string.Format(Constants.QUOTE_BASE, postId);
            var result2 = await _webManager.DownloadHtml(url2);
            HtmlDocument doc2 = result2.Document;

            var htmlThread = await threadManager.GetThreadHtml(doc2);

            var forumReplyEntity = new ForumReplyEntity();
            try
            {
                string quote = WebUtility.HtmlDecode(textNode.InnerText);
                forumReplyEntity.PreviousPostsRaw = htmlThread;
                string bookmark = bookmarkNode.OuterHtml.Contains("checked") ? "yes" : "no";
                forumReplyEntity.MapEditPostInformation(quote, postId, bookmark);
                return forumReplyEntity;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Could not parse newReply form data.");
            }
        }

        public async Task<ForumReplyEntity> GetReplyCookies(long postId)
        {
            string url = string.Format(Constants.QUOTE_BASE, postId);
            var result = await _webManager.DownloadHtml(url);
            HtmlDocument doc = result.Document;

            var formNodes = doc.DocumentNode.Descendants("input").ToArray();

            var formKeyNode = formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("formkey"));

            var formCookieNode = formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("form_cookie"));

            var textAreaNodes = doc.DocumentNode.Descendants("textarea").ToArray();

            var textNode = textAreaNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("message"));

            var threadIdNode = formNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("threadid"));

            var threadManager = new ThreadManager();

            var htmlThread = await threadManager.GetThreadHtml(doc);

            var forumReplyEntity = new ForumReplyEntity();
            try
            {
                string formKey = formKeyNode.GetAttributeValue("value", "");
                string formCookie = formCookieNode.GetAttributeValue("value", "");
                string quote = WebUtility.HtmlDecode(textNode.InnerText);
                string threadId = threadIdNode.GetAttributeValue("value", "");
                forumReplyEntity.PreviousPostsRaw = htmlThread;
                forumReplyEntity.MapThreadInformation(formKey, formCookie, quote, threadId);
                return forumReplyEntity;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Could not parse newReply form data.");
            }
        }

        public static string HtmlEncode(string text)
        {
            //TODO: Taken directly off of the internet to fix Unicode support. Need to enhance.
            char[] chars = WebUtility.HtmlEncode(text).ToCharArray();
            var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (char c in chars)
            {
                int value = Convert.ToInt32(c);
                if (value > 127)
                    result.AppendFormat("&#{0};", value);
                else
                    result.Append(c);
            }

            result.Replace("&quot;", "\"");
            return result.ToString();
        }


        public async Task<string> CreatePreviewEditPost(ForumReplyEntity forumReplyEntity)
        {
            if (forumReplyEntity == null)
                return string.Empty;
            var form = new MultipartFormDataContent
            {
                {new StringContent("updatepost"), "action"},
                {new StringContent(forumReplyEntity.PostId.ToString()), "postid"},
                {new StringContent(HtmlEncode(forumReplyEntity.Message)), "message"},
                {new StringContent(forumReplyEntity.ParseUrl.ToString()), "parseurl"},
                {new StringContent("2097152"), "MAX_FILE_SIZE"},
                {new StringContent("Preview Post"), "preview"}
            };
            var response = await this._webManager.PostFormData(Constants.EDIT_POST, form);
            var stream = await response.Content.ReadAsStreamAsync();
            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var replyNodes = doc.DocumentNode.Descendants("div").ToArray();

                var previewNode = replyNodes.FirstOrDefault(node => node.GetAttributeValue("class", "").Equals("inner postbody"));
                return previewNode == null ? string.Empty : FixPostHtml(previewNode.OuterHtml);
            }
        }

        public async Task<string> CreatePreviewPost(ForumReplyEntity forumReplyEntity)
        {
            if(forumReplyEntity == null)
                return string.Empty;
            var form = new MultipartFormDataContent
            {
                {new StringContent("postreply"), "action"},
                {new StringContent(forumReplyEntity.ThreadId), "threadid"},
                {new StringContent(forumReplyEntity.FormKey), "formkey"},
                {new StringContent(forumReplyEntity.FormCookie), "form_cookie"},
                {new StringContent(HtmlEncode(forumReplyEntity.Message)), "message"},
                {new StringContent(forumReplyEntity.ParseUrl.ToString()), "parseurl"},
                {new StringContent("2097152"), "MAX_FILE_SIZE"},
                {new StringContent("Submit Reply"), "submit"},
                {new StringContent("Preview Reply"), "preview"}
            };
            var response = await this._webManager.PostFormData(Constants.NEW_REPLY, form);
            var stream = await response.Content.ReadAsStreamAsync();
            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var replyNodes = doc.DocumentNode.Descendants("div").ToArray();

                var previewNode = replyNodes.FirstOrDefault(node => node.GetAttributeValue("class", "").Equals("inner postbody"));
                return previewNode == null ? string.Empty : FixPostHtml(previewNode.OuterHtml);
            }
        }

        private static string FixPostHtml(String postHtml)
        {
            return "<!DOCTYPE html><html>" + Constants.HTML_HEADER + "<body>" + postHtml + "</body></html>";
        }

        public async Task<bool> SendPost(ForumReplyEntity forumReplyEntity)
        {
            if (forumReplyEntity == null)
                return false;
            var form = new MultipartFormDataContent
            {
                {new StringContent("postreply"), "action"},
                {new StringContent(forumReplyEntity.ThreadId), "threadid"},
                {new StringContent(forumReplyEntity.FormKey), "formkey"},
                {new StringContent(forumReplyEntity.FormCookie), "form_cookie"},
                {new StringContent(HtmlEncode(forumReplyEntity.Message)), "message"},
                {new StringContent(forumReplyEntity.ParseUrl.ToString()), "parseurl"},
                {new StringContent("2097152"), "MAX_FILE_SIZE"},
                {new StringContent("Submit Reply"), "submit"}
            };
            var response = await this._webManager.PostFormData(Constants.NEW_REPLY, form);

            return true;
        }

        public async Task<bool> SendUpdatePost(ForumReplyEntity forumReplyEntity)
        {
            if (forumReplyEntity == null)
                return false;
            var form = new MultipartFormDataContent
            {
                {new StringContent("updatepost"), "action"},
                {new StringContent(forumReplyEntity.PostId.ToString()), "postid"},
                {new StringContent(HtmlEncode(forumReplyEntity.Message)), "message"},
                {new StringContent(forumReplyEntity.ParseUrl.ToString()), "parseurl"},
                 {new StringContent(forumReplyEntity.Bookmark), "bookmark"},
                {new StringContent("2097152"), "MAX_FILE_SIZE"},
                {new StringContent("Save Changes"), "submit"}
            };
            var response = await this._webManager.PostFormData(Constants.EDIT_POST, form);

            return true;
        }

        public async Task<string> GetQuoteString(long postId)
        {
            string url = string.Format(Constants.QUOTE_BASE, postId);
            var result = await _webManager.DownloadHtml(url);
            HtmlDocument doc = result.Document;

            var textAreaNodes = doc.DocumentNode.Descendants("textarea").ToArray();

            var textNode = textAreaNodes.FirstOrDefault(node => node.GetAttributeValue("name", "").Equals("message"));

            try
            {
                return  WebUtility.HtmlDecode(textNode.InnerText);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Could not parse newReply form data.");
            }
        }

        
    }
}
