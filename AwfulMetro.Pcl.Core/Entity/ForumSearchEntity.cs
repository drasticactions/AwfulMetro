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
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    public class ForumSearchEntity
    {
        public string Tag { get; private set; }

        public string ThreadTitle { get; private set; }

        public string PostContent { get; private set; }

        public long PostId { get; private set; }

        public long ThreadId { get; private set; }

        public string ForumTitle { get; private set; }

        public long ForumId { get; private set; }

        public string Author { get; private set; }

        public long AuthorId { get; private set; }

        public int ReplyCount { get; private set; }

        public int ThreadViewCount { get; private set; }

        public string Date { get; private set; }

        public void Parse(HtmlNode searchRow)
        {
            if (!searchRow.Descendants("td").FirstOrDefault().InnerText.Contains("no permission"))
            {
                Tag =
                    searchRow.Descendants("td")
                        .FirstOrDefault()
                        .Descendants("img")
                        .FirstOrDefault()
                        .GetAttributeValue("src", string.Empty);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                ThreadTitle =
                    WebUtility.HtmlDecode(
                        searchRow.Descendants("td").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
                ThreadId =
                    Int64.Parse(
                        Regex.Match(
                            searchRow.Descendants("td")
                                .FirstOrDefault()
                                .Descendants("a")
                                .FirstOrDefault()
                                .GetAttributeValue("href", string.Empty)
                                .Split('=')[1], @"\d+").Value);
                PostContent =
                    WebUtility.HtmlDecode(
                        searchRow.Descendants("td").FirstOrDefault().Descendants("div").FirstOrDefault().InnerText);
                PostId =
                    Int64.Parse(
                        Regex.Match(
                            searchRow.Descendants("td")
                                .FirstOrDefault()
                                .Descendants("a")
                                .LastOrDefault()
                                .GetAttributeValue("href", string.Empty)
                                .Split('=')[3], @"\d+").Value);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                ForumTitle =
                    WebUtility.HtmlDecode(
                        searchRow.Descendants("td").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
                ForumId =
                    Int64.Parse(
                        Regex.Match(
                            searchRow.Descendants("td")
                                .FirstOrDefault()
                                .Descendants("a")
                                .FirstOrDefault()
                                .GetAttributeValue("href", string.Empty)
                                .Split('=')[1], @"\d+").Value);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                Author =
                    WebUtility.HtmlDecode(
                        searchRow.Descendants("td").FirstOrDefault().Descendants("a").FirstOrDefault().InnerText);
                AuthorId =
                    Convert.ToInt64(
                        searchRow.Descendants("td")
                            .FirstOrDefault()
                            .Descendants("a")
                            .FirstOrDefault()
                            .GetAttributeValue("href", string.Empty)
                            .Split('=')[2]);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                ReplyCount = Convert.ToInt32(searchRow.Descendants("td").FirstOrDefault().InnerText);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                ThreadViewCount = Convert.ToInt32(searchRow.Descendants("td").FirstOrDefault().InnerText);
                searchRow.Descendants("td").FirstOrDefault().Remove();
                Date = searchRow.Descendants("td").FirstOrDefault().Descendants("span").FirstOrDefault().InnerText;
                searchRow.Descendants("td").FirstOrDefault().Remove();
            }
        }
    }
}