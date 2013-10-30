using BusinessObjects.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace BusinessObjects.Manager
{
    /// <summary>
    /// Originally hacked away from Awful Forum Viewer (https://github.com/bootlegrobot/awful2).
    /// 
    /// </summary>
    public class AuthManager
    {
        private static UserMetadata _user;

        /// <summary>
        /// Gets the cookie container used by all web requests. The cookies belong to the
        /// active user.
        /// </summary>
        public static IEnumerable<Cookie> CookieJar { get { return _cookieJar; } }
        /// <summary>
        /// Gets the instance of the active user. All server requests are made as this user.
        /// </summary>
        public static UserMetadata ActiveUser { get { return _user; } }

        /// <summary>
        /// Sets the global cookie container for all future web requests. If this container is not set,
        /// certain requests may fail. This method is typically called after authentication cookies are
        /// obtained (see AwfulLoginRequest).
        /// </summary>
        /// <param name="jar">The container of cookies used to authenticate requests.</param>
        public static void SetCookieJar(IEnumerable<Cookie> jar)
        {
            _cookieJar.Clear();
            if (jar != null)
                _cookieJar.AddRange(jar);
        }
        private const string ACCEPT = "text/html, application/xhtml+xml, */*";
        private const string POST_CONTENT_TYPE = "application/x-www-form-urlencoded";
        public const int DefaultTimeoutInMilliseconds = 60000;
        private static List<Cookie> _cookieJar = new List<Cookie>();
        private const string COOKIE_DOMAIN_URL = "http://fake.forums.somethingawful.com";
        private const string LOGIN_URL = "http://forums.somethingawful.com/account.php?";
        private string _status;
        public string Status
        {
            get { return this._status; }
            set { this._status = value; }
        }

        public static event EventHandler<LoginEventArgs> LoginSuccessful;

        private static void OnLoginSuccessful(AuthManager sender, string username, List<Cookie> cookie)
        {
            if (LoginSuccessful != null)
            {
                var user = new UserMetadata() { Username = username, Cookies = cookie };
                LoginSuccessful(sender, new LoginEventArgs(user));
            }
        }

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

        public static async Task<HttpClientHandler> CreateGetRequestHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            handler.UseCookies = true;
            handler.CookieContainer = await AuthManager.Loadcookie(Constants.COOKIE_FILE);
            return handler;
        }

        public static async Task<HttpWebRequest> CreateGetRequest(string url)
        {
            var uri = new Uri(url);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = ACCEPT;
            request.CookieContainer = await AuthManager.Loadcookie(Constants.COOKIE_FILE);
            request.Method = "GET";
            request.UseDefaultCredentials = false;
            return request;
        }

        private static CookieContainer GetCookiesForUri(Uri uri)
        {
            var container = new CookieContainer();
            var cookies = CookieJar;
            foreach (Cookie cookie in cookies) { container.Add(uri, cookie); }
            return container;
        }


        public List<Cookie> Authenticate(string username, string password, int timeout = DefaultTimeoutInMilliseconds)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new LoginFailedException("The network is unavailable. Check your network settings and please try again.");

            HttpWebRequest request =  AuthManager.CreatePostRequest(LOGIN_URL);
            var signal = new AutoResetEvent(false);
            var result = request.BeginGetRequestStream(callback => { signal.Set(); },
                request);
            //signal.WaitOne();

            bool uploaded = UploadLoginData(result, username, password);
            if (!uploaded)
                throw new LoginFailedException();

            List<Cookie> success = ProcessLoginResults(result.AsyncState as HttpWebRequest,
                timeout);
            List<Cookie> cookieList = new List<Cookie>(success.Count);
            var container = new CookieContainer();
            foreach (Cookie cookie in success)
            {
                var awfulCookie = new Cookie(
                    cookie.Name,
                    cookie.Value,
                    "/",
                    ".somethingawful.com");
                cookieList.Add(awfulCookie);
            }

            if (success != null)
                OnLoginSuccessful(this, username, success);
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            
            return success;
        }

        private List<Cookie> ProcessLoginResults(HttpWebRequest httpWebRequest, int timeout)
        {
            AutoResetEvent signal = new AutoResetEvent(false);
            var result = httpWebRequest.BeginGetResponse(callback => { signal.Set(); }, httpWebRequest);
            return ParseLoginResponse(result);
        }

        private bool UploadLoginData(IAsyncResult result, 
            string username, string password) 
        {
            bool success = false;

            try
            {
                HttpWebRequest request = result.AsyncState as HttpWebRequest;
                using (StreamWriter writer = new StreamWriter(request.EndGetRequestStream(result)))
                {
                    var postData = String.Format("action=login&username={0}&password={1}",
                        username.Replace(" ", "+"),
                        WebUtility.UrlEncode(password));

                    writer.Write(postData);
                    success = true;
                }
            }

            catch (Exception ex) { success = false; }
            return success;
        }

        private async void Savecookie(string filename, CookieContainer rcookie, Uri uri)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (StorageStreamTransaction transaction = await sampleFile.OpenTransactedWriteAsync())
            {
                CookieSerializer.Serialize(rcookie.GetCookies(uri), uri, transaction.Stream.AsStream());
                await transaction.CommitAsync();
            }
        }

        public static async Task<CookieContainer> Loadcookie(string filename)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile;
            try
            {
                sampleFile = await localFolder.GetFileAsync(filename);
                using (Stream stream = await sampleFile.OpenStreamForReadAsync())
                {
                    return CookieSerializer.Deserialize(stream, new Uri("http://fake.forums.somethingawful.com"));
                }
            }
            catch
            {
                //Ignore, we will ask for log in information.
            }
            return new CookieContainer();
        }

        private List<Cookie> ParseLoginResponse(IAsyncResult callback)
        {
            bool success = false;
            var request = callback.AsyncState as HttpWebRequest;
            CookieCollection collection = new CookieCollection();
            HttpWebResponse response;
            try
            {
                response = request.EndGetResponse(callback) as HttpWebResponse;

                success = request.CookieContainer.Count >= 2;

                if (!success)
                    throw new LoginFailedException();

                string html = null;

                using (var reader = new StreamReader(response.GetResponseStream()))
                    html = reader.ReadToEnd();


                collection = request.CookieContainer.GetCookies(
                    new Uri(COOKIE_DOMAIN_URL));
                Savecookie(Constants.COOKIE_FILE, request.CookieContainer, new Uri(COOKIE_DOMAIN_URL));
                
            }
            catch
            {
                //Ignore, if it fails than the user did not enter their username/password correctly. 
            }

            return ManageCookies(collection);    
        }

        private List<Cookie> ManageCookies(CookieCollection collection)
        {
            var cookies = collection;
            List<Cookie> cookieList = new List<Cookie>(cookies.Count);
            var container = new CookieContainer();
            foreach (Cookie cookie in cookies)
            {
                var awfulCookie = new Cookie(
                    cookie.Name,
                    cookie.Value,
                    "/",
                    ".somethingawful.com");
                cookieList.Add(awfulCookie);
            }

            return cookieList;
        }
    }

    public class LoginFailedException : Exception {

        public LoginFailedException() : base() { }

        public LoginFailedException(string p) : base(p) { }
    }

    public class LoginEventArgs : EventArgs
    {
        public UserMetadata User { get; private set; }

        internal LoginEventArgs(UserMetadata metadata) { this.User = metadata; }
    }

}

