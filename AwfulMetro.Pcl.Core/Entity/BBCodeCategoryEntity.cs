using System.Collections.Generic;

namespace AwfulMetro.Core.Entity
{
    public class BBCodeCategoryEntity
    {
        public BBCodeCategoryEntity(string category, IEnumerable<BBCodeEntity> list)
        {
            List = list;
            Category = category;
        }

        public IEnumerable<BBCodeEntity> List { get; private set; }

        public string Category { get; private set; }
    }
}