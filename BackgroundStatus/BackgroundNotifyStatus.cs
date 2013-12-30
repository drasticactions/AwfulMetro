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
            var forumCategory = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty, false);
            List<ForumThreadEntity> forumThreadEntities = await _threadManager.GetBookmarks(forumCategory);
            if (localSettings.Values.ContainsKey("_threadIds"))
            {
                _threadIds = (List<long>) localSettings.Values["_threadIds"];
                forumThreadEntities = (List<ForumThreadEntity>)forumThreadEntities.Where(thread => _threadIds.Contains(thread.ThreadId));
            }

            CreateBookmarkLiveTiles(forumThreadEntities);
        }

        private void CreateBookmarkLiveTiles(IEnumerable<ForumThreadEntity> forumThreads)
        {
            foreach (ForumThreadEntity thread in forumThreads)
            {
                if (thread.RepliesSinceLastOpened > 0)
                NotifyStatusTile.CreateBookmarkLiveTile(thread);
            }
        }
    }
}