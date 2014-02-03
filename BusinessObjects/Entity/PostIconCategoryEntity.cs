using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Entity
{
    public class PostIconCategoryEntity
    {
        public PostIconCategoryEntity(string category, IEnumerable<PostIconEntity> list)
        {
            List = list;
            Category = category;
        }

        public IEnumerable<PostIconEntity> List { get; private set; }

        public string Category { get; private set; }
    }
}
