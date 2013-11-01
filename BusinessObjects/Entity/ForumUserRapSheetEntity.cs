using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace AwfulMetro.Core.Entity
{
    public class ForumUserRapSheetEntity
    {
        public String Type { get; private set; }

        public String Date { get; private set; }

        public String HorribleJerk { get; private set; }

        public String PunishmentReason { get; private set; }

        public String RequestedBy { get; private set; }

        public String ApprovedBy { get; private set; }

        public long ThreadId { get; private set; }

        public long HorribleJerkId { get; private set; }

        public long ApprovedById { get; private set; }

        public long RequestedById { get; private set; }

        public void Parse(HtmlNode RapSheetNode)
        {
            var rapSheetRows = RapSheetNode.Descendants("td");
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
