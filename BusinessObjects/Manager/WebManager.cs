using System.Net.NetworkInformation;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Manager
{
    public class WebManager : IWebManager
    {
        private readonly ILocalStorageManager _localStorageManager;
        public WebManager(ILocalStorageManager localStorageManager)
        {
            this._localStorageManager = localStorageManager;
        }

        public WebManager() : this(new LocalStorageManager()) { }

        static WebManager()
        {
            CookieJar = new List<Cookie>();
        }

        private const string ACCEPT = "text/html, application/xhtml+xml, */*";

        private const string POST_CONTENT_TYPE = "application/x-www-form-urlencoded";

        public static List<Cookie> CookieJar { get; private set; }

        public bool IsNetworkAvailable
        {
            get
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
        }
        private static CookieContainer GetCookiesForUri(Uri uri)
        {
            var container = new CookieContainer();
            var cookies = CookieJar;
            foreach (Cookie cookie in cookies)
            {
                container.Add(uri, cookie);
            }
            return container;
        }

        public async Task<CookieContainer> PostData(string url, string data)
        {
            var uri = new Uri(url);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = ACCEPT;
            request.CookieContainer = GetCookiesForUri(uri);
            request.Method = "POST";
            request.ContentType = POST_CONTENT_TYPE;
            request.UseDefaultCredentials = false;

            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                writer.Write(data);
            }

            var response = await request.GetResponseAsync();
            return request.CookieContainer;

        }

        private static async Task<HttpWebRequest> CreateGetRequest(string url, ILocalStorageManager localStorageManager)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = ACCEPT;
            request.CookieContainer = await localStorageManager.LoadCookie(Constants.COOKIE_FILE);
            request.Method = "GET";
            request.UseDefaultCredentials = false;
            return request;
        }


        public async Task<Result> DownloadHtml(string url)
        {
            var request = await CreateGetRequest(url, this._localStorageManager);

            using (var response = await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                return new Result(doc, response.ResponseUri.AbsoluteUri);
            }
        }

        public class Result
        {
            public HtmlDocument Document { get; private set; }
            public string AbsoluteUri { get; private set; }

            public Result(HtmlDocument document, string absoluteUri)
            {
                this.Document = document;
                this.AbsoluteUri = absoluteUri;
            }
        }

        public static async Task<HtmlDocument> DownloadHtmlClient(HttpClient request, string url)
        {
            var response = await request.GetAsync(url);
            var stream = await response.Content.ReadAsStreamAsync();
            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                return doc;
            }
        }
    }
}
