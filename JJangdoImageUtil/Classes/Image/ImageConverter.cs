using System;
using System.Runtime.InteropServices;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Advanced;
using System.IO;
using SixLabors.ImageSharp.Processing;

namespace JJangdoImageUtil
{
    using Bitmap = System.Drawing.Bitmap;
    using Icon = System.Drawing.Icon;

    public partial class ImageConverter
    {
        private readonly static ImageFormatManager ImageFormatManager = Configuration.Default.ImageFormatsManager;

        public static IImageEncoder FindEncoder(IImageFormat imageFormat)
        {
            return ImageFormatManager.FindEncoder(imageFormat);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static bool DestroyIcon(IntPtr handle);

        public static Image ConvertToJpeg(Image image, Stream stream)
        {
            stream.Reset();
            image.SaveAsJpeg(stream);
            stream.Position = 0;
            return Image.Load(stream);
        }

        public static Image ConvertToPng(Image image, Stream stream)
        {
            stream.Reset();
            image.SaveAsPng(stream);
            stream.Position = 0;
            return Image.Load(stream);
        }

        public static Image ConvertToBmp(Image image, Stream stream)
        {
            stream.Reset();
            image.SaveAsBmp(stream);
            stream.Position = 0;
            return Image.Load(stream);
        }

        public static Image ConvertToTiff(Image image, Stream stream)
        {
            stream.Reset();
            image.SaveAsTiff(stream);
            stream.Position = 0;
            return Image.Load(stream);
        }

        public static Image ConvertToWebp(Image image, Stream stream)
        {
            stream.Reset();
            image.SaveAsWebp(stream);
            stream.Position = 0;
            return Image.Load(stream);
        }

        public static Image ConvertToGif(Image image, Stream stream)
        {
            stream.Reset();
            image.SaveAsGif(stream);
            stream.Position = 0;
            return Image.Load(stream);
        }

        public static Bitmap ConvertToIco(Image image, Stream stream)
        {
            stream.Reset();
            image.Mutate(x => x.Rotate(RotateMode.Rotate90));
            // image.Mutate(x => x.Resize(128, 128));      // 128 x 128 크기로 변경
            image.SaveAsPng(stream);
            stream.Position = 0;
            

            Bitmap bitmap = new Bitmap(stream);
            IntPtr hIcon = bitmap.GetHbitmap();
            Icon icon = Icon.FromHandle(hIcon);

            DestroyIcon(hIcon);

            stream.Reset();
            icon.Save(stream);
            return bitmap;
        }

        
    }
}
