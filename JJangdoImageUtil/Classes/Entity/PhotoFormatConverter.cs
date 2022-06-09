// 작성자 : 윤정도
// 이미지 포맷을 색상형태로 변환해주는 컨버터

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


namespace JJangdoImageUtil
{
    public class PhotoFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageFormat = (ImageFormat)value;
            return imageFormat.ToBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
