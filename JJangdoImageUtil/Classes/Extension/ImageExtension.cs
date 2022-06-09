// @기반 소스 : https://gist.github.com/vurdalakov/00d9471356da94454b372843067af24e

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace JJangdoImageUtil
{

    using Icon = System.Drawing.Icon;
    using Bitmap = System.Drawing.Bitmap;

    
    public static class ImageExtension
    {
        public static Bitmap ToBitmap<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {
            using (var stream = new MemoryStream())
            {
                var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
                image.Save(stream, imageEncoder);
                stream.Position = 0;
                return new Bitmap(stream);
            }
        }

        public static Bitmap ToBitmap<TPixel>(this Image<TPixel> image, MemoryStream stream) where TPixel : unmanaged, IPixel<TPixel>
        {
            stream.Reset();
            var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
            image.Save(stream, imageEncoder);
            stream.Position = 0;
            return new Bitmap(stream);
        }

        public static Bitmap ToBitmap(this Image image) 
        {
            using (var stream = new MemoryStream())
            {
                var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
                image.Save(stream, imageEncoder);
                stream.Position = 0;
                return new Bitmap(stream);
            }
        }

        public static Bitmap ToBitmap(this Image image, MemoryStream stream)
        {
            stream.Reset();
            var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
            image.Save(stream, imageEncoder);
            stream.Seek(0, SeekOrigin.Begin);
            return new Bitmap(stream);
        }

        public static Image ToImageSharpImage(this Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0;
                return Image.Load(stream);
            }
        }

        public static Image ToImageSharpImage(this Bitmap bitmap, MemoryStream stream)
        {
            stream.Reset();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;
            return Image.Load(stream);
        }

      
    }
}
