using Windows.UI.Popups;
using AwfulMetro.Common;
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
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class EditReplyPage : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly SmileManager _smileManager = new SmileManager();
        private List<SmileCategoryEntity> _smileCategoryList = new List<SmileCategoryEntity>();
        private IEnumerable<BBCodeCategoryEntity> _bbCodeList = new List<BBCodeCategoryEntity>();
        private ForumThreadEntity _forumThread;
        private ForumReplyEntity _forumReply = new ForumReplyEntity();
        private readonly ReplyManager _replyManager = new ReplyManager();
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ObservableDictionary DefaultViewModel
        {
            get { return _defaultViewModel; }
        }
        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public EditReplyPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            PreviousPostsWebView.ScriptNotify += PreviousPostsWebView_ScriptNotify;
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        private async void PreviousPostsWebView_ScriptNotify(object sender, NotifyEventArgs e)
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
                case "quote":
                    LoadingProgressBar.Visibility = Visibility.Visible;
                    var quoteString = await _replyManager.GetQuoteString(Convert.ToInt64(command.Id));
                    quoteString = string.Concat(Environment.NewLine, quoteString);
                    string replyText = string.IsNullOrEmpty(ReplyText.Text) ? string.Empty : ReplyText.Text;
                    if (replyText != null) ReplyText.Text = replyText.Insert(ReplyText.Text.Length, quoteString);
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                    break;
                default:
                    var msgDlg = new MessageDialog("Working on it!")
                    {
                        DefaultCommandIndex = 1
                    };
                    await msgDlg.ShowAsync();
                    break;
            }
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
            LoadingProgressBar.Visibility = Visibility.Visible;
            var jsonObjectString = (string)e.NavigationParameter;
            long threadId = Convert.ToInt64(jsonObjectString);
            _forumReply = await _replyManager.GetReplyCookiesForEdit(threadId);
            ReplyText.Text = _forumReply.Quote;
            PreviousPostsWebView.NavigateToString(_forumReply.PreviousPostsRaw);
            LoadingProgressBar.Visibility = Visibility.Collapsed;

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

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            _forumReply.MapMessage(ReplyText.Text);
            var replyManager = new ReplyManager();
            var result = await replyManager.SendUpdatePost(_forumReply);
            if (result)
            {
                Frame.GoBack();
            }
            else
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                var msgDlg = new Windows.UI.Popups.MessageDialog("Error making reply!");
                await msgDlg.ShowAsync();
            }
        }

        private async void SimilesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            ItemGridView.Visibility = Visibility.Visible;
            PreviousPostsWebView.Visibility = Visibility.Collapsed;
            if (!_smileCategoryList.Any())
            {
                _smileCategoryList = await _smileManager.GetSmileList();
            }
            DefaultViewModel["Groups"] = _smileCategoryList;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void BBcodeButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            ItemGridView.Visibility = Visibility.Visible;
            PreviousPostsWebView.Visibility = Visibility.Collapsed;
            if (!_bbCodeList.Any())
            {
                _bbCodeList = BBCodeManager.BBCodes;
            }
            DefaultViewModel["Groups"] = _bbCodeList;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            PostPreviewRaw.Visibility = Visibility.Collapsed;
            PreviewPostGrid.Visibility = Visibility.Visible;
            _forumReply.MapMessage(ReplyText.Text);
            var replyManager = new ReplyManager();
            var result = await replyManager.CreatePreviewEditPost(_forumReply);
            if (!string.IsNullOrEmpty(result))
            {
                PostPreviewRaw.NavigateToString(result);
                PostPreviewRaw.Visibility = Visibility.Visible;
            }
            else
            {
                var msgDlg = new Windows.UI.Popups.MessageDialog("No text?! What the fuck good is showing you a preview then! Type some shit in and try again!");
                await msgDlg.ShowAsync();
                PreviewPostGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void LastPostsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ItemGridView.Visibility = Visibility.Collapsed;
            PreviousPostsWebView.Visibility = Visibility.Visible;
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            object item = e.ClickedItem;

            if (item.GetType() == typeof(SmileEntity))
            {
                var smile = (SmileEntity)e.ClickedItem;
                ReplyText.Text = ReplyText.Text.Insert(ReplyText.Text.Length, smile.Title);
            }

            if (item.GetType() != typeof (BBCodeEntity)) return;
            var bbcode = (BBCodeEntity)e.ClickedItem;
            if (!string.IsNullOrEmpty(ReplyText.SelectedText))
            {
                string selectedText = "[{0}]" + ReplyText.SelectedText + "[/{0}]";
                ReplyText.SelectedText = string.Format(selectedText, bbcode.Code);
            }
            else
            {
                string text = string.Format("[{0}][/{0}]", bbcode.Code);
                string replyText = string.IsNullOrEmpty(ReplyText.Text) ? string.Empty : ReplyText.Text;
                if (replyText != null) ReplyText.Text = replyText.Insert(ReplyText.SelectionStart, text);
            }
        }
    }
}
