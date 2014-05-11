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
using AwfulMetro.Pcl.Core.Tools;
using AwfulMetro.ViewModels;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainForumsPage : Page
    {
        private NavigationHelper navigationHelper;
        private MainForumsPageViewModel _vm;
        private ThreadListPageViewModel _threadVm;
        private ApplicationDataContainer _localSettings;
        public MainForumsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.navigationHelper = new NavigationHelper(this);
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
            _localSettings = ApplicationData.Current.LocalSettings;
            if (_localSettings.Values.ContainsKey(Constants.BOOKMARK_STARTUP))
            {
                var bookmarks = (bool) _localSettings.Values[Constants.BOOKMARK_STARTUP];
                if (bookmarks)
                {
                    ForumsPivot.SelectedIndex = 1;
                }
            }
            var forum = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty, false);
            if(_threadVm.ForumPageScrollingCollection == null)
            _threadVm.Initialize(forum);
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
            if(_vm == null)
            _vm = (MainForumsPageViewModel)DataContext;
            if(_threadVm == null)
            _threadVm = (ThreadListPageViewModel) BookmarksPivotItem.DataContext;
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var forumEntity = ((ForumEntity)e.ClickedItem);
            string jsonObjectString = JsonConvert.SerializeObject(forumEntity);
            Frame.Navigate(typeof(ThreadListPage), jsonObjectString);
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            var forum = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty, false);
            _threadVm.Initialize(forum);
        }

        private void PrivateMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PrivateMessagePage));
        }

        private void ForumThreadList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var forumThread = ((ForumThreadEntity)e.ClickedItem);
            string jsonObjectString = JsonConvert.SerializeObject(forumThread);
            Frame.Navigate(typeof(ThreadPage), jsonObjectString);
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (SettingsPage));
        }

        private void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            var message = string.Format("Are you sure you want to logout?{0}I mean, I could care less. It's up to you...", Environment.NewLine);
            var msgBox =
                new MessageDialog(message,
                    "Make all your wildest dreams come true...");
            var okButton = new UICommand("Of course!") { Invoked = OkButtonClick };
            var cancelButton = new UICommand("Never!") { Invoked = cancelButtonClick };
            msgBox.Commands.Add(okButton);
            msgBox.Commands.Add(cancelButton);
            msgBox.ShowAsync();
        }

        private async void OkButtonClick(IUICommand command)
        {
            var authenticationManager = new AuthenticationManager();
            var result = await authenticationManager.Logout();
            var localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey(Constants.BOOKMARK_BACKGROUND))
            {
                BackgroundTaskUtils.UnregisterBackgroundTasks(BackgroundTaskUtils.BackgroundTaskName);
                localSettings.Values[Constants.BOOKMARK_BACKGROUND] = false;
            }

            if (result)
            {
                Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                var msgBox =
                new MessageDialog("Could not log you out! You're stuck here forever! HA HA HA!",
                    "Logout error");
                msgBox.ShowAsync();
            }
        }

        private void cancelButtonClick(IUICommand command)
        {
            return;
        }
    }
}
