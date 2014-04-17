// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Manager;
using AwfulMetro.ViewModels;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ThreadPage : Page
    {
        private ThreadViewModel _vm;
        private readonly NavigationHelper _navigationHelper;
        private readonly PostManager _postManager = new PostManager();
        private readonly ThreadManager _threadManager = new ThreadManager();
        private ForumThreadEntity _forumThread;
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private int _zoomSize;
        public ThreadPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            ThreadFullView.ScriptNotify += WebView_ScriptNotify;
            ThreadSnapView.ScriptNotify += WebView_ScriptNotify;
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
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
                    Frame.Navigate(typeof (UserProfileView), command.Id);
                    break;
                case "post_history":
                    Frame.Navigate(typeof (UserPostHistoryPage), command.Id);
                    break;
                case "rap_sheet":
                    Frame.Navigate(typeof (RapSheetView), command.Id);
                    break;
                case "quote":
                    Frame.Navigate(typeof (ReplyView), command.Id);
                    break;
                case "edit":
                    Frame.Navigate(typeof (EditReplyPage), command.Id);
                    break;
                case "markAsLastRead":
                    await _threadManager.MarkPostAsLastRead(_forumThread, Convert.ToInt32(command.Id));
                    int nextPost = Convert.ToInt32(command.Id) + 1;
                    await ThreadFullView.InvokeScriptAsync("ScrollToTable", new[] { nextPost.ToString() });
                    await ThreadSnapView.InvokeScriptAsync("ScrollToTable", new[] { nextPost.ToString() });
                    NotifyStatusTile.CreateToastNotification("Post marked as last read! Now smash this computer and live your life!");
                    break;
                case "setFont":
                    if (_localSettings.Values.ContainsKey("zoomSize"))
                    {
                        _zoomSize = Convert.ToInt32(_localSettings.Values["zoomSize"]);
                        ThreadFullView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
                        ThreadSnapView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
                    }
                    else
                    {
                        _zoomSize = 14;
                    }
                    break;
                case "openThread":
                    // Because we are coming from an existing thread, rather than the thread lists, we need to get the thread information beforehand.
                    // However, right now the managers are not set up to support this. The thread is getting downloaded twice, when it really only needs to happen once.
                    var threadManager = new ThreadManager();
                    var thread =  await threadManager.GetThread(command.Id);
                    if (thread == null)
                    {
                        var error = new MessageDialog("Specified post was not found in the live forums.")
                        {
                            DefaultCommandIndex = 1
                        };
                        await error.ShowAsync();
                        break;
                    }
                    string jsonObjectString = JsonConvert.SerializeObject(thread);
                    Frame.Navigate(typeof (ThreadPage), jsonObjectString);
                    break;
                default:
                    var msgDlg = new MessageDialog("Not working yet!")
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
            var jsonObjectString = (string) e.NavigationParameter;
            _forumThread = JsonConvert.DeserializeObject<ForumThreadEntity>(jsonObjectString);
            if (_forumThread == null) return;
            _vm.GetForumPosts(_forumThread);
            CurrentPageSelector.ItemsSource = Enumerable.Range(1, _forumThread.TotalPages).ToArray();
            CurrentPageSelector.SelectedValue = _forumThread.CurrentPage;
            BackButton.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButton.IsEnabled = _forumThread.TotalPages != _forumThread.CurrentPage;
            ReplyButton.IsEnabled = !_forumThread.IsLocked;
            ReplyButtonSnap.IsEnabled = !_forumThread.IsLocked;

            // Set the default focus on the page to either one of the web views.
            CheckOrientation();

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_forumThread.CurrentPage <= 1) return;
            // TODO: Remove duplicate buttons and find a better way to handle navigation
            CurrentPageSelector.SelectedIndex--;
            CurrentPageSelectorSnap.SelectedIndex--;
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Remove duplicate buttons and find a better way to handle navigation
            if (_forumThread.CurrentPage == _forumThread.TotalPages) return;
            CurrentPageSelector.SelectedIndex++;
            CurrentPageSelectorSnap.SelectedIndex++;
        }

        private async void CurrentPageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentPageSelector == null || CurrentPageSelector.SelectedValue == null) return;
            if (_forumThread.CurrentPage == (int) CurrentPageSelector.SelectedValue) return;
            loadingProgressBar.Visibility = Visibility.Visible;
            _forumThread.CurrentPage = (int) CurrentPageSelector.SelectedValue;
            BackButton.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButton.IsEnabled = _forumThread.CurrentPage != _forumThread.TotalPages;
            BackButtonSnap.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButtonSnap.IsEnabled = _forumThread.CurrentPage != _forumThread.TotalPages;
            _forumThread.ScrollToPost = 1;
            _forumThread.ScrollToPostString = "#pti1";
            string html = await _postManager.GetThreadPostInformation(_forumThread);
            ThreadFullView.NavigateToString(html);
            ThreadSnapView.NavigateToString(html);
            CurrentPageSelectorSnap.SelectedValue = CurrentPageSelector.SelectedValue;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void CurrentPageSelectorSnap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentPageSelectorSnap == null || CurrentPageSelectorSnap.SelectedValue == null) return;
            if (_forumThread.CurrentPage == (int) CurrentPageSelectorSnap.SelectedValue) return;
            loadingProgressBar.Visibility = Visibility.Visible;
            _forumThread.CurrentPage = (int) CurrentPageSelectorSnap.SelectedValue;
            BackButton.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButton.IsEnabled = _forumThread.CurrentPage != _forumThread.TotalPages;
            BackButtonSnap.IsEnabled = _forumThread.CurrentPage > 1;
            ForwardButtonSnap.IsEnabled = _forumThread.CurrentPage != _forumThread.TotalPages;
            _forumThread.ScrollToPost = 1;
            _forumThread.ScrollToPostString = "#pti1";
            string html = await _postManager.GetThreadPostInformation(_forumThread);
            ThreadFullView.NavigateToString(html);
            ThreadSnapView.NavigateToString(html);
            CurrentPageSelector.SelectedValue = CurrentPageSelectorSnap.SelectedValue;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            string jsonObjectString = JsonConvert.SerializeObject(_forumThread);
            Frame.Navigate(typeof (ReplyView), jsonObjectString);
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
            string html = await _postManager.GetThreadPostInformation(_forumThread);
            ThreadFullView.NavigateToString(html);
            ThreadSnapView.NavigateToString(html);
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void CheckOrientation()
        {
            ApplicationView currentView = ApplicationView.GetForCurrentView();
            if (currentView.Orientation == ApplicationViewOrientation.Landscape)
            {
                ThreadFullView.Focus(FocusState.Programmatic);
            }
            else
            {
                ThreadSnapView.Focus(FocusState.Programmatic);
            }
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

        private async void BookmarkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var threadIdList = new List<long> {_forumThread.ThreadId};
            await _threadManager.AddBookmarks(threadIdList);
        }

        private async void ThreadFullView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            try
            {
                if (_forumThread.ScrollToPost > 0)
                {
                    await ThreadFullView.InvokeScriptAsync("ScrollToDiv", new[] {_forumThread.ScrollToPostString});
                }
            }
            catch (Exception)
            {
            }
        }

        private async void ThreadSnapView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            try
            {
                if (_forumThread.ScrollToPost > 0)
                {
                    await ThreadSnapView.InvokeScriptAsync("ScrollToDiv", new[] {_forumThread.ScrollToPostString});
                }
            }
            catch (Exception)
            {
            }
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
            _vm = (ThreadViewModel) DataContext;
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

        private async void ReturnToParentForum_OnClick(object sender, RoutedEventArgs e)
        {
            var msgDlg = new MessageDialog("Not working yet!")
            {
                DefaultCommandIndex = 1
            };
            await msgDlg.ShowAsync();
        }

        private void FontIncrease_Click(object sender, RoutedEventArgs e)
        {
            _zoomSize += 1;
            ThreadFullView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
            ThreadSnapView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
            _localSettings.Values["zoomSize"] = _zoomSize;
        }

        private void FontDecrease_Click(object sender, RoutedEventArgs e)
        {
            _zoomSize -= 1;
            ThreadFullView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
            ThreadSnapView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
            _localSettings.Values["zoomSize"] = _zoomSize;
        }

        private void RemoveStyle_Click(object sender, RoutedEventArgs e)
        {
            _zoomSize = 14;
            ThreadFullView.InvokeScriptAsync("RemoveCustomStyle", null);
            ThreadSnapView.InvokeScriptAsync("RemoveCustomStyle", null);
            _localSettings.Values["zoomSize"] = null;
        }
    }
}