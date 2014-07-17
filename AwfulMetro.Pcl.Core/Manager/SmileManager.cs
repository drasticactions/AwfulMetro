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
using System.Net;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class SmileManager
    {
        private readonly IWebManager _webManager;

        public SmileManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public SmileManager() : this(new WebManager())
        {
        }

        public async Task<List<SmileCategoryEntity>> GetSmileList()
        {
            var smileCategoryList = new List<SmileCategoryEntity>();

            //inject this
            HtmlDocument doc = (await _webManager.GetData(Constants.SMILE_URL)).Document;

            IEnumerable<HtmlNode> smileCategoryTitles =
                doc.DocumentNode.Descendants("div")
                    .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("inner"))
                    .Descendants("h3");
            List<string> categoryTitles =
                smileCategoryTitles.Select(smileCategoryTitle => WebUtility.HtmlDecode(smileCategoryTitle.InnerText))
                    .ToList();
            IEnumerable<HtmlNode> smileNodes =
                doc.DocumentNode.Descendants("ul")
                    .Where(node => node.GetAttributeValue("class", string.Empty).Contains("smilie_group"));
            int smileCount = 0;
            foreach (HtmlNode smileNode in smileNodes)
            {
                var smileList = new List<SmileEntity>();
                IEnumerable<HtmlNode> smileIcons = smileNode.Descendants("li");
                foreach (HtmlNode smileIcon in smileIcons)
                {
                    var smileEntity = new SmileEntity();
                    smileEntity.Parse(smileIcon);
                    smileList.Add(smileEntity);
                }
                smileCategoryList.Add(new SmileCategoryEntity(categoryTitles[smileCount], smileList));
                smileCount++;
            }
            return smileCategoryList;
        }
    }
}