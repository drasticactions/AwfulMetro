using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Entity;
using BusinessObjects.Tools;
using HtmlAgilityPack;


namespace BusinessObjects.Manager
{
    public class RapSheetManager
    {
        public static async Task<List<ForumUserRapSheetEntity>> GetRapSheet(string url)
        {
            List<ForumUserRapSheetEntity> rapSheet = new List<ForumUserRapSheetEntity>();
            HttpWebRequest request = await AuthManager.CreateGetRequest(url);
            HtmlDocument doc = await WebManager.DownloadHtml(request);
            HtmlNode rapSheetNode = doc.DocumentNode.Descendants("table").Where(node => node.GetAttributeValue("class", "").Contains("standard full")).FirstOrDefault();
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
