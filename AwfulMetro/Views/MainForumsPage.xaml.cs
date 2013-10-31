using AwfulMetro.Common;
using AwfulMetro.Views;
using BusinessObjects.Entity;
using BusinessObjects.Manager;
using BusinessObjects.Tools;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace AwfulMetro
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainForumsPage : Page
    {
        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper { get; private set; }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel { get; private set; }

        public MainForumsPage()
        {
            DefaultViewModel = new ObservableDictionary();
            this.InitializeComponent();
            this.NavigationHelper = new NavigationHelper(this);
            this.NavigationHelper.LoadState += navigationHelper_LoadState;
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
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            List<ForumCategoryEntity> forumGroupList = await ForumManager.GetForumCategoryMainPage();
            this.DefaultViewModel["Groups"] = forumGroupList;
            this.DefaultViewModel["ForumCategory"] = forumGroupList;
            loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Zoom using Headers.

            // Determine what group the Button instance represents
            //var group = (sender as FrameworkElement).DataContext;
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            //this.Frame.Navigate(typeof(GroupDetailPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((ForumEntity)e.ClickedItem);
            this.Frame.Navigate(typeof(ThreadListPage), itemId);
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
            this.NavigationHelper.OnNavigatedTo(e);

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
            this.NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void CategoryView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((ForumCategoryEntity)e.ClickedItem);
            itemGridView.ScrollIntoView(itemId);
        }

        private void RapSheetButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RapSheetView));
        }

        private void FrontPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FrontPage));
        }

        private void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            ForumEntity forum = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty);
            forum.IsBookmarks = true;
            this.Frame.Navigate(typeof(ThreadListPage), forum);
        }

        private void ForumEntry_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }

        private void ForumEntry_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
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

    }
}