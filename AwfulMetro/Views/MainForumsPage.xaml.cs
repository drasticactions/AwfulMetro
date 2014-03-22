// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainForumsPage : Page
    {
        private readonly ForumManager _forumManager = new ForumManager();
        private List<long> _forumIds = new List<long>();
        private ForumCategoryEntity _favoritesEntity;
        public MainForumsPage()
        {
            DefaultViewModel = new ObservableDictionary();
            InitializeComponent();
            NavigationHelper = new NavigationHelper(this);
            NavigationHelper.LoadState += navigationHelper_LoadState;
            this.SizeChanged += MainForumsPage_SizeChanged;
        }

        private void MainForumsPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width <= 500)
            {
                VisualStateManager.GoToState(this, "MinimalLayout", true);
            }
            else if (e.NewSize.Width <= e.NewSize.Height)
            {
                VisualStateManager.GoToState(this, "PortraitLayout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLayout", true);
            } 
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
            List<ForumCategoryEntity> forumGroupList = await _forumManager.GetForumCategoryMainPage();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("_forumIds"))
            {
                DeserializeXmlToList((string) localSettings.Values["_forumIds"]);
                var forumEntities = new List<ForumEntity>();
                foreach (var f in forumGroupList.Select(forumGroup => forumGroup.ForumList.Where(forum => _forumIds.Contains(forum.ForumId)).ToList()))
                {
                    forumEntities.AddRange(f);
                }
                _favoritesEntity = new ForumCategoryEntity("Favorites", "forumid=48")
                {
                    ForumList = forumEntities
                };
                DefaultViewModel["Subforums"] = new List<ForumCategoryEntity> { _favoritesEntity };
            }
            DefaultViewModel["Groups"] = forumGroupList;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">
        ///     The GridView (or ListView when the application is snapped)
        ///     displaying the item clicked.
        /// </param>
        /// <param name="e">Event data that describes the item clicked.</param>
        public void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var forumEntity = ((ForumEntity) e.ClickedItem);
            string jsonObjectString = JsonConvert.SerializeObject(forumEntity);
            Frame.Navigate(typeof (ThreadListPage), jsonObjectString);
        }

        public void RapSheetButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (RapSheetView));
        }

        public void FrontPageButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (FrontPage));
        }

        public void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            var forum = new ForumEntity("Bookmarks", Constants.USER_CP, string.Empty, false);
            string jsonObjectString = JsonConvert.SerializeObject(forum);
            Frame.Navigate(typeof (ThreadListPage), jsonObjectString);
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
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void LogoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            var message = string.Format("Are you sure you want to logout?{0}I mean, I could care less. It's up to you...{0}{1}", Environment.NewLine, Constants.ASCII_4);
            var msgBox =
                new MessageDialog(message,
                    "Make all your wildest dreams come true...");
            var okButton = new UICommand("WHY ARE YOU ASKING YES YES YES!") {Invoked = OkButtonClick};
            var cancelButton = new UICommand("No, I like pain and misery.") { Invoked = cancelButtonClick };
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

        private void FavoriteForumButton_OnClick(object sender, RoutedEventArgs e)
        {
            var forumList = new List<ForumEntity>();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (itemGridView.SelectedItems.Any())
            {
                forumList.AddRange(itemGridView.SelectedItems.Cast<ForumEntity>());
                List<long> forumIdList = forumList.Select(forum => forum.ForumId).ToList();
                
                localSettings.Values["_forumIds"] = SerializeListToXml(forumIdList);
                _favoritesEntity = new ForumCategoryEntity("Favorites", "forumid=48")
                {
                    ForumList = forumList
                };
                DefaultViewModel["Subforums"] = new List<ForumCategoryEntity> { _favoritesEntity };
            }
            else
            {
                localSettings.Values["_forumIds"] = null;
                DefaultViewModel["Subforums"] = null;
            }
        }

        private static string SerializeListToXml(List<long> list)
        {
            try
            {
                var xmlIzer = new XmlSerializer(typeof(List<long>));
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

        public void DeserializeXmlToList(string listAsXml)
        {
            try
            {
                var xmlIzer = new XmlSerializer(typeof(List<long>));
                var strReader = new StringReader(listAsXml);
                _forumIds = (xmlIzer.Deserialize(strReader)) as List<long>;
            }

            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                _forumIds = new List<long>();
            }
        }

        private void FavoriteForumList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var forumEntity = ((ForumEntity)e.ClickedItem);
            string jsonObjectString = JsonConvert.SerializeObject(forumEntity);
            Frame.Navigate(typeof(ThreadListPage), jsonObjectString);
        }

        private void PrivateMessagesButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PrivateMessageListView));
        }
    }
}