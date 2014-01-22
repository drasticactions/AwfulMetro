using System.Collections.Generic;

namespace AwfulMetro.Core.Entity
{
    public class SmileCategoryEntity
    {
        public SmileCategoryEntity(string category, IEnumerable<SmileEntity> smileList)
        {
            List = smileList;
            Category = category;
        }

        public IEnumerable<SmileEntity> List { get; private set; }

        public string Category { get; private set; }
    }
}