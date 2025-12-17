using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ImageTool4.Tools
{
    public static class ImageTools
    {

        public static BitmapSource Loadimg()
        {
            Application test = Application.Current;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.bmp, *.jpg, *.png) | *.bmp; *.jpg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                FormatConvertedBitmap grayBitmap = new FormatConvertedBitmap();
                grayBitmap.BeginInit();
                grayBitmap.Source = bitmapImage;
                grayBitmap.DestinationFormat = PixelFormats.Gray8;
                grayBitmap.EndInit();
                return grayBitmap;
            }

            return null;
        }

        public static void Saveimg(BitmapSource bmp)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "BMP Image|*.bmp";

            if (sd.ShowDialog() == true)
            {
                BitmapSource bitmapSource = bmp.Clone();

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (FileStream fileStream = new FileStream(sd.FileName, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }

        public static unsafe Bitmap Dilationimg(Bitmap bmp)
        {
            Bitmap smp = new Bitmap(bmp.Width, bmp.Height);

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData smpData = smp.LockBits(new Rectangle(0, 0, smp.Width, smp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            byte[,] hh = new byte[3, 3]
            {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
            };
            int size = 3;
            int r = size / 2;
            {
                for (int y = r; y < smpData.Height - r; y++)
                {
                    byte* pit = (byte*)bmpData.Scan0 + (y * bmpData.Stride);
                    byte* p_it = (byte*)smpData.Scan0 + (y * bmpData.Stride);
                    for (int x = r; x < smpData.Width - r; x++)
                    {
                        byte max = 0;
                        byte cValue = 0;
                        for (int k = 0; k < 3; k++)
                        {
                            int i = k - r;
                            byte* p__it = (byte*)bmpData.Scan0 + ((y + i) * bmpData.Stride);
                            for (int t = 0; t < 3; t++)
                            {
                                int j = t - r;
                                cValue = (byte)((p__it[x * 3 + j] + p__it[x * 3 + j + 1] + p__it[x * 3 + j + 2]) / 3);
                                if (max < cValue)
                                {
                                    if (hh[k, t] != 0)
                                        max = cValue;
                                }
                            }
                        }
                        p_it[0] = p_it[1] = p_it[2] = max;
                        pit += 3;
                        p_it += 3;
                    }
                }
            }
            smp.UnlockBits(bmpData);
            bmp.UnlockBits(smpData);
            return bmp;
        }

        public static Bitmap Erosionimg(Bitmap bmp)
        {
            Bitmap smp = new Bitmap(bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData smpData = smp.LockBits(new Rectangle(0, 0, smp.Width, smp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            byte[,] hh = new byte[3, 3]
            {
                        { 0, 1, 0},
                        { 1, 1, 1},
                        { 0, 1, 0}
            };
            int size = 3;
            int r = size / 2;
            unsafe
            {
                for (int y = r; y < smpData.Height - r; y++)
                {
                    byte* pit = (byte*)bmpData.Scan0 + (y * bmpData.Stride);
                    byte* p_it = (byte*)smpData.Scan0 + (y * bmpData.Stride);
                    for (int x = r; x < smpData.Width - r; x++)
                    {
                        byte min = 255;
                        byte cValue = 255;
                        for (int k = 0; k < 3; k++)
                        {
                            int i = k - r;
                            byte* p__it = (byte*)bmpData.Scan0 + ((y + i) * bmpData.Stride);
                            for (int t = 0; t < 3; t++)
                            {
                                int j = t - r;
                                cValue = (byte)((p__it[x * 3 + j] + p__it[x * 3 + j + 1] + p__it[x * 3 + j + 2]) / 3);
                                if (min > cValue)
                                {
                                    if (hh[k, t] != 0)
                                        min = cValue;
                                }
                            }
                        }
                        p_it[0] = p_it[1] = p_it[2] = min;
                        pit += 3;
                        p_it += 3;
                    }
                }
                smp.UnlockBits(bmpData);
                bmp.UnlockBits(smpData);
            }
            return bmp;
        }

        public static int[,] HistogramEqualization(Bitmap bmp)
        {
            int[,] Int = Convertors.BitmapToInt(bmp);
            Bitmap smp = Convertors.IntToBitmap(Int);
            BitmapData histo = smp.LockBits(new Rectangle(0, 0, smp.Width, smp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] his = new int[256];
            his.Initialize();

            unsafe
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    byte* pit = (byte*)histo.Scan0 + (y * histo.Stride);
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        his[pit[0]]++;
                        pit += 3;
                    }
                }
                smp.UnlockBits(histo);
                int sum = 0;
                int[,] Histo = new int[bmp.Width, bmp.Height];
                Histo.Initialize();
                int sum2 = bmp.Width * bmp.Height;

                for (int z = 0; z < 256; z++)
                {
                    sum += his[z];

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            if (z == Int[x, y])
                            {
                                Histo[x, y] = (int)Math.Round((double)(255 * sum / sum2));
                            }
                        }
                    }
                }
                return Histo;
            }
        }

        public static unsafe int[] Histogram(Bitmap bmp)
        {
            Bitmap smp;
            smp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb);
            BitmapData histo = smp.LockBits(new Rectangle(0, 0, smp.Width, smp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] his = new int[256];
            his.Initialize();
            int width = bmp.Width;
            int height = bmp.Height;


            for (int z = 0; z < 256; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte* pit = (byte*)histo.Scan0 + (y * histo.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        if (z == pit[0])
                        {
                            his[z] += 1;
                        }

                        pit += 3;
                    }
                }
            }
            return his;
        }

        public static int T(int[] histogram)
        {
            int totalPixels = histogram.Sum();
            float sumB = 0;
            int wB = 0;
            int wF = 0;

            float maxVariance = 0;
            int threshold = 0;
            float sum1 = 0;

            for (int t = 0; t < 256; t++)
            {
                sum1 += t * histogram[t];
            }

            for (int t = 0; t < 256; t++)
            {
                wB += histogram[t];
                if (wB == 0) continue;

                wF = totalPixels - wB;
                if (wF == 0) break;

                sumB += (float)(t * histogram[t]);

                float mB = sumB / wB;
                float mF = (sum1 - sumB) / wF;

                float betweenClassVariance = (float)wB * wF * (mB - mF) * (mB - mF);

                if (betweenClassVariance > maxVariance)
                {
                    maxVariance = betweenClassVariance;
                    threshold = t;
                }
            }

            return threshold;
        }


        public static unsafe byte[,] OtsuBinarization(byte[,] b_mp, int width, int height, int[] his)
        {
            int T_value = T(his);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (b_mp[y, x] < T_value)
                    {
                        b_mp[y, x] = 0;
                    }
                    else
                    {
                        b_mp[y, x] = 255;
                    }
                }
            }
            return b_mp;
        }

        public static int[,] Gaussian(Bitmap bmp)
        {
            double Pi = Math.PI;
            double sigma = 1;
            double[,] f = new double[9, 9];

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    f[x, y] = (double)(1 / (2 * Pi * Math.Pow(sigma, 2)) * (Math.Exp(-(Math.Pow(x - 4, 2) + Math.Pow(y - 4, 2))) / (2 * Math.Pow(sigma, 2))));
                }
            }
            int[,] s__mp = Convertors.BitmapToInt(bmp);
            int[,] r__mp = (int[,])s__mp.Clone();
            int size = 9;
            double sum1 = 0;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    sum1 += f[j, i];
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    f[j, i] /= sum1;
                }
            }

            for (int y = 0; y < (bmp.Height - size); y++)
            {
                for (int x = 0; x < (bmp.Width - size); x++)
                {
                    double sum = 0;
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            sum += (double)(f[j, i] * r__mp[x + j, y + i]);
                            for (int z = 0; z < size - 1; z++)
                            {
                                s__mp[x + z, y + z] = (int)sum;
                            }
                        }
                    }
                }
            }
            return s__mp;
        }

        public static byte[,] Laplacian(Bitmap bmp)
        {
            Bitmap smp;
            smp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format32bppArgb);
            byte[,] b_mp = Convertors.BitmapToByte(bmp);
            byte[,] s_mp = new byte[bmp.Height, bmp.Width];

            for (int y = 1; y < bmp.Height-1; y++)
            {
                for (int x = 1; x < bmp.Width-1; x++)
                {
                    byte Left = b_mp[y, x - 1];
                    byte Right = b_mp[y, x + 1];
                    byte Bottom = b_mp[y - 1, x];
                    byte Top = b_mp[y + 1, x];
                    byte Center = b_mp[y, x];
                    s_mp[y, x] = (byte)((Left + Right + Bottom + Top - 4 * Center)/16);
                    if (s_mp[y, x] < 0)
                    {
                        s_mp[y, x] = 0;
                    }
                    if (s_mp[y, x] > 255)
                    {
                        s_mp[y, x] = 255;
                    }
                }
            }
            return s_mp;
        }

        public static byte[,] Sobel(byte[,] image, int width, int height)
        {
            int[,] gx_k = new int[3, 3]
            {
            {-1, 0, 1},
            {-2, 0, 2},
            {-1, 0, 1}
            };

            int[,] gy_k = new int[3, 3]
            {
            {-1, -2, -1},
            {0, 0, 0},
            {1, 2, 1}
            };

            byte[,] edge = new byte[height, width];

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int gx = 0, gy = 0;

                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            gx += image[y + j, x + i] * gx_k[j + 1, i + 1];
                            gy += image[y + j, x + i] * gy_k[j + 1, i + 1];
                        }
                    }
                    edge[y, x] = (byte)Math.Min(Math.Sqrt(gx * gx + gy * gy), 255);
                }
            }

            return edge;
        }

        public static Complex[,] PadImage(byte[,] image)
        {
            int Width = image.GetLength(0);
            int Height = image.GetLength(1);

            int newWidth = (int)Math.Pow(2, Math.Ceiling(Math.Log(Width, 2)));
            int newHeight = (int)Math.Pow(2, Math.Ceiling(Math.Log(Height, 2)));

            Complex[,] paddedImage = new Complex[newWidth, newHeight];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    paddedImage[i, j] = new Complex(image[i, j], 0);
                }
            }

            for (int i = Width; i < newWidth; i++)
            {
                for (int j = Height; j < newHeight; j++)
                {
                    paddedImage[i, j] = Complex.Zero;
                }
            }

            return paddedImage;
        }

        public static void FFT1(Complex[] buffer)
        {
            int n = buffer.Length;
            if (n <= 1)
            {
                return;
            }

            var even = new Complex[n / 2];
            var odd = new Complex[n / 2];

            for (int i = 0; i < n / 2; i++)
            {
                even[i] = buffer[i * 2];
                odd[i] = buffer[i * 2 + 1];
            }

            FFT1(even);
            FFT1(odd);

            for (int i = 0; i < n / 2; i++)
            {
                Complex exp = Complex.Exp(-Complex.ImaginaryOne * 2 * Math.PI * i / n) * odd[i];
                buffer[i] = even[i] + exp;
                buffer[i + n / 2] = even[i] - exp;
            }
        }


        //이미지 변환 후 최적의 크기로 맞춰서 이미지 패딩
        //이미지 형식 변환 하여 복소수행렬 생성, 푸리에 변환 ㄱㄱ
        //시각적 분석의 편의성을 위해 로그 변환 후 사분면 이동을 통해 중심으로
        //정규화 후 BitmapImage로 변환하기 위해 8비트로 바꾸고 변환
        public static Complex[,] FFT2(Complex[,] input)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            //열
            for (int j = 0; j < height; j++)
            {
                Complex[] col = new Complex[width];
                for (int i = 0; i < width; i++)
                {
                    col[i] = input[i, j];
                }
                FFT1(col);

                for (int i = 0; i < width; i++)
                {
                    input[i, j] = col[i];
                }

            }
            //행
            for (int i = 0; i < width; i++)
            {
                Complex[] row = new Complex[height];
                for (int j = 0; j < height; j++)
                {
                    row[j] = input[i, j];
                }

                FFT1(row);

                for (int j = 0; j < height; j++)
                {
                    input[i, j] = row[j];
                }
            }
            return input;
        }

        public static double[,] Shift(double[,] image)
        {
            int width = image.GetLength(0);
            int height = image.GetLength(1);
            double[,] shiftedImage = new double[width, height];

            int halfWidth = width / 2;
            int halfHeight = height / 2;

            for (int i = 0; i < halfWidth; i++)
            {
                for (int j = 0; j < halfHeight; j++)
                {
                    // 1<->4
                    shiftedImage[i, j] = image[i + halfWidth, j + halfHeight];
                    shiftedImage[i + halfWidth, j + halfHeight] = image[i, j];
                    // 2<->3
                    shiftedImage[i, j + halfHeight] = image[i + halfWidth, j];
                    shiftedImage[i + halfWidth, j] = image[i, j + halfHeight];
                }
            }

            return shiftedImage;
        }

        public static BitmapSource TemplitLoadimg()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.bmp, *.jpg, *.png) | *.bmp; *.jpg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                FormatConvertedBitmap grayBitmap = new FormatConvertedBitmap();
                grayBitmap.BeginInit();
                grayBitmap.Source = bitmapImage;
                grayBitmap.DestinationFormat = System.Windows.Media.PixelFormats.Gray8;
                grayBitmap.EndInit();
                return grayBitmap;
            }
            return null;
        }

        /*public static BitmapSource TemplitMatching(BitmapSource bitmapImage, BitmapSource templitImage)
        {
            int bitmapImageWidth = bitmapImage.PixelWidth;
            int bitmapImageHeight = bitmapImage.PixelHeight;

            int templitImageWidth = templitImage.PixelWidth;
            int templitImageHeight = templitImage.PixelHeight;

            int stride = bitmapImageWidth * 4;

            byte[,] b_mp = Convertors.BitmapSourceToByteArray2(bitmapImage, bitmapImageWidth, bitmapImageHeight);
            byte[,] t_mp = Convertors.BitmapSourceToByteArray2(templitImage, templitImageWidth, templitImageHeight);

            double max = 0;
            double k = 0;

            int bestX = 0;
            int bestY = 0;

            for (int y = templitImageHeight / 2; y < bitmapImageHeight - templitImageHeight / 2; y++)
            {
                for (int x = templitImageWidth / 2; x < bitmapImageWidth - templitImageWidth / 2; x++)
                {
                    double sum = 0;
                    double b_mpPixelsum = 0;
                    double t_mpPixelsum = 0;
                    double b_mpavgsum = 0;
                    double t_mpavgsum = 0;
                    double b_mp_avgPowsum = 0;
                    double t_mp_avgPowsum = 0;
                    double corrcoeff = 0;

                    for (int j = 0; j < templitImageHeight; j++)
                    {
                        for (int i = 0; i < templitImageWidth; i++)
                        {
                            b_mpPixelsum += b_mp[y + j - templitImageHeight / 2, x + i - templitImageWidth / 2];
                            t_mpPixelsum += t_mp[j, i];
                            b_mpavgsum = b_mpPixelsum / (templitImageWidth * templitImageHeight);
                            t_mpavgsum = t_mpPixelsum / (templitImageWidth * templitImageHeight);
                            sum += ((b_mp[y + j - templitImageHeight / 2, x + i - templitImageWidth / 2] - b_mpavgsum) * (t_mp[j, i] - t_mpavgsum));
                            b_mp_avgPowsum += Math.Pow(b_mp[y + j - templitImageHeight / 2, x + i - templitImageWidth / 2] - b_mpavgsum, 2);
                            t_mp_avgPowsum += Math.Pow(t_mp[j, i] - t_mpavgsum, 2);
                        }
                    }

                    corrcoeff = sum / Math.Sqrt(b_mp_avgPowsum * t_mp_avgPowsum);
                    k = corrcoeff / (templitImageWidth * templitImageHeight);

                    if (max < k)
                    {
                        max = k;
                        bestX = x;
                        bestY = y;
                    }
                }
            }

            for (int y = bestY - templitImageHeight / 2; y < bestY + templitImageHeight / 2; y++)
            {
                for (int x = bestX - templitImageWidth / 2; x < bestX + templitImageWidth / 2; x++)
                {
                    if (y == bestY - templitImageHeight / 2 || y == bestY + templitImageHeight / 2 - 1 ||x == bestX - templitImageWidth / 2 || x == bestX + templitImageWidth / 2 - 1)
                    {
                        b_mp[y, x] = 255;
                    }
                }
            }

            return Convertors.ByteArrayToBitmapSource(b_mp, bitmapImageWidth, bitmapImageHeight, stride);
        }*/

        public static BitmapSource TemplitMatching(BitmapSource bitmapImage, BitmapSource templitImage)
        {
            int bitmapImageWidth = bitmapImage.PixelWidth;
            int bitmapImageHeight = bitmapImage.PixelHeight;

            int templitImageWidth = templitImage.PixelWidth;
            int templitImageHeight = templitImage.PixelHeight;

            int stride = bitmapImageWidth * 4;

            byte[,] b_mp = Convertors.BitmapSourceToByteArray2(bitmapImage, bitmapImageWidth, bitmapImageHeight);
            byte[,] t_mp = Convertors.BitmapSourceToByteArray2(templitImage, templitImageWidth, templitImageHeight);

            double min = double.MaxValue;

            int bestX = 0;
            int bestY = 0;

            object lockObj = new object();

            Parallel.For(templitImageHeight / 2, bitmapImageHeight - templitImageHeight / 2, y =>
            {
                for (int x = templitImageWidth / 2; x < bitmapImageWidth - templitImageWidth / 2; x++)
                {
                    double sum = 0;

                    for (int j = 0; j < templitImageHeight; j++)
                    {
                        for (int i = 0; i < templitImageWidth; i++)
                        {
                            sum += (int)Math.Pow(b_mp[y + j - templitImageHeight / 2, x + i - templitImageWidth / 2] - t_mp[j, i], 2);
                        }
                    }

                    lock (lockObj)
                    {
                        if (sum < min)
                        {
                            min = sum;
                            bestX = x;
                            bestY = y;
                        }
                    }
                }
            });


            for (int y = bestY - templitImageHeight / 2; y < bestY + templitImageHeight / 2; y++)
            {
                for (int x = bestX - templitImageWidth / 2; x < bestX + templitImageWidth / 2; x++)
                {
                    if (y == bestY - templitImageHeight / 2 || y == bestY + templitImageHeight / 2-1  || x == bestX - templitImageWidth / 2 || x == bestX + templitImageWidth / 2-1)
                    {
                        b_mp[y, x] = 255;
                    }
                }
            }

            return Convertors.ByteArrayToBitmapSource(b_mp, bitmapImageWidth, bitmapImageHeight, stride);

        }
    }
}

