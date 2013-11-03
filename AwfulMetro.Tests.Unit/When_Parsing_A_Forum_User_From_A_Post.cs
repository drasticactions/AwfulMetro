using System;
using AwfulMetro.Core.Entity;
using AwfulMetro.Tests.Unit.Helpers;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace AwfulMetro.Tests.Unit
{
    // ReSharper disable InconsistentNaming
    [TestClass]
    public class When_Parsing_A_Forum_User_From_A_Post
    {
        private const string DefaultPostHtml = "\n<tr class=\"altcolor2\" id=\"pti1\">\n\t<td class=\"userinfo userid-41741\">\n\t\t<dl class=\"userinfo\">\n\t\t\t<dt class=\"author\" title=\"\">The Dave</dt>\n\t\t\t<dd class=\"registered\">Sep  9, 2003</dd>\n\t\t\t<dd class=\"title\">\n\t\t\t\t<div class=\"bbc-center\"><img src=\"http://fi.somethingawful.com/customtitles/title-the_dave-2.gif\" alt=\"\" class=\"img\" border=\"0\"><br>\n</div><br>\n\t\t\t\t<br class=\"pb\">\n\t\t\t</dd>\n\t\t</dl>\n\t</td>\n\t<td class=\"postbody\">\n\t\t<!-- BeginContentMarker -->\n\t\t<!-- google_ad_section_start -->\n\t\tOh wow, great work! That really elevates the app for me. Now can you change the background color behind the text? It might be tedius but I might associate a different color with each thread tag.\n\t\t<!-- google_ad_section_end -->\n\t\t<!-- EndContentMarker -->\n\n\n\t\t<p class=\"editedby\">\n\t</td>\n</tr>\n<tr class=\"altcolor2\">\n\t<td class=\"postdate\">\n\t\t<a class=\"lastseen_icon\" href=\"/showthread.php?action=setseen&amp;threadid=3573078&amp;index=41\" title=\"Mark all replies as read to this point\"><img src=\"http://fi.somethingawful.com/style/posticon-seen.gif\" alt=\"\" border=\"0\"></a>\n\t\t<a href=\"#post421111821\" title=\"Link to this post\">#</a>\n\t\t<a class=\"user_jump\" title=\"Show posts by this user\" href=\"/showthread.php?threadid=3573078&amp;userid=41741\">?</a>\n\t\tOct 30, 2013 15:29\n\t</td>\n\t<td class=\"postlinks\">\n\t\t<ul class=\"profilelinks\">\n\t\t\t<li><a href=\"member.php?action=getinfo&amp;userid=41741\">Profile</a></li>\n\t\t\t<li><a href=\"private.php?action=newmessage&amp;userid=41741\">Message</a></li>\n\t\t\t<li><a href=\"search.php?action=do_search_posthistory&amp;userid=41741\">Post History</a></li>\n\t\t</ul>\n\t\t<ul class=\"postbuttons\">\n\n\t\t\t\n\t\t\t<li class=\"alertbutton\"><a href=\"modalert.php?postid=421111821&amp;username=The+Dave\"><img src=\"http://forumimages.somethingawful.com/images/button-report.gif\" border=\"0\" alt=\"Alert Moderators\"></a>&nbsp;&nbsp;</li>\n\t\t\t\n\t\t\t<li><a href=\"newreply.php?action=newreply&amp;postid=421111821\"><img src=\"http://fi.somethingawful.com/images/sa-quote.gif\" alt=\"Quote\" title=\"\"></a></li>\n\n\n\t\t</ul>\n\t</td>\n</tr>\n";
        private const string PostWithNoAvatar = "\n<tr class=\"altcolor1\" id=\"pti22\">\n\t<td class=\"userinfo userid-181013\">\n\t\t<dl class=\"userinfo\">\n\t\t\t<dt class=\"author\" title=\"\">Sir Cornelius</dt>\n\t\t\t<dd class=\"registered\">Oct 29, 2011</dd>\n\t\t\t<dd class=\"title\">\n\t\t\t\t<br>\n\t\t\t\t<br class=\"pb\">\n\t\t\t</dd>\n\t\t</dl>\n\t</td>\n\t<td class=\"postbody\">\n\t\t<!-- BeginContentMarker -->\n\t\t<!-- google_ad_section_start -->\n\t\t<div class=\"bbc-block\"><h4><a class=\"quote_link\" href=\"/showthread.php?goto=post&postid=421292371#post421292371\" rel=\"nofollow\">Blue Raider posted:</a></h4><blockquote>\r\nI bet axemaniac still has them.<br>\r\n</blockquote></div>\n\r\n<span class=\"bbc-spoiler\"><br>\r\nAxemaniac is a totally alright dude and all, but I don't think we can possible bother him with minor tasks like this. If Axemaniac has the picture in question stored and has decided not to share, he probably has his reasons. I don't know how that picture fits in with the whole scheme about world-dominance and all, but we're most likely better off not asking him.<br>\r\n</span>\n\t\t<!-- google_ad_section_end -->\n\t\t<!-- EndContentMarker -->\n\n\n\t\t<p class=\"editedby\">\n\t</td>\n</tr>\n<tr class=\"altcolor1\">\n\t<td class=\"postdate\">\n\t\t<a class=\"lastseen_icon\" href=\"/showthread.php?action=setseen&amp;threadid=3580098&amp;index=102\" title=\"Mark all replies as read to this point\"><img src=\"http://fi.somethingawful.com/style/posticon-new.gif\" alt=\"\" border=\"0\"></a>\n\t\t<a href=\"#post421293183\" title=\"Link to this post\">#</a>\n\t\t<a class=\"user_jump\" title=\"Show posts by this user\" href=\"/showthread.php?threadid=3580098&amp;userid=181013\">?</a>\n\t\tNov  3, 2013 13:49\n\t</td>\n\t<td class=\"postlinks\">\n\t\t<ul class=\"profilelinks\">\n\t\t\t<li><a href=\"member.php?action=getinfo&amp;userid=181013\">Profile</a></li>\n\t\t\t<li><a href=\"search.php?action=do_search_posthistory&amp;userid=181013\">Post History</a></li>\n\t\t</ul>\n\t\t<ul class=\"postbuttons\">\n\n\t\t\t\n\t\t\t<li class=\"alertbutton\"><a href=\"modalert.php?postid=421293183&amp;username=Sir+Cornelius\"><img src=\"http://forumimages.somethingawful.com/images/button-report.gif\" border=\"0\" alt=\"Alert Moderators\"></a>&nbsp;&nbsp;</li>\n\t\t\t\n\t\t\t<li><a href=\"newreply.php?action=newreply&amp;postid=421293183\"><img src=\"http://fi.somethingawful.com/images/sa-quote.gif\" alt=\"Quote\" title=\"\"></a></li>\n\n\n\t\t</ul>\n\t</td>\n</tr>\n";
        private const string PostWithCustomTitle = "\n<tr class=\"altcolor1\" id=\"pti4\">\n\t<td class=\"userinfo userid-39373\">\n\t\t<dl class=\"userinfo\">\n\t\t\t<dt class=\"author\" title=\"\">Ithaqua</dt>\n\t\t\t<dd class=\"registered\">Jul 17, 2003</dd>\n\t\t\t<dd class=\"title\">\n\t\t\t\t<img alt=\"\" border=\"0\" src=\"http://fi.somethingawful.com/customtitles/title-ithaqua.gif\"><br>Only in Kenya.<br>\n\t\t\t\t<br class=\"pb\">\n\t\t\t</dd>\n\t\t</dl>\n\t</td>\n\t<td class=\"postbody\">\n\t\t<!-- BeginContentMarker -->\n\t\t<!-- google_ad_section_start -->\n\t\tI'm probably breaking everything horribly while refactoring. I'm changing just enough to start putting unit tests in place, then the real fun can start.\n\t\t<!-- google_ad_section_end -->\n\t\t<!-- EndContentMarker -->\n\n\n\t\t<p class=\"editedby\">\n\t</td>\n</tr>\n<tr class=\"altcolor1\">\n\t<td class=\"postdate\">\n\t\t<a class=\"lastseen_icon\" href=\"/showthread.php?action=setseen&amp;threadid=3573078&amp;index=44\" title=\"Mark all replies as read to this point\"><img src=\"http://fi.somethingawful.com/style/posticon-seen.gif\" alt=\"\" border=\"0\"></a>\n\t\t<a href=\"#post421148569\" title=\"Link to this post\">#</a>\n\t\t<a class=\"user_jump\" title=\"Show posts by this user\" href=\"/showthread.php?threadid=3573078&amp;userid=39373\">?</a>\n\t\tOct 31, 2013 14:34\n\t</td>\n\t<td class=\"postlinks\">\n\t\t<ul class=\"profilelinks\">\n\t\t\t<li><a href=\"member.php?action=getinfo&amp;userid=39373\">Profile</a></li>\n\t\t\t<li><a href=\"private.php?action=newmessage&amp;userid=39373\">Message</a></li>\n\t\t\t<li><a href=\"search.php?action=do_search_posthistory&amp;userid=39373\">Post History</a></li>\n\t\t</ul>\n\t\t<ul class=\"postbuttons\">\n\n\t\t\t\n\t\t\t<li class=\"alertbutton\"><a href=\"modalert.php?postid=421148569&amp;username=Ithaqua\"><img src=\"http://forumimages.somethingawful.com/images/button-report.gif\" border=\"0\" alt=\"Alert Moderators\"></a>&nbsp;&nbsp;</li>\n\t\t\t\n\t\t\t<li><a href=\"editpost.php?action=editpost&amp;postid=421148569\"><img src=\"http://fi.somethingawful.com/images/sa-edit.gif\" alt=\"Edit\" title=\"\"></a></li>\n\t\t\t<li><a href=\"newreply.php?action=newreply&amp;postid=421148569\"><img src=\"http://fi.somethingawful.com/images/sa-quote.gif\" alt=\"Quote\" title=\"\"></a></li>\n\n\n\t\t</ul>\n\t</td>\n</tr>\n";

        [TestMethod]
        public void Null_User_Throws_An_Exception()
        {
            ((Action)(() => ForumUserEntity.FromPost(null))).AssertThrowsExpectedException<NullReferenceException>();
        }

        [TestMethod]
        public void Unexpected_Html_Throws_An_Exception()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml("<html><h1>oh god how did this get here i am not good at unit testing</h1></html>");
            ((Action)(() => ForumUserEntity.FromPost(doc.DocumentNode))).AssertThrowsExpectedException<NullReferenceException>();
        }

        [TestMethod]
        public void Username_Is_Successfully_parsed()
        {
            var user = GetParsedEntity(DefaultPostHtml);
            Assert.AreEqual("The Dave", user.Username); 
        }

        [TestMethod]
        public void DateJoined_Is_Successfully_parsed()
        {
            var user = GetParsedEntity(DefaultPostHtml);
            Assert.AreEqual("Sep  9, 2003", user.DateJoined);
        }

        [TestMethod]
        public void AvatarTitle_Is_Empty_String_When_No_Title_Exists()
        {
            var user = GetParsedEntity(DefaultPostHtml);
            Assert.AreEqual(string.Empty, user.AvatarTitle);
        }

        [TestMethod]
        public void AvatarTitle_Is_Parsed_When_Title_Exists()
        {
            var user = GetParsedEntity(PostWithCustomTitle);
            Assert.AreEqual("Only in Kenya.", user.AvatarTitle);
        }

        [TestMethod]
        public void AvatarLink_Is_Null_When_No_Image_Exists()
        {
            var user = GetParsedEntity(PostWithNoAvatar);
            Assert.IsNull(user.AvatarLink);
        }

        [TestMethod]
        public void AvatarLink_Is_Parsed_When_Image_Exists()
        {
            var user = GetParsedEntity(DefaultPostHtml);
            Assert.AreEqual("<!DOCTYPE html><html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"ms-appx-web:///Assets/ui-light.css\"></head><body style=\"background-color: white;\"></head><body><img src=\"http://fi.somethingawful.com/customtitles/title-the_dave-2.gif\" alt=\"\" class=\"img\" border=\"0\"></body></html>", user.AvatarLink);
        }

        [TestMethod]
        public void Post_Is_Parsed()
        {
            var user = GetParsedEntity(DefaultPostHtml);
            Assert.AreEqual(41741, user.Id);
        }


        private static ForumUserEntity GetParsedEntity(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return ForumUserEntity.FromPost(doc.DocumentNode);
        }
    }
}
