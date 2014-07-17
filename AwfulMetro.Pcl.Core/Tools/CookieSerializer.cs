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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace AwfulMetro.Core.Tools
{
    public class CookieSerializer
    {
        public static void Serialize(CookieCollection cookies, Uri address, Stream stream)
        {
            var serializer = new DataContractSerializer(typeof (IEnumerable<Cookie>));
            IEnumerable<Cookie> cookieList = cookies.OfType<Cookie>();

            serializer.WriteObject(stream, cookieList);
        }

        public static CookieContainer Deserialize(Uri uri, Stream stream)
        {
            var container = new CookieContainer();
            var serializer = new DataContractSerializer(typeof (IEnumerable<Cookie>));
            var cookies = (IEnumerable<Cookie>) serializer.ReadObject(stream);
            var cookieCollection = new CookieCollection();

            // TODO: HUGE HACK. For some reason Windows Phone does not use the Domain Key on a cookie, but only the domain when making requests.
            // Windows 8 won't break on it, but Windows Phone will, since the Domain Key and Domain are different on SA.
            // We need to move this code to a more common place.

            foreach (Cookie cookie in cookies)
            {
                var fixedCookie = new Cookie(cookie.Name, cookie.Value, "/", ".somethingawful.com");
                cookieCollection.Add(fixedCookie);
            }
            container.Add(uri, cookieCollection);
            return container;
        }
    }
}