using BusinessObjects.Entity;
using BusinessObjects.Manager;
using BusinessObjects.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BackgroundStatus
{
    public sealed class BackgroundNotifyStatus : IBackgroundTask
    {  
        List<long> ThreadIds = new List<long>();
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            if (NotifyStatusTile.IsInternet())
            {
                await Update(taskInstance);
            }
            deferral.Complete();

        }

        private async Task Update(IBackgroundTaskInstance taskInstance)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            ForumEntity forumCategory = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty);
            var ForumCollection = await ThreadManager.GetBookmarks(forumCategory);
            var ForumThreads = ForumCollection.ForumThreadList.Where(thread => thread.RepliesSinceLastOpened > 0).ToList();

            if (localSettings.Values.ContainsKey("ThreadIds"))
            {
                ThreadIds = (List<long>)localSettings.Values["ThreadIds"];
                ForumThreads = (List<ForumThreadEntity>)ForumThreads.Where(thread => ThreadIds.Contains(thread.ThreadId));
            }

            this.CreateBookmarkLiveTiles(ForumThreads);
        }

        private void CreateBookmarkLiveTiles(List<ForumThreadEntity> forumThreads)
        {
            foreach(var thread in forumThreads)
            {
                NotifyStatusTile.CreateBookmarkLiveTile(thread);
            }
        }

       
    }
}
