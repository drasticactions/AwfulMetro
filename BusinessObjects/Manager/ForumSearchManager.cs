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
using Windows.Storage;
using Windows.Storage.Streams;

namespace BusinessObjects.Manager
{
    public class ForumSearchManager
    {
        public static async Task<List<ForumSearchEntity>> GetSearchResults(String url)
        {
            List<ForumSearchEntity> searchResults = new List<ForumSearchEntity>();
            HttpWebRequest request = await AuthManager.CreateGetRequest(url);
            HtmlDocument doc = await WebManager.DownloadHtml(request);
            HtmlNode searchNode = doc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Contains("inner")).FirstOrDefault();
            url = Constants.BASE_URL + "search.php" + searchNode.Descendants("a").FirstOrDefault().GetAttributeValue("href", "");
            request = await AuthManager.CreateGetRequest(url);
            doc = await WebManager.DownloadHtml(request);
            //Test persisting Html from search page.
            //HtmlDocument doc = new HtmlDocument();
            //string html = await LoadSearchPage("search.txt");
            //doc.LoadHtml(html);
            searchResults = ForumSearchManager.ParseSearchRows(doc.DocumentNode.Descendants("table").Where(node => node.GetAttributeValue("id", "").Contains("main_full")).FirstOrDefault());
            //ForumSearchManager.SaveSearchPage("search.txt", doc.DocumentNode.OuterHtml.ToString());
            return searchResults;
        }

        private static List<ForumSearchEntity> ParseSearchRows(HtmlNode searchNode)
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

        private static async void SaveSearchPage(string filename, string text)
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

        public static async Task<string> LoadSearchPage(string filename)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile;
            string test;
            try
            {
                sampleFile = await localFolder.GetFileAsync(filename);
                var stream = await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                var size = stream.Size;
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
