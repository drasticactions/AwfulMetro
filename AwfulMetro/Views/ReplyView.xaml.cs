using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ReplyView : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper _navigationHelper;
        private IEnumerable<BBCodeCategoryEntity> _bbCodeList = new List<BBCodeCategoryEntity>();
        private ForumPostEntity _forumPost;
        private ForumThreadEntity _forumThread;
        private List<SmileCategoryEntity> _smileCategoryList = new List<SmileCategoryEntity>();


        public ReplyView()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        }

        private readonly SmileManager smileManager = new SmileManager();
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
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            _forumPost = e.NavigationParameter as ForumPostEntity;
            if(_forumPost != null)
            ReplyText.Text = string.Format(Constants.QUOTE_EXP, _forumPost.User.Username, _forumPost.PostId, _forumPost.PostFormatted);

            _forumThread = e.NavigationParameter as ForumThreadEntity;
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

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ReplyText_TextChanged(object sender, TextChangedEventArgs e)
        {
            PostCount.Text = string.Format("{0}/50000", ReplyText.Text.Length);
        }

        private async void SimilesButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            if (!_smileCategoryList.Any())
            {
                _smileCategoryList = await smileManager.GetSmileList();
            }
            DefaultViewModel["Groups"] = _smileCategoryList;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void BBcodeButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            if (!_bbCodeList.Any())
            {
                _bbCodeList = BBCodeManager.BBCodes;
            }
            DefaultViewModel["Groups"] = _bbCodeList;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            object item = e.ClickedItem;

            if (item.GetType() == typeof (SmileEntity))
            {
                var smile = (SmileEntity) e.ClickedItem;
                ReplyText.Text = ReplyText.Text.Insert(ReplyText.Text.Length, smile.Title);
            }

            if (item.GetType() == typeof (BBCodeEntity))
            {
                var bbcode = (BBCodeEntity) e.ClickedItem;
                if (!string.IsNullOrEmpty(ReplyText.SelectedText))
                {
                    string selectedText = "[{0}]" + ReplyText.SelectedText + "[/{0}]";
                    ReplyText.SelectedText = string.Format(selectedText, bbcode.Code);
                }
                else
                {
                    string text = string.Format("[{0}][/{0}]", bbcode.Code);
                    string replyText = string.IsNullOrEmpty(ReplyText.Text) ? string.Empty : ReplyText.Text;
                    ReplyText.Text = replyText.Insert(ReplyText.SelectionStart, text);
                }
            }
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
