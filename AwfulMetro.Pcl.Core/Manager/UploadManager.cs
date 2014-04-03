using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using AwfulMetro.Core.Entity;
using Newtonsoft.Json;

namespace AwfulMetro.Core.Manager
{
    public class UploadManager
    {
        public static async Task<ImgurEntity> UploadImgur(IRandomAccessStream fileStream)
        {
            try
            {
                var imageData = new byte[fileStream.Size];
                for (int i = 0; i < imageData.Length; i++)
                {
                    imageData[i] = (byte) fileStream.AsStreamForRead().ReadByte();
                }
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.imgur.com/3/image");
                request.Headers.Authorization = new AuthenticationHeaderValue("Client-ID", "e5c018ac1f4c157");
                var form = new MultipartFormDataContent();
                var t = new StreamContent(fileStream.AsStream());
                // TODO: See if this is the correct way to use imgur's v3 api. I can't see why we would still need to convert images to base64.
                string base64Img = Convert.ToBase64String(imageData);
                t.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                form.Add(new StringContent(base64Img), @"image");
                form.Add(new StringContent("file"), "type");
                request.Content = form;
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                string responseString = await response.Content.ReadAsStringAsync();
                if (responseString == null) return null;
                var imgurEntity = JsonConvert.DeserializeObject<ImgurEntity>(responseString);
                return imgurEntity;
            }
            catch (WebException)
            {
            }
            catch (IOException)
            {
                return null;
            }
            return null;
        }
    }
}