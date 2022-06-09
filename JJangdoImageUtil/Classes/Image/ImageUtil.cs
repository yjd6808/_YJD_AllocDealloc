// 작성자 : 윤정도
// 이미지 관련 부가기능

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


        // 파일의 헤더를 읽어서 이미지 파일여부를 확실하게 검사
        // 보통 확장자로 검사하는데 좀 더 안정적인 이미지 로딩을 위해서 이렇게 했다.
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
