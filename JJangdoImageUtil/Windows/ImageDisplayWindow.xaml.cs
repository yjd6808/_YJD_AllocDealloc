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
        public ImageDisplayWindow(Image displayImage)
        {
            InitializeComponent();

            _displayImage.BeginInit();
            _displayImage.Source = displayImage.Source.CloneCurrentValue();
            _displayImage.EndInit();

            App.ImageDisplayWindow = this;
            App.ImageDisplayWindow.Width = _displayImage.Source.Width;
            App.ImageDisplayWindow.Height = _displayImage.Source.Height;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();

            App.ImageDisplayWindow = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtility.MoveToCenterFromBase(App.Window, this);
            WindowUtility.AdjustPosition(App.Window, this, 10);
        }
    }
}
