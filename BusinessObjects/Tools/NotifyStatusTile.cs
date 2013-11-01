using AwfulMetro.Core.Entity;
using Windows.Networking.Connectivity;
using Windows.UI.Notifications;

namespace AwfulMetro.Core.Tools
{
    public class NotifyStatusTile
    {
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        public static void CreateBookmarkLiveTile(ForumThreadEntity forumThread)
        {
            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text05);
            var tileAttributes = tileXml.GetElementsByTagName("text");
            tileAttributes[0].AppendChild(tileXml.CreateTextNode(forumThread.Name));
            tileAttributes[1].AppendChild(tileXml.CreateTextNode(string.Format("Killed By: {0}", forumThread.KilledBy)));
            tileAttributes[2].AppendChild(tileXml.CreateTextNode(string.Format("Unread Posts: {0}", forumThread.RepliesSinceLastOpened)));
            //var imageElement = tileXml.GetElementsByTagName("image");
            //imageElement[0].Attributes[1].NodeValue = "http://fi.somethingawful.com/forums/posticons/lf-arecountry.gif" ;
            var tileNotification = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            
        }
    }
}
