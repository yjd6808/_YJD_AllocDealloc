// 작성자 : 윤정도

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Text.RegularExpressions;
using System.Windows.Media.Animation;

namespace JJangdoImageUtil
{
    public partial class MainWindow : Window
    {
        public PhotoCollection Photos;
        public ObservableCollectionConsumer<Photo> PhotosConsumer;

        private Photo _draggedPhoto;
        private Photo _onMousedPhoto;

        public MainWindow()
        {
            InitializeComponent();
            
            // App.xaml에 ObjectDataProvider 추가해줘야 정상 동작
            Photos = (PhotoCollection)(Application.Current.Resources["Photos"] as ObjectDataProvider)?.Data;
            Photos.Dispatcher = Dispatcher;
            Photos.CollectionChanged += Photos_CollectionChanged;


            PhotosConsumer = new ObservableCollectionConsumer<Photo>(50, Photos, Dispatcher);
            PhotosConsumer.Start();
            App.Window = this;
            PhotoConsumerJobFactory.Dispatcher = Dispatcher;

            PhotosConsumer.OnEnqueueJob += PhotosConsumer_OnEnqueueJob; ;
            PhotosConsumer.OnFinishedJob += PhotosConsumer_OnFinishedJob;
            PhotosConsumer.OnCanceledJob += PhotosConsumer_OnCanceledJob;
            PhotosConsumer.OnUpdatedJob += PhotosConsumer_OnUpdatedJob;
            PhotosConsumer.OnStartedJob += PhotosConsumer_OnStartedJob;

            _circularProgress.SetColor(Brushes.LightSteelBlue);
            _circularProgress.SetScale(0.8);

            // @참고 : https://stackoverflow.com/questions/4623882/mouseleftbuttondown-not-recognized-by-a-listbox
            // 리스트 박스의 마우스 왼쪽 클릭 안되는 이유 : 리스트박스아이템이 이 이벤트를 처리하게 되어있기 때문에
            _photoListBox.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(_photoListBox_MouseLeftButtonDown), true);
            _photoListBox.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(_photoListBox_MouseLeftButtonUp), true);

