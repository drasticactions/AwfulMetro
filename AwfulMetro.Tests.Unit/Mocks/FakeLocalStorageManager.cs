using System;
using System.Net;
using System.Threading.Tasks;
using AwfulMetro.Core.Manager;

namespace AwfulMetro.Tests.Unit.Mocks
{
    public class FakeLocalStorageManager : ILocalStorageManager
    {
        public CookieContainer CookiesToReturn { get; set; }
        public CookieContainer SavedCookies { get; private set; }
        public Uri SavedUri { get; private set; }

        public Task SaveCookie(string filename, CookieContainer rcookie, Uri uri)
        {
            SavedCookies = rcookie;
            SavedUri = uri;
            return Task.FromResult(string.Empty);
        }

        public Task<CookieContainer> LoadCookie(string filename)
        {
            return Task.FromResult(CookiesToReturn);
        }
    }
}