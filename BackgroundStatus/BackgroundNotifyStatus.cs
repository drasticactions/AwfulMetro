using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;

namespace AwfulMetro.BackgroundStatus
{
    public sealed class BackgroundNotifyStatus : IBackgroundTask
    {
        private readonly ThreadManager _threadManager = new ThreadManager();
        private List<long> _threadIds = new List<long>();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            if (NotifyStatusTile.IsInternet())
            {
                await Update(taskInstance);
            }
            deferral.Complete();
        }

        private async Task Update(IBackgroundTaskInstance taskInstance)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            var forumCategory = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty, false);
            ObservableCollection<ForumThreadEntity> forumThreadEntities = await _threadManager.GetBookmarks(forumCategory, 1);
            CreateBookmarkLiveTiles(forumThreadEntities);

            if (localSettings.Values.ContainsKey("_threadIds") && !string.IsNullOrEmpty((string)localSettings.Values["_threadIds"]))
            {
                DeserializeXmlToList((string) localSettings.Values["_threadIds"]);
                List<ForumThreadEntity> list =
                    forumThreadEntities.Where(thread => _threadIds.Contains(thread.ThreadId)).ToList();
                CreateToastNotifications(list);
            }
        }

        private void CreateBookmarkLiveTiles(IEnumerable<ForumThreadEntity> forumThreads)
        {
            foreach (ForumThreadEntity thread in forumThreads.Where(thread => thread.RepliesSinceLastOpened > 0))
            {
                NotifyStatusTile.CreateBookmarkLiveTile(thread);
            }
        }

        private void CreateToastNotifications(IEnumerable<ForumThreadEntity> forumThreads)
        {
            foreach (ForumThreadEntity thread in forumThreads.Where(thread => thread.RepliesSinceLastOpened > 0))
            {
                NotifyStatusTile.CreateToastNotification(thread);
            }
        }

        public void DeserializeXmlToList(string listAsXml)
        {
            try
            {
                var xmlIzer = new XmlSerializer(typeof (List<long>));
                var strReader = new StringReader(listAsXml);
                _threadIds = (xmlIzer.Deserialize(strReader)) as List<long>;
            }

            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                _threadIds = new List<long>();
            }
        }
    }
}