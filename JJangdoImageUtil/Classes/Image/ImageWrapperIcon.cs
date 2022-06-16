// 작성자 : 윤정도
// ICO 형식을 다룸

using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJangdoImageUtil
{
    using Bitmap = System.Drawing.Bitmap;
    using Icon = System.Drawing.Icon;
    using Image = System.Drawing.Image;

    public class ImageWrapperIcon : ImageWrapper
    {
        public static int IconSize = 128;

        private Bitmap _iconBitmap;
        

        public Bitmap IconBitmap => _iconBitmap;

        public ImageWrapperIcon()
        {
        }

        public ImageWrapperIcon(string path) : base(path)
        {
        }

        public ImageWrapperIcon(ImageWrapperGeneral generalWrapper)
        {
            _stream = new MemoryStream();
            _iconBitmap = ImageConverter.ConvertToIco(generalWrapper.SourceImage, _stream, IconSize, out _format);
        }

        protected override void Initialize(string path)
        {
            _stream = new MemoryStream();
            _iconBitmap = Image.FromFile(path) as Bitmap;
            _iconBitmap.Save(_stream, System.Drawing.Imaging.ImageFormat.Png);
            _format = ImageFormat.Ico;
        }

       
        public ImageWrapperGeneral ToGeneral()
        {
            return new ImageWrapperGeneral(this);
        }

        public override int GetWidth()
        {
            return _iconBitmap.Width;
        }
        public override int GetHeight()
        {
            return _iconBitmap.Height;
        }

        

        public override ImageWrapper ToGif()
        {
            ImageWrapperGeneral general = ToGeneral();
            ImageFormat format;
            general.SourceImage = ImageConverter.ConvertToGif(general.SourceImage, general.Stream, out format);
            general.Format = format;
            return general;
        }

        public override ImageWrapper ToJpeg()
        {
            ImageWrapperGeneral general = ToGeneral();
            ImageFormat format;
            general.SourceImage = ImageConverter.ConvertToJpeg(general.SourceImage, general.Stream, out format);
            general.Format = format;
            return general;
        }

        public override ImageWrapper ToPng()
        {
            // 바꿀때 Png로 변경 후 다른 타입으로 변경하기 때문에 걍 바로 반환하면 댐
            return ToGeneral();
        }

        public override ImageWrapper ToTiff()
        {
            ImageWrapperGeneral general = ToGeneral();
            ImageFormat format;
            general.SourceImage = ImageConverter.ConvertToTiff(general.SourceImage, general.Stream, out format);
            general.Format = format;
            return general;
        }

        public override ImageWrapper ToWebp()
        {
            ImageWrapperGeneral general = ToGeneral();
            ImageFormat format;
            general.SourceImage = ImageConverter.ConvertToWebp(general.SourceImage, general.Stream, out format);
            general.Format = format;
            return general;
        }

        public override ImageWrapper ToBmp()
        {
            ImageWrapperGeneral general = ToGeneral();
            ImageFormat format;
            general.SourceImage = ImageConverter.ConvertToBmp(general.SourceImage, general.Stream, out format);
            general.Format = format;
            return general;
        }

        public override ImageWrapper ToIco()
        {
            // 변경할 필요 없음
            return this;
        }

        public override void RotateClockWise()
        {
            _stream.Reset();
            _iconBitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
            _iconBitmap.Save(_stream, System.Drawing.Imaging.ImageFormat.Png);
        }

        public override void RotateCounterClockWise()
        {
            _stream.Reset();
            _iconBitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
            _iconBitmap.Save(_stream, System.Drawing.Imaging.ImageFormat.Png);
        }

        public override void SetScale(float scaleX, float scaleY, bool keepAspectRatio)
        {
            SetSize((int)(_iconBitmap.Width * scaleX), (int)(_iconBitmap.Height * scaleY), keepAspectRatio);
        }

        public override void SetSize(int width, int height, bool keepAspectRatio)
        {
            if (keepAspectRatio)
            {
                int sourceImgWidth = _iconBitmap.Width;
                int sourceImgHeight = _iconBitmap.Height;

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
            _iconBitmap = new Bitmap(_iconBitmap, width, height);
            _iconBitmap.Save(_stream, System.Drawing.Imaging.ImageFormat.Png);
        }

        public override void SetWidth(int width, bool keepAspectRatio)
        {
            SetSize(width, _iconBitmap.Height, keepAspectRatio);
        }

        public override void SetHeight(int height, bool keepAspectRatio)
        {
            SetSize(_iconBitmap.Width, height, keepAspectRatio);
        }


        // @Icon To Base64 참고 : https://stackoverflow.com/questions/42038872/how-to-convert-icon-into-a-base64-string
        public override string ToBase64String()
        {
            return "data:image/ico;base64," + Convert.ToBase64String(_stream.ToArray());
        }

        public override void SaveToFile(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                _stream.Position = 0;
                Icon icon = ImageConverter.ConvertPngToIcon(_stream, IconSize);
                icon.Save(fileStream);
                icon.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _iconBitmap?.Dispose();
                _iconBitmap = null;
            }

            base.Dispose(disposing);
        }
    }
}

