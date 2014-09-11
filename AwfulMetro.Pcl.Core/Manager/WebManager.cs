#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Exceptions;
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
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = await _localStorageManager.LoadCookie(Constants.COOKIE_FILE),
                UseCookies = true,
                UseDefaultCredentials = false
            };
            var httpClient = new HttpClient(handler);
            HttpResponseMessage result = await httpClient.PostAsync(url, form);
            return result;
        }


        public async Task<Result> GetData(string url)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                
                CookieContainer = await _localStorageManager.LoadCookie(Constants.COOKIE_FILE),
                UseCookies = true,
                UseDefaultCredentials = false
            };
            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.IfModifiedSince = DateTimeOffset.UtcNow;
            HttpResponseMessage result = await httpClient.GetAsync(url);
            if (!result.IsSuccessStatusCode)
            {
                // TODO: Create error handling manager to directly handle this type of error.
                throw new WebManagerException(string.Format("Failed to load page: {0}", string.Concat(result.StatusCode, Environment.NewLine, url)));
            }
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