using System;
using System.Globalization;
using System.Windows.Data;

namespace EasyVideoScreensaver
{
    public class VideoUriRepresentation : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string uri = value as string;
            if (uri != null && uri.StartsWith("file://"))
            {
                return uri.Substring(7);
            }
            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}