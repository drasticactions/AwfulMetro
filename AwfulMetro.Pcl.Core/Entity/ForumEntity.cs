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
        public string Name { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public int CurrentPage { get; set; }

        public bool IsSubforum { get; set; }

        public int TotalPages { get; set; }

        public long ForumId { get; set; }

        public bool IsBookmarks { get; set; }

        public void SetForumId()
        {
            if (string.IsNullOrEmpty(Location))
            {
                ForumId = 0;
                return;
            }

            string[] forumId = Location.Split('=');
            if (forumId.Length > 1)
            {
                ForumId = Convert.ToInt64(Location.Split('=')[1]);
            }
        }
    }
}