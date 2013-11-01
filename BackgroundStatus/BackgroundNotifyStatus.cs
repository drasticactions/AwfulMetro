using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var forumCategory = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty);
            ForumCollectionEntity forumCollection = await _threadManager.GetBookmarks(forumCategory);
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