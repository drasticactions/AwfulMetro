using HtmlAgilityPack;
using System;
using System.Linq;

namespace AwfulMetro.Core.Entity
{
    public class ForumUserRapSheetEntity
    {
        public string Type { get; private set; }

        public string Date { get; private set; }

        public string HorribleJerk { get; private set; }

        public string PunishmentReason { get; private set; }

        public string RequestedBy { get; private set; }

        public string ApprovedBy { get; private set; }

        public long ThreadId { get; private set; }

        public long HorribleJerkId { get; private set; }

        public long ApprovedById { get; private set; }

        public long RequestedById { get; private set; }

        public void Parse(HtmlNode rapSheetNode)
        {
            var rapSheetRows = rapSheetNode.Descendants("td");
            this.Type = rapSheetRows.First().Descendants("b").FirstOrDefault().InnerText;
            rapSheetRows.First().Remove();
            this.Date = rapSheetRows.First().InnerText;
            rapSheetRows.First().Remove();
            this.HorribleJerk = rapSheetRows.First().Descendants("a").FirstOrDefault().InnerText;
            this.HorribleJerkId = Convert.ToInt64(rapSheetRows.First().Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Split('=')[3]);
            rapSheetRows.First().Remove();
            this.PunishmentReason = rapSheetRows.First().InnerText;
            rapSheetRows.First().Remove();
            this.RequestedBy = rapSheetRows.First().Descendants("a").FirstOrDefault().InnerText;
            this.RequestedById = Convert.ToInt64(rapSheetRows.First().Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Split('=')[3]);
            rapSheetRows.First().Remove();
            this.ApprovedBy = rapSheetRows.First().Descendants("a").FirstOrDefault().InnerText;
            this.ApprovedById = Convert.ToInt64(rapSheetRows.First().Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Split('=')[3]);
            rapSheetRows.First().Remove();
        }
    }
}
