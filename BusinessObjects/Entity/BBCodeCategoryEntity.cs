using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entity
{
    public class BBCodeCategoryEntity
    {
         public List<BBCodeEntity> List { get; private set; }

        public string Category { get; private set; }

        public BBCodeCategoryEntity(string category, List<BBCodeEntity> list)
        {
            this.List = list;
            this.Category = category;
        }
    }
}
