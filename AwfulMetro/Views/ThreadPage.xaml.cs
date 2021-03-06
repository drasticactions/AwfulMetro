﻿#region copyright
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
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
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Entity;
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
        private ThreadViewModel _vm = Locator.ViewModels.ThreadVm;
        private readonly NavigationHelper _navigationHelper;
        private readonly PostManager _postManager = new PostManager();
        private readonly ThreadManager _threadManager = new ThreadManager();
        private ForumThreadEntity _forumThread;
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
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
            try
            {
                string stringJson = e.Value;
                var command = JsonConvert.DeserializeObject<ReplyView.ThreadCommand>(stringJson);
                switch (command.Command)
                {
                    case "profile":
                        Frame.Navigate(typeof(UserProfileView), command.Id);
                        break;
                    case "openPost":
                        break;
                    case "post_history":
                        Frame.Navigate(typeof(UserPostHistoryPage), command.Id);
                        break;
                    case "rap_sheet":
                        Frame.Navigate(typeof(RapSheetView), command.Id);
                        break;
                    case "quote":
                        Frame.Navigate(typeof(ReplyView), command.Id);
                        break;
                    case "edit":
                        Frame.Navigate(typeof(EditReplyPage), command.Id);
                        break;
                    case "scrollToPost":
                        if (command.Id != null)
                        {
                            await ThreadFullView.InvokeScriptAsync("ScrollToDiv", new[] { string.Concat("#postId", command.Id) });
                            await ThreadSnapView.InvokeScriptAsync("ScrollToDiv", new[] { string.Concat("#postId", command.Id) });
                        }
                        else if (!string.IsNullOrEmpty(_vm.ForumThreadEntity.ScrollToPostString))
                        {
                            ThreadFullView.InvokeScriptAsync("ScrollToDiv", new[] { _vm.ForumThreadEntity.ScrollToPostString });
                            ThreadSnapView.InvokeScriptAsync("ScrollToDiv", new[] { _vm.ForumThreadEntity.ScrollToPostString });
                        }
                        break;
                    case "markAsLastRead":
                        await _threadManager.MarkPostAsLastRead(_forumThread, Convert.ToInt32(command.Id));
                        int nextPost = Convert.ToInt32(command.Id) + 1;
                        await ThreadFullView.InvokeScriptAsync("ScrollToDiv", new[] { string.Concat("#postId", nextPost.ToString()) });
                        await ThreadSnapView.InvokeScriptAsync("ScrollToDiv", new[] { string.Concat("#postId", nextPost.ToString()) });
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
                            // _zoomSize = 20;
                        }
                        break;
                    case "openThread":
                        var query = Extensions.ParseQueryString(command.Id);
                        if (query.ContainsKey("action") && query["action"].Equals("showPost"))
                        {
                            var postManager = new PostManager();
                            var html = await postManager.GetPost(Convert.ToInt32(query["postid"]));
                            return;
                        }
                        var threadManager = new ThreadManager();
                        var threadEntity = new ForumThreadEntity();
                        var thread = await threadManager.GetThread(threadEntity, command.Id);
                        if (thread == null)
                        {
                            var error = new MessageDialog("Specified post was not found in the live forums.")
                            {
                                DefaultCommandIndex = 1
                            };
                            await error.ShowAsync();
                            break;
                        }
                        string jsonObjectString = JsonConvert.SerializeObject(threadEntity);
                        Frame.Navigate(typeof(ThreadPage), jsonObjectString);
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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
            var jsonObjectString = (string)e.NavigationParameter;
            _forumThread = JsonConvert.DeserializeObject<ForumThreadEntity>(jsonObjectString);
            if (_forumThread == null) return;
            await _vm.GetForumPosts(_forumThread);

            // Set the default focus on the page to either one of the web views.
            CheckOrientation();

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
            if (_vm.ForumThreadEntity.CurrentPage <= 1) return;
            _vm.ForumThreadEntity.CurrentPage--;
            _vm.ForumThreadEntity.ScrollToPost = 0;
            _vm.ForumThreadEntity.ScrollToPostString = string.Empty;
            _vm.GetForumPosts(_vm.ForumThreadEntity);
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.ForumThreadEntity.CurrentPage >= _vm.ForumThreadEntity.TotalPages) return;
            _vm.ForumThreadEntity.CurrentPage++;
            _vm.ForumThreadEntity.ScrollToPost = 0;
            _vm.ForumThreadEntity.ScrollToPostString = string.Empty;
            _vm.GetForumPosts(_vm.ForumThreadEntity);
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
            await _vm.GetForumPosts(_vm.ForumThreadEntity);
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
            try
            {
                await ThreadFullView.InvokeScriptAsync("ScrollToBottom", null);
                await ThreadSnapView.InvokeScriptAsync("ScrollToBottom", null);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
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
                _vm.IsLoading = false;
                if (_forumThread.ScrollToPost > 0)
                {
                    await ThreadFullView.InvokeScriptAsync("ScrollToDiv", new[] {_forumThread.ScrollToPostString});
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Webview Failer {0}", ex);
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
            catch (Exception ex)
            {
                Debug.WriteLine("Webview Failer {0}", ex);
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
            _vm.Html = string.Empty;
            _forumThread = Locator.ViewModels.ThreadVm.ForumThreadEntity;
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

        private async void FontIncrease_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _zoomSize += 1;
                await ThreadFullView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
                await ThreadSnapView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
                _localSettings.Values["zoomSize"] = _zoomSize;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private async void FontDecrease_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _zoomSize -= 1;
                await ThreadFullView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
                await ThreadSnapView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
                _localSettings.Values["zoomSize"] = _zoomSize;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private async void RemoveStyle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _zoomSize = 14;
                await ThreadFullView.InvokeScriptAsync("RemoveCustomStyle", null);
                await ThreadSnapView.InvokeScriptAsync("RemoveCustomStyle", null);
                _localSettings.Values["zoomSize"] = null;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private void PageNumberButton_OnClick(object sender, RoutedEventArgs e)
        {
            ChangePage();
        }

        private void PageNumberTextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ChangePage();
            }
        }

        private async void ChangePage()
        {
            int userInputPageNumber = 0;
            try
            {
                // TODO: This shit needs to be removed. It should be a user control / command, not crap on the view.
                // That could be said for all of this code... but still. One step at a time.
                string test = PageNumberTextBox.Text;
                if (string.IsNullOrEmpty(PageNumberTextBox.Text))
                {
                    test = PageNumberTextBox2.Text;
                }
                userInputPageNumber = Convert.ToInt32(test);
            }
            catch (Exception)
            {
                Debug.WriteLine("User Inputted invalid value. Ignore the error.");
            }

            if (userInputPageNumber < 1 || userInputPageNumber > _vm.ForumThreadEntity.TotalPages) return;
            if (CurrentPageButton.Flyout != null) CurrentPageButton.Flyout.Hide();
            _vm.ForumThreadEntity.CurrentPage = userInputPageNumber;
            _vm.ForumThreadEntity.ScrollToPost = 0;
            _vm.ForumThreadEntity.ScrollToPostString = string.Empty;
            await _vm.GetForumPosts(_vm.ForumThreadEntity);
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ThreadListPage));
        }
    }
}