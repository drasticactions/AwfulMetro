using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using Windows.UI.ViewManagement;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ThreadPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private ForumThreadEntity _forumThread;
        private List<ForumPostEntity> _threadPosts;

        public ThreadPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        }

        //TODO: inject this
        private readonly PostManager _postManager = new PostManager();
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
            _forumThread = (ForumThreadEntity) e.NavigationParameter;
            pageTitle.Text = _forumThread.Name;

            _threadPosts = await _postManager.GetThreadPosts(_forumThread);
            CurrentPageSelector.ItemsSource = Enumerable.Range(1, _forumThread.TotalPages).ToArray();
            CurrentPageSelector.SelectedValue = _forumThread.CurrentPage;
            BackButton.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButton.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;
            ReplyButton.IsEnabled = !_forumThread.IsLocked;
            DefaultViewModel["Posts"] = _threadPosts;
            if (_forumThread.ScrollToPost > 0)
            {

                ThreadListFullScreen.ScrollIntoView(_threadPosts[_forumThread.ScrollToPost]);
            }
            loadingProgressBar.Visibility = Visibility.Collapsed;

            // TODO: Remove duplicate buttons and find a better way to handle navigation
            BackButtonSnap.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButtonSnap.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;
            CurrentPageSelectorSnap.ItemsSource = Enumerable.Range(1, _forumThread.TotalPages).ToArray();
            CurrentPageSelectorSnap.SelectedValue = _forumThread.CurrentPage;
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

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_forumThread.CurrentPage > 1)
            {
                loadingProgressBar.Visibility = Visibility.Visible;
                _forumThread.CurrentPage--;

                // TODO: Remove duplicate buttons and find a better way to handle navigation
                CurrentPageSelectorSnap.SelectedIndex--;
                BackButtonSnap.IsEnabled = _forumThread.CurrentPage > 1;
                ForwardButtonSnap.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;

                CurrentPageSelector.SelectedIndex--;
                BackButton.IsEnabled = _forumThread.CurrentPage > 1;
                ForwardButton.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;
                List<ForumPostEntity> threadPosts = await _postManager.GetThreadPosts(_forumThread);
                DefaultViewModel["Posts"] = threadPosts;
                loadingProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            _forumThread.CurrentPage++;

            // TODO: Remove duplicate buttons and find a better way to handle navigation
            CurrentPageSelectorSnap.SelectedIndex++;
            BackButtonSnap.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButtonSnap.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;

            CurrentPageSelector.SelectedIndex++;
            BackButton.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButton.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;
            _threadPosts = await _postManager.GetThreadPosts(_forumThread);
            DefaultViewModel["Posts"] = _threadPosts;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void CurrentPageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentPageSelector != null && CurrentPageSelector.SelectedValue != null)
            {
                if (_forumThread.CurrentPage != (int) CurrentPageSelector.SelectedValue)
                {
                    loadingProgressBar.Visibility = Visibility.Visible;
                    _forumThread.CurrentPage = (int) CurrentPageSelector.SelectedValue;
                    BackButton.IsEnabled = _forumThread.CurrentPage > 1;
                    ForwardButton.IsEnabled = _forumThread.CurrentPage != _forumThread.TotalPages;
                    List<ForumPostEntity> threadPosts = await _postManager.GetThreadPosts(_forumThread);
                    DefaultViewModel["Posts"] = threadPosts;
                    loadingProgressBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (ReplyView), _forumThread);
        }

        private void UserProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if (button == null) return;
            var forumPost = (ForumPostEntity) button.DataContext;
            Frame.Navigate(typeof (UserProfileView), forumPost.User);
        }

        private void RapSheetButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if (button == null) return;
            var forumPost = (ForumPostEntity) button.DataContext;
            Frame.Navigate(typeof (RapSheetView), forumPost.User.Id);
        }

        private void PostHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if (button == null) return;
            var forumPost = (ForumPostEntity) button.DataContext;
            Frame.Navigate(typeof (UserPostHistoryPage), forumPost.User.Id);
        }

        private void QuoteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if (button != null)
            {
                var forumPost = (ForumPostEntity) button.DataContext;
                Frame.Navigate(typeof (ReplyView), forumPost);
            }
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

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            _threadPosts = await _postManager.GetThreadPosts(_forumThread);
            DefaultViewModel["Posts"] = _threadPosts;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }


        private void ChangeViewTemplate(double width)
        {
            ApplicationView currentView = ApplicationView.GetForCurrentView();
            
            // TODO: Add Portrait View State
            VisualStateManager.GoToState(this,
                currentView.Orientation == ApplicationViewOrientation.Landscape ? "FullScreen" : "Snapped", false);
        }

        private void GoToLastPostButton_Click(object sender, RoutedEventArgs e)
        {
            //ThreadListSnapped.ScrollIntoView(_threadPosts.Last());
            ThreadListFullScreen.ScrollIntoView(_threadPosts.Last());
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
            Rect bounds = Window.Current.Bounds;
            ChangeViewTemplate(bounds.Width);

            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
