using System.Collections.Generic;

namespace AwfulMetro.Core.Entity
{
    public class BBCodeCategoryEntity
    {
        public IEnumerable<BBCodeEntity> List { get; private set; }

        public string Category { get; private set; }

        public BBCodeCategoryEntity(string category, IEnumerable<BBCodeEntity> list)
        {
            this.List = list;
            this.Category = category;
        }
    }
}
