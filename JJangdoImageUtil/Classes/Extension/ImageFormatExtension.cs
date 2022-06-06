
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
