using System;
using System.IO;
using System.Net;
using System.Text;
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

                const int maxUriLength = 32766;
                //TODO: See if this is the correct way to use imgur's v3 api. I can't see why
                // we would still need to convert images to base64 and put them on the query string.
                string base64Img = Convert.ToBase64String(imageData);
                var sb = new StringBuilder();

                for (int i = 0; i < base64Img.Length; i += maxUriLength)
                {
                    sb.Append(Uri.EscapeDataString(base64Img.Substring(i, Math.Min(maxUriLength, base64Img.Length - i))));
                }

                string uploadRequestString = "title=" + "Awful_Image" +
                                             "&caption=" + "Awful_Image" + "&image=" + sb;

                var webRequest = (HttpWebRequest) WebRequest.Create("https://api.imgur.com/3/image");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContinueTimeout = 10000;
                
                // We could hide the API key, but considering what this app is, I don't think it matters.s
                webRequest.Headers["Authorization"] = "Client-ID " + "e5c018ac1f4c157";
                Stream stream = await webRequest.GetRequestStreamAsync();
                var streamWriter = new StreamWriter(stream);
                streamWriter.Write(uploadRequestString);

                WebResponse response = await webRequest.GetResponseAsync();
                Stream responseStream = response.GetResponseStream();
                var responseReader = new StreamReader(responseStream);

                string responseString = await responseReader.ReadToEndAsync();
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