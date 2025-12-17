using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Drawing;

namespace ImageTool4
{
    public class PreviewViewModel : INotifyPropertyChanged
    {
        public BitmapSource _imageSource;
        public BitmapSource ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));                
            }
        }

        public Rect _zoomRegion;
        public Rect ZoomRegion
        {
            get { return _zoomRegion; }
            set
            {
                _zoomRegion = value;
                OnPropertyChanged(nameof(ZoomRegion));
            }
        }

        public PreviewViewModel (BitmapSource bitmapSource)
        {
            ImageSource = bitmapSource;
        }

        public void UpdateZoomRegion(Rect rect)
        {
            ZoomRegion = new Rect(rect.X,rect.Y, rect.Width, rect.Height);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
