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
            _iconBitmap = ImageConverter.ConvertToIco(generalWrapper.SourceImage, _stream);
            _format = ImageFormat.Ico;
        }

        protected override void Initialize(string path)
        {
            _stream = new MemoryStream();
            _iconBitmap = Image.FromFile(path) as Bitmap;
            _iconBitmap.Save(_stream, System.Drawing.Imaging.ImageFormat.Png);
            _format = ImageFormat.Ico;
        }

        public Icon CreateIcon()
        {
            return _iconBitmap.ToIcon();
        }

        public ImageWrapperGeneral ToGeneral()
        {
            return new ImageWrapperGeneral(this);
        }

        public override ImageWrapper ToGif()
        {
            ImageWrapperGeneral general = ToGeneral();
            general.SourceImage = ImageConverter.ConvertToGif(general.SourceImage, general.Stream);
            return general;
        }

        public override ImageWrapper ToJpeg()
        {
            ImageWrapperGeneral general = ToGeneral();
            general.SourceImage = ImageConverter.ConvertToJpeg(general.SourceImage, general.Stream);
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
            general.SourceImage = ImageConverter.ConvertToTiff(general.SourceImage, general.Stream);
            return general;
        }

        public override ImageWrapper ToWebp()
        {
            ImageWrapperGeneral general = ToGeneral();
            general.SourceImage = ImageConverter.ConvertToWebp(general.SourceImage, general.Stream);
            return general;
        }

        public override ImageWrapper ToBmp()
        {
            ImageWrapperGeneral general = ToGeneral();
            general.SourceImage = ImageConverter.ConvertToBmp(general.SourceImage, general.Stream);
            return general;
        }

        public override ImageWrapper ToIco()
        {
            // 변경할 필요 없음
            return this;
        }


        // @Icon To Base64 참고 : https://stackoverflow.com/questions/42038872/how-to-convert-icon-into-a-base64-string
        public override string ToBase64String()
        {
            return Convert.ToBase64String(_stream.ToArray());
        }

        public override void SaveToFile(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                Icon icon = CreateIcon();
                icon.Save(fileStream);
            }
        }
    }
}

