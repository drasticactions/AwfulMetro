using System;

namespace AwfulMetro.Core.Entity
{
    public class ForumEntity
    {
        /// <summary>
        ///     Represents a forum (Ex. GBS).
        /// </summary>
        /// <param name="name">The name of the forum.</param>
        /// <param name="location">The URL location of the forum.</param>
        /// <param name="description">The forum description.</param>
        /// <param name="isSubforum">If this specific forum is a subforum or not.</param>
        public ForumEntity(string name, string location, string description, bool isSubforum)
        {
            Name = name;
            IsSubforum = isSubforum;
            Location = location;
            string[] forumId = location.Split('=');
            if (forumId.Length > 1)
            {
                ForumId = Convert.ToInt64(location.Split('=')[1]);
            }
            Description = description;
            CurrentPage = 1;
            TotalPages = 1;
            IsBookmarks = name == "Bookmarks";
        }

        public string Name { get; private set; }

        public string Location { get; private set; }

        public string Description { get; private set; }

        public int CurrentPage { get; set; }

        public bool IsSubforum { get; set; }

        public int TotalPages { get; set; }

        public long ForumId { get; private set; }

        public bool IsBookmarks { get; private set; }
    }
}