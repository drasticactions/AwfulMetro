// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using AwfulMetro.Common;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using AwfulMetro.Core.Tools;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace AwfulMetro.Views
{
    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FrontPage : Page
    {
        private readonly ObservableDictionary _defaultViewModel = new ObservableDictionary();
        private readonly FrontPageManager _frontPageManager = new FrontPageManager();
        private readonly NavigationHelper _navigationHelper;
        private FrontPageArticleEntity _mainArticle;

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
            HtmlDocument doc = await _frontPageManager.GetFrontPage();
            DefaultViewModel["PopularThreads"] = _frontPageManager.GetPopularThreads(doc);
            DefaultViewModel["PopularTrends"] = _frontPageManager.GetPopularTrends(doc);
            List<FrontPageArticleEntity> frontPageArticles = _frontPageManager.GetFrontPageArticles(doc);

            FrontPageArticleEntity mainArticle = frontPageArticles.FirstOrDefault();
            DefaultViewModel["MainArticle"] = mainArticle;
            _mainArticle = mainArticle;
            frontPageArticles.Remove(mainArticle);

            DefaultViewModel["FrontPageArticles"] = frontPageArticles;
            DefaultViewModel["FrontPageFeatures"] = _frontPageManager.GetFeatures(doc);
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
            if (threadEntity == null) return;
            thread.ParseFromPopularThread(threadEntity.Title, threadEntity.Id);
            string jsonObjectString = JsonConvert.SerializeObject(thread);
            Frame.Navigate(typeof (ThreadPage), jsonObjectString);
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

        private async void MainArticleGrid_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(Constants.FRONT_PAGE + _mainArticle.ArticleLink));
            //Frame.Navigate(typeof(ArticleViewer), string.Format("http://www.somethingawful.com/{0}", _mainArticle.ArticleLink));
        }

        private async void FeaturesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var articleEntity = e.ClickedItem as FrontPageArticleEntity;
            if (articleEntity == null) return;
            await Launcher.LaunchUriAsync(new Uri(Constants.FRONT_PAGE + articleEntity.ArticleLink));
            //Frame.Navigate(typeof(ArticleViewer), string.Format("http://www.somethingawful.com/{0}", articleEntity.ArticleLink));
        }

        private async void NewArticlesListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var articleEntity = e.ClickedItem as FrontPageArticleEntity;
            if (articleEntity == null) return;
            await Launcher.LaunchUriAsync(new Uri(Constants.FRONT_PAGE + articleEntity.ArticleLink));
            //Frame.Navigate(typeof(ArticleViewer), string.Format("http://www.somethingawful.com/{0}", articleEntity.ArticleLink));
        }

        private async void ForumTrendList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var articleEntity = e.ClickedItem as PopularThreadsTrendsEntity;
            if (articleEntity == null) return;
            await Launcher.LaunchUriAsync(new Uri(Constants.FRONT_PAGE + articleEntity.LocationUrl));
            //Frame.Navigate(typeof(ArticleViewer), articleEntity.LocationUrl);
        }
    }
}