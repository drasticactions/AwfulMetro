using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
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
    public sealed partial class ReplyView : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper _navigationHelper;
        private IEnumerable<BBCodeCategoryEntity> _bbCodeList = new List<BBCodeCategoryEntity>();
        private ForumThreadEntity _forumThread;
        private List<SmileCategoryEntity> _smileCategoryList = new List<SmileCategoryEntity>();
        private ForumReplyEntity _forumReply = new ForumReplyEntity();
        private readonly ReplyManager _replyManager = new ReplyManager();

        public ReplyView()
        {
            InitializeComponent();
            _navigationHelper = new NavigationHelper(this);
            PreviousPostsWebView.ScriptNotify += PreviousPostsWebView_ScriptNotify;
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
        }

        private async void PreviousPostsWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string stringJson = e.Value;
            var command = JsonConvert.DeserializeObject<ThreadCommand>(stringJson);
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
                    loadingProgressBar.Visibility = Visibility.Visible;
                    var quoteString = await _replyManager.GetQuoteString(Convert.ToInt64(command.Id));
                    quoteString = string.Concat(Environment.NewLine, quoteString);
                    string replyText = string.IsNullOrEmpty(ReplyText.Text) ? string.Empty : ReplyText.Text;
                    if (replyText != null) ReplyText.Text = replyText.Insert(ReplyText.Text.Length, quoteString);
                    loadingProgressBar.Visibility = Visibility.Collapsed;
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

        public class ThreadCommand
        {
            public string Command { get; set; }
            public string Id { get; set; }
        }

        private readonly SmileManager _smileManager = new SmileManager();
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
            loadingProgressBar.Visibility = Visibility.Visible;
            var jsonObjectString = (string)e.NavigationParameter;
            long threadId = 0;
            try
            {
                _forumThread = JsonConvert.DeserializeObject<ForumThreadEntity>(jsonObjectString);
            }
            catch (Exception)
            {
                threadId = Convert.ToInt64(jsonObjectString);
            }
            
            //_forumPost = e.NavigationParameter as ForumPostEntity;
            if (_forumThread != null)
            {
                _forumReply = await _replyManager.GetReplyCookies(_forumThread);
            }
            else
            {
                _forumReply = await _replyManager.GetReplyCookies(threadId);
            }
            ReplyText.Text = _forumReply.Quote;
            PreviousPostsWebView.NavigateToString(_forumReply.PreviousPostsRaw);
            loadingProgressBar.Visibility = Visibility.Collapsed;
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

        private async void PostButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            _forumReply.MapMessage(ReplyText.Text);
            var replyManager = new ReplyManager();
            var result = await replyManager.SendPost(_forumReply);
            if (result)
            {
                Frame.GoBack();
            }
            else
            {
                loadingProgressBar.Visibility = Visibility.Collapsed;
                var msgDlg = new Windows.UI.Popups.MessageDialog("Error making reply!");
                await msgDlg.ShowAsync();
            }
        }

        private async void SimilesButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            ItemGridView.Visibility = Visibility.Visible;
            PreviousPostsWebView.Visibility = Visibility.Collapsed;
            if (!_smileCategoryList.Any())
            {
                _smileCategoryList = await _smileManager.GetSmileList();
            }
            DefaultViewModel["Groups"] = _smileCategoryList;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void BBcodeButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            ItemGridView.Visibility = Visibility.Visible;
            PreviousPostsWebView.Visibility = Visibility.Collapsed;
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

            if (item.GetType() != typeof (BBCodeEntity)) return;
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
                if (replyText != null) ReplyText.Text = replyText.Insert(ReplyText.SelectionStart, text);
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

        private async void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            PostPreviewRaw.Visibility = Visibility.Collapsed;
            PreviewPostGrid.Visibility = Visibility.Visible;
            _forumReply.MapMessage(ReplyText.Text);
            var replyManager = new ReplyManager();
            var result = await replyManager.CreatePreviewPost(_forumReply);
            if (!string.IsNullOrEmpty(result))
            {
                PostPreviewRaw.NavigateToString(result);
                PostPreviewRaw.Visibility = Visibility.Visible;
            }
            else
            {
                var msgDlg = new MessageDialog("No text?! What the fuck good is showing you a preview then! Type some shit in and try again!");
                await msgDlg.ShowAsync();
                PreviewPostGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void LastPostsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ItemGridView.Visibility = Visibility.Collapsed;
            PreviousPostsWebView.Visibility = Visibility.Visible;
        }

        private async void ImageUploadButton_OnClick(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            var file = await openPicker.PickSingleFileAsync();
            if (file == null) return;
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            var result = await UploadManager.UploadImgur(stream);
            if (result == null)
            {
                var msgDlg = new MessageDialog("Something went wrong with the upload. :-(.");
                msgDlg.ShowAsync();
                return;
            }

            // We have got an image up on Imgur! Time to get it into the reply box!

            string imgLink = string.Format("[TIMG]{0}[/TIMG]", result.data.link);
            ReplyText.Text = ReplyText.Text.Insert(ReplyText.Text.Length, imgLink);
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }
    }
}
