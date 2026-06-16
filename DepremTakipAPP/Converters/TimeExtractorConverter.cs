using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace DepremTakipAPP.Converters
{
    public class TimeExtractorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateStr && dateStr.Contains(" "))
            {
                var parts = dateStr.Split(' ');
                if (parts.Length > 1)
                    return parts[1]; // saat kısmı
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
