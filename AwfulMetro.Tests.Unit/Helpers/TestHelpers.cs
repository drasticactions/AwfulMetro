using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace AwfulMetro.Tests.Unit.Helpers
{
    public static class TestHelpers
    {
        // There's no ExpectedExceptionAttribute for Windows Store apps! Why must Microsoft make my life so hard?!
        public static void AssertThrowsExpectedException<T>(this Action a) where T : Exception
        {
            try
            {
                a();
            }
            catch (T)
            {
                return;
            }

            Assert.Fail("The expected exception was not thrown");
        }

        public static async Task AssertThrowsExpectedException<TException>(this Func<Task> a) where TException : Exception
        {
            try
            {
                await a();
            }
            catch (TException)
            {
                return;
            }

            Assert.Fail("The expected exception was not thrown");
        }
    }
}
