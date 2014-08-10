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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
using AwfulMetro.Core.Tools;
using AwfulMetro.Tools;
using AwfulMetro.ViewModels;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ThreadListPage : Page, IDisposable
    {


        private ThreadListPageViewModel _vm = Locator.ViewModels.ThreadListPageVm;
        private readonly ForumManager _forumManager = new ForumManager();
        private readonly NavigationHelper _navigationHelper;
        private readonly ThreadManager _threadManager = new ThreadManager();
        private ForumEntity _forumEntity;

        public ThreadListPage()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
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

        private string ViewStateStringSnapped { get; set; }

        private string ViewStateStringFullScreen { get; set; }
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
            //loadingProgressBar.Visibility = Visibility.Visible;

            var jsonObjectString = (string) e.NavigationParameter;
            _forumEntity = JsonConvert.DeserializeObject<ForumEntity>(jsonObjectString);
            if (_forumEntity == null) return;
            if (_vm.ForumEntity == null || _vm.ForumEntity.ForumId != _forumEntity.ForumId)
                _vm.Initialize(_forumEntity);

            // TODO: This is stupid shit that should be removed.
            ViewStateStringFullScreen = "FullScreen" + GetViewStateString(_forumEntity.ForumId);
            ViewStateStringSnapped = "Snapped" + GetViewStateString(_forumEntity.ForumId);
        }

        private string GetViewStateString(long forumId)
        {
            switch (forumId)
            {
                case 219:
                    return "_YOSPOS";
                case 26:
                    return "_FYAD";
                default:
                    return string.Empty;
            }
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
            var forumThread = ((ForumThreadEntity)e.ClickedItem);
            string jsonObjectString = JsonConvert.SerializeObject(forumThread);
            Frame.Navigate(typeof(ThreadPage), jsonObjectString);
        }

        private void SubForumList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var forumEntity = ((ForumEntity) e.ClickedItem);
            string jsonObjectString = JsonConvert.SerializeObject(forumEntity);
            Frame.Navigate(typeof (ThreadListPage), jsonObjectString);
        }

        private async void AddThreadButton_Click(object sender, RoutedEventArgs e)
        {
            string jsonObjectString = JsonConvert.SerializeObject(_forumEntity);
            Frame.Navigate(typeof(NewThreadView), jsonObjectString);
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
                VisualStateManager.GoToState(this, ViewStateStringFullScreen, false);
            }
            else
            {
                VisualStateManager.GoToState(this, width <= 620 ? ViewStateStringSnapped : ViewStateStringFullScreen, false);
            }
        }

        private void BookmarkSettings_Click(object sender, RoutedEventArgs e)
        {
            var up = new BookmarkSettingsFlyout();
            up.Show();
        }

        private void ForumThreadList_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private async void FavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshBarButton.IsEnabled = false;
            var threadlist = new List<ForumThreadEntity>();
            if (ForumThreadList.SelectedItems.Any())
            {
                threadlist.AddRange(ForumThreadList.SelectedItems.Cast<ForumThreadEntity>());
            }
            List<long> threadIdList = threadlist.Select(thread => thread.ThreadId).ToList();
            await _threadManager.AddBookmarks(threadIdList);
            if (threadIdList.Count == 1)
            {
                var msgDlg =
                    new MessageDialog(string.Format("'{0}' has been bookmarked! Good for you!{1}{2}",
                        threadlist.First().Name, Environment.NewLine, Constants.ASCII_1))
                    {
                        DefaultCommandIndex = 1
                    };
                await msgDlg.ShowAsync();
            }
            else
            {
                string message = string.Format("{0} Bookmarks created!", threadIdList.Count);
                var msgDlg = new MessageDialog(message)
                {
                    DefaultCommandIndex = 1
                };
                await msgDlg.ShowAsync();
            }
            RefreshBarButton.IsEnabled = true;
        }

        private async void UnreadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var threadlist = new List<ForumThreadEntity>();
            RefreshBarButton.IsEnabled = false;
            if (ForumThreadList.SelectedItems.Any())
            {
                threadlist.AddRange(ForumThreadList.SelectedItems.Cast<ForumThreadEntity>());
            }
            List<long> threadIdList = threadlist.Select(thread => thread.ThreadId).ToList();
            await _threadManager.MarkThreadUnread(threadIdList);

            if (threadIdList.Count == 1)
            {
                var msgDlg =
                    new MessageDialog(
                        string.Format("'{0}' is now \"Unread\"! Now go outside and do something productive!{1}{2}",
                            threadlist.First().Name, Environment.NewLine, Constants.ASCII_1))
                    {
                        DefaultCommandIndex = 1
                    };
                await msgDlg.ShowAsync();
            }
            else
            {
                string message = string.Format("{0} are now \"Unread\"! My life now is now complete!!",
                    threadIdList.Count);
                var msgDlg = new MessageDialog(message)
                {
                    DefaultCommandIndex = 1
                };
                await msgDlg.ShowAsync();
            }
            RefreshBarButton.IsEnabled = true;
        }

        private void ForumThreadList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotificationButton.IsEnabled = ForumThreadList.SelectedItems.Any();
        }

        private async void NotificationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var threadlist = new List<ForumThreadEntity>();
            if (ForumThreadList.SelectedItems.Any())
            {
                threadlist.AddRange(ForumThreadList.SelectedItems.Cast<ForumThreadEntity>());
            }
            List<long> threadIdList = threadlist.Select(thread => thread.ThreadId).ToList();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["_threadIds"] = SerializeListToXml(threadIdList);
            var msgDlg = new MessageDialog(string.Format("{0} thread notifications added!", threadIdList.Count))
            {
                DefaultCommandIndex = 1
            };
            await msgDlg.ShowAsync();

        }

        public static string SerializeListToXml(List<long> list)
        {
            try
            {
                var xmlIzer = new XmlSerializer(typeof (List<long>));
                var writer = new StringWriter();
                xmlIzer.Serialize(writer, list);
                Debug.WriteLine(writer.ToString());
                return writer.ToString();
            }

            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                return String.Empty;
            }
        }

        private async void RemoveNotificationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["_threadIds"] = string.Empty;
            var msgDlg = new MessageDialog("Thread notifications removed!");
            msgDlg.DefaultCommandIndex = 1;
            await msgDlg.ShowAsync();
        }

        private void RefreshBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.Initialize(_forumEntity);
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

        private void ReturnToMainButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainForumsPage));
        }

        public void Dispose()
        {
        }
    }
}