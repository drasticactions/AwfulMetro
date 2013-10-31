using AwfulMetro.Common;
using BusinessObjects.Entity;
using BusinessObjects.Manager;
using BusinessObjects.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ThreadPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ForumThreadEntity forumThread;

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


        public ThreadPage()
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
            forumThread = (ForumThreadEntity)e.NavigationParameter;
            pageTitle.Text = forumThread.Name;
            String urlLocation = string.Empty;
            if(forumThread.HasBeenViewed)
            {
                forumThread.Location = forumThread.Location + Constants.GOTO_NEW_POST;
            }
            List<ForumPostEntity> threadPosts = await PostManager.GetThreadPosts(forumThread);
            CurrentPageSelector.ItemsSource = Enumerable.Range(1, forumThread.TotalPages).ToArray();
            CurrentPageSelector.SelectedValue = forumThread.CurrentPage;
            BackButton.IsEnabled = forumThread.CurrentPage > 1 ? true : false;
            ForwardButton.IsEnabled = forumThread.TotalPages != forumThread.CurrentPage ? true : false;
            ReplyButton.IsEnabled = !forumThread.IsLocked;
            this.DefaultViewModel["Posts"] = threadPosts;
            if (forumThread.ScrollToPost > 0)
            {
                //ThreadList.ScrollIntoView(threadPosts[forumThread.ScrollToPost - 1]);
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
            var bounds = Window.Current.Bounds;
            if (bounds.Width <= 620)
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
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if(forumThread.CurrentPage > 1)
            {
                loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                forumThread.CurrentPage--;
                BackButton.IsEnabled = forumThread.CurrentPage > 1 ? true : false;
                ForwardButton.IsEnabled = forumThread.TotalPages != forumThread.CurrentPage ? true : false;
                List<ForumPostEntity> threadPosts = await PostManager.GetThreadPosts(forumThread);
                this.DefaultViewModel["Posts"] = threadPosts;
                loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            forumThread.CurrentPage++;
            BackButton.IsEnabled = forumThread.CurrentPage > 1 ? true : false;
            ForwardButton.IsEnabled = forumThread.TotalPages != forumThread.CurrentPage ? true : false;
            List<ForumPostEntity> threadPosts = await PostManager.GetThreadPosts(forumThread);
            this.DefaultViewModel["Posts"] = threadPosts;
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void CurrentPageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (forumThread.CurrentPage != (int)CurrentPageSelector.SelectedValue)
            {
                loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
                forumThread.CurrentPage = (int)CurrentPageSelector.SelectedValue;
                BackButton.IsEnabled = forumThread.CurrentPage > 1 ? true : false;
                ForwardButton.IsEnabled = forumThread.CurrentPage != forumThread.TotalPages ? true : false;
                List<ForumPostEntity> threadPosts = await PostManager.GetThreadPosts(forumThread);
                this.DefaultViewModel["Posts"] = threadPosts;
                loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ReplyView), forumThread);
        }

        private void UserProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var forumPost = (ForumPostEntity)button.DataContext;
            this.Frame.Navigate(typeof(UserProfileView), forumPost.User);
        }

        private void RapSheetButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var forumPost = (ForumPostEntity)button.DataContext;
            this.Frame.Navigate(typeof(RapSheetView), forumPost.User.id);
        }

        private void PostHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var forumPost = (ForumPostEntity)button.DataContext;
            this.Frame.Navigate(typeof(UserPostHistoryPage), forumPost.User.id);
        }

        private void QuoteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var forumPost = (ForumPostEntity)button.DataContext;
            this.Frame.Navigate(typeof(ReplyView), forumPost);
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
            if (e.Size.Width <= 620)
            {
                VisualStateManager.GoToState(this, "Snapped", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "FullScreen", false);
            }

            //else if (e.Size.Height > e.Size.Width)
            //{
            //   //VisualStateManager.GoToState(this, state.State, transitions);
            //}
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            List<ForumPostEntity> threadPosts = await PostManager.GetThreadPosts(forumThread);
            this.DefaultViewModel["Posts"] = threadPosts;
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
