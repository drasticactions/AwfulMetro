using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using AwfulMetro.Core.Tools;

namespace AwfulMetro.Core.Manager
{
    public interface ILocalStorageManager
    {
        Task SaveCookie(string filename, CookieContainer rcookie, Uri uri);
        Task<CookieContainer> LoadCookie(string filename);
    }

    public class LocalStorageManager : ILocalStorageManager
    {
        public async Task SaveCookie(string filename, CookieContainer rcookie, Uri uri)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (StorageStreamTransaction transaction = await sampleFile.OpenTransactedWriteAsync())
            {
                CookieSerializer.Serialize(rcookie.GetCookies(uri), uri, transaction.Stream.AsStream());
                await transaction.CommitAsync();
            }
        }

        public async Task<CookieContainer> LoadCookie(string filename)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync(filename);
                using (Stream stream = await sampleFile.OpenStreamForReadAsync())
                {
                    return CookieSerializer.Deserialize(new Uri(Constants.COOKIE_DOMAIN_URL), stream);
                }
            }
            catch
            {
                //Ignore, we will ask for log in information.
            }
            return new CookieContainer();
        }
    }
}
