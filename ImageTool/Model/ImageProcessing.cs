using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageTool4.Model
{
    public class ImageProcessing    /////
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

        public BitmapSource _changeImage;
        public BitmapSource ChangeImage
        {
            get => _changeImage;
            set
            {
                _changeImage = value;
                OnPropertyChanged(nameof(ChangeImage));
            }
        }

        public BitmapSource _templitImage;
        public BitmapSource TemplitImage
        {
            get => _templitImage;
            set
            {
                _templitImage = value;
                OnPropertyChanged(nameof(TemplitImage));
            }
        }

        public ImageProcessing Clone()
        {
            return MemberwiseClone() as ImageProcessing;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}