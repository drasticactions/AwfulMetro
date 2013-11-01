using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using BusinessObjects.Entity;
using BusinessObjects.Manager;
using BusinessObjects.Tools;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=286574

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class UserProfileView : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper _navigationHelper;

        public UserProfileView()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
        }

        private readonly ForumUserManager forumUserManager = new ForumUserManager();
        private readonly RapSheetManager rapSheetManager = new RapSheetManager();
        private readonly ForumSearchManager forumSearchManager = new ForumSearchManager();

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
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
            var user = (ForumUserEntity) e.NavigationParameter;
            long userId;
            if (user == null)
            {
                userId = (long) e.NavigationParameter;
                user = new ForumUserEntity();
            }
            else
            {
                userId = user.Id;
            }

            DefaultViewModel["UserEntity"] = await forumUserManager.GetUserFromProfilePage(user, userId);
            DefaultViewModel["RapSheet"] =
                await rapSheetManager.GetRapSheet(Constants.BASE_URL + string.Format(Constants.USER_RAP_SHEET, userId));
            DefaultViewModel["UserSearch"] =
                await forumSearchManager.GetSearchResults(string.Format(Constants.USER_POST_HISTORY, userId));
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
