using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=286574

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class UserProfileView : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly ForumSearchManager _forumSearchManager = new ForumSearchManager();
        private readonly ForumUserManager _forumUserManager = new ForumUserManager();
        private readonly NavigationHelper _navigationHelper;
        private readonly RapSheetManager _rapSheetManager = new RapSheetManager();

        public UserProfileView()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        ///     This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }

        /// <summary>
        ///     NavigationHelper is used on each page to aid in navigation and
        ///     process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }


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
            long userId;
            userId = Convert.ToInt64(e.NavigationParameter);
            ForumUserEntity userProfile = await _forumUserManager.GetUserFromProfilePage(userId);
            pageTitle.Text = string.Format("Profile - {0}", userProfile.Username);
            DefaultViewModel["UserEntity"] = userProfile;
            string rapsheetHtml = await _rapSheetManager.GetRapSheet(Constants.BASE_URL + string.Format(Constants.USER_RAP_SHEET, userId));
            string postHistoryHtml = await _forumSearchManager.GetSearchResults(string.Format(Constants.USER_POST_HISTORY, userId));
            if (string.IsNullOrEmpty(rapsheetHtml))
            {
                RapSheetTextBlock.Text =
                    string.Format(
                        "Sorry, but you can't make yourself feel better by judging this user. They've done nothing wrong!{0}Don't you feel like shit now? Knowing that you are not as good as this, pure, goon?{0}I wish I could help you, but I'm just an awful forum viewer.{0}I can't fix your fucked up life and post history.",
                        Environment.NewLine);
                RapSheetWebView.Visibility = Visibility.Collapsed;
                RapSheetTextBlock.Visibility = Visibility.Visible;
            }
            if (string.IsNullOrEmpty(postHistoryHtml))
            {
                DenyPostHistoryTextBlock.Text = string.Format("What? No post history?!? What has brought such tragedy?!{0}Oh yeah, you need platinum access to see this. Get that first.", Environment.NewLine);
                PostHistoryWebView.Visibility = Visibility.Collapsed;
                DenyPostHistoryTextBlock.Visibility = Visibility.Visible;
            }
            DefaultViewModel["RapSheetHtml"] = rapsheetHtml;
            DefaultViewModel["UserSearchHtml"] = postHistoryHtml;
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
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}