// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class EditReplyPage : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly ReplyManager _replyManager = new ReplyManager();
        private readonly SmileManager _smileManager = new SmileManager();
        private readonly NavigationHelper navigationHelper;
        private IEnumerable<BBCodeCategoryEntity> _bbCodeList = new List<BBCodeCategoryEntity>();
        private ForumReplyEntity _forumReply = new ForumReplyEntity();
        private ForumThreadEntity _forumThread;
        private List<SmileCategoryEntity> _smileCategoryList = new List<SmileCategoryEntity>();
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public EditReplyPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            PreviewLastPostWebView.ScriptNotify += PreviousPostsWebView_ScriptNotify;
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;
        }

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
            get { return navigationHelper; }
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
                    Frame.Navigate(typeof(ReplyView), command.Id);
                    break;
                case "edit":
                    Frame.Navigate(typeof(EditReplyPage), command.Id);
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
            LoadingProgressBar.Visibility = Visibility.Visible;
            var jsonObjectString = (string) e.NavigationParameter;
            long threadId = Convert.ToInt64(jsonObjectString);
            _forumReply = await _replyManager.GetReplyCookiesForEdit(threadId);
            if (_forumReply == null)
            {
                var msgDlg = new MessageDialog("You can't edit this post!");
                await msgDlg.ShowAsync();
                Frame.GoBack();
                return;
            }
            ReplyText.Text = _forumReply.Quote;
            PreviewLastPostWebView.NavigateToString(_forumReply.PreviousPostsRaw);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
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

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            _forumReply.MapMessage(ReplyText.Text);
            var replyManager = new ReplyManager();
            bool result = await replyManager.SendUpdatePost(_forumReply);
            if (result)
            {
                Frame.GoBack();
            }
            else
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                var msgDlg = new MessageDialog("Error making reply!");
                await msgDlg.ShowAsync();
            }
        }

        private async void SimilesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            ItemGridView.Visibility = Visibility.Visible;
            PreviewLastPostWebView.Visibility = Visibility.Collapsed;
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
            PreviewLastPostWebView.Visibility = Visibility.Collapsed;
            if (!_bbCodeList.Any())
            {
                _bbCodeList = BBCodeManager.BBCodes;
            }
            DefaultViewModel["Groups"] = _bbCodeList;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            ItemGridView.Visibility = Visibility.Collapsed;
            PreviewLastPostWebView.Visibility = Visibility.Visible;

            _forumReply.MapMessage(ReplyText.Text);
            var replyManager = new ReplyManager();
            string result = await replyManager.CreatePreviewPost(_forumReply);
            if (!string.IsNullOrEmpty(result))
            {
                PreviewLastPostWebView.NavigateToString(result);
                PreviewLastPostWebView.Visibility = Visibility.Visible;
            }
            else
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                string messageText =
                    string.Format(
                        "No text?! What good is showing you a preview then! Type something in and try again!{0}{1}",
                        Environment.NewLine, Constants.ASCII_2);
                var msgDlg = new MessageDialog(messageText);
                await msgDlg.ShowAsync();
            }
        }

        private async void ImageUploadButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file == null) return;
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            ImgurEntity result = await UploadManager.UploadImgur(stream);
            if (result == null)
            {
                var msgDlg = new MessageDialog("Something went wrong with the upload. :-(.");
                msgDlg.ShowAsync();
                return;
            }

            // We have got an image up on Imgur! Time to get it into the reply box!

            string imgLink = string.Format("[TIMG]{0}[/TIMG]", result.data.link);
            ReplyText.Text = ReplyText.Text.Insert(ReplyText.Text.Length, imgLink);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void LastPostsButton_OnClick(object sender, RoutedEventArgs e)
        {
            PreviewLastPostWebView.NavigateToString(_forumReply.PreviousPostsRaw);
            ItemGridView.Visibility = Visibility.Collapsed;
            PreviewLastPostWebView.Visibility = Visibility.Visible;
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
            InsertBbCode(bbcode);
        }

        private void InsertBbCode(BBCodeEntity bbcode)
        {
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
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private bool _isCtrlKeyPressed;

        private void ReplyText_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Control) _isCtrlKeyPressed = true;
            else if (_isCtrlKeyPressed)
            {
                // U and Q are used with system commands, so we can't catpure them here.
                // We need different commands for them...
                switch (e.Key)
                {
                    case VirtualKey.U:
                        InsertBbCode(new BBCodeEntity("u", "u"));
                        break;
                    case VirtualKey.B:
                        InsertBbCode(new BBCodeEntity("b", "b"));
                        break;
                    case VirtualKey.S:
                        InsertBbCode(new BBCodeEntity("s", "s"));
                        break;
                    case VirtualKey.Q:
                        InsertBbCode(new BBCodeEntity("quote", "quote"));
                        break;
                }
                _isCtrlKeyPressed = false;
            }
        }

        private void ReplyText_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Control) _isCtrlKeyPressed = false;
        }
    }
}