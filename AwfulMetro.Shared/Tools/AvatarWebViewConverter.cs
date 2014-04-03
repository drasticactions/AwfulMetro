using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AwfulMetro.Tools
{
    public class AvatarWebViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var avatarUrl = (string) value;
            return string.IsNullOrEmpty(avatarUrl) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}