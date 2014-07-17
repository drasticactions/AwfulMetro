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
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using AwfulMetro.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using AwfulMetro.ViewModels;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReplyPage : Page
    {
        private NavigationHelper navigationHelper;
        private ReplyViewModel _vm;
        public ReplyPage()
        {
            this.InitializeComponent();
            _vm = (ReplyViewModel) DataContext;
            this.navigationHelper = new NavigationHelper(this);
            ThreadWebView.ScriptNotify += ThreadWebView_ScriptNotify;
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs fileOpenPickerContinuationEventArgs)
        {
            if (fileOpenPickerContinuationEventArgs.Files == null) return;
            var file = fileOpenPickerContinuationEventArgs.Files.First();
            if (file == null) return;
            LoadingProgressBar.Visibility = Visibility.Visible;
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            ImgurEntity result = null;
            try
            {
                result = await UploadManager.UploadImgur(stream);
            }
            catch (Exception)
            {

                Debug.WriteLine("Error with Imgur *SHOCK*");
            }
            if (result == null)
            {
                var msgDlg = new MessageDialog("Something went wrong with the upload. My heart bleeds for you.");
                msgDlg.ShowAsync();
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                return;
            }

            // We have got an image up on Imgur! Time to get it into the reply box!

            string imgLink = string.Format("[TIMG]{0}[/TIMG]", result.data.link);
            ReplyTextBox.Text = ReplyTextBox.Text.Insert(ReplyTextBox.Text.Length, imgLink);
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var jsonObjectString = (string)e.NavigationParameter;
            bool result = await _vm.Initialize(jsonObjectString);
            _vm.GetSmiliesPhone();
            if (result)
            {
                ReplyTextBox.Text = _vm.ForumReplyEntity.Quote;
                return;
            }
            var msgDlg = new MessageDialog("You can't reply in this thread!");
            await msgDlg.ShowAsync();
            Frame.GoBack();
        }

        private class ThreadCommand
        {
            public string Command { get; set; }
            public string Id { get; set; }
        }

        private async void ThreadWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string stringJson = e.Value;
            var command = JsonConvert.DeserializeObject<ThreadCommand>(stringJson);
            var replyManager = new ReplyManager();
            switch (command.Command)
            {
                case "quote":
                    LoadingProgressBar.Visibility = Visibility.Visible;
                    string quoteString = await replyManager.GetQuoteString(Convert.ToInt64(command.Id));
                    quoteString = string.Concat(Environment.NewLine, quoteString);
                    string replyText = string.IsNullOrEmpty(ReplyTextBox.Text) ? string.Empty : ReplyTextBox.Text;
                    if (replyText != null) ReplyTextBox.Text = replyText.Insert(ReplyTextBox.Text.Length, quoteString);
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                    break;
                case "edit":
                    //Frame.Navigate(typeof(EditReplyPage), command.Id);
                    break;
                case "openThread":
                    // Because we are coming from an existing thread, rather than the thread lists, we need to get the thread information beforehand.
                    // However, right now the managers are not set up to support this. The thread is getting downloaded twice, when it really only needs to happen once.
                    var threadManager = new ThreadManager();
                    var thread = await threadManager.GetThread(new ForumThreadEntity(), command.Id);
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
                case "setFont":
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
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void PreviewPostButton_OnClick(object sender, RoutedEventArgs e)
        {
            bool result = await _vm.GetPreviewPost(ReplyTextBox.Text);
            if (!result)
            {
                string messageText =
                string.Format(
        "No text?! What good is showing you a preview then! Type something in and try again!{0}{1}",
        Environment.NewLine, Constants.ASCII_2);
                var msgDlg = new MessageDialog(messageText);
                await msgDlg.ShowAsync();
                return;
            }
            FlyoutBase.ShowAttachedFlyout(ContentRoot);
        }

        private void PreviousPostsButton_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.GetPreviousPosts();
            FlyoutBase.ShowAttachedFlyout(ContentRoot);
        }

        private void ImageUploadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.PickSingleFileAndContinue();
        }

        private async void PostButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            _vm.ForumReplyEntity.MapMessage(ReplyTextBox.Text);
            var replyManager = new ReplyManager();
            bool result = await replyManager.SendPost(_vm.ForumReplyEntity);
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

        private void SmiliesButton_OnClick(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(SmiliesButton);
        }

        private void SmiliesPickerFlyout_OnItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            var smileList = args.AddedItems;
            var smile = smileList.First() as SmileEntity;
            if (smile == null) return;
            ReplyTextBox.Text = ReplyTextBox.Text.Insert(ReplyTextBox.Text.Length, smile.Title);
        }

        private void BbCodesButton_OnClick(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(BbCodesButton);
        }

        private void BBCodePickerFlyout_OnItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            var bbCodeList = args.AddedItems;
            var bbcode = bbCodeList.First() as BBCodeEntity;
            if (bbcode == null) return;
            if (!string.IsNullOrEmpty(ReplyTextBox.SelectedText))
            {
                string selectedText = "[{0}]" + ReplyTextBox.SelectedText + "[/{0}]";
                ReplyTextBox.SelectedText = string.Format(selectedText, bbcode.Code);
            }
            else
            {
                string text = string.Format("[{0}][/{0}]", bbcode.Code);
                string replyText = string.IsNullOrEmpty(ReplyTextBox.Text) ? string.Empty : ReplyTextBox.Text;
                if (replyText != null) ReplyTextBox.Text = replyText.Insert(ReplyTextBox.SelectionStart, text);
            }
        }
    }
}
