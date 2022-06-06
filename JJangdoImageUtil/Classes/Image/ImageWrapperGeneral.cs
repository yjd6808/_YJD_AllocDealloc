using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Advanced;


namespace JJangdoImageUtil
{
    public class ImageWrapperGeneral : ImageWrapper
    {
        public Image SourceImage { get; set; }


        public ImageWrapperGeneral()
        {
        }

        public ImageWrapperGeneral(string path) : base(path)
        {
        }

        public ImageWrapperGeneral(ImageWrapperIcon iconWrapper)
        {
            _stream = new MemoryStream();
            _format = ImageFormat.Png;

            SourceImage = iconWrapper.IconBitmap.ToImageSharpImage(_stream);
        }

        protected override void Initialize(string path)
        {
            _stream = new MemoryStream();
            SourceImage = Image.Load(path, out var imageFormat);
            SourceImage.Save(Stream, ImageConverter.FindEncoder(imageFormat));
            _format = imageFormat.ToFormat();
        }

        public override ImageWrapper ToGif()
        {
            if (_format == ImageFormat.Gif)
                return this;

            SourceImage = ImageConverter.ConvertToGif(SourceImage, _stream);
            return this;
        }

        public override ImageWrapper ToJpeg()
        {
            if (_format == ImageFormat.Jpeg)
                return this;

            SourceImage = ImageConverter.ConvertToJpeg(SourceImage, _stream);
            return this;
        }

        public override ImageWrapper ToPng()
        {
            if (_format == ImageFormat.Png)
                return this;

            SourceImage = ImageConverter.ConvertToPng(SourceImage, _stream);
            return this;
        }

        public override ImageWrapper ToTiff()
        {
            if (_format == ImageFormat.Tiff)
                return this;

            SourceImage = ImageConverter.ConvertToTiff(SourceImage, _stream);
            return this;
        }

        public override ImageWrapper ToWebp()
        {
            if (_format == ImageFormat.Webp)
                return this;

            SourceImage = ImageConverter.ConvertToWebp(SourceImage, _stream);
            return this;
        }

        public override ImageWrapper ToBmp()
        {
            if (_format == ImageFormat.Bmp)
                return this;

            SourceImage = ImageConverter.ConvertToBmp(SourceImage, _stream);
            return this;
        }

        public override ImageWrapper ToIco()
        {
            return new ImageWrapperIcon(this);
        }

        public override void SaveToFile(string path)
        {
            SourceImage.Save(path);
        }

        public override string ToBase64String()
        {
            IImageFormat format = _format.ToFormat();
            return SourceImage.ToBase64String(format);
        }
    }
}
