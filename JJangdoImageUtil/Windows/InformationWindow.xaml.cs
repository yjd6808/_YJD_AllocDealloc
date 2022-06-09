using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// <summary>
    /// Interaction logic for InformationWindow.xaml
    /// </summary>
    public partial class InformationWindow : Window
    {
        public InformationWindow()
        {
            InitializeComponent();

            WindowUtility.MoveToCenterFromBase(App.Window, this);
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();

            App.InformationWindow = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://blog.naver.com/wjdeh313");
        }
    }
}
