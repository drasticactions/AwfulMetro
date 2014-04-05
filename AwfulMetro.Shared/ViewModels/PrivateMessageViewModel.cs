using System;
using System.Collections.Generic;
using System.Text;
using AwfulMetro.Common;
using AwfulMetro.Pcl.Core.Tools;

namespace AwfulMetro.ViewModels
{
    public class PrivateMessageViewModel : NotifierBase
    {
        private PrivateMessageScrollingCollection _privateMessageScrollingCollection;

        public PrivateMessageScrollingCollection PrivateMessageScrollingCollection
        {
            get { return _privateMessageScrollingCollection; }
            set
            {
                SetProperty(ref _privateMessageScrollingCollection, value);
                OnPropertyChanged();
            }
        }

        public void GetPrivateMessages()
        {
            PrivateMessageScrollingCollection = new PrivateMessageScrollingCollection();
        }
    }
}
