// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
using System.Windows.Media.Imaging;

namespace JJangdoImageUtil
{
    public class Photo : INotifyPropertyChanged
    {
        protected readonly Uri _source;
        protected ImageWrapper _sourceImage;
        protected ImageFormat _format;
        protected Image _imgControl;

        public event PropertyChangedEventHandler PropertyChanged;


        public Photo(string path)
        {
            ImageFormat format;

            _source = new Uri(path);
            _sourceImage = ImageWrapper.Load(path, out format);

            if (_sourceImage == null)
                throw new Exception("new Photo(string path) failed");

            _format = format;
        }

        public Photo(string path, ImageFormat format)
        {
            _source = new Uri(path);
            _sourceImage = ImageWrapper.Load(path, out format);
            _format = format;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged; 

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

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
        

        public ImageFormat Format => _sourceImage.Format;
        public Stream StreamSource        
        {
            get
            {
                return _sourceImage.Stream;
            }
            set
            {
                OnPropertyChanged("StreamSource");
            }
        }

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
            _sourceImage = _sourceImage.ToJpeg();
        }

        public void ToWebp()
        {
            _sourceImage = _sourceImage.ToWebp();
        }

        public void ToPng()
        {
            _sourceImage = _sourceImage.ToPng();
        }

        public void ToBmp()
        {
            _sourceImage = _sourceImage.ToBmp();
        }

        public void ToTiff()
        {
            _sourceImage = _sourceImage.ToTiff();
        }

        public void ToGif()
        {
            _sourceImage = _sourceImage.ToGif();
        }

        public void ToIco()
        {
            _sourceImage = _sourceImage.ToIco();
        }

        public void Save(string path)
        {
            _sourceImage.SaveToFile(path);
        }


        public string Base64String => _sourceImage.ToBase64String();
        
        public async static Task<Photo> CreateAsnc(string path)
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
                bitmapImg.StreamSource = StreamSource;
                bitmapImg.EndInit();

                _imgControl.Source = bitmapImg;
                _imgControl.EndInit();
            });
        }
    }
}