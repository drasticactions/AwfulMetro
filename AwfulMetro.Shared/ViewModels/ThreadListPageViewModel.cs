using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;

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

        public ThreadListPageViewModel()
        {
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

        public async void Initialize(ForumEntity forumEntity)
        {
            IsBookmarks = forumEntity.IsBookmarks;
            ForumTitle = forumEntity.Name;
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
    }
}
