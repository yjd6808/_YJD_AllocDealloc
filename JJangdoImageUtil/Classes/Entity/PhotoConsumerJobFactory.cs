using System;
using System.Collections.Generic;

namespace JJangdoImageUtil
{
    public static class PhotoConsumerJobFactory
    {
        public static readonly string ADD_PHOTO_TEXT = "이미지 추가 작업 진행 중...";

        public static CreateMultiJob<Photo> AddPhotosMultiJob(List<string> files)
        {
            var funcs = new List<Func<Photo>>();

            foreach (var file in files)
                funcs.Add(() => new Photo(file));

            return new CreateMultiJob<Photo>(ADD_PHOTO_TEXT, funcs);
        }

        public static CreateSingleJob<Photo> AddPhotosSingleJob(List<string> files)
        {
            var funcs = new List<Func<Photo>>();

            foreach (var file in files)
                funcs.Add(() => new Photo(file));

            return new CreateSingleJob<Photo>(ADD_PHOTO_TEXT, funcs);
        }
    }
}
