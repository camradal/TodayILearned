using System;
using System.Globalization;
using System.Windows.Data;

namespace TodayILearned.Utilities
{
    public class WikiUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var url = value as string;
            if (!string.IsNullOrEmpty(url) && url.Contains("en.wikipedia"))
            {
                url = url.Replace("en.wikipedia", "en.m.wikipedia");
            }
            return url;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
