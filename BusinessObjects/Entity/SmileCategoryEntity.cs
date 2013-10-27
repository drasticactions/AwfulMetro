using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entity
{
    public class SmileCategoryEntity
    {
        public List<SmileEntity> SmileList { get; private set; }

        public string Category { get; private set; }

        public SmileCategoryEntity(string category, List<SmileEntity> smileList)
        {
            this.SmileList = smileList;
            this.Category = category;
        }
    }
}
