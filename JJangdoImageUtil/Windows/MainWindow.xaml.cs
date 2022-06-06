// 작성자 : 윤정도

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using MoreLinq.Extensions;
using MoreLinq.Experimental;
using System.Threading;
using System.Diagnostics;

namespace JJangdoImageUtil
{
    public partial class MainWindow : Window
    {
        public PhotoCollection Photos;
        public ObservableCollectionConsumer<Photo> PhotosConsumer;

        public MainWindow()
        {
            InitializeComponent();

            // App.xaml에 ObjectDataProvider 추가해줘야 정상 동작
            Photos = (PhotoCollection)(Application.Current.Resources["Photos"] as ObjectDataProvider)?.Data;
            Photos.Dispatcher = Dispatcher;
            PhotosConsumer = new ObservableCollectionConsumer<Photo>(50, Photos, Dispatcher);
            PhotosConsumer.Start();
            ObservableJob.SetDispatcher(Dispatcher);

            PhotosConsumer.OnEnqueueJob += PhotosConsumer_OnEnqueueJob; ;
            PhotosConsumer.OnFinishedJob += PhotosConsumer_OnFinishedJob;
            PhotosConsumer.OnCanceledJob += PhotosConsumer_OnCanceledJob;
            PhotosConsumer.OnUpdatedJob += PhotosConsumer_OnUpdatedJob;
            PhotosConsumer.OnStartedJob += PhotosConsumer_OnStartedJob;
        }

        private void PhotosConsumer_OnEnqueueJob(object sender, EventArgs e)
        {
            _showWaitingJobButton.Content = $"{PhotosConsumer.JobCount}";
        }

        private void PhotosConsumer_OnStartedJob(ObservableJob startedJob)
        {
            SetVisibleProgress(true);

            _progressBar.Value = 0;
            _progressJobName.Text = startedJob.GetName();
            _progressText.Text = $"{0} / {startedJob.TotalTargetCount()}";
        }

        private void PhotosConsumer_OnUpdatedJob(ObservableJob doingJob)
        {
            int completedCount = doingJob.CompletedTargetCount();
            int failedCount = doingJob.FailedTargetCount();
            int totalCount = doingJob.TotalTargetCount();

            _progressBar.Value = (double)(completedCount + failedCount) / totalCount * 100.0;
            _progressText.Text = $"{completedCount + failedCount} / {totalCount}";
        }

        private void PhotosConsumer_OnFinishedJob(ObservableJob finishedJob)
        {
            int waitingJobCount = PhotosConsumer.WaitingJobCount;

            if (waitingJobCount == 0)
                SetVisibleProgress(false);

            _showWaitingJobButton.Content = $"{PhotosConsumer.WaitingJobCount}";
        }

        private void PhotosConsumer_OnCanceledJob(ObservableJob canceledJob)
        {
            int waitingJobCount = PhotosConsumer.WaitingJobCount;

            if (waitingJobCount == 0)
                SetVisibleProgress(false);

            _showWaitingJobButton.Content = $"{PhotosConsumer.WaitingJobCount}";
        }

        private void SetVisibleProgress(bool visible)
        {
            if (visible)
            {
                _progressBar.Visibility = Visibility.Visible;
                _progressJobName.Visibility = Visibility.Visible;
                _progressText.Visibility = Visibility.Visible;
            }
            else
            {
                _progressBar.Visibility = Visibility.Hidden;
                _progressJobName.Visibility = Visibility.Hidden;
                _progressText.Visibility = Visibility.Hidden;
            }
        }

        private void _window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Opacity = 0.3;
                DragMove();
                Opacity = 1.0;
            }
        }

        private void _minimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void _maximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void _closeBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.Stop();

            Close();
        }

        private void _saveGifBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(new UpdateMultiJob<Photo>("작업", (t) =>
            {
                t.ToGif();
            }));

            PhotosConsumer.EnqueueJob(new UpdateSingleJob<Photo>("작업", (t) =>
            {
                t.UpdateBitmapSource();
            }));
        }

        private void _photoListGrid_Drop(object sender, DragEventArgs e)
        {
            // 드래그 앤 드랍
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<string> files = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
                int aloreadExistCount = 0;

                // 중복된 파일이 있는지 검사
                files = files.FindAll(file =>
                {
                    foreach (Photo photo in Photos)
                    {
                        if (Path.Equals(photo.Source, file))
                        {
                            aloreadExistCount++;
                            return false;
                        }
                    }

                    return true;
                });

                if (files.Count == 0)
                {
                    MsgBox.ShowTopMost($"드랍한 이미지 파일중 추가 가능한 이미지 파일이 없습니다.\n(이미 등록된 이미지 파일 수 : {aloreadExistCount})");
                    return;
                }

                // 이미지 파일이 아닌녀석들 제거
                files.RemoveAll((x) => !ImageUtil.IsImageFile(x));
                PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.AddPhotosMultiJob(files));
            }
        }

        private void _photoListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void _photoListBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            ContentControl contentControl = sender as ContentControl;

            if (contentControl == null)
                return;

            Photo photo = contentControl.DataContext as Photo;

            if (photo == null)
                return;

            var imgCtrl = contentControl.FindChild<Image>();

            if (imgCtrl == null)
                return;

            photo.SetImageControl(imgCtrl);
        }
    }
}
