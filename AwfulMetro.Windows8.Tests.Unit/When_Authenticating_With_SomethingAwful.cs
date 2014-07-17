﻿using System;
using System.Net;
using System.Threading.Tasks;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.Tests.Unit.Mocks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace AwfulMetro.Tests.Unit
{
    [TestClass]
    // ReSharper disable InconsistentNaming
    public class When_Authenticating_With_SomethingAwful
    {
        [TestMethod]
        public async Task Providing_Valid_Credentials_Returns_True()
        {
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri("http://foo.com"), new Cookie("Foo", "bar"));
            cookieContainer.Add(new Uri("http://foo.com"), new Cookie("Bar", "Foo"));
            var webManager = new FakeWebManager
            {
                CookiesToReturn = cookieContainer,
            };

            var localStorage = new FakeLocalStorageManager();
            var am = new AuthenticationManager(webManager, localStorage);
            const bool expected = true;
            bool actual = await am.Authenticate("foo", "bar");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Providing_Invalid_Credentials_Returns_False()
        {
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri("http://foo.com"), new Cookie("Bar", "Foo"));
            var webManager = new FakeWebManager
            {
                CookiesToReturn = cookieContainer
            };

            var localStorage = new FakeLocalStorageManager();
            var am = new AuthenticationManager(webManager, localStorage);
            const bool expected = false;
            bool actual = await am.Authenticate("foo", "bar");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task Network_Interface_Being_Unavailable_Throws_Exception()
        {
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri("http://foo.com"), new Cookie("Bar", "Foo"));
            var webManager = new FakeWebManager
            {
                CookiesToReturn = cookieContainer,
                IsNetworkAvailable = false
            };

            var localStorage = new FakeLocalStorageManager();
            var am = new AuthenticationManager(webManager, localStorage);
            try
            {
                await am.Authenticate("foo", "bar");
            }
            catch (Exception)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public async Task Cookies_Are_Stored_Successfully()
        {
            const string expectedUri = "http://fake.forums.somethingawful.com/";

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri(expectedUri), new Cookie("Foo", "bar"));
            cookieContainer.Add(new Uri(expectedUri), new Cookie("Bar", "Foo"));
            var webManager = new FakeWebManager
            {
                CookiesToReturn = cookieContainer,
            };

            var localStorage = new FakeLocalStorageManager();
            var am = new AuthenticationManager(webManager, localStorage);

            await am.Authenticate("foo", "bar");

            Assert.AreEqual(expectedUri, localStorage.SavedUri.AbsoluteUri);
            Assert.AreEqual(2, localStorage.SavedCookies.Count);
            CookieCollection storedCookies = localStorage.SavedCookies.GetCookies(new Uri(expectedUri));
            Assert.IsNotNull(storedCookies["Foo"]);
            Assert.IsNotNull(storedCookies["Bar"]);
        }

        [TestMethod]
        public async Task Cookies_Are_Retrieved_Successfully()
        {
            const string expectedUri = "http://fake.forums.somethingawful.com/";

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(new Uri(expectedUri), new Cookie("Foo", "bar"));
            cookieContainer.Add(new Uri(expectedUri), new Cookie("Bar", "Foo"));
            var webManager = new FakeWebManager
            {
                CookiesToReturn = cookieContainer,
            };

            var localStorage = new FakeLocalStorageManager();
            var am = new AuthenticationManager(webManager, localStorage);

            await am.Authenticate("foo", "bar");
            localStorage.CookiesToReturn = localStorage.SavedCookies;

            CookieContainer cookies = await localStorage.LoadCookie(Constants.COOKIE_FILE);

            Assert.AreEqual(2, cookies.Count);
        }
    }
}