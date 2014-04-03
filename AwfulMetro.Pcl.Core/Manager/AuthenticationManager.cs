using System;
using System.Net;
using System.Threading.Tasks;
using AwfulMetro.Core.Tools;

namespace AwfulMetro.Core.Manager
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly ILocalStorageManager _localStorageManager;
        private readonly IWebManager _webManager;

        public AuthenticationManager(IWebManager webManager, ILocalStorageManager localStorageManager)
        {
            _webManager = webManager;
            _localStorageManager = localStorageManager;
        }

        public AuthenticationManager() : this(new WebManager(), new LocalStorageManager())
        {
        }

        public string Status { get; set; }

        public async Task<bool> Logout()
        {
            return await _localStorageManager.RemoveCookies(Constants.COOKIE_FILE);
        }

        public async Task<bool> Authenticate(string username, string password,
            int timeout = Constants.DEFAULT_TIMEOUT_IN_MILLISECONDS)
        {
            if (!_webManager.IsNetworkAvailable)
            {
                throw new LoginFailedException(
                    "The network is unavailable. Check your network settings and please try again.");
            }

            try
            {
                return await SendLoginData(username, password);
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> SendLoginData(string username, string password)
        {
            CookieContainer cookies = await _webManager.PostData(
                Constants.LOGIN_URL, string.Format(
                    "action=login&username={0}&password={1}",
                    username.Replace(" ", "+"),
                    WebUtility.UrlEncode(password)));

            if (cookies.Count < 2)
            {
                return false;
            }

            await _localStorageManager.SaveCookie(Constants.COOKIE_FILE, cookies, new Uri(Constants.COOKIE_DOMAIN_URL));
            return true;
        }
    }
}