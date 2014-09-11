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

using System.Diagnostics;
using Windows.Storage;
using Windows.UI.Popups;
using AwfulMetro.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.Pcl.Core.Manager;
using AwfulMetro.Tools;
using AwfulMetro.ViewModels;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThreadPage : Page
    {
        private NavigationHelper navigationHelper;
        private ThreadViewModel _vm;
        private ForumThreadEntity _forumThread;
        private int _zoomSize;
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        private readonly ThreadManager _threadManager = new ThreadManager();
        public ThreadPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            ThreadWebView.ScriptNotify += WebView_ScriptNotify;
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var jsonObjectString = (string)e.NavigationParameter;
            _forumThread = JsonConvert.DeserializeObject<ForumThreadEntity>(jsonObjectString);
            if (_forumThread == null) return;
            await _vm.GetForumPosts(_forumThread);
        }

        private async void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (_vm.ForumThreadEntity == null)
            {
                return;
            }
            if (e == null)
            {
                return;
            }
            string stringJson = e.Value;
            ThreadCommand command = null;
            try
            {
                command = JsonConvert.DeserializeObject<ThreadCommand>(stringJson);
            }
            catch (Exception ex)
            {
                AwfulDebugger.SendMessageDialogAsync("A thread javascript command failed", ex);
            }
            if (command == null)
            {
                return;
            }
            switch (command.Command)
            {
                case "quote":
                    Frame.Navigate(typeof(ReplyPage), command.Id);
                    break;
                case "edit":
                    Frame.Navigate(typeof(EditPage), command.Id);
                    break;
                case "setFont":
                    SetFontSize();
                    break;
                case "scrollToPost":
                    if (!string.IsNullOrEmpty(_vm.ForumThreadEntity.ScrollToPostString))
                        try
                        {
                            await ThreadWebView.InvokeScriptAsync("ScrollToDiv", new[] { _vm.ForumThreadEntity.ScrollToPostString });
                        }
                        catch (Exception ex)
                        {
                            AwfulDebugger.SendMessageDialogAsync("A thread javascript command failed", ex);
                        }
                    break;
                case "markAsLastRead":
                    await _threadManager.MarkPostAsLastRead(_forumThread, Convert.ToInt32(command.Id));
                    int nextPost = Convert.ToInt32(command.Id) + 1;

                    try
                    {
                        await ThreadWebView.InvokeScriptAsync("ScrollToDiv", new[] { string.Concat("#postId", nextPost.ToString()) });
                    }
                    catch (Exception ex)
                    {
                        AwfulDebugger.SendMessageDialogAsync("A thread javascript command failed", ex);
                        return;
                    }
                    var message = new MessageDialog("Post marked as last read! Now go on and live your life!")
                        {
                            DefaultCommandIndex = 1
                        };
                    await message.ShowAsync();
                    break;
                case "openThread":
                      var query = Extensions.ParseQueryString(command.Id);
                    if (query.ContainsKey("action") && query["action"].Equals("showPost"))
                    {
                        var postManager = new PostManager();
                        try
                        {
                            var html = await postManager.GetPost(Convert.ToInt32(query["postid"]));
                        }
                        catch (Exception ex)
                        {
                            AwfulDebugger.SendMessageDialogAsync("A thread javascript command failed", ex);
                            return;
                        }
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

        private void SetFontSize()
        {
            if (_localSettings.Values.ContainsKey("zoomSize"))
            {
                _zoomSize = Convert.ToInt32(_localSettings.Values["zoomSize"]);
                try
                {
                    ThreadWebView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
                }
                catch (Exception ex)
                {
                    AwfulDebugger.SendMessageDialogAsync("A thread javascript command failed", ex);
                }
            }
            else
            {
                _zoomSize = 14;
            }
        }

        public class ThreadCommand
        {
            public string Command { get; set; }
            public string Id { get; set; }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _vm = (ThreadViewModel)DataContext;
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
           await _vm.GetForumPosts(_vm.ForumThreadEntity);
        }

        private async void ThreadWebView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            try
            {
                SetFontSize();
                if (_forumThread != null && _forumThread.ScrollToPost > 0)
                {
                    try
                    {
                        await ThreadWebView.InvokeScriptAsync("ScrollToDiv", new[] { _forumThread.ScrollToPostString });
                    }
                    catch (Exception ex)
                    {
                        AwfulDebugger.SendMessageDialogAsync("A thread javascript command failed", ex);
                    }
                }
                _vm.IsLoading = false;
            }
            catch (Exception)
            {
            }
        }

        private async void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm.ForumThreadEntity == null)
            {
                await AwfulDebugger.SendMessageDialogAsync("Can't reply in this thread, refresh and try again.", new Exception("ForumThreadEntitiy is Null"));
                return;
            }
            if (_vm.ForumThreadEntity.CurrentPage <= 1) return;
            _vm.ForumThreadEntity.CurrentPage--;
            _vm.GetForumPosts(_vm.ForumThreadEntity);
        }

        private async void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm.ForumThreadEntity == null)
            {
                await AwfulDebugger.SendMessageDialogAsync("Can't reply in this thread, refresh and try again.", new Exception("ForumThreadEntitiy is Null"));
                return;
            }
            if (_vm.ForumThreadEntity.CurrentPage >= _vm.ForumThreadEntity.TotalPages) return;
            _vm.ForumThreadEntity.CurrentPage++;
            _vm.GetForumPosts(_vm.ForumThreadEntity);

        }

        private void ScrollToLastPostButton_OnClick(object sender, RoutedEventArgs e)
        {
            ThreadWebView.InvokeScriptAsync("ScrollToBottom", null);
        }

        private async void ReplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm.ForumThreadEntity == null)
            {
                await AwfulDebugger.SendMessageDialogAsync("Can't reply in this thread, refresh and try again.", new Exception("ForumThreadEntitiy is Null"));
                return;
            }
            string jsonObjectString = JsonConvert.SerializeObject(_forumThread);
            Frame.Navigate(typeof(ReplyPage), jsonObjectString);
        }

        private void PageNumberButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm.ForumThreadEntity == null) return;
            var userInputPageNumber = 0;
            try
            {
                userInputPageNumber = Convert.ToInt32(PageNumberTextBox.Text);
            }
            catch (Exception)
            {
                Debug.WriteLine("User Inputted invalid value. Ignore the error.");
            }

            if (userInputPageNumber < 1 || userInputPageNumber > _vm.ForumThreadEntity.TotalPages) return;
            if (CurrentPageButton.Flyout != null) CurrentPageButton.Flyout.Hide();
            _vm.ForumThreadEntity.CurrentPage = userInputPageNumber;
            _vm.GetForumPosts(_vm.ForumThreadEntity);
        }

        private void JumpToLastPageButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm.ForumThreadEntity == null) return;
            _vm.ForumThreadEntity.CurrentPage = _vm.ForumThreadEntity.TotalPages;
            _vm.GetForumPosts(_vm.ForumThreadEntity);
        }

        private void FontIncrease_Click(object sender, RoutedEventArgs e)
        {
            _zoomSize += 1;
            ThreadWebView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
            _localSettings.Values["zoomSize"] = _zoomSize;
        }

        private void FontDecrease_Click(object sender, RoutedEventArgs e)
        {
            _zoomSize -= 1;
            ThreadWebView.InvokeScriptAsync("ResizeWebviewFont", new[] { _zoomSize.ToString() });
            _localSettings.Values["zoomSize"] = _zoomSize;
        }

        private void RemoveStyle_Click(object sender, RoutedEventArgs e)
        {
            _zoomSize = 14;
            ThreadWebView.InvokeScriptAsync("RemoveCustomStyle", null);
            _localSettings.Values["zoomSize"] = null;
        }
    }
}
