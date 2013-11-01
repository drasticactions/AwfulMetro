using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using Windows.Storage;
using Windows.Storage.Streams;

namespace AwfulMetro.Core.Manager
{
    public class ForumSearchManager
    {
        private readonly IWebManager webManager;
        public ForumSearchManager(IWebManager webManager)
        {
            this.webManager = webManager;
        }

        public ForumSearchManager() : this(new WebManager()) { }

        public async Task<List<ForumSearchEntity>> GetSearchResults(String url)
        {
            var searchResults = new List<ForumSearchEntity>();

            //inject this
            var doc = (await webManager.DownloadHtml(url)).Document;

            HtmlNode searchNode = doc.DocumentNode.Descendants("div").FirstOrDefault(node => node.GetAttributeValue("class", "").Contains("inner"));
            url = Constants.BASE_URL + "search.php" + searchNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");

            doc = (await webManager.DownloadHtml(url)).Document;
            //Test persisting Html from search page.
            //HtmlDocument doc = new HtmlDocument();
            //string html = await LoadSearchPage("search.txt");
            //doc.LoadHtml(html);
            searchResults = ParseSearchRows(doc.DocumentNode.Descendants("table").FirstOrDefault(node => node.GetAttributeValue("id", "").Contains("main_full")));
            //ForumSearchManager.SaveSearchPage("search.txt", doc.DocumentNode.OuterHtml.ToString());
            return searchResults;
        }

        private List<ForumSearchEntity> ParseSearchRows(HtmlNode searchNode)
        {
            List<ForumSearchEntity> searchRowEntityList = new List<ForumSearchEntity>();
            var searchRowNodes = searchNode.Descendants("tr");
            searchRowNodes.FirstOrDefault().Remove();
            foreach (HtmlNode searchRow in searchRowNodes)
            {
                ForumSearchEntity searchRowEntity = new ForumSearchEntity();
                searchRowEntity.Parse(searchRow);
                if(!string.IsNullOrEmpty(searchRowEntity.Author))
                    searchRowEntityList.Add(searchRowEntity);
            }
            return searchRowEntityList;
        }

        private async void SaveSearchPage(string filename, string text)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            var stream = await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            using (var outputStream = stream.GetOutputStreamAt(0))
            {
                DataWriter dataWriter = new DataWriter(outputStream);
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
                var stream = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);
                string test;
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    test = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);

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
