using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using BusinessObjects.Entity;
using BusinessObjects.Manager;
using BusinessObjects.Tools;

namespace BackgroundStatus
{
    public sealed class BackgroundNotifyStatus : IBackgroundTask
    {
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
            var forumCategory = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty);
            ForumCollectionEntity forumCollection = await ThreadManager.GetBookmarks(forumCategory);
            List<ForumThreadEntity> forumThreads =
                forumCollection.ForumThreadList.Where(thread => thread.RepliesSinceLastOpened > 0).ToList();

            if (localSettings.Values.ContainsKey("_threadIds"))
            {
                _threadIds = (List<long>) localSettings.Values["_threadIds"];
                forumThreads =
                    (List<ForumThreadEntity>) forumThreads.Where(thread => _threadIds.Contains(thread.ThreadId));
            }

            CreateBookmarkLiveTiles(forumThreads);
        }

        private void CreateBookmarkLiveTiles(IEnumerable<ForumThreadEntity> forumThreads)
        {
            foreach (ForumThreadEntity thread in forumThreads)
            {
                NotifyStatusTile.CreateBookmarkLiveTile(thread);
            }
        }
    }
}