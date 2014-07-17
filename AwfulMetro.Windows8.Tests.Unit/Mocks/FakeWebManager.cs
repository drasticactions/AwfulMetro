using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AwfulMetro.Core.Manager;

namespace AwfulMetro.Tests.Unit.Mocks
{
    public class FakeWebManager : IWebManager
    {
        public FakeWebManager()
        {
            IsNetworkAvailable = true;
        }

        public CookieContainer CookiesToReturn { get; set; }

        public HttpResponseMessage PostResponseMessage { get; set; }
        public WebManager.Result ResultToReturn { get; set; }
        public bool IsNetworkAvailable { get; set; }

        public Task<WebManager.Result> GetData(string uri)
        {
            return Task.FromResult(ResultToReturn);
        }

        public Task<CookieContainer> PostData(string uri, string data)
        {
            return Task.FromResult(CookiesToReturn);
        }

        public Task<HttpResponseMessage> PostFormData(string uri, MultipartFormDataContent form)
        {
            return Task.FromResult(PostResponseMessage);
        }
    }
}