using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;

namespace AwfulMetro.Core.Manager
{
    public class WebManager : IWebManager
    {
        private const string ACCEPT = "text/html, application/xhtml+xml, */*";

        private const string POST_CONTENT_TYPE = "application/x-www-form-urlencoded";

        private const string REPLY_BOUNDARY = "----WebKitFormBoundaryYRBJZZBPUZAdxj3S";
        private const string EDIT_BOUNDARY = "----WebKitFormBoundaryksMFcMGBHc3jdB0P";
        private const string REPLY_CONTENT_TYPE = "multipart/form-data; boundary=" + REPLY_BOUNDARY;
        private const string EDIT_CONTENT_TYPE = "multipart/form-data; boundary=" + EDIT_BOUNDARY;
        private readonly ILocalStorageManager _localStorageManager;

        static WebManager()
        {
            CookieJar = new List<Cookie>();
        }

        public WebManager(ILocalStorageManager localStorageManager)
        {
            _localStorageManager = localStorageManager;
        }

        public WebManager() : this(new LocalStorageManager())
        {
        }

        public static List<Cookie> CookieJar { get; private set; }

        public bool IsNetworkAvailable
        {
            get { return NetworkInterface.GetIsNetworkAvailable(); }
        }

        public async Task<CookieContainer> PostData(string url, string data)
        {
            var uri = new Uri(url);
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Accept = ACCEPT;
            request.CookieContainer = await _localStorageManager.LoadCookie(Constants.COOKIE_FILE);
            request.Method = "POST";
            request.ContentType = POST_CONTENT_TYPE;
            request.UseDefaultCredentials = false;

            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                writer.Write(data);
            }

            WebResponse response = await request.GetResponseAsync();
            return request.CookieContainer;
        }

        public async Task<HttpResponseMessage> PostFormData(string url, MultipartFormDataContent form)
        {
            // TODO: This is a temp solution. Every post should use HttpWebRequest or HttpClient, but not both. 
            var handler = new HttpClientHandler
            {
                CookieContainer = await _localStorageManager.LoadCookie(Constants.COOKIE_FILE),
                UseCookies = true,
                UseDefaultCredentials = false
            };
            var httpClient = new HttpClient(handler);
            HttpResponseMessage result = await httpClient.PostAsync(url, form);
            return result;
        }


        public async Task<Result> DownloadHtml(string url)
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = await _localStorageManager.LoadCookie(Constants.COOKIE_FILE),
                UseCookies = true,
                UseDefaultCredentials = false
            };
            var httpClient = new HttpClient(handler);
            HttpResponseMessage result = await httpClient.GetAsync(url);
            Stream stream = await result.Content.ReadAsStreamAsync();
            using (var reader = new StreamReader(stream))
            {
                string html = reader.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                return new Result(doc, result.RequestMessage.RequestUri.AbsoluteUri);
            }
        }

        private static CookieContainer GetCookiesForUri(Uri uri)
        {
            var container = new CookieContainer();
            List<Cookie> cookies = CookieJar;
            foreach (Cookie cookie in cookies)
            {
                container.Add(uri, cookie);
            }
            return container;
        }

        public class Result
        {
            public Result(HtmlDocument document, string absoluteUri)
            {
                Document = document;
                AbsoluteUri = absoluteUri;
            }

            public HtmlDocument Document { get; private set; }
            public string AbsoluteUri { get; private set; }
        }
    }
}