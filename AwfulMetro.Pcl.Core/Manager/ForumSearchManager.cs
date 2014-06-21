using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class ForumSearchManager
    {
        private readonly IWebManager _webManager;

        public ForumSearchManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public ForumSearchManager() : this(new WebManager())
        {
        }

        public async Task<string> GetSearchResults(String url)
        {
            try
            {
                string htmlBody = await PathIO.ReadTextAsync("ms-appx:///Assets/thread.html");

                var doc2 = new HtmlDocument();

                doc2.LoadHtml(htmlBody);

                HtmlNode bodyNode = doc2.DocumentNode.Descendants("body").FirstOrDefault();

                //inject this
                HtmlDocument doc = (await _webManager.GetData(url)).Document;

                HtmlNode searchNode =
                    doc.DocumentNode.Descendants("div")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("inner"));
                url = string.Format("{0}search.php{1}", Constants.BASE_URL, searchNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty));

                doc = (await _webManager.GetData(url)).Document;
                var tableNode = doc.DocumentNode.Descendants("table")
                    .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("main_full"));

                tableNode.SetAttributeValue("style", "width: 100%");

                HtmlNode[] linkNodes = tableNode.Descendants("a").ToArray();

                if (!linkNodes.Any())
                {
                    // User has no items on their rap sheet, return nothing back.
                    return string.Empty;
                }

                foreach (var linkNode in linkNodes)
                {
                    var divNode = HtmlNode.CreateNode(linkNode.InnerText);
                    linkNode.ParentNode.ReplaceChild(divNode.ParentNode, linkNode);
                }

                HtmlNode[] jumpPostsNodes = tableNode.Descendants("div").Where(node => node.GetAttributeValue("align", string.Empty).Equals("right")).ToArray();

                foreach (var jumpPost in jumpPostsNodes)
                {
                    jumpPost.Remove();
                }

                bodyNode.InnerHtml = tableNode.OuterHtml;
                return WebUtility.HtmlDecode(WebUtility.HtmlDecode(doc2.DocumentNode.OuterHtml));
            }
            catch (Exception)
            {
                // Person does not have platinum.
                return string.Empty;
            }
        }

        private List<ForumSearchEntity> ParseSearchRows(HtmlNode searchNode)
        {
            var searchRowEntityList = new List<ForumSearchEntity>();
            IEnumerable<HtmlNode> searchRowNodes = searchNode.Descendants("tr");
            searchRowNodes.FirstOrDefault().Remove();
            foreach (HtmlNode searchRow in searchRowNodes)
            {
                var searchRowEntity = new ForumSearchEntity();
                searchRowEntity.Parse(searchRow);
                if (!string.IsNullOrEmpty(searchRowEntity.Author))
                    searchRowEntityList.Add(searchRowEntity);
            }
            return searchRowEntityList;
        }

        private async void SaveSearchPage(string filename, string text)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile =
                await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            IRandomAccessStream stream = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);
            using (IOutputStream outputStream = stream.GetOutputStreamAt(0))
            {
                var dataWriter = new DataWriter(outputStream);
                dataWriter.WriteString(text);
                await dataWriter.StoreAsync();
                await outputStream.FlushAsync();
            }
        }

        public async Task<string> LoadSearchPage(string filename)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync(filename);
                IRandomAccessStream stream = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);
                string test;
                using (IInputStream inputStream = stream.GetInputStreamAt(0))
                {
                    test = await FileIO.ReadTextAsync(sampleFile);
                }
                return test;
            }
            catch
            {
                //Ignore, we will ask for log in information.
            }
            return string.Empty;
        }
    }
}