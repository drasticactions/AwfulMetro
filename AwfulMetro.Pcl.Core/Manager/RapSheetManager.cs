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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using AwfulMetro.Core.Entity;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class RapSheetManager
    {
        private readonly IWebManager _webManager;

        public RapSheetManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public RapSheetManager() : this(new WebManager())
        {
        }

        public async Task<string> GetRapSheet(string url)
        {
            try
            {
                HtmlDocument doc = (await _webManager.GetData(url)).Document;

                string html = await PathIO.ReadTextAsync("ms-appx:///Assets/thread.html");

                var doc2 = new HtmlDocument();

                doc2.LoadHtml(html);

                HtmlNode bodyNode = doc2.DocumentNode.Descendants("body").FirstOrDefault();
                HtmlNode rapSheetNode =
                    doc.DocumentNode.Descendants("table")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("standard full"));
                rapSheetNode.Descendants("tr").FirstOrDefault().Remove();

                rapSheetNode.SetAttributeValue("style", "width: 100%");

                HtmlNode[] linkNodes = rapSheetNode.Descendants("a").ToArray();

                if (!linkNodes.Any())
                {
                    // User has no items on their rap sheet, return nothing back.
                    return string.Empty;
                }

                // TODO: Fix Rap sheet/post history views to support links.
                foreach (var linkNode in linkNodes)
                {
                    var divNode = HtmlNode.CreateNode(linkNode.InnerText);
                    linkNode.ParentNode.ReplaceChild(divNode.ParentNode, linkNode);
                }

                bodyNode.InnerHtml = rapSheetNode.OuterHtml;
                return WebUtility.HtmlDecode(WebUtility.HtmlDecode(doc2.DocumentNode.OuterHtml));
            }
            catch (Exception)
            {
                // Error when parsing. Probably a bad user ID.
               return string.Empty;
            }

        }
    }
}