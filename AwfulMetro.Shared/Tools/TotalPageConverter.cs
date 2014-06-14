using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace AwfulMetro.Tools
{
    public class TotalPageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int totalPages = (int) value;
            return string.Concat("1/", totalPages);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
