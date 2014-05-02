﻿using Windows.UI.Popups;
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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var jsonObjectString = (string) e.NavigationParameter;
            _forumThread = JsonConvert.DeserializeObject<ForumThreadEntity>(jsonObjectString);
            if (_forumThread == null) return;
            _vm.GetForumPosts(_forumThread);
        }

        private async void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string stringJson = e.Value;
            var command = JsonConvert.DeserializeObject<ThreadCommand>(stringJson);
            switch (command.Command)
            {
                case "quote":
                    Frame.Navigate(typeof(ReplyPage), command.Id);
                    break;
                case "edit":
                    //Frame.Navigate(typeof(EditReplyPage), command.Id);
                    break;
                case "setFont":
                    break;
                case "scrollToPost":
                    if (!string.IsNullOrEmpty(_vm.ForumThreadEntity.ScrollToPostString))
                    await ThreadWebView.InvokeScriptAsync("ScrollToDiv", new[] { _vm.ForumThreadEntity.ScrollToPostString });
                    break;
                case "markAsLastRead":
                    await _threadManager.MarkPostAsLastRead(_forumThread, Convert.ToInt32(command.Id));
                    int nextPost = Convert.ToInt32(command.Id) + 1;
                    await ThreadWebView.InvokeScriptAsync("ScrollToDiv", new[] { string.Concat("#postId", nextPost.ToString()) });
                    var message = new MessageDialog("Post marked as last read! Now go on and live your life!")
                        {
                            DefaultCommandIndex = 1
                        };
                    await message.ShowAsync();
                    break;
                case "openThread":
                    // Because we are coming from an existing thread, rather than the thread lists, we need to get the thread information beforehand.
                    // However, right now the managers are not set up to support this. The thread is getting downloaded twice, when it really only needs to happen once.
                    var threadManager = new ThreadManager();
                    var thread = await threadManager.GetThread(new ForumThreadEntity(), command.Id);
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
            _vm = (ThreadViewModel) DataContext;
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.GetForumPosts(_forumThread);
        }

        private async void ThreadWebView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            try
            {
                if (_forumThread.ScrollToPost > 0)
                {
                    await ThreadWebView.InvokeScriptAsync("ScrollToDiv", new[] { _forumThread.ScrollToPostString });
                }
            }
            catch (Exception)
            {
            }
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm.ForumThreadEntity.CurrentPage <= 1) return;
            _vm.ForumThreadEntity.CurrentPage--;
            _vm.GetForumPosts(_vm.ForumThreadEntity);
        }

        private void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_vm.ForumThreadEntity.CurrentPage >= _vm.ForumThreadEntity.TotalPages) return;
            _vm.ForumThreadEntity.CurrentPage++;
            _vm.GetForumPosts(_vm.ForumThreadEntity);

        }

        private void ScrollToLastPostButton_OnClick(object sender, RoutedEventArgs e)
        {
            ThreadWebView.InvokeScriptAsync("ScrollToBottom", null);
        }

        private void ReplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            string jsonObjectString = JsonConvert.SerializeObject(_forumThread);
            Frame.Navigate(typeof(ReplyPage), jsonObjectString);
        }
    }
}
