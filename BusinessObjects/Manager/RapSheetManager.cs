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
                HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;

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