using AwfulMetro.Core.Tools;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Manager
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly IWebManager _webManager;
        private readonly ILocalStorageManager _localStorageManager;

        public AuthenticationManager(IWebManager webManager, ILocalStorageManager localStorageManager)
        {
            this._webManager = webManager;
            this._localStorageManager = localStorageManager;
        }

        public AuthenticationManager() : this(new WebManager(), new LocalStorageManager()) { }

        public string Status { get; set; }
   
        public async Task<bool> Authenticate(string username, string password, int timeout = Constants.DEFAULT_TIMEOUT_IN_MILLISECONDS)
        {
            if (!this._webManager.IsNetworkAvailable)
            {
                throw new LoginFailedException("The network is unavailable. Check your network settings and please try again.");
            }

            try
            {
                return await this.SendLoginData(username, password);
            }
            catch
            {
                return false;    
            }
        }

        private async Task<bool> SendLoginData(string username, string password)
        {
            var cookies = await this._webManager.PostData(
                Constants.LOGIN_URL, string.Format(
                    "action=login&username={0}&password={1}",
                    username.Replace(" ", "+"),
                    WebUtility.UrlEncode(password)));

            if (cookies.Count < 2)
            {
                return false;
            }

            await this._localStorageManager.SaveCookie(Constants.COOKIE_FILE, cookies, new Uri(Constants.COOKIE_DOMAIN_URL));
            return true;
        }
    }
}

