using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using Windows.UI.ViewManagement;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ThreadPage : Page
    {
        private readonly NavigationHelper _navigationHelper;
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly ThreadManager _threadManager = new ThreadManager();
        private ForumThreadEntity _forumThread;
        private List<ForumPostEntity> _threadPosts;

        public ThreadPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            ThreadFullView.ScriptNotify += WebView_ScriptNotify;
            ThreadSnapView.ScriptNotify += WebView_ScriptNotify;
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

        private async void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string stringJson = e.Value;
            var command = JsonConvert.DeserializeObject<ReplyView.ThreadCommand>(stringJson);
            switch (command.Command)
            {
                case "profile":
                    Frame.Navigate(typeof(UserProfileView), command.Id);
                    break;
                case "post_history":
                    Frame.Navigate(typeof(UserPostHistoryPage), command.Id);
                    break;
                case "rap_sheet":
                    Frame.Navigate(typeof(RapSheetView), command.Id);
                    break;
                case "quote":
                    Frame.Navigate(typeof(ReplyView),  command.Id);
                    break;
                case "edit":
                    Frame.Navigate(typeof(EditReplyPage),  command.Id);
                    break;
                default:
                    var msgDlg = new MessageDialog("Working on it!")
                    {
                        DefaultCommandIndex = 1
                    };
                    await msgDlg.ShowAsync();
                    break;
            }
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
            var jsonObjectString = (string)e.NavigationParameter;
            _forumThread = JsonConvert.DeserializeObject<ForumThreadEntity>(jsonObjectString);
            if (_forumThread == null) return;
            pageTitle.Text = _forumThread.Name;
            var html = await _postManager.GetThreadPostInformation(_forumThread);
            ThreadFullView.NavigateToString(html);
            ThreadSnapView.NavigateToString(html);
            CurrentPageSelector.ItemsSource = Enumerable.Range(1, _forumThread.TotalPages).ToArray();
            CurrentPageSelector.SelectedValue = _forumThread.CurrentPage;
            BackButton.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButton.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;
            ReplyButton.IsEnabled = !_forumThread.IsLocked;
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
            if (_forumThread.CurrentPage <= 1) return;
            loadingProgressBar.Visibility = Visibility.Visible;
            _forumThread.CurrentPage--;

            // TODO: Remove duplicate buttons and find a better way to handle navigation
            CurrentPageSelectorSnap.SelectedIndex--;
            BackButtonSnap.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButtonSnap.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;

            CurrentPageSelector.SelectedIndex--;
            BackButton.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButton.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;
            var html = await _postManager.GetThreadPostInformation(_forumThread);
            ThreadFullView.NavigateToString(html);
            ThreadSnapView.NavigateToString(html);
            loadingProgressBar.Visibility = Visibility.Collapsed;
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
            var html = await _postManager.GetThreadPostInformation(_forumThread);
            ThreadFullView.NavigateToString(html);
            ThreadSnapView.NavigateToString(html);
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void CurrentPageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentPageSelector == null || CurrentPageSelector.SelectedValue == null) return;
            if (_forumThread.CurrentPage == (int) CurrentPageSelector.SelectedValue) return;
            loadingProgressBar.Visibility = Visibility.Visible;
            _forumThread.CurrentPage = (int) CurrentPageSelector.SelectedValue;
            BackButton.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButton.IsEnabled = _forumThread.CurrentPage != _forumThread.TotalPages;
            var html = await _postManager.GetThreadPostInformation(_forumThread);
            ThreadFullView.NavigateToString(html);
            ThreadSnapView.NavigateToString(html);
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            var jsonObjectString = JsonConvert.SerializeObject(_forumThread);
            Frame.Navigate(typeof(ReplyView), jsonObjectString);
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
            var html = await _postManager.GetThreadPostInformation(_forumThread);
            ThreadFullView.NavigateToString(html);
            ThreadSnapView.NavigateToString(html);
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }


        private void ChangeViewTemplate(double width)
        {
            ApplicationView currentView = ApplicationView.GetForCurrentView();
            
            // TODO: Add Portrait View State
            VisualStateManager.GoToState(this,
                currentView.Orientation == ApplicationViewOrientation.Landscape ? "FullScreen" : "Snapped", false);
        }

        private async void GoToLastPostButton_Click(object sender, RoutedEventArgs e)
        {
            await ThreadFullView.InvokeScriptAsync("ScrollToBottom", null);
            await ThreadSnapView.InvokeScriptAsync("ScrollToBottom", null);
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

        private async void BookmarkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var threadIdList = new List<long> {_forumThread.ThreadId};
            await _threadManager.AddBookmarks(threadIdList);
        }

        private async void ThreadFullView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (_forumThread.ScrollToPost > 0)
            {
                await ThreadFullView.InvokeScriptAsync("ScrollToDiv", new[] { _forumThread.ScrollToPostString });
            }
        }

        private async void ThreadSnapView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (_forumThread.ScrollToPost > 0)
            {
                await ThreadSnapView.InvokeScriptAsync("ScrollToDiv", new[] {_forumThread.ScrollToPostString});
            }
        }
    }
}
