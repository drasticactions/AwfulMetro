using System.Linq;
using AwfulMetro.Core.Entity;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace AwfulMetro.Windows8.Tests.Unit
{
    // ReSharper disable InconsistentNaming
    [TestClass]
    public class When_Parsing_A_Forum_Thread
    {
        private const string DefaultThread =
            "<tr class=\"thread\" id=\"thread3580313\">\n\t<td class=\"star \"></td>\n\t<td class=\"icon\"><a href=\"/forumdisplay.php?forumid=1&amp;posticon=122\"><img src=\"http://fi.somethingawful.com/forums/posticons/icons-08/drugs.png#122\" alt=\"\"></a></td>\n\t<td class=\"title\">\n\t\t<div class=\"title_inner\">\n\t\t\t\n\t\t\t<div class=\"info\">\n\t\t\t\t<a href=\"showthread.php?threadid=3580313\" class=\"thread_title\">gbs depression crew</a>\n\t\t\t\t\n\t\t\t</div>\n\t\t</div>\n\t</td>\n\t<td class=\"author\"><a href=\"member.php?action=getinfo&amp;userid=60252\">Puppy Galaxy</a></td>\n\t<td class=\"replies\">18</td>\n\t<td class=\"views\">102</td>\n\t<td class=\"rating\">&nbsp;</td>\n\t<td class=\"lastpost\"><div class=\"date\">15:07 Nov  3, 2013</div><a class=\"author\" href=\"showthread.php?goto=lastpost&amp;threadid=3580313\">gigawhite</a></td>\n</tr>";

        private const string StickyLockedThread =
            "<tr class=\"thread\" id=\"thread3577971\">\n\t<td class=\"star \"></td>\n\t<td class=\"icon\"><a href=\"/forumdisplay.php?forumid=1&amp;posticon=67\"><img src=\"http://fi.somethingawful.com/forums/posticons/icons-08/newbie.png#67\" alt=\"\"></a></td>\n\t<td class=\"title title_sticky\">\n\t\t<div class=\"title_inner\">\n\t\t\t\n\t\t\t<div class=\"info\">\n\t\t\t\t<a href=\"showthread.php?threadid=3577971\" class=\"thread_title\">Make Me A New Avatar, And I'll Mail You Candy</a>\n\t\t\t\t<div class=\"title_pages\">Pages: <a class=\"pagenumber\" href=\"showthread.php?threadid=3577971&amp;pagenumber=1\">1</a><a class=\"pagenumber\" href=\"showthread.php?threadid=3577971&amp;pagenumber=2\">2</a></div>\n\t\t\t</div>\n\t\t</div>\n\t</td>\n\t<td class=\"author\"><a href=\"member.php?action=getinfo&amp;userid=45450\">Call Me Abey</a></td>\n\t<td class=\"replies\">74</td>\n\t<td class=\"views\">15921</td>\n\t<td class=\"rating\">&nbsp;</td>\n\t<td class=\"lastpost\"><div class=\"date\">12:25 Nov  3, 2013</div><a class=\"author\" href=\"showthread.php?goto=lastpost&amp;threadid=3577971\">Ootrek</a></td>\n</tr>";

        private const string LockedThread =
            "<tr class=\"thread closed\" id=\"thread3579167\">\n\t<td class=\"star \"></td>\n\t<td class=\"icon\"><a href=\"/forumdisplay.php?forumid=188&amp;posticon=0\"><img src=\"http://fi.somethingawful.com/images/shitpost.gif#0\" alt=\"\"></a></td>\n\t<td class=\"title\">\n\t\t<div class=\"title_inner\">\n\t\t\t\n\t\t\t<div class=\"info\">\n\t\t\t\t<a href=\"showthread.php?threadid=3579167\" class=\"thread_title\">A guy was probated for 4,000 days.</a>\n\t\t\t\t\n\t\t\t</div>\n\t\t</div>\n\t</td>\n\t<td class=\"author\"><a href=\"member.php?action=getinfo&amp;userid=162660\">Volcott</a></td>\n\t<td class=\"replies\">5</td>\n\t<td class=\"views\">1199</td>\n\t<td class=\"rating\">&nbsp;</td>\n\t<td class=\"lastpost\"><div class=\"date\">13:10 Nov  2, 2013</div><a class=\"author\" href=\"showthread.php?goto=lastpost&amp;threadid=3579167\">Ralp</a></td>\n</tr>";

        private const string PreviouslyViewedThreadWithNoNewPosts =
            "<tr class=\"thread\" id=\"thread3580029\">\n\t<td class=\"star \"></td>\n\t<td class=\"icon\"><a href=\"/forumdisplay.php?forumid=1&amp;posticon=255\"><img src=\"http://fi.somethingawful.com/forums/posticons/fyad-falconry.gif#255\" alt=\"\"></a></td>\n\t<td class=\"title\">\n\t\t<div class=\"title_inner\">\n\t\t\t<div class=\"lastseen\"><a href=\"/showthread.php?action=resetseen&amp;threadid=3580029\" class=\"x\" title=\"Mark as unread\">X</a></div>\n\t\t\t<div class=\"info\">\n\t\t\t\t<a href=\"showthread.php?threadid=3580029\" class=\"thread_title\">My old boss and a friend stabbed a dude in the neck. But he is a good guy!</a>\n\t\t\t\t\n\t\t\t</div>\n\t\t</div>\n\t</td>\n\t<td class=\"author\"><a href=\"member.php?action=getinfo&amp;userid=199865\">Vahakyla</a></td>\n\t<td class=\"replies\">21</td>\n\t<td class=\"views\">325</td>\n\t<td class=\"rating\">&nbsp;</td>\n\t<td class=\"lastpost\"><div class=\"date\">15:04 Nov  3, 2013</div><a class=\"author\" href=\"showthread.php?goto=lastpost&amp;threadid=3580029\">Watch Out Smarmy</a></td>\n</tr>";

        private const string ThreadWithUnreadPosts =
            "<tr class=\"thread\" id=\"thread3579523\">\n\t<td class=\"star \"></td>\n\t<td class=\"icon\"><a href=\"/forumdisplay.php?forumid=1&amp;posticon=692\"><img src=\"http://fi.somethingawful.com/forums/posticons/dd-9-11.gif#692\" alt=\"\"></a></td>\n\t<td class=\"title\">\n\t\t<div class=\"title_inner\">\n\t\t\t<div class=\"lastseen\"><a href=\"/showthread.php?action=resetseen&amp;threadid=3579523\" class=\"x\" title=\"Mark as unread\">X</a><a title=\"Jump to last read post\" href=\"/showthread.php?threadid=3579523&amp;goto=newpost\" class=\"count\"><b>287</b></a></div>\n\t\t\t<div class=\"info\">\n\t\t\t\t<a href=\"showthread.php?threadid=3579523\" class=\"thread_title\">Was this like what GBS was in the early days (OLD REGDATES ASSEMBLE)</a>\n\t\t\t\t<div class=\"title_pages\">Pages: <a class=\"pagenumber\" href=\"showthread.php?threadid=3579523&amp;pagenumber=1\">1</a><a class=\"pagenumber\" href=\"showthread.php?threadid=3579523&amp;pagenumber=2\">2</a><a class=\"pagenumber\" href=\"showthread.php?threadid=3579523&amp;pagenumber=3\">3</a><a class=\"pagenumber\" href=\"showthread.php?threadid=3579523&amp;pagenumber=4\">4</a><a class=\"pagenumber\" href=\"showthread.php?threadid=3579523&amp;pagenumber=5\">5</a><a class=\"pagenumber\" href=\"showthread.php?threadid=3579523&amp;pagenumber=6\">6</a><a class=\"pagenumber\" href=\"showthread.php?threadid=3579523&amp;pagenumber=7\">7</a>... <a class=\"pagenumber\" href=\"showthread.php?threadid=3579523&amp;goto=lastpost\">Last</a></div>\n\t\t\t</div>\n\t\t</div>\n\t</td>\n\t<td class=\"author\"><a href=\"member.php?action=getinfo&amp;userid=55801\">Toad on a Hat</a></td>\n\t<td class=\"replies\">326</td>\n\t<td class=\"views\">10050</td>\n\t<td class=\"rating\">&nbsp;</td>\n\t<td class=\"lastpost\"><div class=\"date\">15:05 Nov  3, 2013</div><a class=\"author\" href=\"showthread.php?goto=lastpost&amp;threadid=3579523\">Meme Emulator</a></td>\n</tr>";

        [TestMethod]
        public void Name_Is_Parsed_Successfully()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.AreEqual("gbs depression crew", thread.Name);
        }

        [TestMethod]
        public void KilledBy_Is_Parsed_Successfully()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.AreEqual("gigawhite", thread.KilledBy);
        }

        [TestMethod]
        public void IsSticky_Is_True_When_Thread_Is_Sticky()
        {
            ForumThreadEntity thread = ParseEntity(StickyLockedThread);
            Assert.IsTrue(thread.IsSticky);
        }

        [TestMethod]
        public void IsSticky_Is_False_When_Thread_Is_Not_Sticky()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.IsFalse(thread.IsSticky);
        }

        [TestMethod]
        public void IsLocked_Is_True_When_Thread_Is_Locked_And_Not_Sticky()
        {
            ForumThreadEntity thread = ParseEntity(LockedThread);
            Assert.IsTrue(thread.IsLocked);
        }

        [TestMethod]
        public void IsLocked_Is_False_When_Thread_Is_Locked_And_Sticky()
        {
            ForumThreadEntity thread = ParseEntity(StickyLockedThread);
            Assert.IsFalse(thread.IsLocked);
        }

        [TestMethod]
        public void IsLocked_Is_False_When_Thread_Is_Not_Locked()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.IsFalse(thread.IsLocked);
        }

        [TestMethod]
        public void CanMarkAsUnread_Is_True_For_Previously_Viewed_Threads_With_Unread_Posts()
        {
            ForumThreadEntity thread = ParseEntity(ThreadWithUnreadPosts);
            Assert.IsTrue(thread.CanMarkAsUnread);
        }

        [TestMethod]
        public void CanMarkAsUnread_Is_True_For_Previously_Viewed_Threads_Without_Unread_Posts()
        {
            ForumThreadEntity thread = ParseEntity(PreviouslyViewedThreadWithNoNewPosts);
            Assert.IsTrue(thread.CanMarkAsUnread);
        }

        [TestMethod]
        public void CanMarkAsUnread_Is_False_For_Unread_Threads()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.IsFalse(thread.CanMarkAsUnread);
        }

        [TestMethod]
        public void HasBeenViewed_Is_True_For_Previously_Viewed_Threads()
        {
            ForumThreadEntity thread = ParseEntity(PreviouslyViewedThreadWithNoNewPosts);
            Assert.IsTrue(thread.HasBeenViewed);
        }

        [TestMethod]
        public void HasBeenViewed_Is_False_For_Unread_Threads()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.IsFalse(thread.HasBeenViewed);
        }

        [TestMethod]
        public void Author_Is_Parsed_Successfully()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.AreEqual("Puppy Galaxy", thread.Author);
        }

        [TestMethod]
        public void RepliesSinceLastOpened_Is_Zero_When_Not_Present()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.AreEqual(0, thread.RepliesSinceLastOpened);
        }

        [TestMethod]
        public void RepliesSinceLastOpened_Is_Parsed_Successfully_When_Present()
        {
            ForumThreadEntity thread = ParseEntity(ThreadWithUnreadPosts);
            Assert.AreEqual(287, thread.RepliesSinceLastOpened);
        }

        [TestMethod]
        public void ReplyCount_Is_Parsed_Successfully()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.AreEqual(18, thread.ReplyCount);
        }

        [TestMethod]
        public void TotalPages_Is_Calculated_Successfully_For_Single_Page_Thread()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.AreEqual(1, thread.TotalPages);
        }

        [TestMethod]
        public void TotalPages_Is_Calculated_Successfully_For_Multi_Page_Thread()
        {
            ForumThreadEntity thread = ParseEntity(ThreadWithUnreadPosts);
            Assert.AreEqual(9, thread.TotalPages);
        }

        [TestMethod]
        public void Location_Is_Parsed_Successfully()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.AreEqual("http://forums.somethingawful.com/showthread.php?threadid=3580313&perpage=40",
                thread.Location);
        }

        [TestMethod]
        public void ThreadId_Is_Parsed_Successfully()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.AreEqual(3580313, thread.ThreadId);
        }

        [TestMethod]
        public void ImageIconLocation_Is_Parsed_Successfully()
        {
            ForumThreadEntity thread = ParseEntity(DefaultThread);
            Assert.AreEqual("http://fi.somethingawful.com/forums/posticons/icons-08/drugs.png#122",
                thread.ImageIconLocation);
        }

        private ForumThreadEntity ParseEntity(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var thread = new ForumThreadEntity();

            thread.Parse(
                doc.DocumentNode.Descendants("tr")
                    .First(node => node.GetAttributeValue("class", string.Empty).StartsWith("thread")));
            return thread;
        }
    }
}