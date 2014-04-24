using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using AwfulMetro.Core.Entity;

namespace AwfulMetro.Tools
{
    public class BackButtonEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var thread = value as ForumThreadEntity;
            if (thread == null) return false;
            return thread.CurrentPage > 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
