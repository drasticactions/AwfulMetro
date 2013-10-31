using AwfulMetro.Common;
using BusinessObjects.Entity;
using BusinessObjects.Manager;
using BusinessObjects.Tools;
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

namespace AwfulMetro.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ReplyView : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private List<SmileCategoryEntity> smileCategoryList = new List<SmileCategoryEntity>();
        private IEnumerable<BBCodeCategoryEntity> bbCodeList = new List<BBCodeCategoryEntity>();
        private ForumThreadEntity forumThread;
        private ForumPostEntity forumPost;

        private readonly SmileManager smileManager = new SmileManager();
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


        public ReplyView()
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
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var item = e.NavigationParameter;
            if(item.GetType() == typeof(ForumThreadEntity))
            {
                forumThread = (ForumThreadEntity)e.NavigationParameter;
            }
            else
            {
                forumPost = (ForumPostEntity)e.NavigationParameter;
                ReplyText.Text = string.Format(Constants.QUOTE_EXP, forumPost.User.Username, forumPost.PostId, forumPost.PostFormatted);
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

        private void PostButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReplyText_TextChanged(object sender, TextChangedEventArgs e)
        {
            PostCount.Text = string.Format("{0}/50000", ReplyText.Text.Length);
        }

        private async void SimilesButton_Click(object sender, RoutedEventArgs e)
        {
            this.loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (!smileCategoryList.Any())
            {
                smileCategoryList = await smileManager.GetSmileList();
            }
            this.DefaultViewModel["Groups"] = smileCategoryList;
            this.loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void BBcodeButton_Click(object sender, RoutedEventArgs e)
        {
            this.loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (!bbCodeList.Any())
            {
                bbCodeList = BBCodeManager.BBCodes;
            }
            this.DefaultViewModel["Groups"] = bbCodeList;
            this.loadingProgressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem;

            if (item.GetType() == typeof(SmileEntity))
            {
                SmileEntity smile = (SmileEntity)e.ClickedItem;
                ReplyText.Text = ReplyText.Text.Insert(ReplyText.Text.Length, smile.Title);
            }

            if (item.GetType() == typeof(BBCodeEntity))
            {
                BBCodeEntity bbcode = (BBCodeEntity)e.ClickedItem;
                if (!string.IsNullOrEmpty(ReplyText.SelectedText))
                {
                    string selectedText = "[{0}]" + ReplyText.SelectedText + "[/{0}]";
                    ReplyText.SelectedText = string.Format(selectedText, bbcode.Code);
                }
                else
                {
                    string text = string.Format("[{0}][/{0}]", bbcode.Code);
                    ReplyText.Text = ReplyText.Text.Insert(ReplyText.SelectionStart, text);
                }
            }
        }
    }
}
