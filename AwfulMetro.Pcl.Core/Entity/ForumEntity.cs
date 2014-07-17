#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion
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