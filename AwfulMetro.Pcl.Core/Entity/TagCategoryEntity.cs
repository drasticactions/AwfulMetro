using System.Collections.Generic;

namespace AwfulMetro.Core.Entity
{
    public class TagCategoryEntity
    {
        public TagCategoryEntity(string category, IEnumerable<TagEntity> list)
        {
            List = list;
            Category = category;
        }

        public IEnumerable<TagEntity> List { get; private set; }

        public string Category { get; private set; }
    }
}