#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion
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
            var forumCategory = new ForumEntity()
            {
                Name = "Bookmarks",
                IsSubforum = false,
                IsBookmarks = true,
                Location = Constants.USER_CP
            };

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