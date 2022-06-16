using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JJangdoImageUtil
{
    public partial class ImageDisplayWindow : Window
    {
        private const int _startMenuPadding = 30;
        private const int _padding = 10;
        private Photo _displayingPhoto;

        public ImageDisplayWindow(Photo displayPhoto)
        {
            InitializeComponent();

            _displayingPhoto = displayPhoto;

            _displayImage.BeginInit();
            _displayImage.Source = _displayingPhoto.BindedControl.Source;
            _displayImage.EndInit();
            _imageNameTextBox.Text = $"{_displayingPhoto.FileName} {_displayingPhoto.SourceImage.GetWidth()}x{_displayingPhoto.SourceImage.GetHeight()}";

            App.ImageDisplayWindow = this;

            // 실제로 되어야할 윈도우 크기 (초과할 경우 강제로 고정되기 땜에 이값이 실제로 되진 않음)
            double realWindowWidth = _displayingPhoto.Size.Width + _displayerImageBorder.BorderThickness.Left * 2;
            double realWindowHeight = _displayingPhoto.Size.Height + _titleGrid.Height + _displayerImageBorder.BorderThickness.Top * 2;

            if (AdjustSize(realWindowWidth, realWindowHeight, out double adjustWindowWidth, out double adjustWindowHeight))
            {
                this.Width = adjustWindowWidth;
                this.Height = adjustWindowHeight;
                this.Topmost = true;

                _imageNameTextBox.Text += $" (사이즈 초과로 자동조절됨)";
            }
            else
            {
                this.Width = realWindowWidth;
                this.Height = realWindowHeight;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Opacity = 0.3;
                DragMove();
                Opacity = 1.0;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtility.MoveToCenterFromBase(App.Window, this);
            WindowUtility.AdjustPosition(App.Window, this, 10);
        }

        private void _closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();

            App.ImageDisplayWindow = null;
        }

        private void _minimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private bool AdjustSize(double realWindowWidth, double realWindowHeight, out double adjustWindowWidth, out double adjustWindowHeight)
        {
            Rect closestDisplayRect = WindowUtility.ClosestDisplayRect(App.Window);

            double paddedDisplayWidth = closestDisplayRect.Width - _padding * 2;
            double paddedDisplayHeight = closestDisplayRect.Height - _padding - _startMenuPadding; // 시작메뉴 높이도 고려하자.

            if (realWindowWidth > paddedDisplayWidth && realWindowHeight > paddedDisplayHeight)
            {
                // 윈도우 너비, 높이가 디스플레이 너비, 높이를 모두 초과하는 경우
                // 보통 디스플레이가 세로길이가 더 짧으니 짧은쪽기준으로 맞춰주자.

                // 먼저 기존 너비와 높이 비율을 구한다.
                double windowSizeRatio = realWindowWidth / realWindowHeight;

                realWindowHeight = paddedDisplayHeight;
                realWindowWidth = realWindowHeight * windowSizeRatio;

                this.Top = _padding;
                this.Left = _padding;
            }
            else if (realWindowWidth > paddedDisplayWidth)
            {
                realWindowHeight = realWindowHeight * paddedDisplayWidth / realWindowWidth;
                realWindowWidth = paddedDisplayWidth;
                this.Left = _padding;
            }
            else if (realWindowHeight > paddedDisplayHeight)
            {
                realWindowWidth = realWindowWidth * paddedDisplayHeight / realWindowHeight;
                realWindowHeight = paddedDisplayHeight;
                this.Top = _padding;
            }
            else
            {
                adjustWindowWidth = 0;
                adjustWindowHeight = 0;
                return false;
            }

            adjustWindowWidth  = realWindowWidth;
            adjustWindowHeight = realWindowHeight;
            return true;
        }
    }
}
