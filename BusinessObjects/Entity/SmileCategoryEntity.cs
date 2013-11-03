using System.Collections.Generic;

namespace AwfulMetro.Core.Entity
{
    public class SmileCategoryEntity
    {
        public IEnumerable<SmileEntity> List { get; private set; }

        public string Category { get; private set; }

        public SmileCategoryEntity(string category, IEnumerable<SmileEntity> smileList)
        {
            this.List = smileList;
            this.Category = category;
        }
    }
}
