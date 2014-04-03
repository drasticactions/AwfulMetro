using System.Threading.Tasks;
using AwfulMetro.Core.Tools;

namespace AwfulMetro.Core.Manager
{
    public interface IAuthenticationManager
    {
        string Status { get; }

        Task<bool> Authenticate(string userName, string password,
            int timeout = Constants.DEFAULT_TIMEOUT_IN_MILLISECONDS);
    }
}