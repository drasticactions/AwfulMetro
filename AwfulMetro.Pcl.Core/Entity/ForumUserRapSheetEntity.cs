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
using HtmlAgilityPack;

namespace AwfulMetro.Core.Entity
{
    public class ForumUserRapSheetEntity
    {
        private ForumUserRapSheetEntity()
        {
        }

        public string PunishmentType { get; private set; }

        // Should be DateTime
        public string Date { get; private set; }

        public string HorribleJerk { get; private set; }

        public string PunishmentReason { get; private set; }

        public string RequestedBy { get; private set; }

        public string ApprovedBy { get; private set; }

        public long ThreadId { get; private set; }

        public long HorribleJerkId { get; private set; }

        public long ApprovedById { get; private set; }

        public long RequestedById { get; private set; }

        public static ForumUserRapSheetEntity FromRapSheet(HtmlNode rapSheetNode)
        {
            var rapSheet = new ForumUserRapSheetEntity();

            List<HtmlNode> rapSheetData = rapSheetNode.Descendants("td").ToList();
            rapSheet.PunishmentType = rapSheetData[0].Descendants("b").FirstOrDefault().InnerText;
            rapSheet.Date = rapSheetData[1].InnerText;

            rapSheet.HorribleJerk = rapSheetData[2].Descendants("a").FirstOrDefault().InnerText;
            rapSheet.HorribleJerkId =
                Convert.ToInt64(
                    rapSheetData[2].Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty).Split('=')
                        [3]);

            rapSheet.PunishmentReason = rapSheetData[3].InnerText;

            rapSheet.RequestedBy = rapSheetData[4].Descendants("a").FirstOrDefault().InnerText;
            rapSheet.RequestedById =
                Convert.ToInt64(
                    rapSheetData[4].Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty).Split('=')
                        [3]);

            rapSheet.ApprovedBy = rapSheetData[5].Descendants("a").FirstOrDefault().InnerText;
            rapSheet.ApprovedById =
                Convert.ToInt64(
                    rapSheetData[5].Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty).Split('=')
                        [3]);

            return rapSheet;
        }
    }
}