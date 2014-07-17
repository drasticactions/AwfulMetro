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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;

namespace AwfulMetro.Core.Tools
{
    public class PageScrollingCollection :  ObservableCollection<ForumThreadEntity>, INotifyPropertyChanged, ISupportIncrementalLoading
    {
        public PageScrollingCollection(ForumEntity forumEntity, int pageCount)
        {
            HasMoreItems = true;
            IsLoading = false;
            PageCount = pageCount;
            ForumEntity = forumEntity;
        }

        private int PageCount { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            private set
            {
                _isLoading = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsLoading"));
            }
        }

        private ForumEntity ForumEntity { get; set; }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }

        public bool HasMoreItems { get; protected set; }

        public async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {
            IsLoading = true;
            var threadManager = new ThreadManager();
            ObservableCollection<ForumThreadEntity> forumThreadEntities;
            if (ForumEntity.IsBookmarks)
            {
                forumThreadEntities = await threadManager.GetBookmarks(ForumEntity, PageCount);
            }
            else
            {
                forumThreadEntities = await threadManager.GetForumThreads(ForumEntity, PageCount);
            }
            foreach (ForumThreadEntity forumThreadEntity in forumThreadEntities.Where(forumThreadEntity => !forumThreadEntity.IsAnnouncement))
            {
                Add(forumThreadEntity);
            }
            if (forumThreadEntities.Any(node => !node.IsAnnouncement))
            {
                HasMoreItems = true;
                PageCount++;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
            return new LoadMoreItemsResult {Count = count};
        }

        public async Task<LoadMoreItemsResult> RefreshBookmarkedThreads()
        {
            IsLoading = true;
            var threadManager = new ThreadManager();
            for (var index = 1; index <= PageCount; index ++)
            {
                ObservableCollection<ForumThreadEntity> forumThreadEntities = await threadManager.GetBookmarks(ForumEntity, index);
                if (!forumThreadEntities.Any()) continue;
                for (int i = 0; i < this.Count; i++)
                {
                    var updatedThread = forumThreadEntities.FirstOrDefault(node => node.ThreadId == this[i].ThreadId);
                    if (updatedThread != null)
                    {
                        if (forumThreadEntities[i].ThreadId == this[i].ThreadId)
                        {
                            // Rather than update the entire thread, just update
                            this[i].RepliesSinceLastOpened = updatedThread.RepliesSinceLastOpened;
                            this[i].ReplyCount = updatedThread.ReplyCount;
                            this[i].HasSeen = updatedThread.HasSeen;
                            this[i].KilledBy = updatedThread.KilledBy;
                        }
                        else
                        {
                            // Replace the reference
                            this[i] = updatedThread;
                        }
                    }
                    else
                    {
                        this.RemoveItem(i);
                    }
                }
            }
            IsLoading = false;
            return new LoadMoreItemsResult { Count = 0 };
        }
    }
}