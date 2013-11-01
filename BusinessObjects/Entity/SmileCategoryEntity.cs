using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Entity
{
    public class SmileCategoryEntity
    {
        public List<SmileEntity> List { get; private set; }

        public string Category { get; private set; }

        public SmileCategoryEntity(string category, List<SmileEntity> smileList)
        {
            this.List = smileList;
            this.Category = category;
        }
    }
}
