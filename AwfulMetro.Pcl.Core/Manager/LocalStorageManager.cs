#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion
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
        Task<bool> RemoveCookies(string filename);
    }

    public class LocalStorageManager : ILocalStorageManager
    {
        public async Task SaveCookie(string filename, CookieContainer rcookie, Uri uri)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile =
                await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

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

        public async Task<bool> RemoveCookies(string filename)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync(filename);
                sampleFile.DeleteAsync();
                return true;
            }
            catch
            {
                //Ignore, we will ask for log in information.
                return false;
            }
        }
    }
}