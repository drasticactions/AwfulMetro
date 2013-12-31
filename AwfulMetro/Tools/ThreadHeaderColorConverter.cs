using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using AwfulMetro.Core.Entity;

namespace AwfulMetro.Tools
{
    public class ThreadHeaderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as ForumThreadEntity;
            if (item == null) return null;
            if (item.IsSticky) return new SolidColorBrush(Colors.Yellow);
            if (item.IsLocked) return new SolidColorBrush(Colors.Black);
            return new SolidColorBrush(Color.FromArgb(255,65,91,100));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
