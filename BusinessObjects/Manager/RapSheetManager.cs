using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using HtmlAgilityPack;


namespace AwfulMetro.Core.Manager
{
    public class RapSheetManager
    {
        private readonly IWebManager webManager;
        public RapSheetManager(IWebManager webManager)
        {
            this.webManager = webManager;
        }

        public RapSheetManager() : this(new WebManager()) { }

        public async Task<List<ForumUserRapSheetEntity>> GetRapSheet(string url)
        {
            List<ForumUserRapSheetEntity> rapSheet = new List<ForumUserRapSheetEntity>();
            //inject this
            var doc = (await webManager.DownloadHtml(url)).Document;
            
            HtmlNode rapSheetNode = doc.DocumentNode.Descendants("table").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("standard full"));
            rapSheetNode.Descendants("tr").FirstOrDefault().Remove();
            if (rapSheetNode.Descendants("tr").Any())
            {
                foreach (HtmlNode tableRow in rapSheetNode.Descendants("tr"))
                {
                    ForumUserRapSheetEntity row = new ForumUserRapSheetEntity();
                    row.Parse(tableRow);
                    rapSheet.Add(row);
                }
            }
            return rapSheet;
        }

    }
}
