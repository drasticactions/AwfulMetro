using BusinessObjects.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Storage;

namespace BusinessObjects.Manager
{
    /* TODO: Tightly coupled to the following. Break dependencies to enable testing.
     * WebRequest  
     * local storage
     * NetworkInterface (candidate for a shim IMO)
    */
    public class AuthManager : IAuthenticationManager
    {
        public static List<Cookie> CookieJar { get; private set; }

        private const string ACCEPT = "text/html, application/xhtml+xml, */*";

        private const string POST_CONTENT_TYPE = "application/x-www-form-urlencoded";

        static AuthManager()
        {
            CookieJar = new List<Cookie>();
        }

        private const string COOKIE_DOMAIN_URL = "http://fake.forums.somethingawful.com";

        private const string LOGIN_URL = "http://forums.somethingawful.com/account.php?";

        public string Status { get; set; }
   
        public static HttpWebRequest CreatePostRequest(string url)
        {
            var uri = new Uri(url);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = ACCEPT;
            request.CookieContainer = GetCookiesForUri(uri);
            request.Method = "POST";
            request.ContentType = POST_CONTENT_TYPE;
            request.UseDefaultCredentials = false;
            return request;
        }

        public static async Task<HttpWebRequest> CreateGetRequest(string url)
        {
            var uri = new Uri(url);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = ACCEPT;
            request.CookieContainer = await AuthManager.LoadCookie(Constants.COOKIE_FILE);
            request.Method = "GET";
            request.UseDefaultCredentials = false;
            return request;
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


        public async Task<bool> Authenticate(string username, string password, int timeout = Constants.DEFAULT_TIMEOUT_IN_MILLISECONDS)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                throw new LoginFailedException("The network is unavailable. Check your network settings and please try again.");
            }

            try
            {
                await this.SendLoginData(username, password);
            }
            catch
            {
                return false;    
            }

            return true;

        }

        public async Task<bool> SendLoginData(string username, string password)
        {
            HttpWebRequest request = AuthManager.CreatePostRequest(LOGIN_URL);

            using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                var postData = string.Format(
                    "action=login&username={0}&password={1}",
                    username.Replace(" ", "+"),
                    WebUtility.UrlEncode(password));

                writer.Write(postData);
            }

            await request.GetResponseAsync();
            if (request.CookieContainer.Count < 2)
            {
                return false;
            }

            SaveCookie(Constants.COOKIE_FILE, request.CookieContainer, new Uri(COOKIE_DOMAIN_URL));
            return true;
        }

        private static async void SaveCookie(string filename, CookieContainer rcookie, Uri uri)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile =
                await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (StorageStreamTransaction transaction = await sampleFile.OpenTransactedWriteAsync())
            {
                CookieSerializer.Serialize(rcookie.GetCookies(uri), uri, transaction.Stream.AsStream());
                await transaction.CommitAsync();
            }
        }

        public static async Task<CookieContainer> LoadCookie(string filename)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync(filename);
                using (Stream stream = await sampleFile.OpenStreamForReadAsync())
                {
                    return CookieSerializer.Deserialize(stream, new Uri(COOKIE_DOMAIN_URL));
                }
            }
            catch
            {
                //Ignore, we will ask for log in information.
            }
            return new CookieContainer();
        }

    }

    public class LoginFailedException : Exception
    {
        public LoginFailedException() 
        {
        }
        public LoginFailedException(string message) : base(message)
        {
        }
    }
}

