using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class TagManager
    {
        private readonly IWebManager _webManager;

        public TagManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public TagManager() : this(new WebManager())
        {
        }

        public async Task<List<TagCategoryEntity>> GetTagList(long forumId)
        {
            var tagList = new List<TagEntity>();

            //inject this
            HtmlDocument doc = (await _webManager.GetData(string.Format(Constants.NEW_THREAD, forumId))).Document;

            IEnumerable<HtmlNode> icons =
                doc.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", string.Empty).Equals("posticon"));

            foreach (HtmlNode icon in icons)
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