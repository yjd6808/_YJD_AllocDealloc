// 작성자 : 윤정도
// 이미지 포맷에 대한 확장기능

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Tiff;

namespace JJangdoImageUtil
{
    public static class ImageFormatExtension
    {
        public static IImageFormat ToFormat(this ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.Jpeg: return JpegFormat.Instance;
                case ImageFormat.Gif: return GifFormat.Instance;
                case ImageFormat.Bmp: return BmpFormat.Instance;
                case ImageFormat.Png: return PngFormat.Instance;
                case ImageFormat.Tiff: return TiffFormat.Instance;
                case ImageFormat.Webp: return WebpFormat.Instance;
                default: return null;
            }
        }

        public static string ToExtensionString(this ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.Jpeg: return ".jpg";
                case ImageFormat.Gif: return ".gif";
                case ImageFormat.Bmp: return ".bmp";
                case ImageFormat.Png: return ".png";
                case ImageFormat.Tiff: return ".tiff";
                case ImageFormat.Webp: return ".webp";
                case ImageFormat.Ico: return ".ico";
                default: return null;
            }
        }

        public static ImageFormat ToFormat(this IImageFormat imageFormat)
        {
            if (imageFormat is JpegFormat)
                return ImageFormat.Jpeg;
            if (imageFormat is GifFormat)
                return ImageFormat.Gif;
            if (imageFormat is BmpFormat)
                return ImageFormat.Bmp;
            if (imageFormat is PngFormat)
                return ImageFormat.Png;
            if (imageFormat is TiffFormat)
                return ImageFormat.Tiff;
            if (imageFormat is WebpFormat)
                return ImageFormat.Webp;

            return ImageFormat.Unknown;
        }


        // @색상 리스트 : https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.colors?view=windowsdesktop-6.0
        public static Brush ToBrush(this ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.Jpeg: return Brushes.DarkKhaki;
                case ImageFormat.Gif: return Brushes.LightCoral;
                case ImageFormat.Bmp: return Brushes.HotPink;
                case ImageFormat.Png: return Brushes.SeaGreen;
                case ImageFormat.Tiff: return Brushes.MediumAquamarine;
                case ImageFormat.Webp: return Brushes.DeepSkyBlue;
                case ImageFormat.Ico: return Brushes.Gray;
                default: return Brushes.Black;
            }
        }

    }
}
