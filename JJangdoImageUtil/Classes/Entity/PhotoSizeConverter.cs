using System;
using System.Globalization;
using System.Windows.Data;
using SixLabors.ImageSharp;

namespace JJangdoImageUtil
{
    public class PhotoSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = (Size)value;
            return size.Width + " x " + size.Height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}