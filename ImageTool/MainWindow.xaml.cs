using ImageTool4.Tools;
using ImageTool4.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Point = System.Windows.Point;

namespace ImageTool4
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public double ratio = 1.0F;
        public Point imgPoint1;
        public Rect imgRect1;
        public Point clickPoint1;
        public Point imgPoint2;
        public Rect imgRect2;
        public Point clickPoint2;
        public MainViewModel mainVM;
        public PreviewViewModel previewVM;

        public MainWindow()
        {
            InitializeComponent();
            mainVM = new MainViewModel();
            DataContext = mainVM;

            ImageBox1.MouseWheel += new MouseWheelEventHandler(ImageBox1_MouseWheel);

            imgPoint1 = new Point(ImageBox1.Width / 2, ImageBox1.Height / 2);
            imgRect1 = new Rect(0, 0, ImageBox1.Width, ImageBox1.Height);
            ratio = 1.0;
            clickPoint1 = imgPoint1;

            ImageBox1.InvalidateVisual();

            ImageBox2.MouseWheel += new MouseWheelEventHandler(ImageBox2_MouseWheel);

            imgPoint2 = new Point(ImageBox2.Width / 2, ImageBox2.Height / 2);
            imgRect2 = new Rect(0, 0, ImageBox2.Width, ImageBox2.Height);
            ratio = 1.0;
            clickPoint2 = imgPoint1;

            ImageBox2.InvalidateVisual();

            previewVM = new PreviewViewModel(mainVM.ImageModel.ImageSource);
        }


        public void PreView_Click(object sender, RoutedEventArgs e)
        {
            previewVM.ImageSource = mainVM.ImageModel.ImageSource;
            Preview preview = new Preview() { DataContext = previewVM };
            preview.Show();

        }
        public double zoomScale = 1.0;



        public void ImageBox1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
            
            if (ImageBox1.Source == null) return;

            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            ratio *= zoomFactor;

            if (ratio > 100.0) ratio = 100.0;
            if (ratio < 1.0) ratio = 1.0;

            imgRect1.Width = ImageBox1.ActualWidth * ratio;
            imgRect1.Height = ImageBox1.ActualHeight * ratio;

            imgRect1.X = -(zoomFactor * (imgPoint1.X - imgRect1.X) - imgPoint1.X);
            imgRect1.Y = -(zoomFactor * (imgPoint1.Y - imgRect1.Y) - imgPoint1.Y);

            AdjustImagePosition1();
            UpdateImageDisplay1();

            Point currentPoint = Mouse.GetPosition(ImageBox1);
            previewVM.UpdateZoomRegion(new Rect(currentPoint.X, currentPoint.Y - 25, 50, 50));
        }

        public void ImageBox1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                clickPoint1 = e.GetPosition(ImageBox1);
            }
        }

        public void ImageBox1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = Mouse.GetPosition(ImageBox1);

                imgRect1.X += currentPoint.X - clickPoint1.X;
                imgRect1.Y += currentPoint.Y - clickPoint1.Y;

                AdjustImagePosition1();
                UpdateImageDisplay1();

                clickPoint1 = currentPoint;
            }
            else
            {
                Point currentPoint = Mouse.GetPosition(ImageBox1);
                imgPoint1.X = currentPoint.X;
                imgPoint1.Y = currentPoint.Y;
            }
        }

        public void AdjustImagePosition1()
        {
            if (imgRect1.X > 0) imgRect1.X = 0;
            if (imgRect1.Y > 0) imgRect1.Y = 0;

            if (imgRect1.X + imgRect1.Width < ImageBox1.ActualWidth)
                imgRect1.X = ImageBox1.ActualWidth - imgRect1.Width;

            if (imgRect1.Y + imgRect1.Height < ImageBox1.ActualHeight)
                imgRect1.Y = ImageBox1.ActualHeight - imgRect1.Height;
        }

        public void UpdateImageDisplay1()
        {
            var imageSource = ImageBox1.Source as BitmapSource;
            if (imageSource == null) return;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(ratio, ratio));
            transformGroup.Children.Add(new TranslateTransform(imgRect1.X, imgRect1.Y));

            ImageBox1.RenderTransform = transformGroup;
        }

        public void ImageBox2_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;

            if (ImageBox2.Source == null) return;

            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            ratio *= zoomFactor;

            if (ratio > 100.0) ratio = 100.0;
            if (ratio < 1.0) ratio = 1.0;

            imgRect2.Width = ImageBox2.ActualWidth * ratio;
            imgRect2.Height = ImageBox2.ActualHeight * ratio;

            imgRect2.X = -(zoomFactor * (imgPoint2.X - imgRect2.X) - imgPoint2.X);
            imgRect2.Y = -(zoomFactor * (imgPoint2.Y - imgRect2.Y) - imgPoint2.Y);

            AdjustImagePosition2();
            UpdateImageDisplay2();
        }

        public void ImageBox2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                clickPoint2 = e.GetPosition(ImageBox2);
            }
        }

        public void ImageBox2_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = Mouse.GetPosition(ImageBox2);

                imgRect2.X += (currentPoint.X - clickPoint2.X) / 5;
                imgRect2.Y += (currentPoint.Y - clickPoint2.Y) / 5;

                AdjustImagePosition2();
                UpdateImageDisplay2();

                clickPoint1 = currentPoint;
            }
            else
            {
                Point currentPoint = Mouse.GetPosition(ImageBox2);
                imgPoint2.X = currentPoint.X;
                imgPoint2.Y = currentPoint.Y;
            }
        }

        public void AdjustImagePosition2()
        {
            if (imgRect2.X > 0) imgRect2.X = 0;
            if (imgRect2.Y > 0) imgRect2.Y = 0;

            if (imgRect2.X + imgRect2.Width < ImageBox2.ActualWidth)
                imgRect2.X = ImageBox2.ActualWidth - imgRect2.Width;

            if (imgRect2.Y + imgRect2.Height < ImageBox2.ActualHeight)
                imgRect2.Y = ImageBox2.ActualHeight - imgRect2.Height;
        }

        public void UpdateImageDisplay2()
        {
            var imageSource = ImageBox2.Source as BitmapSource;
            if (imageSource == null) return;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(ratio, ratio));
            transformGroup.Children.Add(new TranslateTransform(imgRect2.X, imgRect2.Y));

            ImageBox2.RenderTransform = transformGroup;
        }
    }
}