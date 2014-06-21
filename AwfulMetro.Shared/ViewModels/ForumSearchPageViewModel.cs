using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AwfulMetro.Common;
using AwfulMetro.Pcl.Core.Entity;
using AwfulMetro.Pcl.Core.Manager;

namespace AwfulMetro.ViewModels
{
    public class ForumSearchPageViewModel : NotifierBase
    {
        private bool _isLoading;
        private List<ForumUserSearchEntity> _forumUserList;

        public List<ForumUserSearchEntity> ForumUserList
        {
            get { return _forumUserList; }
            set
            {
                SetProperty(ref _forumUserList, value);
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

        public async void GetUsernameList(string username)
        {
            IsLoading = true;
            var searchManager = new SearchManager();
            ForumUserList = await searchManager.GetUsernames(username);
            IsLoading = false;
        }
    }
}
