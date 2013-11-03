using AwfulMetro.Core.Tools;
using System;
using System.Collections.Generic;

namespace AwfulMetro.Core.Entity
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
            this.CategoryId = Convert.ToInt64(location.Split('=')[1]);
            this.ForumList = new List<ForumEntity>();
        }
        public string Name { get; private set; }

        public string Location { get; private set; }

        public long CategoryId { get; private set; }

        /// <summary>
        /// The forums that belong to that category (Ex. GBS, FYAD)
        /// </summary>
        public List<ForumEntity> ForumList { get; set; }
    }
}
