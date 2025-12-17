using ImageTool4.Model;
using ImageTool4.Tools;
using ImageTool4.ViewModel;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageTool4
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ImageProcessing imageModel;
        public ImageProcessing ImageModel
        {
            get => imageModel;
            set
            {
                imageModel = value;
                OnPropertyChanged(nameof(ImageModel));
            }
        }

        public double _scale = 1.0;
        public double Scale
        {
            get => _scale;
            set
            {
                _scale = Math.Max(0.1, Math.Min(value, 10.0));
                OnPropertyChanged(nameof(Scale));
            }
        }

        //public Rect _zoomRegion;
        //public Rect ZoomRegion
        //{
        //    get { return _zoomRegion; }
        //    set
        //    {
        //        _zoomRegion = value;
        //        OnPropertyChanged(nameof(ZoomRegion));
        //    }
        //}

        //public Rect UpdateZoomRegion(double zoomX, double zoomY, double width, double height)
        //{
        //    ZoomRegion = new Rect(zoomX, zoomY, width, height);
        //    return ZoomRegion;
        //}

        public ICommand LoadImageCommand { get; set; }
        public ICommand SaveImageCommand { get; set; }
        public ICommand DilationCommand { get; set; }
        public ICommand ErosionCommand { get; set; }
        public ICommand HistogramCommand { get; set; }
        public ICommand OtsuBinaryCommand { get; set; }
        public ICommand GaussianCommand { get; set; }
        public ICommand LaplaceCommand { get; set; }
        public ICommand SobelCommand { get; set; }
        public ICommand FFTCommand { get; set; }
        public ICommand TemplateCommand { get; set; }

        public MainViewModel()
        {
            ImageModel = new ImageProcessing();
            LoadImageCommand = new RelayCommand(Load_Click);
            SaveImageCommand = new RelayCommand(Save_Click);
            DilationCommand = new RelayCommand(Dilation_Click);
            ErosionCommand = new RelayCommand(Erosion_Click);
            HistogramCommand = new RelayCommand(His_Click);
            OtsuBinaryCommand = new RelayCommand(Otsu_Click);
            GaussianCommand = new RelayCommand(Gaussian_Click);
            LaplaceCommand = new RelayCommand(Laplace_Click);
            SobelCommand = new RelayCommand(Sobel_Click);
            FFTCommand = new RelayCommand(FFT_Click);
            TemplateCommand = new RelayCommand(Temp_Click);
        }

        public void Load_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            temp.ImageSource = ImageTools.Loadimg();
            ImageModel = temp;
            Scale = 1.0;
        }

        public void Save_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            ImageTools.Saveimg(ImageModel.ChangeImage);
            ImageModel = temp;
        }

        public void Dilation_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            BitmapSource CloneImage = ImageModel.ImageSource.Clone();
            Bitmap bmp = Convertors.BitmapSourceToBitmap(CloneImage);
            BitmapSource changeImage = Convertors.BitmapToBitmapSource(ImageTools.Dilationimg(bmp));
            temp.ChangeImage = changeImage;
            ImageModel = temp;
        }

        public void Erosion_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            BitmapSource CloneImage = ImageModel.ImageSource.Clone();
            Bitmap bmp = Convertors.BitmapSourceToBitmap(CloneImage);
            BitmapSource changeImage = Convertors.BitmapToBitmapSource(ImageTools.Erosionimg(bmp));
            temp.ChangeImage = changeImage;
            ImageModel = temp;
        }

        public void His_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            BitmapSource CloneImage = ImageModel.ImageSource.Clone();
            Bitmap bmp = Convertors.BitmapSourceToBitmap(CloneImage);
            Bitmap writebmp = Convertors.IntToBitmap(ImageTools.HistogramEqualization(bmp));
            BitmapSource changeImage = Convertors.BitmapToBitmapSource(writebmp);
            temp.ChangeImage = changeImage;
            ImageModel = temp;
        }

        public void Otsu_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            BitmapSource CloneImage = ImageModel.ImageSource.Clone();
            byte[,] bytes = Convertors.BitmapSourceToByteArray2(CloneImage, (int)CloneImage.PixelWidth, (int)CloneImage.PixelHeight);
            int width = bytes.GetLength(1);
            int height = bytes.GetLength(0);
            int[] a = ImageTools.Histogram(Convertors.BitmapSourceToBitmap(CloneImage));
            byte[,] Otsu = ImageTools.OtsuBinarization(bytes, width, height, a);
            BitmapSource changeImage = Convertors.ByteArrayToBitmapSource2(Otsu, width, height);
            temp.ChangeImage = changeImage;
            ImageModel = temp;
        }

        public void Gaussian_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            BitmapSource CloneImage = ImageModel.ImageSource.Clone();
            Bitmap bitmap = Convertors.BitmapSourceToBitmap(CloneImage);
            int[,] GaussianImage = ImageTools.Gaussian(bitmap);
            Bitmap readimg = Convertors.IntToBitmap(GaussianImage);
            BitmapSource chageImage = Convertors.BitmapToBitmapSource(readimg);
            temp.ChangeImage = chageImage;
            ImageModel = temp;
        }

        public void Laplace_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            BitmapSource CloneImage = ImageModel.ImageSource.Clone();
            Bitmap bitmap = Convertors.BitmapSourceToBitmap(CloneImage);
            byte[,] LaplaceImage = ImageTools.Laplacian(bitmap);
            BitmapSource changeImage = Convertors.ByteArrayToBitmapSource2(LaplaceImage, LaplaceImage.GetLength(1), LaplaceImage.GetLength(0));
            temp.ChangeImage = changeImage;
            ImageModel = temp;
        }

        public void Sobel_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            BitmapSource CloneImage = ImageModel.ImageSource.Clone();
            byte[,] bytes = Convertors.BitmapSourceToByteArray2(CloneImage, (int)CloneImage.PixelWidth, (int)CloneImage.PixelHeight);
            int width = bytes.GetLength(1);
            int height = bytes.GetLength(0);
            byte[,] sobelImage = ImageTools.Sobel(bytes, width, height);
            BitmapSource changeImage = Convertors.ByteArrayToBitmapSource2(sobelImage, width, height);
            temp.ChangeImage = changeImage;
            ImageModel = temp;
        }

        public void FFT_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            BitmapSource CloneImage = ImageModel.ImageSource.Clone();
            byte[,] bytes = Convertors.BitmapSourceToByteArray2(CloneImage, (int)CloneImage.PixelWidth, (int)CloneImage.PixelHeight);
            Complex[,] padImage = ImageTools.PadImage(bytes);
            Complex[,] fftImage = ImageTools.FFT2(padImage);
            double[,] logImage = Convertors.LogTransform(fftImage);
            double[,] shiftImage = ImageTools.Shift(logImage);
            byte[,] normalImage = Convertors.Normalize(shiftImage);
            BitmapSource changeImage = Convertors.ByteArrayToBitmapSource2(normalImage, (int)CloneImage.PixelWidth, (int)CloneImage.PixelHeight);
            temp.ChangeImage = changeImage;
            ImageModel = temp;
        }

        public void Temp_Click(object obj)
        {
            ImageProcessing temp = ImageModel.Clone();
            ImageModel.TemplitImage = ImageTools.TemplitLoadimg();
            BitmapSource changeImage = ImageTools.TemplitMatching(ImageModel.ImageSource, ImageModel.TemplitImage);
            temp.ChangeImage = changeImage;
            ImageModel = temp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

