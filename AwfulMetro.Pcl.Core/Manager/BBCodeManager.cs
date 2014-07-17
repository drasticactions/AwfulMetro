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
using AwfulMetro.Core.Entity;

namespace AwfulMetro.Core.Manager
{
    public static class BBCodeManager
    {
        private static IEnumerable<BBCodeCategoryEntity> bbCodes;

        public static IEnumerable<BBCodeCategoryEntity> BBCodes
        {
            get { return bbCodes ?? (bbCodes = GetBBCodes()); }
        }

        private static IEnumerable<BBCodeCategoryEntity> GetBBCodes()
        {
            var bbCodeCategoryList = new List<BBCodeCategoryEntity>();
            var bbCodeList = new List<BBCodeEntity>
            {
                new BBCodeEntity("url", "url"),
                new BBCodeEntity("email", "email"),
                new BBCodeEntity("img", "img"),
                new BBCodeEntity("timg", "timg"),
                new BBCodeEntity("video", "video"),
                new BBCodeEntity("b", "b"),
                new BBCodeEntity("s", "s"),
                new BBCodeEntity("u", "u"),
                new BBCodeEntity("i", "i"),
                new BBCodeEntity("spoiler", "spoiler"),
                new BBCodeEntity("fixed", "fixed"),
                new BBCodeEntity("super", "super"),
                new BBCodeEntity("sub", "sub"),
                new BBCodeEntity("size", "size"),
                new BBCodeEntity("color", "color"),
                new BBCodeEntity("quote", "quote"),
                new BBCodeEntity("url", "url"),
                new BBCodeEntity("pre", "pre"),
                new BBCodeEntity("code", "code"),
                new BBCodeEntity("php", "php"),
                new BBCodeEntity("list", "list")
            };
            bbCodeCategoryList.Add(new BBCodeCategoryEntity("BBCode", bbCodeList));
            return bbCodeCategoryList;
        }
    }
}