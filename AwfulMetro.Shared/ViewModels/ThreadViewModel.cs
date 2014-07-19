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
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Pcl.Core.Entity;
using AwfulMetro.Pcl.Core.Manager;
using AwfulMetro.Pcl.Core.Tools;

namespace AwfulMetro.ViewModels
{
    public class ThreadViewModel : NotifierBase
    {
        private ForumThreadEntity _forumThreadEntity;

        private List<ForumThreadEntity> _linkedThreads = new List<ForumThreadEntity>(); 

        private string _html;

        private string _threadTitle;

        private bool _isLoading;

        private IEnumerable<int> _pageNumbers;

        public IEnumerable<int> PageNumbers
        {
            get { return _pageNumbers; }
            set
            {
                SetProperty(ref _pageNumbers, value);
                OnPropertyChanged();
            }
        }

        public List<ForumThreadEntity> LinkedThreads
        {
            get { return _linkedThreads; }
            set
            {
                SetProperty(ref _linkedThreads, value);
                OnPropertyChanged();
            }
        }

        public ForumThreadEntity ForumThreadEntity
        {
            get { return LinkedThreads.LastOrDefault(); }
        }

        public string ThreadTitle
        {
            get { return _threadTitle; }
            set
            {
                SetProperty(ref _threadTitle, value);
                OnPropertyChanged();
            }
        }

        public string Html
        {
            get { return _html; }
            set
            {
                SetProperty(ref _html, value);
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged();
            }
        }

        public async void GetForumPosts()
        {
            IsLoading = true;
            ThreadTitle = ForumThreadEntity.Name;
            PostManager postManager = new PostManager();
            await postManager.GetThreadPosts(ForumThreadEntity);
            // Get the current reply count
            if (ForumThreadEntity.ScrollToPost != 0)
            {
                ForumThreadEntity.RepliesSinceLastOpened = ForumThreadEntity.RepliesSinceLastOpened - (ForumThreadEntity.ForumPosts.Count - ForumThreadEntity.ScrollToPost);
            }
            Html = await HtmlFormater.FormatThreadHtml(ForumThreadEntity);
            PageNumbers = Enumerable.Range(1, ForumThreadEntity.TotalPages).ToArray();
            OnPropertyChanged("ForumThreadEntity");
            IsLoading = false;
        }
    }
}
