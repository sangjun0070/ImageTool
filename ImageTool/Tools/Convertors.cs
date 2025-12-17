using ImageTool4.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ImageTool4.Tools
{
    public static class Convertors
    {
        public static Bitmap BitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memoryStream);
                memoryStream.Position = 0;

                using (Bitmap bitmap = new Bitmap(memoryStream))
                {
                    return new Bitmap(bitmap);
                }
            }
        }

        public static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            BitmapSource bitmapSource;
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
            bitmapSource.Freeze();

            return bitmapSource;
        }

        public static unsafe int[,] BitmapToInt(Bitmap bmp)
        {
            Bitmap smp;
            smp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format32bppArgb);

            BitmapData smpData = smp.LockBits(new Rectangle(0, 0, smp.Width, smp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int[,] b_mp = new int[bmp.Width, bmp.Height];

            for (int y = 0; y < smp.Height; y++)
            {
                byte* pit = (byte*)smpData.Scan0 + (y * smpData.Stride);
                for (int x = 0; x < smp.Width; x++)
                {
                    byte r = (byte)((pit[0] + pit[1] + pit[2]) / 3);
                    b_mp[x,y] = (int)r;

                    pit += 3;
                }
            }
            smp.UnlockBits(smpData);
            return b_mp;
        }

        public static unsafe byte[,] BitmapToByte(Bitmap bmp)
        {
            Bitmap smp;
            smp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format32bppArgb);
            BitmapData smpData = smp.LockBits(new Rectangle(0, 0, smp.Width, smp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte[,] b_mp = new byte[bmp.Height, bmp.Width];

            for (int y = 0; y < smp.Height; y++)
            {
                byte* pit = (byte*)smpData.Scan0 + (y * smpData.Stride);
                for (int x = 0; x < smp.Width; x++)
                {
                    byte r = (byte)((pit[0] + pit[1] + pit[2]) / 3);
                    b_mp[y, x] = r;

                    pit += 3;
                }
            }
            smp.UnlockBits(smpData);
            return b_mp;
        }

        public static unsafe Bitmap IntToBitmap(int[,] Input)
        {
            Bitmap Write = new Bitmap(Input.GetLength(0), Input.GetLength(1));
            BitmapData bmpd = Write.LockBits(new Rectangle(0, 0, Input.GetLength(0), Input.GetLength(1)), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            for (int y = 0; y < Input.GetLength(1); y++)
            {
                byte* pit = (byte*)bmpd.Scan0 + (y * bmpd.Stride);
                for (int x = 0; x < Input.GetLength(0); x++)
                {
                    pit[0] = pit[1] = pit[2] = (byte)Input[x, y];
                    pit[3] = 255;
                    pit += 4;
                }
            }

            Write.UnlockBits(bmpd);

            return Write;
        }

        //public static byte[] BitmapSourceToByteArray(BitmapSource bitmapSource)
        //{
        //    int stride = (bitmapSource.PixelWidth * bitmapSource.Format.BitsPerPixel + 7) / 8;
        //    int arraySize = stride * bitmapSource.PixelHeight;
        //    byte[] pixelData = new byte[arraySize];
        //    bitmapSource.CopyPixels(pixelData, stride, 0);
        //    return pixelData;
        //}

        public static BitmapSource ByteArrayToBitmapSource(byte[,] pixelData, int width, int height, int stride)
        {
            byte[] pixelArray = new byte[height * stride];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixelArray[y * stride + x] = pixelData[y, x];
                }
            }

            BitmapSource bitmap = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null, pixelArray, stride);

            return bitmap;
        }

        public static BitmapSource ByteArrayToBitmapSource2(byte[,] byteArray, int width, int height)
        {
            int stride = width;
            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), byteArray, stride, 0);

            return bitmap;
        }

        public static byte[,] BitmapSourceToByteArray2(BitmapSource bitmapSource, int width, int height)
        {
            int stride = width;
            byte[,] byteArray = new byte[height, width];
            byte[] pixels = new byte[height * stride];
            bitmapSource.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byteArray[y, x] = pixels[y * stride + x];
                }
            }

            return byteArray;
        }

        public static double[,] LogTransform(Complex[,] fftResult)
        {
            int width = fftResult.GetLength(0);
            int height = fftResult.GetLength(1);
            double[,] logTransformed = new double[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    logTransformed[i, j] = Math.Log(1 + fftResult[i, j].Magnitude);
                }
            }

            return logTransformed;
        }

        public static byte[,] Normalize(double[,] image)
        {
            int width = image.GetLength(0);
            int height = image.GetLength(1);

            double min = image.Cast<double>().Min();
            double max = image.Cast<double>().Max();

            byte[,] normalizedImage = new byte[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    normalizedImage[i, j] = (byte)((image[i, j] - min) / (max - min) * 255);
                }
            }

            return normalizedImage;
        }
    }
}
