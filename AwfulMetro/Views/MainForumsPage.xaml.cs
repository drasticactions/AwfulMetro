using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Views;
using BusinessObjects.Entity;
using BusinessObjects.Manager;
using BusinessObjects.Tools;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace AwfulMetro
{
    /// <summary>
    ///     A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainForumsPage : Page
    {
        private readonly ForumManager forumManager = new ForumManager();
        public MainForumsPage()
        {
            DefaultViewModel = new ObservableDictionary();
            InitializeComponent();
            NavigationHelper = new NavigationHelper(this);
            NavigationHelper.LoadState += navigationHelper_LoadState;
        }


        /// <summary>
        ///     NavigationHelper is used on each page to aid in navigation and
        ///     process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper { get; private set; }

        /// <summary>
        ///     This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel { get; private set; }

        /// <summary>
        ///     Populates the page with content passed during navigation.  Any saved state is also
        ///     provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        ///     The source of the event; typically <see cref="NavigationHelper" />
        /// </param>
        /// <param name="e">
        ///     Event data that provides both the navigation parameter passed to
        ///     <see cref="Frame.Navigate(Type, Object)" /> when this page was initially requested and
        ///     a dictionary of state preserved by this page during an earlier
        ///     session.  The state will be null the first time a page is visited.
        /// </param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            List<ForumCategoryEntity> forumGroupList = await forumManager.GetForumCategoryMainPage();
            DefaultViewModel["Groups"] = forumGroupList;
            DefaultViewModel["ForumCategory"] = forumGroupList;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        private void Header_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Zoom using Headers.

            // Determine what group the Button instance represents
            //var group = (sender as FrameworkElement).DataContext;
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            //this.Frame.Navigate(typeof(GroupDetailPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        ///     Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">
        ///     The GridView (or ListView when the application is snapped)
        ///     displaying the item clicked.
        /// </param>
        /// <param name="e">Event data that describes the item clicked.</param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((ForumEntity) e.ClickedItem);
            Frame.Navigate(typeof (ThreadListPage), itemId);
        }

        private void CategoryView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((ForumCategoryEntity) e.ClickedItem);
            itemGridView.ScrollIntoView(itemId);
        }

        private void RapSheetButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (RapSheetView));
        }

        private void FrontPageButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (FrontPage));
        }

        private void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            var forum = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty);
            forum.IsBookmarks = true;
            Frame.Navigate(typeof (ThreadListPage), forum);
        }

        private void ForumEntry_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element != null)
            {
                FlyoutBase.ShowAttachedFlyout(element);
            }
        }

        private void ForumEntry_Holding(object sender, HoldingRoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
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

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
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
            NavigationHelper.OnNavigatedTo(e);

            Rect bounds = Window.Current.Bounds;
            if (bounds.Width <= 620)
            {
                VisualStateManager.GoToState(this, "Snapped", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "FullScreen", false);
            }

            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}