using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Security.Credentials;
using Windows.UI.Notifications;
using BusinessObjects;
using BusinessObjects.Entity;
using BusinessObjects.Manager;
using BusinessObjects.Tools;
using Windows.Storage;

namespace BackgroundStatus
{
    public sealed class BackgroundNotifyStatus : IBackgroundTask
    {

        BackgroundTaskDeferral _deferral = null;
        IBackgroundTaskInstance _taskInstance = null;
        volatile bool _cancelled = false;
        StreamContent _currentStream;

       
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
            //deferral.Complete();
        }

        private void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            _cancelled = true;
            if (_currentStream != null)
            {
                //_currentStream.s;
            }
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelled = true;
            if (_currentStream != null)
            {
                //_currentStream.s;
            }
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
