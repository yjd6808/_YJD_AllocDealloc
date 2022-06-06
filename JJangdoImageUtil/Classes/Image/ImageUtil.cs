// 작성자 : 윤정도
// 
// 이미지 관련 다루는 전체적인 기능 제공

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
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

    public class ImageUtil
    {
        public static bool IsImageFile(FileInfo fileInfo)
        {
            string ext = fileInfo.Extension.ToLower();

            if (string.IsNullOrWhiteSpace(ext))
                return false;

             switch (ext)
             {
                    case ".jpg":
                    case ".png":    
                    case ".bmp":    
                    case ".tiff":   
                    case ".webp":   
                    case ".gif":
                    case ".ico":
                    return true;
             }

            return false;
        }


        public static ImageFormat DetectImageFormat(FileInfo fileInfo)
        {
            IImageFormat detectedFormat = Image.DetectFormat(fileInfo.FullName);

            if (detectedFormat == null)
            {
                System.Drawing.Image bitmap = System.Drawing.Image.FromFile(fileInfo.FullName);

                if (bitmap.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                    return ImageFormat.Ico;


                return ImageFormat.Unknown;
            }

            return detectedFormat.ToFormat();
        }

        public static bool IsImageFile(string path)
        {
            return IsImageFile(new FileInfo(path));
        }
    }
}
