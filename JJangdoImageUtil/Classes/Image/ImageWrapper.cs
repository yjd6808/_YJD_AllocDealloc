using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace JJangdoImageUtil
{
    public abstract class ImageWrapper : IDisposable
    {
        protected MemoryStream _stream;
        protected ImageFormat _format;
        protected bool _disposed;

        protected ImageWrapper()
        {
            _stream = new MemoryStream();
            _format = ImageFormat.Unknown;
        }

        ~ImageWrapper()
        {
            Dispose(false);
        }

        protected ImageWrapper(string path)
        {
            Initialize(path);
        }

        public MemoryStream Stream => _stream;

        public ImageFormat Format 
        {
            get => _format;
            set => _format = value;
        }

        public Size Size => new Size(GetWidth(), GetHeight());

        protected abstract void Initialize(string path);
        public abstract int GetWidth();
        public abstract int GetHeight();

        public abstract ImageWrapper ToGif();
        public abstract ImageWrapper ToJpeg();
        public abstract ImageWrapper ToPng();
        public abstract ImageWrapper ToTiff();
        public abstract ImageWrapper ToWebp();
        public abstract ImageWrapper ToBmp();
        public abstract ImageWrapper ToIco();
        public abstract void RotateClockWise();
        public abstract void RotateCounterClockWise();
        public abstract string ToBase64String();
        public abstract void SetScale(float scaleX, float scaleY, bool keepAspectRatio);
        public abstract void SetSize(int width, int height, bool keepAspectRatio);
        public abstract void SetWidth(int width, bool keepAspectRatio);
        public abstract void SetHeight(int height, bool keepAspectRatio);
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

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                _stream?.Dispose();
                _stream = null;
            }

            // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
            // TODO: 큰 필드를 null로 설정합니다.

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            // 소멸자를 가지고 있는 객체는 소멸자 큐에 등록이 되는데
            // 이 함수를 호출해서 소멸자 큐에서 이 객체에 대해서 다시 소멸자를 호출하지 않도록 해준다.
            GC.SuppressFinalize(this);  
                                        
        }
    }
}
