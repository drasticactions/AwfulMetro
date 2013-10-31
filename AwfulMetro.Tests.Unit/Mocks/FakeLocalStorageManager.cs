using System;
using System.Net;
using System.Threading.Tasks;
using BusinessObjects.Manager;

namespace AwfulMetro.Tests.Unit.Mocks
{
    public class FakeLocalStorageManager : ILocalStorageManager
    {
        public CookieContainer CookiesToReturn { get; set; }
        public CookieContainer SavedCookies { get; private set; }
        public Uri SavedUri { get; private set; }
        public Task SaveCookie(string filename, CookieContainer rcookie, Uri uri)
        {
            this.SavedCookies = rcookie;
            this.SavedUri = uri;
            return Task.FromResult(string.Empty);
        }

        public Task<CookieContainer> LoadCookie(string filename)
        {
            return Task.FromResult(this.CookiesToReturn);
        }
    }
}