using BusinessObjects.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entity
{
    public class ForumCategoryEntity
    {
        /// <summary>
        /// Represents the categories forums belong to. (Ex: Main, Discussion)
        /// </summary>
        /// <param name="name">The name of the category.</param>
        /// <param name="location">The URL stub where it belongs.</param>
        public ForumCategoryEntity(string name, string location)
        {
            this.Name = name;
            this.Location = Constants.BASE_URL + location;
            this.ForumList = new List<ForumEntity>();
        }
        public String Name { get; private set; }

        public String Location { get; private set; }

        /// <summary>
        /// The forums that belong to that category (Ex. GBS, FYAD)
        /// </summary>
        public List<ForumEntity> ForumList { get; set; }
    }
}
