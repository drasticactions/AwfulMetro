using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using AwfulMetro.Core.Entity;

namespace AwfulMetro.Tools
{
    public class ForwardButtonEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ForumThreadEntity thread = value as ForumThreadEntity;
            if (thread == null) return false;
            return thread.TotalPages != thread.CurrentPage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
