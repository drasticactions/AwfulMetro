using BusinessObjects.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entity
{
    public class ForumEntity
    {
        /// <summary>
        /// Represents a forum (Ex. GBS).
        /// </summary>
        /// <param name="name">The name of the forum.</param>
        /// <param name="location">The URL location of the forum.</param>
        /// <param name="description">The forum description.</param>
        public ForumEntity(string name, string location, String description)
        {
            this.Name = name;
            this.Location = Constants.BASE_URL + location;
            this.Description = description;
            this.CurrentPage = 1;
            this.TotalPages = 1;
        }
        public String Name { get; private set; }

        public String Location { get; set; }

        public String Description { get; private set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
