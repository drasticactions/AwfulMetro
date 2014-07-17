using AwfulMetro.Core.Entity;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace AwfulMetro.Windows8.Tests.Unit
{
    [TestClass]
    // ReSharper disable InconsistentNaming
    public class When_Parsing_A_RapSheet
    {
        private const string DefaultHtml =
            "<tr><td align=\"left\"><b><a href=\"/showthread.php?goto=post&amp;postid=338912066\" target=\"new\">PROBATION</a></b></td><td nowrap=\"\">01/28/08 10:22am</td><td nowrap=\"\"><b><a href=\"/member.php?s=&amp;action=getinfo&amp;userid=39373\" target=\"_blank\">Ithaqua</a></b></td><td>Don't be mean.  User loses posting privileges for 1 day.</td><td nowrap=\"\"><a href=\"/member.php?s=&amp;action=getinfo&amp;userid=42569\" target=\"_blank\" nowrap=\"\">Louisgod</a></td><td nowrap=\"\"><a href=\"/member.php?s=&amp;action=getinfo&amp;userid=16393\" target=\"_blank\" nowrap=\"\">Fistgrrl</a></td></tr>";

        [TestMethod]
        public void Type_Is_Successfully_Parsed()
        {
            ForumUserRapSheetEntity rapsheet = ParseEntity(DefaultHtml);
            Assert.AreEqual("PROBATION", rapsheet.PunishmentType);
        }

        [TestMethod]
        public void Date_Is_Successfully_Parsed()
        {
            ForumUserRapSheetEntity rapsheet = ParseEntity(DefaultHtml);
            Assert.AreEqual("01/28/08 10:22am", rapsheet.Date);
        }

        [TestMethod]
        public void HorribleJerk_Is_Successfully_Parsed()
        {
            ForumUserRapSheetEntity rapsheet = ParseEntity(DefaultHtml);
            Assert.AreEqual("Ithaqua", rapsheet.HorribleJerk);
        }

        [TestMethod]
        public void HorribleJerkId_Is_Successfully_Parsed()
        {
            ForumUserRapSheetEntity rapsheet = ParseEntity(DefaultHtml);
            Assert.AreEqual(39373, rapsheet.HorribleJerkId);
        }

        [TestMethod]
        public void PunishmentReason_Is_Successfully_Parsed()
        {
            ForumUserRapSheetEntity rapsheet = ParseEntity(DefaultHtml);
            Assert.AreEqual("Don't be mean.  User loses posting privileges for 1 day.", rapsheet.PunishmentReason);
        }

        [TestMethod]
        public void RequestedBy_Is_Successfully_Parsed()
        {
            ForumUserRapSheetEntity rapsheet = ParseEntity(DefaultHtml);
            Assert.AreEqual("Louisgod", rapsheet.RequestedBy);
        }

        [TestMethod]
        public void RequestedById_Is_Successfully_Parsed()
        {
            ForumUserRapSheetEntity rapsheet = ParseEntity(DefaultHtml);
            Assert.AreEqual(42569, rapsheet.RequestedById);
        }

        [TestMethod]
        public void ApprovedBy_Is_Successfully_Parsed()
        {
            ForumUserRapSheetEntity rapsheet = ParseEntity(DefaultHtml);
            Assert.AreEqual("Fistgrrl", rapsheet.ApprovedBy);
        }

        [TestMethod]
        public void ApprovedById_Is_Successfully_Parsed()
        {
            ForumUserRapSheetEntity rapsheet = ParseEntity(DefaultHtml);
            Assert.AreEqual(16393, rapsheet.ApprovedById);
        }

        public static ForumUserRapSheetEntity ParseEntity(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return ForumUserRapSheetEntity.FromRapSheet(doc.DocumentNode);
        }
    }
}