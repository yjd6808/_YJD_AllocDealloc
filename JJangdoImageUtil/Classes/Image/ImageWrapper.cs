using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    public abstract class ImageWrapper
    {
        protected MemoryStream _stream;
        protected ImageFormat _format;

        protected ImageWrapper()
        {
            _stream = new MemoryStream();
            _format = ImageFormat.Unknown;
        }

        protected ImageWrapper(string path)
        {
            Initialize(path);
        }

        public MemoryStream Stream => _stream;
        public ImageFormat Format => _format;

        protected abstract void Initialize(string path);

        public abstract ImageWrapper ToGif();
        public abstract ImageWrapper ToJpeg();
        public abstract ImageWrapper ToPng();
        public abstract ImageWrapper ToTiff();
        public abstract ImageWrapper ToWebp();
        public abstract ImageWrapper ToBmp();
        public abstract ImageWrapper ToIco();
        public abstract string ToBase64String();
        public abstract void SaveToFile(string path);


        public static ImageWrapper Load(string path, out ImageFormat format)
        {
            format = ImageUtil.DetectImageFormat(new FileInfo(path));

            switch (format)
            {
                case ImageFormat.Gif:
                case ImageFormat.Jpeg:
                case ImageFormat.Png:
                case ImageFormat.Tiff:
                case ImageFormat.Webp:
                case ImageFormat.Bmp:
                    return new ImageWrapperGeneral(path);
                case ImageFormat.Ico:
                    return new ImageWrapperIcon(path);
            }

            return null;
        }
    }
}
