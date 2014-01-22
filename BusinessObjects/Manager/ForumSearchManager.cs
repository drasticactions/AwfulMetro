using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<ForumSearchEntity>> GetSearchResults(String url)
        {
            try
            {
                var searchResults = new List<ForumSearchEntity>();

                //inject this
                HtmlDocument doc = (await _webManager.DownloadHtml(url)).Document;

                HtmlNode searchNode =
                    doc.DocumentNode.Descendants("div")
                        .FirstOrDefault(node => node.GetAttributeValue("class", string.Empty).Contains("inner"));
                url = Constants.BASE_URL + "search.php" +
                      searchNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", string.Empty);

                doc = (await _webManager.DownloadHtml(url)).Document;
                //Test persisting Html from search page.
                //HtmlDocument doc = new HtmlDocument();
                //string html = await LoadSearchPage("search.txt");
                //doc.LoadHtml(html);
                searchResults =
                    ParseSearchRows(
                        doc.DocumentNode.Descendants("table")
                            .FirstOrDefault(node => node.GetAttributeValue("id", string.Empty).Contains("main_full")));
                //ForumSearchManager.SaveSearchPage("search.txt", doc.DocumentNode.OuterHtml.ToString());
                return searchResults;
            }
            catch (Exception)
            {
                // Person does not have platinum.
                return null;
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