// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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