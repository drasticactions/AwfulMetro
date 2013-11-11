using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ThreadListPage : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper _navigationHelper;
        private readonly ThreadManager _threadManager = new ThreadManager();
        private ForumEntity _forumCategory;
        private ForumCollectionEntity _forumThreadList;

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
            loadingProgressBar.Visibility = Visibility.Visible;
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]

            _forumCategory = (ForumEntity) e.NavigationParameter;
            BackButton.IsEnabled = _forumCategory.CurrentPage > 1 ? true : false;
            //CurrentPage.Text = _forumCategory.CurrentPage.ToString();

            pageTitle.Text = _forumCategory.Name;
            pageSnapTitle.Text = _forumCategory.Name;
            await GetForumThreads();

            CurrentPageSelector.ItemsSource = Enumerable.Range(1, _forumCategory.TotalPages).ToArray();
            CurrentPageSelector.SelectedValue = _forumCategory.CurrentPage;
            DefaultViewModel["Groups"] = _forumThreadList.ForumType;
            DefaultViewModel["Threads"] = _forumThreadList.ForumThreadList;
            DefaultViewModel["Subforums"] = _forumThreadList.ForumSubcategoryList;
            if (!_forumThreadList.ForumSubcategoryList.Any())
            {
                ThreadViewSnapped.RowDefinitions[1].Height = new GridLength(0);
                ThreadViewFullScreen.ColumnDefinitions[0].Width = new GridLength(0);
            }
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
            if (_forumCategory.IsBookmarks)
            {
                var bookmarksCommand = new SettingsCommand("bookmarkSettings", "Bookmarks",
                    handler =>
                    {
                        var up = new BookmarkSettingsFlyout();
                        up.Show();
                    });
                e.Request.ApplicationCommands.Add(bookmarksCommand);
            }
        }

        private void ForumThreadList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((ForumThreadEntity) e.ClickedItem);
            Frame.Navigate(typeof (ThreadPage), itemId);
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_forumCategory.CurrentPage > 1)
            {
                loadingProgressBar.Visibility = Visibility.Visible;
                _forumCategory.CurrentPage--;
                CurrentPageSelector.SelectedValue = _forumCategory.CurrentPage;
                _forumThreadList = await _threadManager.GetForumThreadsAndSubforums(_forumCategory);
                DefaultViewModel["Groups"] = _forumThreadList.ForumType;
                DefaultViewModel["Threads"] = _forumThreadList.ForumThreadList;
                DefaultViewModel["Subforums"] = _forumThreadList.ForumSubcategoryList;
                loadingProgressBar.Visibility = Visibility.Collapsed;
            }
            BackButton.IsEnabled = _forumCategory.CurrentPage > 1 ? true : false;
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            _forumCategory.CurrentPage++;
            CurrentPageSelector.SelectedValue = _forumCategory.CurrentPage;
            BackButton.IsEnabled = _forumCategory.CurrentPage > 1 ? true : false;
            ForwardButton.IsEnabled = _forumCategory.CurrentPage != _forumCategory.TotalPages ? true : false;
            _forumThreadList = await _threadManager.GetForumThreadsAndSubforums(_forumCategory);
            DefaultViewModel["Groups"] = _forumThreadList.ForumType;
            DefaultViewModel["Threads"] = _forumThreadList.ForumThreadList;
            DefaultViewModel["Subforums"] = _forumThreadList.ForumSubcategoryList;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void CurrentPageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentPageSelector != null && CurrentPageSelector.SelectedValue != null)
            {
                var currentPageValue = (int) CurrentPageSelector.SelectedValue;
                if (currentPageValue != _forumCategory.CurrentPage)
                {
                    loadingProgressBar.Visibility = Visibility.Visible;
                    _forumCategory.CurrentPage = (int) CurrentPageSelector.SelectedValue;
                    BackButton.IsEnabled = _forumCategory.CurrentPage > 1 ? true : false;
                    ForwardButton.IsEnabled = _forumCategory.CurrentPage != _forumCategory.TotalPages ? true : false;
                    _forumThreadList = await _threadManager.GetForumThreadsAndSubforums(_forumCategory);
                    DefaultViewModel["Groups"] = _forumThreadList.ForumType;
                    DefaultViewModel["Threads"] = _forumThreadList.ForumThreadList;
                    DefaultViewModel["Subforums"] = _forumThreadList.ForumSubcategoryList;
                    loadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SubForumList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((ForumEntity) e.ClickedItem);
            Frame.Navigate(typeof (ThreadListPage), itemId);
        }

        private void AddThreadButton_Click(object sender, RoutedEventArgs e)
        {
            ForumEntity itemId = _forumCategory;
            Frame.Navigate(typeof (CreateThreadView), itemId);
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
                if (width <= 620)
                {
                    VisualStateManager.GoToState(this, "Snapped", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Portrait", false);
                }
            }
        }

        private void BookmarkSettings_Click(object sender, RoutedEventArgs e)
        {
            var up = new BookmarkSettingsFlyout();
            up.Show();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await GetForumThreads();
        }

        private async Task GetForumThreads()
        {
            if (_forumCategory.IsBookmarks)
            {
                AddThreadButton.Visibility = Visibility.Collapsed;
                BookmarkSettings.Visibility = Visibility.Visible;
                ForwardButton.Visibility = Visibility.Collapsed;
                BackButton.Visibility = Visibility.Collapsed;
                _forumThreadList = await _threadManager.GetBookmarks(_forumCategory);
            }
            else
            {
                // TODO: Add/Refactor function to support page specific loads.
                _forumThreadList = await _threadManager.GetForumThreadsAndSubforums(_forumCategory);
            }

            DefaultViewModel["Groups"] = _forumThreadList.ForumType;
            DefaultViewModel["Threads"] = _forumThreadList.ForumThreadList;
            DefaultViewModel["Subforums"] = _forumThreadList.ForumSubcategoryList;
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="GridCS.Common.NavigationHelper.LoadState" />
        /// and
        /// <see cref="GridCS.Common.NavigationHelper.SaveState" />
        /// .
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
            throw new NotImplementedException();
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

        private void UnreadButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ForumThreadList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                FavoriteButton.IsEnabled = ForumThreadList.SelectedItems.Any();
                UnreadButton.IsEnabled = ForumThreadList.SelectedItems.Any();
        }
    }
}