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
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    public class PopularThreadsTrendsEntity
    {
        public string Tag { get; private set; }

        public string Title { get; private set; }

        public long Id { get; private set; }

        public string LocationUrl { get; private set; }

        public void ParseThread(HtmlNode popularThreadsNode)
        {
            Tag = popularThreadsNode.Descendants("img").FirstOrDefault().GetAttributeValue("src", string.Empty);
            Title = WebUtility.HtmlDecode(popularThreadsNode.Descendants("a").FirstOrDefault().InnerText);
            Id =
                Convert.ToInt64(
                    popularThreadsNode.Descendants("a")
                        .FirstOrDefault()
                        .GetAttributeValue("href", string.Empty)
                        .Split('=')[1]);
        }

        public void ParseTrend(HtmlNode popularTrendsNode)
        {
            Title = WebUtility.HtmlDecode(popularTrendsNode.Descendants("a").FirstOrDefault().InnerText);
            LocationUrl = Constants.FRONT_PAGE +
                          popularTrendsNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty);
        }
    }
}