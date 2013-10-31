using AwfulMetro.Common;
using BusinessObjects.Entity;
using BusinessObjects.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace AwfulMetro.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ThreadListPage : Page
    {

        private NavigationHelper navigationHelper;
        ForumCollectionEntity forumThreadList;
        ForumEntity forumCategory;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public ThreadListPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
            forumCategory = (ForumEntity)e.NavigationParameter;
            BackButton.IsEnabled = forumCategory.CurrentPage > 1 ? true : false;
            //CurrentPage.Text = forumCategory.CurrentPage.ToString();
            
            pageTitle.Text = forumCategory.Name;
            pageSnapTitle.Text = forumCategory.Name;
            if(forumCategory.IsBookmarks)
            {
                BookmarkSettings.Visibility = Windows.UI.Xaml.Visibility.Visible;
                forumThreadList = await ThreadManager.GetBookmarks(forumCategory);
            }
            else
            {
                forumThreadList = await ThreadManager.GetForumThreadsAndSubforums(forumCategory);
            }
           
            CurrentPageSelector.ItemsSource = Enumerable.Range(1, forumCategory.TotalPages).ToArray();
            CurrentPageSelector.SelectedValue = forumCategory.CurrentPage;
            this.DefaultViewModel["Groups"] = forumThreadList.ForumType;
            this.DefaultViewModel["Threads"] = forumThreadList.ForumThreadList;
            this.DefaultViewModel["Subforums"] = forumThreadList.ForumSubcategoryList;
            if(!forumThreadList.ForumSubcategoryList.Any())
            {
                ThreadViewSnapped.RowDefinitions[1].Height = new GridLength(0);
                ThreadViewFullScreen.ColumnDefinitions[0].Width = new GridLength(0);
            }
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
            var bounds = Window.Current.Bounds;
            if (bounds.Height > bounds.Width)
            {
                VisualStateManager.GoToState(this, "Portrait", false);
            }
            else if (bounds.Width <= 620)
            {
                VisualStateManager.GoToState(this, "Snapped", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "FullScreen", false);
            }

            this.Loaded += PageLoaded;
            this.Unloaded += PageUnloaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= onCommandsRequested;
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        /// <summary>
        /// Handler for the CommandsRequested event. Add custom SettingsCommands here.
        /// </summary>
        /// <param name="e">Event data that includes a vector of commands (ApplicationCommands)</param>
        void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs e)
        {
            if (forumCategory.IsBookmarks)
            {
                SettingsCommand bookmarksCommand = new SettingsCommand("bookmarkSettings", "Bookmarks",
                    (handler) =>
                    {
                        BookmarkSettingsFlyout up = new BookmarkSettingsFlyout();
                        up.Show();
                    });
                e.Request.ApplicationCommands.Add(bookmarksCommand);
            }

        }

        private void ForumThreadList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((ForumThreadEntity)e.ClickedItem);
            this.Frame.Navigate(typeof(ThreadPage), itemId);
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (forumCategory.CurrentPage > 1)
            {
                loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                forumCategory.CurrentPage--;
                CurrentPageSelector.SelectedValue = forumCategory.CurrentPage;
                forumThreadList = await ThreadManager.GetForumThreadsAndSubforums(forumCategory);
                this.DefaultViewModel["Groups"] = forumThreadList.ForumType;
                this.DefaultViewModel["Threads"] = forumThreadList.ForumThreadList;
                this.DefaultViewModel["Subforums"] = forumThreadList.ForumSubcategoryList;
                loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            BackButton.IsEnabled = forumCategory.CurrentPage > 1 ? true : false;
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            forumCategory.CurrentPage++;
            CurrentPageSelector.SelectedValue = forumCategory.CurrentPage;
            BackButton.IsEnabled = forumCategory.CurrentPage > 1 ? true : false;
            ForwardButton.IsEnabled = forumCategory.CurrentPage != forumCategory.TotalPages ? true : false;
            forumThreadList = await ThreadManager.GetForumThreadsAndSubforums(forumCategory);
            this.DefaultViewModel["Groups"] = forumThreadList.ForumType;
            this.DefaultViewModel["Threads"] = forumThreadList.ForumThreadList;
            this.DefaultViewModel["Subforums"] = forumThreadList.ForumSubcategoryList;
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void CurrentPageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((int)CurrentPageSelector.SelectedValue != forumCategory.CurrentPage)
            {
                loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                forumCategory.CurrentPage = (int)CurrentPageSelector.SelectedValue;
                BackButton.IsEnabled = forumCategory.CurrentPage > 1 ? true : false;
                ForwardButton.IsEnabled = forumCategory.CurrentPage != forumCategory.TotalPages ? true : false;
                forumThreadList = await ThreadManager.GetForumThreadsAndSubforums(forumCategory);
                this.DefaultViewModel["Groups"] = forumThreadList.ForumType;
                this.DefaultViewModel["Threads"] = forumThreadList.ForumThreadList;
                this.DefaultViewModel["Subforums"] = forumThreadList.ForumSubcategoryList;
                loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void SubForumList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((ForumEntity)e.ClickedItem);
            this.Frame.Navigate(typeof(ThreadListPage), itemId);
        }

        private void AddThreadButton_Click(object sender, RoutedEventArgs e)
        {
            var itemId = forumCategory;
            this.Frame.Navigate(typeof(CreateThreadView), itemId);
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Window_SizeChanged;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (e.Size.Height > e.Size.Width)
            {
                VisualStateManager.GoToState(this, "Portrait", false);
            }
            else if (e.Size.Width <= 620)
            {
                VisualStateManager.GoToState(this, "Snapped", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "FullScreen", false);
            }
        }

        private void BookmarkSettings_Click(object sender, RoutedEventArgs e)
        {
            BookmarkSettingsFlyout up = new BookmarkSettingsFlyout();
            up.Show();
        }
    }
}
