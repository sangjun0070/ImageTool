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

namespace ImageTool4
{
    /// <summary>
    /// Preview.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Preview : Window
    {
        public Preview()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            ImageSource imageSource = mainWindow.ImageBox1.Source.Clone();
            ViewImageBox1.Source = imageSource;
        }
    }
}
