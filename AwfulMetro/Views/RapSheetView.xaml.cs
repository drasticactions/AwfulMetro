using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using BusinessObjects.Manager;
using BusinessObjects.Tools;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class RapSheetView : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper _navigationHelper;
        private int _currentPage = 1;

        public RapSheetView()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
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
            ForwardButton.IsEnabled = true;
            BackButton.IsEnabled = false;
            if (e.NavigationParameter != null)
            {
                var userId = (long) e.NavigationParameter;
                DefaultViewModel["RapSheet"] =
                    await
                        RapSheetManager.GetRapSheet(Constants.BASE_URL + string.Format(Constants.USER_RAP_SHEET, userId));
            }
            else
            {
                DefaultViewModel["RapSheet"] =
                    await RapSheetManager.GetRapSheet(Constants.BASE_URL + Constants.RAP_SHEET);
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

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPage--;
            BackButton.IsEnabled = _currentPage >= 2 ? true : false;
            DefaultViewModel["RapSheet"] =
                await
                    RapSheetManager.GetRapSheet(Constants.BASE_URL + Constants.RAP_SHEET +
                                                string.Format(Constants.PAGE_NUMBER, _currentPage));
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPage++;
            BackButton.IsEnabled = _currentPage >= 2 ? true : false;
            DefaultViewModel["RapSheet"] =
                await
                    RapSheetManager.GetRapSheet(Constants.BASE_URL + Constants.RAP_SHEET +
                                                string.Format(Constants.PAGE_NUMBER, _currentPage));
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