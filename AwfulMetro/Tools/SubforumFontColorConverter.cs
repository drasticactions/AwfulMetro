using System;
using Windows.UI.Xaml.Data;
using AwfulMetro.Core.Entity;

namespace AwfulMetro.Tools
{
    public class SubforumFontColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as ForumCategoryEntity;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}