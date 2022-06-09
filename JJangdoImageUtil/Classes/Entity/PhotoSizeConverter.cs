// 작성자 : 윤정도
// Size 구조체를 문자열 형태로 변환해주는 컨버터

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