using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Manager
{
    public class TagManager
    {
        private readonly IWebManager webManager;
        public TagManager(IWebManager webManager)
        {
            this.webManager = webManager;
        }

        public TagManager() : this(new WebManager()) { }
        public async Task<List<TagCategoryEntity>> GetTagList(long forumId)
        {
            List<TagEntity> tagList = new List<TagEntity>();

            //inject this
            var doc = (await webManager.DownloadHtml(string.Format(Constants.NEW_THREAD, forumId))).Document;
            
            var iconArray = doc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("posticon"))
                .ToArray();
            foreach(var icon in iconArray)
            {
                TagEntity tag = new TagEntity();
                tag.Parse(icon);
                tagList.Add(tag);
            }
            List<TagCategoryEntity> tagCategoryList = new List<TagCategoryEntity>();
            TagCategoryEntity tagCategory = new TagCategoryEntity("Tags", tagList);
            tagCategoryList.Add(tagCategory);
            return tagCategoryList;
        }

    }
}
