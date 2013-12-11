using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using AwfulMetro.Core.Annotations;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AwfulMetro.Core.Tools
{
    public class PageScrollingCollection : ObservableCollection<ForumThreadEntity>, ISupportIncrementalLoading
    {
        public PageScrollingCollection(ForumEntity forumEntity, int pageCount)
        {
            HasMoreItems = true;
            IsLoading = false;
            PageCount = pageCount + 1;
            ForumEntity = forumEntity;
        }
       
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }

        public bool HasMoreItems { get; protected set; }

        private int PageCount { get; set; }

        private bool IsLoading { [UsedImplicitly] get; set; }

        private ForumEntity ForumEntity { get; set; }

        public async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {
            IsLoading = true;
            var threadManager = new ThreadManager();
            var forumThreadEntities = await threadManager.GetForumThreads(ForumEntity, PageCount);
            foreach (var forumThreadEntity in forumThreadEntities)
            {
                Add(forumThreadEntity);
            }
            if (forumThreadEntities.Any())
            {
                HasMoreItems = true;
                PageCount++;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
            return new LoadMoreItemsResult {Count = count};
        }
    }
}
