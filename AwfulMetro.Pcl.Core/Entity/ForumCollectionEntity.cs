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
using System.Collections.Generic;

namespace AwfulMetro.Core.Entity
{
    public class ForumCollectionEntity
    {
        public ForumCollectionEntity(string forumName, IEnumerable<ForumThreadEntity> forumThreadList,
            IEnumerable<ForumEntity> forumSubcategoryList)
        {
            ForumThreadList = forumThreadList;
            ForumSubcategoryList = forumSubcategoryList;
            ForumType = new List<string> {"Subforums", "Threads"};
            ForumName = forumName;
            CurrentPage = 1;
        }

        public IEnumerable<ForumThreadEntity> ForumThreadList { get; private set; }

        public IEnumerable<ForumEntity> ForumSubcategoryList { get; private set; }

        public string ForumName { get; private set; }

        public IEnumerable<string> ForumType { get; private set; }

        public int CurrentPage { get; private set; }
    }
}