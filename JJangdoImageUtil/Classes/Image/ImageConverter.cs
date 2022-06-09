// 작성자 : 윤정도
// 직접적으로 이미지 변환 기능을 수행해줌

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
        private static readonly ImageFormatManager ImageFormatManager = Configuration.Default.ImageFormatsManager;

        public static IImageEncoder FindEncoder(IImageFormat imageFormat)
        {
            return ImageFormatManager.FindEncoder(imageFormat);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static bool DestroyIcon(IntPtr handle);

        public static Image ConvertToJpeg(Image image, Stream stream, out ImageFormat format)
        {
            stream.Reset();
            image.SaveAsJpeg(stream);
            stream.Position = 0;
            IImageFormat iFormat;
            Image img = Image.Load(stream, out iFormat);
            stream.Position = 0;
            format = iFormat.ToFormat();
            return img;
        }

        public static Image ConvertToPng(Image image, Stream stream, out ImageFormat format)
        {
            stream.Reset();
            image.SaveAsPng(stream);
            stream.Position = 0;
            IImageFormat iFormat;
            Image img = Image.Load(stream, out iFormat);
            stream.Position = 0;
            format = iFormat.ToFormat();
            return img;
        }

        public static Image ConvertToBmp(Image image, Stream stream, out ImageFormat format)
        {
            stream.Reset();
            image.SaveAsBmp(stream);
            stream.Position = 0;
            IImageFormat iFormat;
            Image img = Image.Load(stream, out iFormat);
            stream.Position = 0;
            format = iFormat.ToFormat();
            return img;
        }

        public static Image ConvertToTiff(Image image, Stream stream, out ImageFormat format)
        {
            stream.Reset();
            image.SaveAsTiff(stream);
            stream.Position = 0;
            IImageFormat iFormat;
            Image img = Image.Load(stream, out iFormat);
            stream.Position = 0;
            format = iFormat.ToFormat();
            return img;
        }

        public static Image ConvertToWebp(Image image, Stream stream, out ImageFormat format)
        {
            stream.Reset();
            image.SaveAsWebp(stream);
            stream.Position = 0;
            IImageFormat iFormat;
            Image img = Image.Load(stream, out iFormat);
            stream.Position = 0;
            format = iFormat.ToFormat();
            return img;
        }

        public static Image ConvertToGif(Image image, Stream stream, out ImageFormat format)
        {
            stream.Reset();
            image.SaveAsGif(stream);
            stream.Position = 0;
            IImageFormat iFormat;
            Image img = Image.Load(stream, out iFormat);
            stream.Position = 0;
            format = iFormat.ToFormat();
            return img;
        }


        // Bitmap의 GetHicon() 으로 얻은 핸들 값으로 Icon.FromHandle() 로 생성한 아이콘은 퀄리티가 너무 안좋다.
        // 이 코드를 참고해서 구현했는데 색상이 엄청 예쁘게 잘나온다.
        // @참고 : https://www.codeproject.com/Tips/627823/Fast-and-high-quality-Bitmap-to-icon-converter
        public static Icon ConvertPngToIcon(Stream pngStream, int size = 16) // png이미지의 데이터가 저장된 스트림
        {
            byte[] pngIconHeader = { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            using (MemoryStream iconStream = new MemoryStream())
            {
                pngIconHeader[6] = (byte)size;
                pngIconHeader[7] = (byte)size;
                pngIconHeader[14] = (byte)(pngStream.Length & 255);
                pngIconHeader[15] = (byte)(pngStream.Length / 256);
                pngIconHeader[18] = (byte)(pngIconHeader.Length);

                iconStream.Write(pngIconHeader, 0, pngIconHeader.Length);
                pngStream.CopyTo(iconStream);
                iconStream.Position = 0;
                return new Icon(iconStream);
            }
        }


        public static Bitmap ConvertToIco(Image image, Stream stream, int size, out ImageFormat format)
        {
            // 비율 유지하면서 사이즈 변경
            // @참고 : https://docs.sixlabors.com/api/ImageSharp/SixLabors.ImageSharp.Processing.ResizeMode.html
            //image.Mutate(x =>
            //{
            //    x.Resize(new ResizeOptions
            //    {
            //        Mode = ResizeMode.Max,
            //        Size = new Size(128, 128)
            //    });
            //});

            stream.Reset();
            Bitmap bitmap = new Bitmap(image.ToBitmap(), size, size);
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;

            Icon icon = ConvertPngToIcon(stream, size);
            stream.Position = 0;
            format = ImageFormat.Ico;

            icon.Dispose();         // 안에서 DestroyIcon 호출해줌
            return bitmap;
        }
      
    }
}
