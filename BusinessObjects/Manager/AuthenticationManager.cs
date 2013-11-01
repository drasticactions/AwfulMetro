using BusinessObjects.Tools;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BusinessObjects.Manager
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly IWebManager webManager;
        private readonly ILocalStorageManager localStorageManager;

        public AuthenticationManager(IWebManager webManager, ILocalStorageManager localStorageManager)
        {
            this.webManager = webManager;
            this.localStorageManager = localStorageManager;
        }

        public AuthenticationManager() : this(new WebManager(), new LocalStorageManager()) { }

        public string Status { get; set; }
   
        public async Task<bool> Authenticate(string username, string password, int timeout = Constants.DEFAULT_TIMEOUT_IN_MILLISECONDS)
        {
            if (!webManager.IsNetworkAvailable)
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
            var cookies = await webManager.PostData(
                Constants.LOGIN_URL, string.Format(
                    "action=login&username={0}&password={1}",
                    username.Replace(" ", "+"),
                    WebUtility.UrlEncode(password)));

            if (cookies.Count < 2)
            {
                return false;
            }

            await localStorageManager.SaveCookie(Constants.COOKIE_FILE, cookies, new Uri(Constants.COOKIE_DOMAIN_URL));
            return true;
        }
    }
}