            SetVisibleProgress(false);
        }

        private void Photos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _photoListEmptyPanel.Visibility = Photos.Count == 0 ? Visibility.Visible : Visibility.Hidden;

            Photo oldItem = e.OldItems != null && e.OldItems.Count > 0 ? e.OldItems[0] as Photo : null;
            Photo newItem = e.NewItems != null && e.NewItems.Count > 0 ? e.NewItems[0] as Photo : null;

            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                if (e.OldItems.Count == 0 || e.NewItems.Count == 0)
                    return;

                


            }
        }

        private void PhotosConsumer_OnEnqueueJob(object sender, EventArgs e)
        {
            _showWaitingJobButton.Content = $"{PhotosConsumer.JobCount}";

            _showWaitingJobButtonStoryBoard.Begin();
            _progressStoryBoard.Begin();
        }

        private void PhotosConsumer_OnStartedJob(ObservableJob startedJob)
        {
            SetVisibleProgress(true);

            _progressBar.Value = 0;
            _progressJobName.Text = PhotoConsumerJobFactory.JobNameMap[startedJob.Id];
            _progressText.Text = $"{0} / {startedJob.TotalTargetCount}";
        }

        private void PhotosConsumer_OnUpdatedJob(ObservableJob doingJob)
        {
            int completedCount = doingJob.CompletedTargetCount;
            int failedCount = doingJob.FailedTargetCount;
            int totalCount = doingJob.TotalTargetCount;

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
                _circularProgress.Visibility = Visibility.Visible;
            }
            else
            {
                _progressBar.Visibility = Visibility.Hidden;
                _progressJobName.Visibility = Visibility.Hidden;
                _progressText.Visibility = Visibility.Hidden;
                _circularProgress.Visibility = Visibility.Hidden;
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
            string msg = "진행중인 작업이 있습니다. \n강제 종료시 잘못된 결과를 얻을 수 있습니다.\n정말로 종료하사겠습니까?";

            if (PhotosConsumer.IsProcessing() && MsgBox.Show(msg, "질문", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                return;

            PhotosConsumer.Stop();
            Close();
        }



        private void _photoListGrid_Drop(object sender, DragEventArgs e)
        {
            // 드래그 앤 드랍
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<string> files = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();

                // 이미지 파일이 아닌녀석들 제거
                files.RemoveAll((x) => !ImageUtil.IsImageFile(x));

                int exceptedFiles = files.Count((x) =>
                {
                    using (var fs = new FileStream(x, FileMode.Open, FileAccess.Read))
                    {
                        var frame = BitmapFrame.Create(fs, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                        return frame.PixelWidth >= Photo.PossibleSize.Width || frame.PixelHeight >= Photo.PossibleSize.Height;
                    }
                });

                if (exceptedFiles > 0 && MsgBox.ShowTopMost($"가로 또는 세로가 {Photo.PossibleSize.Width}을 초과하는 이미지가 {exceptedFiles}개 있습니다.\n빠른 작업을 위해 가로와 세로 모두 2000미만의 \n이미지로 표시됩니다. 그래도 진행하시겠습니까?", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;

                PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateAdd(files));
            }
        }

        private void _photoListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PhotosConsumer.IsProcessing())
            {
                MsgBox.ShowTopMost("작업 진행중에는 건드릴 수 없습니다.");
                return;
            }

            Photo photo = _photoListBox.SelectedItem as Photo;

            if (photo == null || photo.BindedControl == null)
                return;


            if (App.ImageDisplayWindow == null)
            {
                App.ImageDisplayWindow = new ImageDisplayWindow(photo);
                App.ImageDisplayWindow.Show();
            }
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
            photo.Update();
        }

        private void _convertGifBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateConvert(ImageFormat.Gif));
        }

        private void _convertPngBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateConvert(ImageFormat.Png));
        }

        private void _convertJpegBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateConvert(ImageFormat.Jpeg));
        }
        private void _convertTiffBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateConvert(ImageFormat.Tiff));
        }

        private void _convertWebpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateConvert(ImageFormat.Webp));
        }

        private void _convertBmpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateConvert(ImageFormat.Bmp));
        }

        private void _convertIcoBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateConvert(ImageFormat.Ico));
        }

        private void _rotateClockWiseBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateRotate(ImageRotate.ClockWise));
        }

        private void _rotateCounterClockWiseBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateRotate(ImageRotate.CounterClockWise));
        }

        private void _saveFileUniqueIdBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSave(dialog.SelectedPath, true));
                }
            }
        }

        private void _saveFileBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSave(dialog.SelectedPath, false));
                }
            }
        }

        private void _copyBase64ImgTagBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateCopyBase64ImgTagSingleJob(true));
        }


        private void _width5ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.05f, 1.00f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _height5ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.00f, 1.05f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _width5ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(0.95f, 1.00f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _height5ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.00f, 0.95f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size5ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.05f, 1.05f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size5ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(0.95f, 0.95f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void NumberTextBox_PreviewInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "[0-9]+");
        }

        private void _widthSpecificBtn_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(_widthTextBox.Text, out int width);

            if (width < 10)
            {
                MsgBox.ShowTopMost("너비를 똑바로 입력해주세요. (10 이상)");
                return;
            }

            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetWidth(width, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _heightSpecificBtn_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(_heightTextBox.Text, out int height);

            if (height < 10)
            {
                MsgBox.ShowTopMost("높이를 똑바로 입력해주세요. (10 이상)");
                return;
            }


            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetHeight(height, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _sizeSpecificBtn_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(_widthTextBox.Text, out int width);
            int.TryParse(_heightTextBox.Text, out int height);

            if (width <= 10 || height <= 10)
            {
                MsgBox.ShowTopMost("너비와 높이를 똑바로 입력해주세요. (10 이상)");
                return;
            }

            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetSize(width, height, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _developerInformationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.InformationWindow == null)
            {
                App.InformationWindow = new InformationWindow();
                App.InformationWindow.Show();
            }
        }

        private void _photoListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
                return;

            IList items = _photoListBox.SelectedItems;

            if (items.Count == 0)
                return;

            if (PhotosConsumer.IsProcessing())
            {
                MsgBox.ShowTopMost("작업중에는 제거할 수 없습니다.");
                return;
            }

            // 하나 삭제되고나면 SelectedItems 목록이 변경되기 땜에 복사해놓고 삭제해야한다.
            // @참고 : https://stackoverflow.com/questions/6398046/wpf-datagrid-remove-selecteditems
            List<Photo> deleteTargets = items.Cast<Photo>().ToList();

            for (int i = 0; i < deleteTargets.Count; i++)
            {
                Photo photo = deleteTargets[i];

                Photos.Remove(photo);
                photo.Dispose();
            }
        }


        private void _size10ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(0.90f, 0.90f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size20ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(0.80f, 0.80f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size40ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(0.60f, 0.60f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size50ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(0.50f, 0.50f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size60ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(0.40f, 0.40f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size80ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(0.20f, 0.20f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size90ScaleDownBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(0.10f, 0.10f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size10ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.10f, 1.10f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size20ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.20f, 1.20f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size40ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.40f, 1.40f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size50ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.50f, 1.50f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size60ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.60f, 1.60f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size80ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.80f, 1.80f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _size90ScaleUpBtn_Click(object sender, RoutedEventArgs e)
        {
            PhotosConsumer.EnqueueJob(PhotoConsumerJobFactory.CreateSetScale(1.90f, 1.90f, _applyAspectRatioCheckBox.IsChecked.Value));
        }

        private void _photoListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PhotosConsumer.IsProcessing())
            {
                MsgBox.ShowTopMost("작업 진행중에는 건드릴 수 없습니다.");
                return;
            }

            
            Photo photo = _photoListBox.SelectedItem as Photo;

            if (photo == null || photo.BindedControl == null)
                return;

            _draggedPhoto = photo;
        }


        private void _photoListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedPhoto != null && _onMousedPhoto != null && _photoListBox.SelectedItems.Count == 1)
            {
                int draggedPhotoIdx = Photos.IndexOf(_draggedPhoto);
                int onMousePhotoIdx = Photos.IndexOf(_onMousedPhoto);

                Photos.Move(draggedPhotoIdx, onMousePhotoIdx);
            }

            _draggedPhoto = null;
            _onMousedPhoto = null;
        }

        private void _photoListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedPhoto == null)
                return;

            Photo photo = _photoListBox.SelectedItem as Photo;

            if (photo == null || photo.BindedControl == null)
                return;

            _onMousedPhoto = photo;
            Mouse.SetCursor(Cursors.Hand);
        }
    }
}
