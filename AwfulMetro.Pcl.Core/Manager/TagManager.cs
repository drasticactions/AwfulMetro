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
using System.Linq;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class TagManager
    {
        private readonly IWebManager _webManager;

        public TagManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public TagManager() : this(new WebManager())
        {
        }

        public async Task<List<TagCategoryEntity>> GetTagList(long forumId)
        {
            var tagList = new List<TagEntity>();

            //inject this
            HtmlDocument doc = (await _webManager.GetData(string.Format(Constants.NEW_THREAD, forumId))).Document;

            IEnumerable<HtmlNode> icons =
                doc.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", string.Empty).Equals("posticon"));

            foreach (HtmlNode icon in icons)
            {
                var tag = new TagEntity();
                tag.Parse(icon);
                tagList.Add(tag);
            }
            var tagCategoryList = new List<TagCategoryEntity>();
            var tagCategory = new TagCategoryEntity("Tags", tagList);
            tagCategoryList.Add(tagCategory);
            return tagCategoryList;
        }
    }
}