using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class RapSheetManager
    {
        private readonly IWebManager _webManager;
        public RapSheetManager(IWebManager webManager)
        {
            this._webManager = webManager;
        }

        public RapSheetManager() : this(new WebManager()) { }

        public async Task<List<ForumUserRapSheetEntity>> GetRapSheet(string url)
        {
            var rapSheets = new List<ForumUserRapSheetEntity>();
            var doc = (await _webManager.DownloadHtml(url)).Document;
            
            HtmlNode rapSheetNode = doc.DocumentNode.Descendants("table").FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("standard full"));
            rapSheetNode.Descendants("tr").FirstOrDefault().Remove();
            if (rapSheetNode.Descendants("tr").Any())
            {
                rapSheets.AddRange(rapSheetNode.Descendants("tr").Select(ForumUserRapSheetEntity.FromRapSheet));
            }
            return rapSheets;
        }

    }
}
