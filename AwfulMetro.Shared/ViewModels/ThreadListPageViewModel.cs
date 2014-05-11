using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using AwfulMetro.Commands;
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
        public ForumEntity ForumEntity { get; set; }
        public ThreadListPageViewModel()
        {
            this.UnreadCommand = new UnreadCommand(ExecuteUnreadCommand);
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

        public async void Initialize(ForumEntity forumEntity)
        {
            this.ForumEntity = forumEntity;
            IsBookmarks = forumEntity.IsBookmarks;
            ForumTitle = forumEntity.Name;
            SubForumEntities = new ObservableCollection<ForumEntity>();
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

        public IUnreadCommand UnreadCommand { protected set; get; }

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
            var msgDlg =
                    new MessageDialog(
                        string.Format("'{0}' is now \"Unread\"! Now go outside and do something productive!{1}{2}",
                            thread.Name, Environment.NewLine, Constants.ASCII_1))
                    {
                        DefaultCommandIndex = 1
                    };
            await msgDlg.ShowAsync();
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
                ForumPageScrollingCollection.Remove(thread);
                var msgDlg =
                       new MessageDialog(string.Format("'{0}' has been removed from your bookmarks! I love you!{1}{2}",
                           thread.Name, Environment.NewLine, Constants.ASCII_1))
                       {
                           DefaultCommandIndex = 1
                       };
                await msgDlg.ShowAsync();
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
