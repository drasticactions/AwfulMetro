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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AwfulMetro.Commands;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.Tools;
using AwfulMetro.Views;
using Newtonsoft.Json;

namespace AwfulMetro.ViewModels
{
    public class ThreadListPageViewModel : NotifierBase
    {
        private readonly ForumManager _forumManager = new ForumManager();
        private readonly ThreadManager _threadManager = new ThreadManager();
        private PageScrollingCollection _forumPageScrollingCollection;
        private ObservableCollection<ForumThreadEntity> _forumThreadEntities;
        private ObservableCollection<ForumEntity> _subForumEntities;
        private ApplicationDataContainer _localSettings;
        private string _forumTitle;
        private bool _isBookmarks;
        public ForumEntity ForumEntity { get; set; }
        public ThreadListPageViewModel()
        {
            this.UnreadCommand = new ThreadCommand(ExecuteUnreadCommand);
            this.LastPageCommand = new ThreadCommand(ExecuteLastPageCommand);
            this.BookmarkCommand = new BookmarkCommand(ExecuteBookmarkCommand);
        }

        public string ForumTitle
        {
            get { return _forumTitle; }
            set
            {
                SetProperty(ref _forumTitle, value);
                OnPropertyChanged();
            }
        }

        public bool IsBookmarks
        {
            get { return _isBookmarks; }
            set
            {
                SetProperty(ref _isBookmarks, value);
                OnPropertyChanged();
            }
        }

        public PageScrollingCollection ForumPageScrollingCollection
        {
            get { return _forumPageScrollingCollection; }
            set
            {
                SetProperty(ref _forumPageScrollingCollection, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ForumEntity> SubForumEntities
        {
            get { return _subForumEntities; }
            set
            {
                SetProperty(ref _subForumEntities, value);
                OnPropertyChanged();
            }
        }

        public async void RefreshForum(ForumEntity forumEntity)
        {
            if (ForumPageScrollingCollection == null) return;
            if (!forumEntity.Name.Equals("Bookmarks")) return;
            try
            {
                await ForumPageScrollingCollection.RefreshBookmarkedThreads();
            }
            catch (Exception ex)
            {
                AwfulDebugger.SendMessageDialogAsync("Failed to refresh bookmarks", ex);
            }
        }

        public async void Initialize(ForumEntity forumEntity)
        {
            this.ForumEntity = forumEntity;
            IsBookmarks = forumEntity.IsBookmarks;
            ForumTitle = forumEntity.Name;
            SubForumEntities = new ObservableCollection<ForumEntity>();
            try
            {
                if (forumEntity.IsBookmarks)
                {
                    _localSettings = ApplicationData.Current.LocalSettings;
                    ForumPageScrollingCollection = new PageScrollingCollection(forumEntity, 1);
                }
                else
                {
                    ForumPageScrollingCollection = new PageScrollingCollection(forumEntity, 1);
                    SubForumEntities = await _forumManager.GetSubForums(forumEntity);
                }
            }
            catch (Exception ex)
            {
                AwfulDebugger.SendMessageDialogAsync("Failed to initialize threads", ex);
            }
        }

        public IThreadCommand UnreadCommand { protected set; get; }

        public IThreadCommand LastPageCommand { protected set; get; }

        public IBookmarkCommand BookmarkCommand { protected set; get; }

        private async void ExecuteUnreadCommand(object param)
        {
            var threadId = (Int64)param;
            var thread = ForumPageScrollingCollection.FirstOrDefault(node => node.ThreadId == threadId);
            if (thread == null)
                return;
            var threadManager = new ThreadManager();
            await threadManager.MarkThreadUnread(new List<long> { thread.ThreadId });
            thread.HasBeenViewed = false;
            thread.HasSeen = false;
            thread.ReplyCount = 0;
        }

        private async void ExecuteLastPageCommand(object param)
        {
            var thread = (ForumThreadEntity)param;
            if (thread == null)
                return;
            thread.CurrentPage = thread.TotalPages;
            string jsonObjectString = JsonConvert.SerializeObject(thread);
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null) rootFrame.Navigate(typeof(ThreadPage), jsonObjectString);
        }

        private async void ExecuteBookmarkCommand(object param)
        {
            var thread = param as ForumThreadEntity;
            if (thread == null)
                return;
            var threadManager = new ThreadManager();
            if (thread.IsBookmark)
            {
                await threadManager.RemoveBookmarks(new List<long> { thread.ThreadId });
                return;
            }
            await threadManager.AddBookmarks(new List<long> { thread.ThreadId });
            var msgDlg2 =
                   new MessageDialog(string.Format("'{0}' has been added to your bookmarks! I love you!{1}{2}",
                       thread.Name, Environment.NewLine, Constants.ASCII_1))
                   {
                       DefaultCommandIndex = 1
                   };
            await msgDlg2.ShowAsync();
        }
    }
}
