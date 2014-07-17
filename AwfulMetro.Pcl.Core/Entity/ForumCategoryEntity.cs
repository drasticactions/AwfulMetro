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
using System.Collections.Generic;
using AwfulMetro.Core.Tools;

namespace AwfulMetro.Core.Entity
{
    public class ForumCategoryEntity
    {
        /// <summary>
        ///     Represents the categories forums belong to. (Ex: Main, Discussion)
        /// </summary>
        /// <param name="name">The name of the category.</param>
        /// <param name="location">The URL stub where it belongs.</param>
        public ForumCategoryEntity(string name, string location)
        {
            Name = name;
            Location = Constants.BASE_URL + location;
            CategoryId = Convert.ToInt64(location.Split('=')[1]);
            ForumList = new List<ForumEntity>();
        }

        public string Name { get; private set; }

        public string Location { get; private set; }

        public long CategoryId { get; private set; }

        /// <summary>
        ///     The forums that belong to that category (Ex. GBS, FYAD)
        /// </summary>
        public List<ForumEntity> ForumList { get; set; }
    }
}