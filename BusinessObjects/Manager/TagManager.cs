using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Manager
{
    public class TagManager
    {
        private readonly IWebManager _webManager;
        public TagManager(IWebManager webManager)
        {
            this._webManager = webManager;
        }

        public TagManager() : this(new WebManager()) { }
        public async Task<List<TagCategoryEntity>> GetTagList(long forumId)
        {
            var tagList = new List<TagEntity>();

            //inject this
            var doc = (await _webManager.DownloadHtml(string.Format(Constants.NEW_THREAD, forumId))).Document;

            var icons = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("posticon"));

            foreach(var icon in icons)
            {
                var tag = new TagEntity();
                tag.Parse(icon);
                tagList.Add(tag);
            }
            var tagCategoryList = new List<TagCategoryEntity>();
            var tagCategory = new TagCategoryEntity("Tags", tagList);
            tagCategoryList.Add(tagCategory);
            return tagCategoryList;
        }

    }
}
