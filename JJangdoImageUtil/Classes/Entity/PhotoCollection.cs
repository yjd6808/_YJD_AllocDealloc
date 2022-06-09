// 작성자 : 윤정도
// 리스트 박스에 바인딩된 콜렉션
// @기반 소스코드 : https://github.com/microsoft/WPF-Samples/tree/master/Sample%20Applications/PhotoViewerDemo

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace JJangdoImageUtil
{
    public class PhotoCollection : ObservableCollection<Photo>
    {
        public Dispatcher Dispatcher { get; set; }

        public void Add(string imagePath)
        {
            ImageFormat imageFormat = ImageUtil.DetectImageFormat(new FileInfo(imagePath));

            if (imageFormat == ImageFormat.Unknown)
                throw new InvalidImageFormatException();

            Add(new Photo(imagePath, imageFormat));
        }

        public async Task AddAsync(string path)
        {
            Photo photo = await Photo.CreateAsnc(path);
            Dispatcher.Invoke(() => Add(photo));
        }
    }
}