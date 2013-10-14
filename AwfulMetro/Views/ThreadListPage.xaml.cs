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
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
            forumCategory = (ForumEntity)e.NavigationParameter;
            BackButton.IsEnabled = forumCategory.CurrentPage > 1 ? true : false;
            //CurrentPage.Text = forumCategory.CurrentPage.ToString();
            
            pageTitle.Text = forumCategory.Name;
            if(forumCategory.IsBookmarks)
            {
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ForumThreadList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((ForumThreadEntity)e.ClickedItem);
            this.Frame.Navigate(typeof(ThreadPage), itemId);
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (forumCategory.CurrentPage > 1)
            {
                forumCategory.CurrentPage--;
                CurrentPageSelector.SelectedValue = forumCategory.CurrentPage;
                forumThreadList = await ThreadManager.GetForumThreadsAndSubforums(forumCategory);
                this.DefaultViewModel["Groups"] = forumThreadList.ForumType;
                this.DefaultViewModel["Threads"] = forumThreadList.ForumThreadList;
                this.DefaultViewModel["Subforums"] = forumThreadList.ForumSubcategoryList;
            }
            BackButton.IsEnabled = forumCategory.CurrentPage > 1 ? true : false;
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            forumCategory.CurrentPage++;
            CurrentPageSelector.SelectedValue = forumCategory.CurrentPage;
            BackButton.IsEnabled = forumCategory.CurrentPage > 1 ? true : false;
            ForwardButton.IsEnabled = forumCategory.CurrentPage != forumCategory.TotalPages ? true : false;
            forumThreadList = await ThreadManager.GetForumThreadsAndSubforums(forumCategory);
            this.DefaultViewModel["Groups"] = forumThreadList.ForumType;
            this.DefaultViewModel["Threads"] = forumThreadList.ForumThreadList;
            this.DefaultViewModel["Subforums"] = forumThreadList.ForumSubcategoryList;
        }

        private async void CurrentPageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((int)CurrentPageSelector.SelectedValue != forumCategory.CurrentPage)
            {
                forumCategory.CurrentPage = (int)CurrentPageSelector.SelectedValue;
                BackButton.IsEnabled = forumCategory.CurrentPage > 1 ? true : false;
                ForwardButton.IsEnabled = forumCategory.CurrentPage != forumCategory.TotalPages ? true : false;
                forumThreadList = await ThreadManager.GetForumThreadsAndSubforums(forumCategory);
                this.DefaultViewModel["Groups"] = forumThreadList.ForumType;
                this.DefaultViewModel["Threads"] = forumThreadList.ForumThreadList;
                this.DefaultViewModel["Subforums"] = forumThreadList.ForumSubcategoryList;
            }
        }

        private void SubForumList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((ForumEntity)e.ClickedItem);
            this.Frame.Navigate(typeof(ThreadListPage), itemId);
        }
    }
}
