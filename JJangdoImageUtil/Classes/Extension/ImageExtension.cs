// @기반 소스 : https://gist.github.com/vurdalakov/00d9471356da94454b372843067af24e

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;


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


        // 이미지 메타데이터 정보중 Orientation 정보를 실제 이미지에 반영
        public static void ApplyExifOrientation(this Image image)
        {
             IExifValue<ushort> orientationExifMetadata = image.Metadata?.ExifProfile?.GetValue(ExifTag.Orientation);
             
            if (orientationExifMetadata == null)
                return;

            ImageOrientation orientation = (ImageOrientation)orientationExifMetadata.Value;

            switch (orientation)
            {
                case ImageOrientation.Default:
                    // 아무것도 안해도댐
                    break;
                case ImageOrientation.DefaultHorizontalMirrored:
                    // 좌우 반전 다시 해줘서 원래상태로.
                    image.Mutate(x => x.RotateFlip(RotateMode.None, FlipMode.Horizontal));      
                    break;
                case ImageOrientation.Rotate180:
                    // 180도 회전된 상태이므로, 다시 180도 돌려줘서 원래상태로...
                    image.Mutate(x => x.RotateFlip(RotateMode.Rotate180, FlipMode.None));       
                    break;
                case ImageOrientation.Rotate180HorizontalMirrored:
                    // 180도 회전 후 좌우 반전 상태이므로, 180도 돌려준 후 다시 좌우 반전해주자.
                    image.Mutate(x => x.RotateFlip(RotateMode.Rotate180, FlipMode.Horizontal));
                    break;
                case ImageOrientation.HorizontalMirroredRotate270:
                    // 좌우 반전 후 270도 우측방향으로 돌아간 상태이므로, 90도 우측방향으로 돌려준 후 다시 좌우 반전해주자.
                    image.Mutate(x => x.RotateFlip(RotateMode.Rotate90, FlipMode.Horizontal));
                    break;
                case ImageOrientation.Rotate270:
                    // 270도 돌아간 상태이므로, 90도 돌려주자.
                    image.Mutate(x => x.RotateFlip(RotateMode.Rotate90, FlipMode.None));
                    break;
                case ImageOrientation.HorizontalMirroredRotate90:
                    // 좌우 반전 후 90도 돌아간 상태이므로, 270도 돌려주고 좌우반전해주자.
                    image.Mutate(x => x.RotateFlip(RotateMode.Rotate270, FlipMode.Horizontal));
                    break;
                case ImageOrientation.Rotate90:
                    image.Mutate(x => x.RotateFlip(RotateMode.Rotate270, FlipMode.None));
                    break;
            }

            // 회전 완료해줬으니 메타데이터 정보를 디폴트로 바꿔주자.
            orientationExifMetadata.TrySetValue(Convert.ToUInt16(ImageOrientation.Default));
        }
    }
}
