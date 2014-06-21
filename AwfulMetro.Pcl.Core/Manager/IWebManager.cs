using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AwfulMetro.Core.Manager
{
    public interface IWebManager
    {
        bool IsNetworkAvailable { get; }
        Task<WebManager.Result> GetData(string uri);
        Task<CookieContainer> PostData(string uri, string data);
        Task<HttpResponseMessage> PostFormData(string uri, MultipartFormDataContent form);
    }
}