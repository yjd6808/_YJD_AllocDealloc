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
using SixLabors.ImageSharp.Metadata.Profiles.Exif;


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
            SourceImage.ApplyExifOrientation();    // 메타데이터 관리까지는 귀찮으므로 오리엔테이션 바로 적용해주자.
            SourceImage.Save(_stream, ImageConverter.FindEncoder(imageFormat));
            _format = imageFormat.ToFormat();
        }

        public override int GetWidth()
        {
            return SourceImage.Width;
        }

        public override int GetHeight()
        {
            return SourceImage.Height;
        }

        public override ImageWrapper ToGif()
        {
            SourceImage = ImageConverter.ConvertToGif(SourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToJpeg()
        {
            SourceImage = ImageConverter.ConvertToJpeg(SourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToPng()
        {
            SourceImage = ImageConverter.ConvertToPng(SourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToTiff()
        {
            SourceImage = ImageConverter.ConvertToTiff(SourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToWebp()
        {
            SourceImage = ImageConverter.ConvertToWebp(SourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToBmp()
        {
            SourceImage = ImageConverter.ConvertToBmp(SourceImage, _stream, out _format);
            return this;
        }

        public override ImageWrapper ToIco()
        {
            return new ImageWrapperIcon(this);
        }

        public override void RotateClockWise()
        {
            _stream.Reset();
            SourceImage.Mutate(x => x.Rotate(RotateMode.Rotate90));
            SourceImage.Save(_stream, _format.ToFormat());
        }

        public override void RotateCounterClockWise()
        {
            _stream.Reset();
            SourceImage.Mutate(x => x.Rotate(-90.0f));
            SourceImage.Save(_stream, _format.ToFormat());
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

        public override void SetScale(float scaleX, float scaleY, bool keepAspectRatio)
        {
            SetSize((int)(SourceImage.Width * scaleX), (int)(SourceImage.Height * scaleY), keepAspectRatio);
        }

        public override void SetSize(int width, int height, bool keepAspectRatio)
        {
            if (keepAspectRatio)
            {
                int sourceImgWidth = SourceImage.Width;
                int sourceImgHeight = SourceImage.Height;

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
            SourceImage.Mutate(x => x.Resize(width, height));
            SourceImage.Save(_stream, _format.ToFormat());
        }

        public override void SetWidth(int width, bool keepAspectRatio)
        {
            SetSize(width, SourceImage.Height, keepAspectRatio);
        }

        public override void SetHeight(int height, bool keepAspectRatio)
        {
            SetSize(SourceImage.Width, height, keepAspectRatio);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SourceImage?.Dispose();
                SourceImage = null;
            }

            base.Dispose(disposing);
        }
    }
}
