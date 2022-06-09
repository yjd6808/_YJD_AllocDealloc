// 작성자 : 윤정도
// 드래그 앤 드랍으로 로딩된 사진
// @기반 소스코드 : https://github.com/microsoft/WPF-Samples/tree/master/Sample%20Applications/PhotoViewerDemo

using System;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Advanced;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JJangdoImageUtil
{
    public class Photo : INotifyPropertyChanged, IDisposable
    {
        private static int SequenceId = 0;

        protected readonly int _uniqueId;
        protected readonly Uri _source;
        protected ImageWrapper _sourceImage;
        protected Image _imgControl;

        public event PropertyChangedEventHandler PropertyChanged;


        public Photo(string path)
        {
            ImageFormat format;

            _uniqueId = Interlocked.Increment(ref SequenceId);
            _source = new Uri(path);
            _sourceImage = ImageWrapper.Load(path, out format);

            if (_sourceImage == null)
                throw new Exception("new Photo(string path) failed");
        }

        public Photo(string path, ImageFormat format)
        {
            _uniqueId = Interlocked.Increment(ref SequenceId);
            _source = new Uri(path);
            _sourceImage = ImageWrapper.Load(path, out format);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged; 

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public ImageWrapper SourceImage
        {
            private set
            {
                _sourceImage = value;

                OnPropertyChanged("SourceImage");
            }
            get => _sourceImage;
        }

        public int UniqueId => _uniqueId;
        public string Source => _source.ToString();

        // @uri에서 파일이름 가져오기 : https://stackoverflow.com/questions/1105593/get-file-name-from-uri-string-in-c-sharp
        public string FileName
        {
            get
            {
                if (_source.IsFile)
                    return Path.GetFileName(_source.LocalPath);

                return "파일 아님";
            }
        }

        public SixLabors.ImageSharp.Size Size => _sourceImage.Size;

        public string Base64String => _sourceImage.ToBase64String();

        public Image BindedControl => _imgControl;


        public string Base64ImgTagString => string.Format($"<img src=\"{Base64String}\"/>");


        public override string ToString() => _source.ToString();


        public void To(ImageFormat Type)
        {
            switch (Type)
            {
                case ImageFormat.Jpeg:  ToJpeg(); break;
                case ImageFormat.Png:   ToPng();  break;
                case ImageFormat.Webp:  ToWebp(); break;
                case ImageFormat.Bmp:   ToBmp();  break;
                case ImageFormat.Tiff:  ToTiff(); break;
                case ImageFormat.Gif:   ToGif();  break;
                case ImageFormat.Ico:   ToIco();  break;
                default: throw new NotImplementedException();
            }
        }

        public void ToJpeg()
        {
            SourceImage = _sourceImage.ToJpeg();
            UpdateBitmapSource();
        }

        public void ToWebp()
        {
            SourceImage = _sourceImage.ToWebp();
            UpdateBitmapSource();
        }

        public void ToPng()
        {
            SourceImage = _sourceImage.ToPng();
            UpdateBitmapSource();
        }

        public void ToBmp()
        {
            SourceImage = _sourceImage.ToBmp();
            UpdateBitmapSource();
        }

        public void ToTiff()
        {
            SourceImage = _sourceImage.ToTiff();
            UpdateBitmapSource();
        }

        public void ToGif()
        {
            SourceImage = _sourceImage.ToGif();
            UpdateBitmapSource();
        }

        public void ToIco()
        {
            SourceImage = _sourceImage.ToIco();
            UpdateBitmapSource();
        }

        public void RotateClockWise()
        {
            _sourceImage.RotateClockWise();
            SourceImage = _sourceImage;
            UpdateBitmapSource();
        }

        public void RotateCounterClockWise()
        {
            _sourceImage.RotateCounterClockWise();
            SourceImage = _sourceImage;
            UpdateBitmapSource();
        }

        public void SetScale(float scaleX, float scaleY, bool keepAspectRatio)
        {
            _sourceImage.SetScale(scaleX, scaleY, keepAspectRatio);
            SourceImage = _sourceImage;
            UpdateBitmapSource();
        }

        public void SetSize(int width, int height, bool keepAspectRatio)
        {
            _sourceImage.SetSize(width, height, keepAspectRatio);
            SourceImage = _sourceImage;
            UpdateBitmapSource();
        }

        public void SetWidth(int width, bool keepAspectRatio)
        {
            _sourceImage.SetWidth(width, keepAspectRatio);
            SourceImage = _sourceImage;
            UpdateBitmapSource();
        }

        public void SetHeight(int height, bool keepAspectRatio)
        {
            _sourceImage.SetHeight(height, keepAspectRatio);
            SourceImage = _sourceImage;
            UpdateBitmapSource();
        }

        public void Save(string directoryPath, bool saveWithUniqueId = true)
        {
            string filePath = string.Empty;

            if (saveWithUniqueId)
                filePath = Path.Combine(directoryPath, _uniqueId + _sourceImage.Format.ToExtensionString());
            else
            {
                filePath = Path.Combine(directoryPath, FileName);
                filePath = Path.ChangeExtension(filePath, _sourceImage.Format.ToExtensionString());
            }

            _sourceImage.SaveToFile(filePath);
        }

        public static async Task<Photo> CreateAsnc(string path)
        {
            var dt = DateTime.Now;
            ImageFormat imageFormat = ImageUtil.DetectImageFormat(new FileInfo(path));

            if (imageFormat == ImageFormat.Unknown)
                return null;

            var photo = await Task.Run(() => new Photo(path, imageFormat));

            var sp = DateTime.Now - dt;
            return photo;
        }

        public void SetImageControl(Image source)
        {
            _imgControl = source;
        }

        public void UpdateBitmapSource()
        {
            _imgControl.Dispatcher.Invoke(() =>
            {
                _imgControl.BeginInit();

                var bitmapImg = new BitmapImage();
                bitmapImg.BeginInit();
                bitmapImg.StreamSource = _sourceImage.Stream;
                bitmapImg.EndInit();

                _imgControl.Source = bitmapImg;
                _imgControl.EndInit();
            });
        }

        public void Dispose()
        {
            _sourceImage?.Dispose();
        }
    }
}