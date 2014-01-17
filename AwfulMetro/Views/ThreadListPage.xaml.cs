using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using AwfulMetro.Core.Tools;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ThreadListPage
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper _navigationHelper;
        private readonly ThreadManager _threadManager = new ThreadManager();
        private readonly ForumManager _forumManager = new ForumManager();
        private ForumEntity _forumEntity;
        private PageScrollingCollection _forumPageScrollingCollection;
        private List<ForumThreadEntity> _forumThreadEntities;
        private List<ForumEntity> _subForumEntities; 
        public ThreadListPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        ///     This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        /// <summary>
        ///     NavigationHelper is used on each page to aid in navigation and
        ///     process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }


        /// <summary>
        ///     Populates the page with content passed during navigation. Any saved state is also
        ///     provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        ///     The source of the event; typically <see cref="NavigationHelper" />
        /// </param>
        /// <param name="e">
        ///     Event data that provides both the navigation parameter passed to
        ///     <see cref="Frame.Navigate(Type, Object)" /> when this page was initially requested and
        ///     a dictionary of state preserved by this page during an earlier
        ///     session. The state will be null the first time a page is visited.
        /// </param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
            loadingProgressBar.Visibility = Visibility.Visible;

            var jsonObjectString = (string) e.NavigationParameter;
            var forumInfo = JsonConvert.DeserializeObject<ForumEntity>(jsonObjectString);
            if (forumInfo == null) return;
            _forumEntity = forumInfo;
            pageTitle.Text = _forumEntity.Name;
            pageSnapTitle.Text = _forumEntity.Name;
            await GetForumThreads();
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     Preserves state associated with this page in case the application is suspended or the
        ///     page is discarded from the navigation cache.  Values must conform to the serialization
        ///     requirements of <see cref="SuspensionManager.SessionState" />.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper" /></param>
        /// <param name="e">
        ///     Event data that provides an empty dictionary to be populated with
        ///     serializable state.
        /// </param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        /// <summary>
        ///     Handler for the CommandsRequested event. Add custom SettingsCommands here.
        /// </summary>
        /// <param name="settingsPane">The settings pane.</param>
        /// <param name="e">Event data that includes a vector of commands (ApplicationCommands)</param>
        private void OnCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs e)
        {
            if (!_forumEntity.IsBookmarks) return;
            var bookmarksCommand = new SettingsCommand("bookmarkSettings", "Bookmarks",
                handler =>
                {
                    var up = new BookmarkSettingsFlyout();
                    up.Show();
                });
            e.Request.ApplicationCommands.Add(bookmarksCommand);
        }

        private void ForumThreadList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((ForumThreadEntity) e.ClickedItem);
            var jsonObjectString = JsonConvert.SerializeObject(itemId);
            Frame.Navigate(typeof(ThreadPage), jsonObjectString);
        }

        private void SubForumList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var forumEntity = ((ForumEntity)e.ClickedItem);
            var jsonObjectString = JsonConvert.SerializeObject(forumEntity);
            this.Frame.Navigate(typeof(ThreadListPage), jsonObjectString);
        }

        private async void AddThreadButton_Click(object sender, RoutedEventArgs e)
        {
            ForumEntity itemId = _forumEntity;
            // TODO: Finish native new thread function.
            await Launcher.LaunchUriAsync(new Uri(string.Format(Constants.NEW_THREAD, itemId.ForumId)));
            //Frame.Navigate(typeof (CreateThreadView), itemId);
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Window_SizeChanged;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            ChangeViewTemplate(e.Size.Width);
        }

        private void ChangeViewTemplate(double width)
        {
            ApplicationView currentView = ApplicationView.GetForCurrentView();

            if (currentView.Orientation == ApplicationViewOrientation.Landscape)
            {
                VisualStateManager.GoToState(this, "FullScreen", false);
            }
            else
            {
                VisualStateManager.GoToState(this, width <= 620 ? "Snapped" : "FullScreen", false);
            }
        }

        private void BookmarkSettings_Click(object sender, RoutedEventArgs e)
        {
            var up = new BookmarkSettingsFlyout();
            up.Show();
        }


        private async Task GetForumThreads()
        {
            RefreshBarButton.IsEnabled = false;
            if (_forumEntity.IsBookmarks)
            {
                AddThreadButton.Visibility = Visibility.Collapsed;
                BookmarkSettings.Visibility = Visibility.Visible;
                _forumThreadEntities = await _threadManager.GetBookmarks(_forumEntity, 1);
                _forumPageScrollingCollection = new PageScrollingCollection(_forumEntity, 1);
                foreach (var forumThread in _forumThreadEntities)
                {
                    _forumPageScrollingCollection.Add(forumThread);
                }
                DefaultViewModel["Threads"] = _forumPageScrollingCollection;
                SubForumList.Visibility = Visibility.Collapsed;
                SubForumListSnapped.Visibility = Visibility.Collapsed;
                NotificationButton.Visibility = Visibility.Visible;
                RemoveNotificationsButton.Visibility = Visibility.Visible;
            }
            else
            {
                _forumPageScrollingCollection = new PageScrollingCollection(_forumEntity, 1);
                _forumThreadEntities = await _threadManager.GetForumThreads(_forumEntity, 1);
                foreach (var forumThread in _forumThreadEntities)
                {
                    _forumPageScrollingCollection.Add(forumThread);
                }
                _subForumEntities = await _forumManager.GetSubForums(_forumEntity);
                DefaultViewModel["Threads"] = _forumPageScrollingCollection;
                DefaultViewModel["Subforums"] = _subForumEntities;
            }
            RefreshBarButton.IsEnabled = true;
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
            Rect bounds = Window.Current.Bounds;
            ChangeViewTemplate(bounds.Width);

            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= OnCommandsRequested;
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ForumThreadList_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private async void FavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var threadlist = new List<ForumThreadEntity>();
            if (ForumThreadList.SelectedItems.Any())
            {
                threadlist.AddRange(ForumThreadList.SelectedItems.Cast<ForumThreadEntity>());
            }
            List<long> threadIdList = threadlist.Select(thread => thread.ThreadId).ToList();
            await _threadManager.AddBookmarks(threadIdList);
        }

        private async void UnreadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var threadlist = new List<ForumThreadEntity>();
            if (ForumThreadList.SelectedItems.Any())
            {
                threadlist.AddRange(ForumThreadList.SelectedItems.Cast<ForumThreadEntity>());
            }
            List<long> threadIdList = threadlist.Select(thread => thread.ThreadId).ToList();
            await _threadManager.MarkThreadUnread(threadIdList);
        }

        private void ForumThreadList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                FavoriteButton.IsEnabled = ForumThreadList.SelectedItems.Any();
                UnreadButton.IsEnabled = ForumThreadList.SelectedItems.Any();
                NotificationButton.IsEnabled = ForumThreadList.SelectedItems.Any();
        }

        private void NotificationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var threadlist = new List<ForumThreadEntity>();
            if (ForumThreadList.SelectedItems.Any())
            {
                threadlist.AddRange(ForumThreadList.SelectedItems.Cast<ForumThreadEntity>());
            }
            List<long> threadIdList = threadlist.Select(thread => thread.ThreadId).ToList();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["_threadIds"] = SerializeListToXml(threadIdList);
        }

        public static string SerializeListToXml(List<long> list)
        {
            try
            {
                var xmlIzer = new XmlSerializer(typeof (List<long>));
                var writer = new StringWriter();
                xmlIzer.Serialize(writer, list);
                System.Diagnostics.Debug.WriteLine(writer.ToString());
                return writer.ToString();
            }

            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc);
                return String.Empty;
            }
        }

        private async void RemoveNotificationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["_threadIds"] = string.Empty;
            var msgDlg = new MessageDialog("Thread notifications removed! (ÅEÉ÷ÅE)Ém");
            msgDlg.DefaultCommandIndex = 1;
            await msgDlg.ShowAsync();
        }

        private async void RefreshBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;

            await GetForumThreads();

            loadingProgressBar.Visibility = Visibility.Collapsed;

        }
    }
}