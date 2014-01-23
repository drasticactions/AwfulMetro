using System.Linq;
using AwfulMetro.Core.Entity;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace AwfulMetro.Tests.Unit
{
    // ReSharper disable once InconsistentNaming
    [TestClass]
    public class When_Parsing_A_Forum_Post
    {
        private const string PostHtmlFromRegularUser =
            "<table class=\"post \" id=\"post421467978\" data-idx=\"161\">\n<tr class=\"altcolor2\" id=\"pti1\">\n\t<td class=\"userinfo userid-72369\">\n\t\t<dl class=\"userinfo\">\n\t\t\t<dt class=\"author\" title=\"\">spacetimecontinuu</dt>\n\t\t\t<dd class=\"registered\">Dec 30, 2004</dd>\n\t\t\t<dd class=\"title\">\n\t\t\t\t<br>\n\t\t\t\t<br class=\"pb\">\n\t\t\t</dd>\n\t\t</dl>\n\t</td>\n\t<td class=\"postbody\">\n\t\t<!-- BeginContentMarker -->\n\t\t<!-- google_ad_section_start -->\n\t\tThe fact that this thread stands alone as a bulwark of non-terrible posting in the veritable wasteland that is new GBS is making me want to do this but I am super poor and super uncreative.  I am debating what to do.\n\t\t<!-- google_ad_section_end -->\n\t\t<!-- EndContentMarker -->\n\n\n\t\t<p class=\"editedby\">\n\t</td>\n</tr>\n<tr class=\"altcolor2\">\n\t<td class=\"postdate\">\n\t\t<a class=\"lastseen_icon\" href=\"/showthread.php?action=setseen&amp;threadid=3581525&amp;index=161\" title=\"Mark all replies as read to this point\"><img src=\"http://fi.somethingawful.com/style/posticon-new.gif\" alt=\"\" border=\"0\"></a>\n\t\t<a href=\"#post421467978\" title=\"Link to this post\">#</a>\n\t\t<a class=\"user_jump\" title=\"Show posts by this user\" href=\"/showthread.php?threadid=3581525&amp;userid=72369\">?</a>\n\t\tNov  6, 2013 05:15\n\t</td>\n\t<td class=\"postlinks\">\n\t\t<ul class=\"profilelinks\">\n\t\t\t<li><a href=\"member.php?action=getinfo&amp;userid=72369\">Profile</a></li>\n\t\t\t<li><a href=\"private.php?action=newmessage&amp;userid=72369\">Message</a></li>\n\t\t\t<li><a href=\"search.php?action=do_search_posthistory&amp;userid=72369\">Post History</a></li>\n\t\t</ul>\n\t\t<ul class=\"postbuttons\">\n\n\t\t\t\n\t\t\t<li class=\"alertbutton\"><a href=\"modalert.php?postid=421467978&amp;username=spacetimecontinuu\"><img src=\"http://forumimages.somethingawful.com/images/button-report.gif\" border=\"0\" alt=\"Alert Moderators\"></a>&nbsp;&nbsp;</li>\n\t\t\t\n\t\t\t<li><a href=\"newreply.php?action=newreply&amp;postid=421467978\"><img src=\"http://fi.somethingawful.com/images/sa-quote.gif\" alt=\"Quote\" title=\"\"></a></li>\n\n\n\t\t</ul>\n\t</td>\n</tr>\n</table>";

        // This just needs a cursory test, since we're already testing parsing of users from post data
        [TestMethod]
        public void Poster_Is_Successful_Parsed()
        {
            ForumPostEntity post = ParseEntity(PostHtmlFromRegularUser);
            Assert.IsNotNull(post.User);
        }

        [TestMethod]
        public void PostDate_Is_Successfully_Parsed()
        {
            ForumPostEntity post = ParseEntity(PostHtmlFromRegularUser);
            Assert.AreEqual("Nov  6, 2013 05:15", post.PostDate);
        }

        [TestMethod]
        public void PostReportLink_Is_Successfully_Parsed()
        {
            Assert.Inconclusive("This property is not being parsed.");
            ForumPostEntity post = ParseEntity(PostHtmlFromRegularUser);
            Assert.AreEqual(string.Empty, post.PostReportLink);
        }

        [TestMethod]
        public void PostLink_Is_Successfully_Parsed()
        {
            Assert.Inconclusive("This property is not being parsed.");
            ForumPostEntity post = ParseEntity(PostHtmlFromRegularUser);
            Assert.AreEqual("", post.PostLink);
        }

        [TestMethod]
        public void PostFormatted_Is_Successfully_Parsed()
        {
            ForumPostEntity post = ParseEntity(PostHtmlFromRegularUser);
            Assert.AreEqual(
                "<!-- BeginContentMarker --> <!-- google_ad_section_start --> The fact that this thread stands alone as a bulwark of non-terrible posting in the veritable wasteland that is new GBS is making me want to do this but I am super poor and super uncreative.  I am debating what to do. <!-- google_ad_section_end --> <!-- EndContentMarker -->   <p class=\"editedby\">",
                post.PostFormatted.Trim());
        }

        [TestMethod]
        public void PostHtml_Is_Successfully_Parsed()
        {
            ForumPostEntity post = ParseEntity(PostHtmlFromRegularUser);
            string expected =
                @"<!DOCTYPE html><html><head><link rel=""stylesheet"" type=""text/css"" href=""ms-appx-web:///Assets/bbcode.css""><link rel=""stylesheet"" type=""text/css"" href=""ms-appx-web:///Assets/forums.css""><link rel=""stylesheet"" type=""text/css"" href=""ms-appx-web:///Assets/main.css""><link rel=""stylesheet"" type=""text/css"" href=""ms-appx-web:///Assets/ui-light.css""><meta name=""MSSmartTagsPreventParsing"" content=""TRUE""><meta http-equiv=""X-UA-Compatible"" content=""chrome=1""><script type=""text/javascript"" src=""ms-appx-web:///Assets/jquery.min.js""></script><link rel=""stylesheet"" type=""text/css"" href=""ms-appx-web:///Assets/jquery-ui.css""><script type=""text/javascript"" src=""ms-appx-web:///Assets/jquery-ui.min.js""></script><script type=""text/javascript"">disable_thread_coloring = true;</script><script type=""text/javascript"" src=""ms-appx-web:///Assets/forums.combined.js""></script><style type=""text/css""></style></head><body>
		<!-- BeginContentMarker -->
		<!-- google_ad_section_start -->
		The fact that this thread stands alone as a bulwark of non-terrible posting in the veritable wasteland that is new GBS is making me want to do this but I am super poor and super uncreative.  I am debating what to do.
		<!-- google_ad_section_end -->
		<!-- EndContentMarker -->


		<p class=""editedby"">
	</body></html>";
            Assert.AreEqual(expected, post.PostHtml);
        }

        [TestMethod]
        public void PostId_Is_Successfully_Parsed()
        {
            ForumPostEntity post = ParseEntity(PostHtmlFromRegularUser);
            Assert.AreEqual(421467978, post.PostId);
        }

        [TestMethod]
        public void IsQuoting_Is_True_When_Quoting()
        {
            Assert.Inconclusive("This property is not being parsed.");
            ForumPostEntity post = ParseEntity(PostHtmlFromRegularUser);
            Assert.IsTrue(post.IsQuoting);
        }

        [TestMethod]
        public void IsQuoting_Is_False_When_Not_Quoting()
        {
            Assert.Inconclusive("This property is not being parsed.");
            ForumPostEntity post = ParseEntity(PostHtmlFromRegularUser);
            Assert.IsFalse(post.IsQuoting);
        }

        private static ForumPostEntity ParseEntity(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var entity = new ForumPostEntity();

            entity.Parse(doc.DocumentNode.Descendants("table").FirstOrDefault());
            return entity;
        }
    }
}