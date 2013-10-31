using System.Net;
using System.Threading.Tasks;
using BusinessObjects.Manager;

namespace AwfulMetro.Tests.Unit.Mocks
{
    public class FakeWebManager : IWebManager
    {
        public CookieContainer CookiesToReturn { get; set; }
        public WebManager.Result ResultToReturn { get; set; }
        public bool IsNetworkAvailable { get; set; }
        public Task<WebManager.Result> DownloadHtml(string uri)
        {
            return Task.FromResult(this.ResultToReturn);
        }

        public Task<CookieContainer> PostData(string uri, string data)
        {
            return Task.FromResult(this.CookiesToReturn);
        }

        public FakeWebManager()
        {
            this.IsNetworkAvailable = true;
        }
    }
}