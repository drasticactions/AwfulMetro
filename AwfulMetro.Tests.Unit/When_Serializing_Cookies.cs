using System;
using System.IO;
using System.Net;
using AwfulMetro.Core.Tools;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace AwfulMetro.Tests.Unit
{
    // ReSharper disable InconsistentNaming
    [TestClass]
    public class When_Serializing_Cookies
    {
        [TestMethod]
        public void A_Cookie_Collection_Can_Be_Serialized()
        {
            //Arrange
            var cookies = new CookieCollection {new Cookie("foo", "bar")};
            var cookieStream = new MemoryStream();

            //Act
            CookieSerializer.Serialize(cookies, new Uri("http://foo.com"), cookieStream);

            //Assert
            Assert.IsTrue(cookieStream.Length > 0);
        }

        [TestMethod]
        public void A_Cookie_Collection_Can_Be_Deserialized()
        {
            //Arrange
            var uri = new Uri("http://foo.com");
            var cookies = new CookieCollection {new Cookie("foo", "bar")};
            var cookieStream = new MemoryStream();

            //Act
            CookieSerializer.Serialize(cookies, uri, cookieStream);
            cookieStream.Seek(0, SeekOrigin.Begin);
            CookieContainer deserialized = CookieSerializer.Deserialize(uri, cookieStream);

            //Assert
            Assert.AreEqual(1, deserialized.Count);
            Assert.IsNotNull(deserialized.GetCookies(uri)["foo"]);
        }
    }
}