using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using AwfulMetro.Core.Entity;
using AwfulMetro.Core.Manager;

namespace AwfulMetro.Pcl.Core.Tools
{
    public class PrivateMessageScrollingCollection : ObservableCollection<PrivateMessageEntity>, ISupportIncrementalLoading, INotifyPropertyChanged
    {
        public PrivateMessageScrollingCollection()
        {
            HasMoreItems = true;
            PageCount = 0;
        }
        private int PageCount { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }

            private set
            {
                _isLoading = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsLoading"));
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }

        public async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {
            IsLoading = true;
            var privateMessageManager = new PrivateMessageManager();
            List<PrivateMessageEntity> privateMessageList;
            try
            {
                privateMessageList = await privateMessageManager.GetPrivateMessages(PageCount);
            }
            catch (Exception)
            {
                // TODO: Expand into specific error catches.
                privateMessageList = null;
                Debug.WriteLine("Error with parsing.");
            }
            if (privateMessageList == null)
            {
                IsLoading = false;
                HasMoreItems = false;
                return new LoadMoreItemsResult { Count = count };
            }
            foreach (var item in privateMessageList)
            {
                Add(item);
            }
            if (privateMessageList.Any())
            {
                HasMoreItems = true;
                PageCount++;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
            return new LoadMoreItemsResult { Count = count };
        }

        public bool HasMoreItems { get; private set; }
    }
}
