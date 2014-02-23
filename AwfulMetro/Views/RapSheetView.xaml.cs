using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class RapSheetView : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper _navigationHelper;
        private readonly RapSheetManager _rapSheetManager = new RapSheetManager();
        private int _currentPage = 1;

        public RapSheetView()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            RapSheetWebView.ScriptNotify += WebView_ScriptNotify;
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

        private async void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string stringJson = e.Value;
            var command = JsonConvert.DeserializeObject<ReplyView.ThreadCommand>(stringJson);
            switch (command.Command)
            {
                case "profile":
                    Frame.Navigate(typeof(UserProfileView), command.Id);
                    break;
                case "post_history":
                    Frame.Navigate(typeof(UserPostHistoryPage), command.Id);
                    break;
                case "rap_sheet":
                    Frame.Navigate(typeof(RapSheetView), command.Id);
                    break;
                case "openThread":
                    // Because we are coming from an existing thread, rather than the thread lists, we need to get the thread information beforehand.
                    // However, right now the managers are not set up to support this. The thread is getting downloaded twice, when it really only needs to happen once.
                    var threadManager = new ThreadManager();
                    var thread = await threadManager.GetThread(command.Id);
                    if (thread == null)
                    {
                        var error = new MessageDialog("Specified post was not found in the live forums.")
                        {
                            DefaultCommandIndex = 1
                        };
                        await error.ShowAsync();
                        break;
                    }
                    string jsonObjectString = JsonConvert.SerializeObject(thread);
                    Frame.Navigate(typeof(ThreadPage), jsonObjectString);
                    break;
                default:
                    var msgDlg = new MessageDialog("Not working yet!")
                    {
                        DefaultCommandIndex = 1
                    };
                    await msgDlg.ShowAsync();
                    break;
            }
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
            string html;

            if (e.NavigationParameter != null && !string.IsNullOrEmpty((string)e.NavigationParameter))
            {
                long userId = Convert.ToInt64(e.NavigationParameter);
                ForwardButton.IsEnabled = false;
                html = await
                        _rapSheetManager.GetRapSheet(Constants.BASE_URL +
                                                     string.Format(Constants.USER_RAP_SHEET, userId));
                NoRapSheetTextBlock.Text =
                    string.Format(
                        "This user has not done anything stupid yet.{0}Sorry to disappoint you, so look at this instead.{0}{1}", System.Environment.NewLine,
                        Constants.ASCII_3);
            }
            else
            {
                html = await _rapSheetManager.GetRapSheet(Constants.BASE_URL + Constants.RAP_SHEET);
                NoRapSheetTextBlock.Text =
    string.Format(
        "Everyone is perfect and has no flaws.{0}Sorry to disappoint you, so look at this instead.{0}{1}", System.Environment.NewLine,
                        Constants.ASCII_3);
            }
            RapSheetWebView.NavigateToString(html);
            if (!string.IsNullOrEmpty(html)) return;
             ForwardButton.IsEnabled = false;
             RapSheetWebView.Visibility = Visibility.Collapsed;
             NoRapSheetTextBlock.Visibility = Visibility.Visible;
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
            string html =
                await
                    _rapSheetManager.GetRapSheet(Constants.BASE_URL + Constants.RAP_SHEET +
                                                 string.Format(Constants.PAGE_NUMBER, _currentPage));
            RapSheetWebView.NavigateToString(html);
        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPage++;
            BackButton.IsEnabled = _currentPage >= 2 ? true : false;
            string html =
                await
                    _rapSheetManager.GetRapSheet(Constants.BASE_URL + Constants.RAP_SHEET +
                                                 string.Format(Constants.PAGE_NUMBER, _currentPage));
            RapSheetWebView.NavigateToString(html);

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