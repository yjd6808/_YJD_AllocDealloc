// 작성자 : 윤정도
// SixLabors에서 제공해주는 이미지 포맷처리만 진행하는 클래스

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
        public Image _sourceImage;
        public Image SourceImage => _sourceImage;


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

            _sourceImage = iconWrapper.IconBitmap.ToImageSharpImage(_stream);
        }

        protected override void Initialize(string path)
        {
            _stream = new MemoryStream();
            _sourceImage = Image.Load(path, out var imageFormat);
            _sourceImage.Save(Stream, ImageConverter.FindEncoder(imageFormat));
            _format = imageFormat.ToFormat();
        }

        public override int GetWidth()
        {
            return _sourceImage.Width;
        }

        public override int GetHeight()
        {
            return _sourceImage.Height;
        }

        public override ImageWrapper ToGif()
        {
            _sourceImage = ImageConverter.ConvertToGif(_sourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToJpeg()
        {
            _sourceImage = ImageConverter.ConvertToJpeg(_sourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToPng()
        {
            _sourceImage = ImageConverter.ConvertToPng(_sourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToTiff()
        {
            _sourceImage = ImageConverter.ConvertToTiff(_sourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToWebp()
        {
            _sourceImage = ImageConverter.ConvertToWebp(_sourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToBmp()
        {
            _sourceImage = ImageConverter.ConvertToBmp(_sourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToIco()
        {
            return new ImageWrapperIcon(this);
        }

        public override void RotateClockWise()
        {
            _stream.Reset();
            _sourceImage.Mutate(x => x.Rotate(RotateMode.Rotate90));
            _sourceImage.Save(_stream, _format.ToFormat());
            _stream.Position = 0;

            //switch (_format)
            //{
            //    case ImageFormat.Gif:
            //    case ImageFormat.Jpeg:
            //    case ImageFormat.Png:
            //    case ImageFormat.Webp:
            //    case ImageFormat.Bmp:
            //    case ImageFormat.Ico:
            //    case ImageFormat.Tiff:
                    
            //        break;
            //    case ImageFormat.Unknown:
            //    {
            //        ToPng();
            //        RotateClockWise();
            //        ToTiff();
            //        break;
            //    }
            //}
        }

        public override void RotateCounterClockWise()
        {
            _stream.Reset();
            _sourceImage.Mutate(x => x.Rotate(-90.0f));
            _sourceImage.Save(_stream, _format.ToFormat());
            _stream.Position = 0;
        }


       

        public override void SaveToFile(string path)
        {
            _sourceImage.Save(path);
        }

        public override string ToBase64String()
        {
            IImageFormat format = _format.ToFormat();
            return _sourceImage.ToBase64String(format);
        }

        public override void SetScale(float scaleX, float scaleY, bool keepAspectRatio)
        {
            SetSize((int)(_sourceImage.Width * scaleX), (int)(_sourceImage.Height * scaleY), keepAspectRatio);
        }

        public override void SetSize(int width, int height, bool keepAspectRatio)
        {
            if (keepAspectRatio)
            {
                int sourceImgWidth = _sourceImage.Width;
                int sourceImgHeight = _sourceImage.Height;

                // 생각1.
                // ex) 기존 이미지 가로 길이 : 400
                //     기존 이미지 세로 길이 : 300
                //     변경 이미지 가로 길이 : 500
                //     변경 이미지 세로 길이 : 300
                //     
                //     originalRatio = 400 / 300
                //     destinationRatio = 500 / 300
                //
                //     detinationRatio > originalRatio 이므로
                //     가로길이가 더 길어진 상태이다.
                //     세로 길이를 높여줘서 크기를 맞춰준다.
                //     x / height = originalRatio
                //     x = width / originalRatio 
                // float originalRatio = (float)(_sourceImage.Width) / _sourceImage.Height;
                // float destinationRatio = (float)(width) / height;


                // 생각2.
                // 변화량이 더 큰 쪽기준으로 크기를 결정하도록 한다. 
                // 너비 변화량, 높이 변화량
                float widthRatio = (float)(width - sourceImgWidth) / sourceImgWidth;
                float heightRatio = (float)(height - sourceImgHeight) / sourceImgHeight;

                float absWidthRatio = Math.Abs(widthRatio);
                float absHeightRatio = Math.Abs(heightRatio);

                if (absWidthRatio > absHeightRatio)
                {
                    sourceImgHeight += (int)(sourceImgHeight * widthRatio);
                    height = sourceImgHeight;
                }
                else
                {
                    sourceImgWidth += (int)(sourceImgWidth * heightRatio);
                    width = sourceImgWidth;
                }
            }

            _stream.Reset();
            _sourceImage.Mutate(x => x.Resize(width, height));
            _sourceImage.Save(_stream, _format.ToFormat());
            _stream.Position = 0;
        }

        public override void SetWidth(int width, bool keepAspectRatio)
        {
            SetSize(width, _sourceImage.Height, keepAspectRatio);
        }

        public override void SetHeight(int height, bool keepAspectRatio)
        {
            SetSize(_sourceImage.Width, height, keepAspectRatio);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sourceImage?.Dispose();
                _sourceImage = null;
            }

            base.Dispose(disposing);
        }
    }
}
