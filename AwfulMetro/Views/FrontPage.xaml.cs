using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using BusinessObjects.Entity;
using BusinessObjects.Manager;
using BusinessObjects.Tools;
using HtmlAgilityPack;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FrontPage : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly NavigationHelper _navigationHelper;
        private readonly FrontPageManager frontPageManager = new FrontPageManager();
        public FrontPage()
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
            HtmlDocument doc = await frontPageManager.GetFrontPage();
            DefaultViewModel["PopularThreads"] = frontPageManager.GetPopularThreads(doc);
            DefaultViewModel["PopularTrends"] = frontPageManager.GetPopularTrends(doc);
            List<FrontPageArticleEntity> frontPageArticles = frontPageManager.GetFrontPageArticles(doc);

            FrontPageArticleEntity mainArticle = frontPageArticles.FirstOrDefault();
            DefaultViewModel["MainArticle"] = mainArticle;
            frontPageArticles.Remove(mainArticle);

            DefaultViewModel["FrontPageArticles"] = frontPageArticles;
            DefaultViewModel["FrontPageFeatures"] = frontPageManager.GetFeatures(doc);

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

        private void PopularThreadList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var threadEntity = e.ClickedItem as PopularThreadsTrendsEntity;
            var thread = new ForumThreadEntity();
            if (threadEntity != null)
            {
                string title = string.IsNullOrEmpty(threadEntity.Title) ? string.Empty : threadEntity.Title;
                thread.ParseFromPopularThread(threadEntity.Title, threadEntity.Id);
                Frame.Navigate(typeof (ThreadPage), thread);
            }
        }

        private async void ArticleImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var image = e.OriginalSource as Image;
            if (image != null)
            {
                var frontPageArticle = image.DataContext as FrontPageArticleEntity;
                if (frontPageArticle != null)
                    await Launcher.LaunchUriAsync(new Uri(Constants.FRONT_PAGE + frontPageArticle.ArticleLink));
            }
        }

        private async void ArticleTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var textBlock = e.OriginalSource as TextBlock;
            if (textBlock != null)
            {
                var frontPageArticle = textBlock.DataContext as FrontPageArticleEntity;
                if (frontPageArticle != null)
                    await Launcher.LaunchUriAsync(new Uri(Constants.FRONT_PAGE + frontPageArticle.ArticleLink));
            }
        }

        private async void AuthorField_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var textBlock = e.OriginalSource as TextBlock;
            if (textBlock != null)
            {
                var frontPageArticle = textBlock.DataContext as FrontPageArticleEntity;
                if (frontPageArticle != null)
                    await Launcher.LaunchUriAsync(new Uri(Constants.FRONT_PAGE + frontPageArticle.AuthorLink));
            }
        }

        private async void FeatureField_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var textBlock = e.OriginalSource as TextBlock;
            if (textBlock != null)
            {
                var frontPageArticle = textBlock.DataContext as FrontPageArticleEntity;
                if (frontPageArticle != null)
                    await Launcher.LaunchUriAsync(new Uri(Constants.FRONT_PAGE + frontPageArticle.FeatureLink));
            }
        }

        private async void TrendTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var textBlock = e.OriginalSource as TextBlock;
            if (textBlock != null)
            {
                var frontPageArticle = textBlock.DataContext as FrontPageArticleEntity;
                if (frontPageArticle != null)
                    await Launcher.LaunchUriAsync(new Uri(Constants.FRONT_PAGE + frontPageArticle.ArticleLink));
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
