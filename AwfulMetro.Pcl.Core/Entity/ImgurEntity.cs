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
namespace AwfulMetro.Core.Entity
{
    public class ImgurEntity
    {
        public Data data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }

        public class Data
        {
            public string id { get; set; }
            public object title { get; set; }
            public object description { get; set; }
            public int datetime { get; set; }
            public string type { get; set; }
            public bool animated { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int size { get; set; }
            public int views { get; set; }
            public int bandwidth { get; set; }
            public bool favorite { get; set; }
            public object nsfw { get; set; }
            public object section { get; set; }
            public string deletehash { get; set; }
            public string link { get; set; }
        }
    }
}