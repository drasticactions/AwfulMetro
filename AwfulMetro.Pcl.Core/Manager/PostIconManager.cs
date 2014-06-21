using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class PostIconManager
    {
        private readonly IWebManager _webManager;

        public PostIconManager()
            : this(new WebManager())
        {
        }

        public PostIconManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public async Task<IEnumerable<PostIconCategoryEntity>> GetPostIcons(ForumEntity forum)
        {
            string url = string.Format(Constants.NEW_THREAD, forum.ForumId);
            WebManager.Result result = await _webManager.GetData(url);
            HtmlDocument doc = result.Document;
            HtmlNode[] pageNodes = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", string.Empty).Equals("posticon")).ToArray();
            var postIconEntityList = new List<PostIconEntity>();
            foreach (var pageNode in pageNodes)
            {
                var postIconEntity = new PostIconEntity();
                postIconEntity.Parse(pageNode);
                postIconEntityList.Add(postIconEntity);
            }
            var postIconCategoryEntity = new PostIconCategoryEntity("Post Icon", postIconEntityList);
            var postIconCategoryList = new List<PostIconCategoryEntity> {postIconCategoryEntity};
            return postIconCategoryList;
        }

        public async Task<IEnumerable<PostIconEntity>> GetPostIconList(ForumEntity forum)
        {
            string url = string.Format(Constants.NEW_THREAD, forum.ForumId);
            WebManager.Result result = await _webManager.GetData(url);
            HtmlDocument doc = result.Document;
            HtmlNode[] pageNodes = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", string.Empty).Equals("posticon")).ToArray();
            var postIconEntityList = new List<PostIconEntity>();
            foreach (var pageNode in pageNodes)
            {
                var postIconEntity = new PostIconEntity();
                postIconEntity.Parse(pageNode);
                postIconEntityList.Add(postIconEntity);
            }
            return postIconEntityList;
        }

        public async Task<IEnumerable<PostIconCategoryEntity>> GetPmPostIcons()
        {
            string url = Constants.NEW_PRIVATE_MESSAGE;
            WebManager.Result result = await _webManager.GetData(url);
            HtmlDocument doc = result.Document;
            HtmlNode[] pageNodes = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", string.Empty).Equals("posticon")).ToArray();
            var postIconEntityList = new List<PostIconEntity>();
            foreach (var pageNode in pageNodes)
            {
                var postIconEntity = new PostIconEntity();
                postIconEntity.Parse(pageNode);
                postIconEntityList.Add(postIconEntity);
            }
            var postIconCategoryEntity = new PostIconCategoryEntity("Post Icon", postIconEntityList);
            var postIconCategoryList = new List<PostIconCategoryEntity> { postIconCategoryEntity };
            return postIconCategoryList;
        }
    }
}
