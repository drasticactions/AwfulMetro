using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace BusinessObjects.Tools
{
    public class CookieSerializer
    {
        public static void Serialize(CookieCollection cookies, Uri address, Stream stream)
        {
            var formatter = new DataContractSerializer(typeof(List<Cookie>));
            List<Cookie> cookieList = cookies.OfType<Cookie>().ToList();

            formatter.WriteObject(stream, cookieList);
        }

        public static CookieContainer Deserialize(Stream stream, Uri uri)
        {
            var cookies = new List<Cookie>();
            var container = new CookieContainer();
            var formatter = new DataContractSerializer(typeof(List<Cookie>));
            cookies = (List<Cookie>)formatter.ReadObject(stream);
            var cookieco = new CookieCollection();

            foreach (Cookie cookie in cookies)
            {
                cookieco.Add(cookie);
            }
            container.Add(uri, cookieco);
            return container;
        }
    }
}
