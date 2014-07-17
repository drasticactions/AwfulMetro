#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
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
using AwfulMetro.Core.Tools;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class NewThreadView : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ForumEntity _forumEntity;
        private readonly SmileManager _smileManager = new SmileManager();
        private readonly ThreadManager _threadManager = new ThreadManager();
        private readonly PostIconManager _postIconManager = new PostIconManager();
        private IEnumerable<BBCodeCategoryEntity> _bbCodeList = new List<BBCodeCategoryEntity>();
        private PostIconEntity _postIcon;
        private NewThreadEntity _newThreadEntity;
        private IEnumerable<PostIconCategoryEntity> _postIconEntities = new List<PostIconCategoryEntity>(); 
        private List<SmileCategoryEntity> _smileCategoryList = new List<SmileCategoryEntity>();
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public NewThreadView()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
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
            var jsonObjectString = (string)e.NavigationParameter;
            var forumInfo = JsonConvert.DeserializeObject<ForumEntity>(jsonObjectString);
            if (forumInfo == null) return;
            pageTitle.Text = string.Format("New Thread - {0}", forumInfo.Name);
            _forumEntity = forumInfo;
            _newThreadEntity = await _threadManager.GetThreadCookies(forumInfo.ForumId);
            if (_newThreadEntity == null)
            {
                var msgDlg = new MessageDialog("You can't make a new thread in this forum!");
                await msgDlg.ShowAsync();
                Frame.GoBack();
                return;
            }
            var blankPostIconEntity = new PostIconEntity {Id = 0, Title = "Shit"};
            PostIconImage.Source = new BitmapImage(new Uri("ms-appx://Assets/shitpost.gif"));
            _postIcon = blankPostIconEntity;
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

        private async void PostButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            _newThreadEntity.MapTo(SubjectTextBox.Text, ReplyText.Text, _forumEntity, _postIcon);
            bool result = await _threadManager.CreateNewThread(_newThreadEntity);
            if (result)
            {
                Frame.GoBack();
            }
            else
            {
                loadingProgressBar.Visibility = Visibility.Collapsed;
                var msgDlg = new MessageDialog("Error making thread!");
                await msgDlg.ShowAsync();
            }
        }

        private async void PostIconButton_OnClick(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            ItemGridView.Visibility = Visibility.Visible;
            PreviewLastPostWebView.Visibility = Visibility.Collapsed;
            if (!_postIconEntities.Any())
            {
                _postIconEntities = await _postIconManager.GetPostIcons(_forumEntity);
            }
            DefaultViewModel["Groups"] = _postIconEntities;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void SimilesButton_Click(object sender, RoutedEventArgs e)
        {
            loadingProgressBar.Visibility = Visibility.Visible;
            ItemGridView.Visibility = Visibility.Visible;
            PreviewLastPostWebView.Visibility = Visibility.Collapsed;
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
            PreviewLastPostWebView.Visibility = Visibility.Collapsed;
            if (!_bbCodeList.Any())
            {
                _bbCodeList = BBCodeManager.BBCodes;
            }
            DefaultViewModel["Groups"] = _bbCodeList;
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private async void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            ItemGridView.Visibility = Visibility.Collapsed;
            PreviewLastPostWebView.Visibility = Visibility.Visible;

            _newThreadEntity.MapTo(SubjectTextBox.Text, ReplyText.Text, _forumEntity, _postIcon);
            string result = await _threadManager.CreateNewThreadPreview(_newThreadEntity);
            if (!string.IsNullOrEmpty(result))
            {
                PreviewLastPostWebView.NavigateToString(result);
                PreviewLastPostWebView.Visibility = Visibility.Visible;
            }
            else
            {
                loadingProgressBar.Visibility = Visibility.Collapsed;
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
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file == null) return;
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            ImgurEntity result = await UploadManager.UploadImgur(stream);
            if (result == null)
            {
                var msgDlg = new MessageDialog("Something went wrong with the upload. :-(.");
                msgDlg.ShowAsync();
                loadingProgressBar.Visibility = Visibility.Collapsed;
                return;
            }

            // We have got an image up on Imgur! Time to get it into the reply box!

            string imgLink = string.Format("[TIMG]{0}[/TIMG]", result.data.link);
            ReplyText.Text = ReplyText.Text.Insert(ReplyText.Text.Length, imgLink);
            loadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            object item = e.ClickedItem;

            if (item.GetType() == typeof(SmileEntity))
            {
                var smile = (SmileEntity)e.ClickedItem;
                ReplyText.Text = ReplyText.Text.Insert(ReplyText.Text.Length, smile.Title);
                return;
            }

            if (item.GetType() == typeof (PostIconEntity))
            {
                _postIcon = (PostIconEntity) e.ClickedItem;
                PostIconImage.Source = new BitmapImage(new Uri(_postIcon.ImageUrl, UriKind.Absolute));
                return;
            }

            if (item.GetType() != typeof(BBCodeEntity)) return;
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
