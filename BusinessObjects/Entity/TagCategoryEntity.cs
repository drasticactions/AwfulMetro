using System.Collections.Generic;

namespace AwfulMetro.Core.Entity
{
    public class TagCategoryEntity
    {
        public IEnumerable<TagEntity> List { get; private set; }

        public string Category { get; private set; }

        public TagCategoryEntity(string category, IEnumerable<TagEntity> list)
        {
            this.List = list;
            this.Category = category;
        }
    }
}
