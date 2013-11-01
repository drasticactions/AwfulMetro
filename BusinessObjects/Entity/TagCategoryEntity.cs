using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Entity
{
    public class TagCategoryEntity
    {
        public List<TagEntity> List { get; private set; }

        public string Category { get; private set; }

        public TagCategoryEntity(string category, List<TagEntity> list)
        {
            this.List = list;
            this.Category = category;
        }
    }
}
