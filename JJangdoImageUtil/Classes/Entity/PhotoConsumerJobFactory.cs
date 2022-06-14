// 작성자 : 윤정도
// 이미지 처리 관련 작업을 생성해주는 팩토리

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace JJangdoImageUtil
{

    public static class PhotoConsumerJobFactory
    {
        public enum JobType
        {
            Add,
            RotateClockwise,
            RotateCounterClockwise,
            ConvertGif,
            ConvertPng,
            ConvertJpeg,
            ConvertTiff,
            ConvertBmp,
            ConvertIco,
            ConvertWebp,
            Save,
            CopyBase64ImgTag,
            ChangeSizeWithScaleValue,
            ChangeSizeWithFixedValue,
        }

        public static readonly string ADD_TEXT = "이미지 추가 작업 진행 중...";
        public static readonly string ROTATE_CLOCKWISE_TEXT = "시계 방향 회전 작업 진행 중...";
        public static readonly string ROTATE_COUNTERCLOCKWISE_TEXT = "반시계 방향 회전 작업 진행 중...";
        public static readonly string CONVERT_GIF_TEXT = "GIF 변환 작업 진행 중...";
        public static readonly string CONVERT_PNG_TEXT = "PNG 변환 작업 진행 중...";
        public static readonly string CONVERT_JPEG_TEXT = "JPEG 변환 작업 진행 중...";
        public static readonly string CONVERT_TIFF_TEXT = "TIFF 변환 작업 진행 중...";
        public static readonly string CONVERT_BMP_TEXT = "BMP 변환 작업 진행 중...";
        public static readonly string CONVERT_ICO_TEXT = "ICO 변환 작업 진행 중...";
        public static readonly string CONVERT_WEBP_TEXT = "WEBP 변환 작업 진행 중...";
        public static readonly string SAVE_TEXT = "이미지 파일로 저장 작업 진행 중...";
        public static readonly string COPY_BASE64_IMG_TAG_TEXT = "BASE64 IMG 클립보드 복사 진행 중...";
        public static readonly string CHANGE_SIZE_WITH_SCALE_VALUE_TEXT = "이미지 크기 변환 진행 중...";
        public static readonly string CHANGE_SIZE_WITH_FIXED_VALUE_TEXT = "이미지 크기 변환 진행 중...";

        public static Dictionary<int, string> JobNameMap = new Dictionary<int, string>()
        {
            { (int)JobType.Add,                      ADD_TEXT                                   },
            { (int)JobType.RotateClockwise,          ROTATE_CLOCKWISE_TEXT                      },
            { (int)JobType.RotateCounterClockwise,   ROTATE_COUNTERCLOCKWISE_TEXT               },
            { (int)JobType.ConvertGif,               CONVERT_GIF_TEXT                           },
            { (int)JobType.ConvertPng,               CONVERT_PNG_TEXT                           },
            { (int)JobType.ConvertJpeg,              CONVERT_JPEG_TEXT                          },
            { (int)JobType.ConvertTiff,              CONVERT_TIFF_TEXT                          },
            { (int)JobType.ConvertBmp,               CONVERT_BMP_TEXT                           },
            { (int)JobType.ConvertIco,               CONVERT_ICO_TEXT                           },
            { (int)JobType.ConvertWebp,              CONVERT_WEBP_TEXT                          },
            { (int)JobType.Save,                     SAVE_TEXT                                  },
            { (int)JobType.CopyBase64ImgTag,         COPY_BASE64_IMG_TAG_TEXT                   },
            { (int)JobType.ChangeSizeWithScaleValue, CHANGE_SIZE_WITH_SCALE_VALUE_TEXT          },
            { (int)JobType.ChangeSizeWithFixedValue, CHANGE_SIZE_WITH_FIXED_VALUE_TEXT          },
        };

        public static readonly StringBuilder Base64Builder = new StringBuilder(10000000);
        public static Dispatcher Dispatcher;

        public static ObservableJob CreateAdd(List<string> files)
        {
            var funcs = new List<Func<Photo>>();

            foreach (var file in files)
                funcs.Add(() => new Photo(file));

            return new CreateMultiJob<Photo>((int)JobType.Add, funcs);
        }

        public static ObservableJob CreateRotate(ImageRotate rotate)
        {
            switch (rotate)
            {
                case ImageRotate.ClockWise:
                    return new UpdateMultiJob<Photo>((int)JobType.RotateClockwise, (photo) => photo.RotateClockWise());
                case ImageRotate.CounterClockWise:
                    return new UpdateMultiJob<Photo>((int)JobType.RotateCounterClockwise, (photo) => photo.RotateCounterClockWise());
            }

            return null;
        }

        public static ObservableJob CreateConvert(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.Gif:
                    return new UpdateMultiJob<Photo>((int)JobType.ConvertGif, (photo) => photo.ToGif());
                case ImageFormat.Png:
                    return new UpdateMultiJob<Photo>((int)JobType.ConvertPng, (photo) => photo.ToPng());
                case ImageFormat.Jpeg:
                    return new UpdateMultiJob<Photo>((int)JobType.ConvertJpeg, (photo) => photo.ToJpeg());
                case ImageFormat.Tiff:
                    return new UpdateMultiJob<Photo>((int)JobType.ConvertTiff, (photo) => photo.ToTiff());
                case ImageFormat.Webp:
                    return new UpdateMultiJob<Photo>((int)JobType.ConvertWebp, (photo) => photo.ToWebp());
                case ImageFormat.Bmp:
                    return new UpdateMultiJob<Photo>((int)JobType.ConvertBmp, (photo) => photo.ToBmp());
                case ImageFormat.Ico:
                    return new UpdateMultiJob<Photo>((int)JobType.ConvertIco, (photo) => photo.ToIco());
            }


            return null;
        }

        public static ObservableJob CreateSave(string directoryPath, bool saveWithUniqueId)
        {
            if (!Directory.Exists(directoryPath))
                throw new Exception("해당 경로가 존재하지 않습니다.");

            return new UpdateMultiJob<Photo>((int)JobType.Save, (photo) => photo.Save(directoryPath, saveWithUniqueId));
        }

        
        public static UpdateSingleJob<Photo> CreateCopyBase64ImgTagSingleJob(bool breakLine)
        {
            // 멀티잡으로 하게되면 Base64Builder 락이 필요해서 싱글로 해야함
            var singleJob = new UpdateSingleJob<Photo>((int)JobType.CopyBase64ImgTag, (photo) => Base64Builder.Append(photo.Base64ImgTagString));

            singleJob.OnStarted += (sender, args) => Base64Builder.Clear();  // 시작할때 버퍼를 비워주도록 하자.
            singleJob.OnFinished += (sender, args) =>
            {
                if (Base64Builder.Length == 0)
                    return;

                // Clipboard가 STA 쓰레드에서 사용가능하기땜에 UI쓰레드가 STA쓰레드이므로 걍 여기서 실행하자.
                // 새로운 STA 쓰레드 만들어서 실행하기에는 아깝다.
                Dispatcher.Invoke(() => Clipboard.SetText(Base64Builder.ToString()));

                // 메시지 박스자체가 닫힐때까지 동기상태로 대기해야하므로 외부 쓰레드에서 보여주도록 하자.
                Task.Run(() => MsgBox.ShowTopMost($"클립보드에 복사되었습니다. (용량 : {(double)(Base64Builder.Length * sizeof(char)) / (1024 * 1024):0.##} MB)"));
            };

            // 개행 추가여부
            if (breakLine)
            {
                singleJob.OnCompleted += (sender, args) => Base64Builder.AppendLine("<br>");
            }

            return singleJob;
        }

        public static ObservableJob CreateSetScale(float scaleX, float scaleY, bool keepAspectRatio)
        {
            return new UpdateMultiJob<Photo>((int)JobType.ChangeSizeWithScaleValue,
                (photo) => photo.SetScale(scaleX, scaleY, keepAspectRatio));
        }

        public static ObservableJob CreateSetWidth(int width, bool keepAspectRatio)
        {
            return new UpdateMultiJob<Photo>((int)JobType.ChangeSizeWithFixedValue,
                (photo) => photo.SetWidth(width, keepAspectRatio));
        }

        public static ObservableJob CreateSetHeight(int height, bool keepAspectRatio)
        {
            return new UpdateMultiJob<Photo>((int)JobType.ChangeSizeWithFixedValue,
                (photo) => photo.SetHeight(height, keepAspectRatio));
        }

        public static ObservableJob CreateSetSize(int width, int height, bool keepAspectRatio)
        {
            return new UpdateMultiJob<Photo>((int)JobType.ChangeSizeWithFixedValue,
                (photo) => photo.SetSize(width, height, keepAspectRatio));
        }
    }
}
