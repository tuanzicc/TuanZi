using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using TuanZi.Collections;
using TuanZi.Extensions;


namespace TuanZi.Drawing
{
    public static class BitmapExtensions
    {
        #region Byte[,]

        public static Color[,] ToPixelArray2D(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                Color[,] pixels = new Color[width, height];
                int offset = data.Stride - width * 3;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[x, y] = Color.FromArgb(ptr[2], ptr[1], ptr[0]);
                        ptr += 3;
                    }
                    ptr += offset;
                }
                return pixels;
            }
        }

        public static byte[,] ToGrayArray2D(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte[,] grayBytes = new byte[width, height];
                int offset = data.Stride - width * 3;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        grayBytes[x, y] = GetGrayValue(ptr[2], ptr[1], ptr[0]);
                        ptr += 3;
                    }
                    ptr += offset;
                }
                bmp.UnlockBits(data);
                return grayBytes;
            }
        }

        public static byte[,] ToGrayArray2D(this Color[,] pixels)
        {
            int width = pixels.GetLength(0), height = pixels.GetLength(1);
            byte[,] grayBytes = new byte[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grayBytes[x, y] = GetGrayValue(pixels[x, y]);
                }
            }
            return grayBytes;
        }

        public static Bitmap ToBitmap(this Color[,] pixels)
        {
            int width = pixels.GetLength(0), height = pixels.GetLength(1);
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int offset = data.Stride - width * 3;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixel = pixels[x, y];
                        ptr[2] = pixel.R;
                        ptr[1] = pixel.G;
                        ptr[0] = pixel.B;
                        ptr += 3;
                    }
                    ptr += offset;
                }
                bmp.UnlockBits(data);
                return bmp;
            }
        }

        public static Bitmap ToBitmap(this byte[,] grayBytes)
        {
            int width = grayBytes.GetLength(0), height = grayBytes.GetLength(1);
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int offset = data.Stride - width * 3;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        ptr[2] = ptr[1] = ptr[0] = grayBytes[x, y];
                        ptr += 3;
                    }
                    ptr += offset;
                }
                bmp.UnlockBits(data);
                return bmp;
            }
        }

        public static byte[,] Binaryzation(this byte[,] grayBytes, byte gray)
        {
            int width = grayBytes.GetLength(0), height = grayBytes.GetLength(1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grayBytes[x, y] = (byte)(grayBytes[x, y] > gray ? 255 : 0);
                }
            }
            return grayBytes;
        }

        public static byte[,] DeepFore(this byte[,] grayBytes, byte gray = 200)
        {
            int width = grayBytes.GetLength(0), height = grayBytes.GetLength(1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (grayBytes[x, y] < gray)
                    {
                        grayBytes[x, y] = 0;
                    }
                }
            }
            return grayBytes;
        }

        public static byte[,] ClearNoiseRound(this byte[,] binBytes, byte gray, int maxNearPoints)
        {
            int width = binBytes.GetLength(0), height = binBytes.GetLength(1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte value = binBytes[x, y];
                    if (value > gray || (x == 0 || y == 0 || x == width - 1 || y == height - 1))
                    {
                        binBytes[x, y] = 255;
                        continue;
                    }
                    int count = 0;
                    if (binBytes[x - 1, y - 1] < gray) count++;
                    if (binBytes[x, y - 1] < gray) count++;
                    if (binBytes[x + 1, y - 1] < gray) count++;
                    if (binBytes[x, y - 1] < gray) count++;
                    if (binBytes[x, y + 1] < gray) count++;
                    if (binBytes[x - 1, y + 1] < gray) count++;
                    if (binBytes[x, y + 1] < gray) count++;
                    if (binBytes[x + 1, y + 1] < gray) count++;
                    if (count < maxNearPoints)
                    {
                        binBytes[x, y] = 255;
                    }
                }
            }
            return binBytes;
        }

        public static byte[,] ClearNoiseArea(this byte[,] binBytes, byte gray, int minAreaPoints)
        {
            int width = binBytes.GetLength(0), height = binBytes.GetLength(1);
            byte[,] newBinBytes = binBytes.Copy();
            Dictionary<byte, Point[]> areaPointDict = new Dictionary<byte, Point[]>();
            byte setGray = 1;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (IsBlack(newBinBytes[x, y]))
                    {
                        Point[] setPoints;
                        newBinBytes.FloodFill(new Point(x, y), setGray, out setPoints);
                        areaPointDict.Add(setGray, setPoints);

                        setGray++;
                        if (setGray >= 255)
                        {
                            setGray = 254;
                        }
                    }
                }
            }
            List<Point[]> pointsList = areaPointDict.Where(m => m.Value.Length < minAreaPoints).Select(m => m.Value).ToList();
            foreach (Point[] points in pointsList)
            {
                foreach (Point point in points)
                {
                    binBytes[point.X, point.Y] = 255;
                }
            }

            return binBytes;
        }

        public static byte[,] FloodFill(this byte[,] binBytes, Point point, byte replacementGray)
        {
            int width = binBytes.GetLength(0), height = binBytes.GetLength(1);
            Stack<Point> stack = new Stack<Point>();
            byte gray = binBytes[point.X, point.Y];
            stack.Push(point);

            while (stack.Count > 0)
            {
                var p = stack.Pop();
                if (p.X <= 0 || p.X >= width || p.Y <= 0 || p.Y >= height)
                {
                    continue;
                }
                if (binBytes[p.X, p.Y] == gray)
                {
                    binBytes[p.X, p.Y] = replacementGray;

                    stack.Push(new Point(p.X - 1, p.Y));
                    stack.Push(new Point(p.X + 1, p.Y));
                    stack.Push(new Point(p.X, p.Y - 1));
                    stack.Push(new Point(p.X, p.Y + 1));
                }
            }

            return binBytes;
        }

        public static byte[,] FloodFill(this byte[,] binBytes, Point point, byte replacementGray, out Point[] points)
        {
            int width = binBytes.GetLength(0), height = binBytes.GetLength(1);
            List<Point> pointList = new List<Point>();
            Stack<Point> stack = new Stack<Point>();
            byte gray = binBytes[point.X, point.Y];
            stack.Push(point);

            while (stack.Count > 0)
            {
                var p = stack.Pop();
                if (p.X <= 0 || p.X >= width || p.Y <= 0 || p.Y >= height)
                {
                    continue;
                }
                if (binBytes[p.X, p.Y] == gray)
                {
                    binBytes[p.X, p.Y] = replacementGray;
                    pointList.Add(p);

                    stack.Push(new Point(p.X - 1, p.Y));
                    stack.Push(new Point(p.X + 1, p.Y));
                    stack.Push(new Point(p.X, p.Y - 1));
                    stack.Push(new Point(p.X, p.Y + 1));
                }
            }

            points = pointList.ToArray();
            return binBytes;
        }

        public static byte[,] ClearBorder(this byte[,] grayBytes, int border)
        {
            int width = grayBytes.GetLength(0), height = grayBytes.GetLength(1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x < border || y < border || x > width - 1 - border || y > height - 1 - border)
                    {
                        grayBytes[x, y] = 255;
                    }
                }
            }
            return grayBytes;
        }

        public static byte[,] AddBorder(this byte[,] grayBytes, int border, byte gray = 255)
        {
            int width = grayBytes.GetLength(0) + border * 2, height = grayBytes.GetLength(1) + border * 2;
            byte[,] newBytes = new byte[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x < border || y < border || x > width - 1 - border || y > height - 1 - border)
                    {
                        newBytes[x, y] = gray;
                    }
                }
            }
            newBytes = grayBytes.DrawTo(newBytes, border, border);
            return newBytes;
        }

        public static byte[,] ClearGray(this byte[,] grayBytes, byte minGray, byte maxGray)
        {
            int width = grayBytes.GetLength(0), height = grayBytes.GetLength(1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte value = grayBytes[x, y];
                    if (minGray <= value && value <= maxGray)
                    {
                        grayBytes[x, y] = 255;
                    }
                }
            }
            return grayBytes;
        }

        public static byte[,] ToValid(this byte[,] binBytes, byte gray = 200)
        {
            int width = binBytes.GetLength(0), height = binBytes.GetLength(1);
            int x1 = width, y1 = height, x2 = 0, y2 = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte value = binBytes[x, y];
                    if (value >= gray)
                    {
                        continue;
                    }
                    if (x1 > x) x1 = x;
                    if (y1 > y) y1 = y;
                    if (x2 < x) x2 = x;
                    if (y2 < y) y2 = y;
                }
            }
            int newWidth = x2 - x1 + 1, newHeight = y2 - y1 + 1;
            byte[,] newBytes = binBytes.Clone(x1, y1, newWidth, newHeight);
            return newBytes;
        }

        public static byte[,] Clone(this byte[,] sourceBytes, int x1, int y1, int width, int height)
        {
            int swidth = sourceBytes.GetLength(0), sheight = sourceBytes.GetLength(1);
            if (swidth - x1 < width)
            {
                throw new ArgumentException("要截取的宽度超出界限");
            }
            if (sheight - y1 < height)
            {
                throw new ArgumentException("要截取的高度超出界限");
            }
            byte[,] newBytes = new byte[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newBytes[x, y] = sourceBytes[x1 + x, y1 + y];
                }
            }
            return newBytes;
        }

        public static byte[,] DrawTo(this byte[,] smallBytes, byte[,] bigBytes, int x1, int y1)
        {
            int smallWidth = smallBytes.GetLength(0),
                smallHeight = smallBytes.GetLength(1),
                bigWidth = bigBytes.GetLength(0),
                bigHeight = bigBytes.GetLength(1);
            if (x1 + smallWidth > bigWidth)
            {
                throw new ArgumentException("大图矩阵宽度无法装下小矩阵宽度");
            }
            if (y1 + smallHeight > bigHeight)
            {
                throw new ArgumentException("大图矩阵高度无法装下小矩阵高度");
            }
            for (int y = 0; y < smallHeight; y++)
            {
                for (int x = 0; x < smallWidth; x++)
                {
                    bigBytes[x1 + x, y1 + y] = smallBytes[x, y];
                }
            }

            return bigBytes;
        }

        public static int[] ShadowY(this byte[,] binBytes)
        {
            int width = binBytes.GetLength(0), height = binBytes.GetLength(1);
            int[] nums = new int[width];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (IsBlack(binBytes[x, y]))
                    {
                        nums[x]++;
                    }
                }
            }
            return nums;
        }

        public static int[] ShadowX(this byte[,] binBytes)
        {
            int width = binBytes.GetLength(0), height = binBytes.GetLength(1);
            int[] nums = new int[height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (IsBlack(binBytes[x, y]))
                    {
                        nums[y]++;
                    }
                }
            }
            return nums;
        }

        public static List<byte[,]> SplitShadowY(this byte[,] binBytes, byte minFontWidth = 0, byte minLines = 0)
        {
            int height = binBytes.GetLength(1);
            int[] shadow = binBytes.ShadowY();
            List<Tuple<int, int>> validXs = new List<Tuple<int, int>>();
            int x1 = 0;
            bool inFont = false;
            for (int x = 0; x < shadow.Length; x++)
            {
                int value = shadow[x];
                if (!inFont)
                {
                    if (value > minLines)
                    {
                        inFont = true;
                        x1 = x;
                    }
                }
                else
                {
                    if (value <= minLines)
                    {
                        inFont = false;
                        if (minFontWidth == 0 || x - x1 > minFontWidth)
                        {
                            validXs.Add(new Tuple<int, int>(x1, x));
                        }
                    }
                }
            }

            List<byte[,]> splits = validXs.Select(valid => binBytes.Clone(valid.Item1, 0, valid.Item2 - valid.Item1 + 1, height).ToValid()).ToList();
            return splits;
        }

        public static string ToCodeString(this byte[,] binBytes, byte gray, bool breakLine = false)
        {
            int width = binBytes.GetLength(0), height = binBytes.GetLength(1);
            string code = string.Empty;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    code += binBytes[x, y] < gray ? 1 : 0;
                }
                if (breakLine)
                {
                    code += "\r\n";
                }
            }
            return code;
        }

        #endregion

        #region Image

        public static byte[] ToBytes(this Bitmap bmp)
        {
            using (Bitmap newBmp = new Bitmap(bmp))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ImageFormat format = newBmp.RawFormat;
                    if (ImageFormat.MemoryBmp.Equals(format))
                    {
                        format = ImageFormat.Bmp;
                    }
                    newBmp.Save(ms, format);
                    return ms.GetBuffer();
                }
            }
        }

        public static Bitmap Rotate(this Bitmap bmp, int angle)
        {
            angle = angle % 360;

            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);

            int w1 = bmp.Width;
            int h1 = bmp.Height;
            int w2 = (int)Math.Max(Math.Abs(w1 * cos - h1 * sin), Math.Abs(w1 * cos + h1 * sin));
            int h2 = (int)Math.Max(Math.Abs(w1 * sin - h1 * cos), Math.Abs(w1 * sin + h1 * cos));

            Bitmap newBmp = new Bitmap(w2, h2);
            using (Graphics graphics = Graphics.FromImage(newBmp))
            {
                graphics.InterpolationMode = InterpolationMode.Bilinear;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                Point offset = new Point((w2 - w1) / 2, (h2 - h1) / 2);

                Rectangle rect = new Rectangle(offset.X, offset.Y, w1, h1);
                Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                graphics.TranslateTransform(center.X, center.Y);
                graphics.RotateTransform(360 - angle);

                graphics.TranslateTransform(-center.X, -center.Y);
                graphics.DrawImage(bmp, rect);

                graphics.ResetTransform();
                graphics.Save();
                graphics.Dispose();
                return newBmp;
            }
        }

        public static Point Rotate(this Point center, Point point, int angle)
        {
            angle = angle % 360;

            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian), sin = Math.Sin(radian);

            double x = (point.X - center.X) * cos + (point.Y - center.Y) * sin + center.X;
            double y = (point.X - center.X) * sin + (point.Y - center.Y) * cos + center.Y;

            return new Point((int)Math.Round(x, 0), (int)Math.Round(y, 0));
        }

        public static Bitmap Zoom(this Bitmap bmp, int width, int height, InterpolationMode model = InterpolationMode.Default)
        {
            Bitmap newBmp = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(newBmp))
            {
                graphics.InterpolationMode = model;
                graphics.DrawImage(bmp, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                return newBmp;
            }
        }

        public static Bitmap Zoom(this Bitmap bmp, double percent, InterpolationMode model = InterpolationMode.Default)
        {
            int width = (int)(bmp.Width * percent);
            int height = (int)(bmp.Height * percent);
            return Zoom(bmp, width, height, model);
        }

        public static Bitmap GrayByPixels(this Bitmap bmp)
        {
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color pixel = bmp.GetPixel(x, y);
                    byte value = GetGrayValue(pixel);
                    newBmp.SetPixel(x, y, Color.FromArgb(value, value, value));
                }
            }
            return newBmp;
        }

        public static Bitmap GrayByLine(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
            BitmapData data = newBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int offset = data.Stride - width * 3;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        ptr[0] = ptr[1] = ptr[2] = GetGrayValue(ptr[2], ptr[1], ptr[0]);
                        ptr += 3;
                    }
                    ptr += offset;
                }
                newBmp.UnlockBits(data);
            }
            return newBmp;
        }

        public static Bitmap DeepFore(this Bitmap bmp, byte gray = 200)
        {
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color pixel = bmp.GetPixel(i, j);
                    if (pixel.R < gray)
                    {
                        newBmp.SetPixel(i, j, Color.Black);
                    }
                }
            }
            return newBmp;
        }

        public static Bitmap ClearNoise(this Bitmap bmp, byte gray, int maxNearPoints)
        {
            maxNearPoints.CheckBetween("maxNearPoints", 1, 4, true, true);
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color piexl = bmp.GetPixel(x, y);
                    if (piexl.R >= gray || (x == 0 || x == bmp.Width - 1 || y == 0 || y == bmp.Height - 1))
                    {
                        bmp.SetPixel(x, y, Color.White);
                        continue;
                    }
                    int count = 0;
                    if (bmp.GetPixel(x - 1, y - 1).R < gray) count++;
                    if (bmp.GetPixel(x, y - 1).R < gray) count++;
                    if (bmp.GetPixel(x + 1, y - 1).R < gray) count++;
                    if (bmp.GetPixel(x - 1, y).R < gray) count++;
                    if (bmp.GetPixel(x + 1, y).R < gray) count++;
                    if (bmp.GetPixel(x - 1, y + 1).R < gray) count++;
                    if (bmp.GetPixel(x, y + 1).R < gray) count++;
                    if (bmp.GetPixel(x + 1, y + 1).R < gray) count++;
                    if (count < maxNearPoints)
                    {
                        bmp.SetPixel(x, y, Color.White);
                    }
                }
            }
            return bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
        }

        public static Bitmap Brightness(this Bitmap bmp, int value)
        {
            value = value < -255 ? -255 : value;
            value = value > 255 ? 255 : value;
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = bmp.Clone(new Rectangle(0, 0, width, height), bmp.PixelFormat);
            BitmapData bmpData = newBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* p = (byte*)bmpData.Scan0;
                int offset = bmpData.Stride - width * 3;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int pix = p[i] + value;
                            if (value < 0)
                            {
                                p[i] = (byte)Math.Max(0, pix);
                            }
                            if (value > 0)
                            {
                                p[i] = (byte)Math.Min(255, pix);
                            }
                        }  
                        p += 3;
                    }  
                    p += offset;
                }  
            }

            newBmp.UnlockBits(bmpData);
            return newBmp;
        }

        public static Bitmap Contrast(this Bitmap bmp, int value)
        {
            value = value < -100 ? -100 : value;
            value = value > 100 ? 100 : value;
            double contrast = (100.0 + value) / 100.0;
            contrast *= contrast;
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = bmp.Clone(new Rectangle(0, 0, width, height), bmp.PixelFormat);
            BitmapData bmpData = newBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* p = (byte*)bmpData.Scan0;
                int offset = bmpData.Stride - width * 3;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            double pixel = ((p[i] / 255.0 - 0.5) * contrast + 0.5) * 255;
                            pixel = pixel < 0 ? 0 : pixel;
                            pixel = pixel > 255 ? 255 : pixel;
                            p[i] = (byte)pixel;
                        }  
                        p += 3;
                    }  
                    p += offset;
                }  
            }
            newBmp.UnlockBits(bmpData);
            return newBmp;
        }

        public static Bitmap Gamma(this Bitmap bmp, float value)
        {
            if (Equals(value, 1.0000f))
            {
                return bmp;
            }
            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics graphics = Graphics.FromImage(newBmp))
            {
                ImageAttributes attribtues = new ImageAttributes();
                attribtues.SetGamma(value, ColorAdjustType.Bitmap);
                graphics.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attribtues);
                return newBmp;
            }
        }

        public static Bitmap SetText(this Bitmap bmp, string text, Font font, Color color, int x, int y)
        {
            Bitmap newBmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
            using (Graphics graphics = Graphics.FromImage(newBmp))
            {
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                SolidBrush brush = new SolidBrush(color);
                graphics.DrawString(text, font, brush, new PointF(x, y));
                return newBmp;
            }
        }

        public static Bitmap ClearBorder(this Bitmap bmp, int border)
        {
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = bmp.Clone(new Rectangle(0, 0, width, height), bmp.PixelFormat);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x < border || y < border || x > width - 1 - border || y > height - 1 - border)
                    {
                        newBmp.SetPixel(x, y, Color.White);
                    }
                }
            }

            return newBmp;
        }

        public static Bitmap Plate(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = new Bitmap(width, height);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Color pixel = bmp.GetPixel(i, j);
                    int r = 255 - pixel.R;
                    int g = 255 - pixel.G;
                    int b = 255 - pixel.B;
                    newBmp.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }
            return newBmp;
        }

        public static Bitmap Emboss(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = new Bitmap(width, height);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Color pixel1 = bmp.GetPixel(i, j);
                    Color pixel2 = bmp.GetPixel(i + 1, j + 1);
                    int r = Math.Abs(pixel1.R - pixel2.R + 128);
                    int g = Math.Abs(pixel1.G - pixel2.G + 128);
                    int b = Math.Abs(pixel1.B - pixel2.B + 128);
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    newBmp.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }
            return newBmp;
        }

        public static Bitmap Soften(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = new Bitmap(width, height);
            int[] gauss = { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int index = 0;
                    int r = 0, g = 0, b = 0;
                    for (int col = -1; col <= 1; col++)
                    {
                        for (int row = -1; row <= 1; row++)
                        {
                            Color pixel = bmp.GetPixel(i + row, j + col);
                            r += pixel.R * gauss[index];
                            g += pixel.G * gauss[index];
                            b += pixel.B * gauss[index];
                            index++;
                        }
                    }
                    r /= 16;
                    g /= 16;
                    b /= 16;
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    newBmp.SetPixel(i - 1, j - 1, Color.FromArgb(r, g, b));
                }
            }
            return newBmp;
        }

        public static Bitmap Sharpen(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = new Bitmap(width, height);
            int[] laplacian = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    int index = 0;
                    int r = 0, g = 0, b = 0;
                    for (int col = -1; col <= 1; col++)
                    {
                        for (int row = -1; row <= 1; row++)
                        {
                            Color pixel = bmp.GetPixel(i + row, j + col);
                            r += pixel.R * laplacian[index];
                            g += pixel.G * laplacian[index];
                            b += pixel.B * laplacian[index];
                            index++;
                        }
                    }
                    r /= 16;
                    g /= 16;
                    b /= 16;
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    newBmp.SetPixel(i - 1, j - 1, Color.FromArgb(r, g, b));
                }
            }
            return newBmp;
        }

        public static Bitmap Atomizing(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = new Bitmap(width, height);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Random rnd = new Random();
                    int k = rnd.Next(123456);
                    int dx = i + k % 19;
                    int dy = j + k % 19;
                    if (dx >= width)
                    {
                        dx = width - 1;
                    }
                    if (dy >= height)
                    {
                        dy = height - 1;
                    }
                    Color pixel = bmp.GetPixel(dx, dy);
                    newBmp.SetPixel(i, j, pixel);
                }
            }
            return newBmp;
        }

        public static Bitmap Binaryzation(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData data = newBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
            for (int j = 0; j < height; j++)
            {
                byte[] scan = new byte[(width + 7) / 8];
                for (int i = 0; i < width; i++)
                {
                    Color pixel = bmp.GetPixel(i, j);
                    if (pixel.GetBrightness() >= 0.5)
                    {
                        scan[i / 8] |= (byte)(0x80 >> (i % 8));
                    }
                }
                Marshal.Copy(scan, 0, (IntPtr)((int)data.Scan0 + data.Stride * j), scan.Length);
            }
            newBmp.UnlockBits(data);
            return newBmp;
        }

        public static Bitmap Binaryzation(this Bitmap bmp, byte threshold)
        {
            int width = bmp.Width, height = bmp.Height;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                byte[,] source = new byte[width, height];
                int offset = data.Stride - width * 3;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        source[x, y] = GetGrayValue(ptr[2], ptr[1], ptr[0]);
                        ptr += 3;
                    }
                    ptr += offset;
                }
                bmp.UnlockBits(data);
                Bitmap newBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData newData = newBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                ptr = (byte*)newData.Scan0;
                offset = newData.Stride - width * 3;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        ptr[0] = ptr[1] = ptr[2] = GetAverageColor(source, i, j, width, height) > threshold ? (byte)255 : (byte)0;
                        ptr += 3;
                    }
                    ptr += offset;
                }
                newBmp.UnlockBits(newData);
                return newBmp;
            }
        }

        public static Bitmap OtsuThreshold(this Bitmap bmp)
        {
            int width = bmp.Width, height = bmp.Height;
            byte threshold = 0;
            int[] hist = new int[256];

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* p = (byte*)data.Scan0;
                int offset = data.Stride - width * 4;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        hist[p[0]]++;
                        p += 4;
                    }
                    p += offset;
                }
                bmp.UnlockBits(data);
            }

            double allSum = 0, smallSum = 0;
            int allPixelNumber = 0, smallPixelNumber = 0;
            for (int i = 0; i < 256; i++)
            {
                allSum += i * hist[i];
                allPixelNumber += hist[i];
            }
            double maxValue = -1.0;
            for (int i = 0; i < 256; i++)
            {
                smallPixelNumber += hist[i];
                int bigPixelNumber = allPixelNumber - smallPixelNumber;
                if (bigPixelNumber == 0)
                {
                    break;
                }
                smallSum += i * hist[i];
                double bigSum = allSum - smallSum;
                double smallProbability = smallSum / smallPixelNumber;
                double bigProbability = bigSum / bigPixelNumber;
                double probability = smallPixelNumber * smallProbability + bigPixelNumber * bigProbability * bigProbability;
                if (probability > maxValue)
                {
                    maxValue = probability;
                    threshold = (byte)i;
                }
            }
            return Threshoding(bmp, threshold);
        }

        public static Bitmap Threshoding(this Bitmap bmp, byte threshold)
        {
            int width = bmp.Width, height = bmp.Height;
            Bitmap newBmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
            BitmapData data = newBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int offset = data.Stride - width * 4;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        byte gray = (byte)((ptr[2] + ptr[1] + ptr[0]) / 3);
                        if (gray >= threshold)
                        {
                            ptr[0] = ptr[1] = ptr[2] = 255;
                        }
                        else
                        {
                            ptr[0] = ptr[1] = ptr[2] = 0;
                        }
                        ptr += 4;
                    }
                    ptr += offset;
                }
                newBmp.UnlockBits(data);
                return newBmp;
            }
        }

        public static Bitmap ToValid(this Bitmap bmp, byte gray, int charCount)
        {
            int posx1 = bmp.Width;
            int posy1 = bmp.Height;
            int posx2 = 0;
            int posy2 = 0;
            for (int i = 0; i < bmp.Height; i++) 
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    int pixelValue = bmp.GetPixel(j, i).R;
                    if (pixelValue < gray) 
                    {
                        if (posx1 > j)
                        {
                            posx1 = j;
                        }
                        if (posy1 > i)
                        {
                            posy1 = i;
                        }

                        if (posx2 < j)
                        {
                            posx2 = j;
                        }
                        if (posy2 < i)
                        {
                            posy2 = i;
                        }
                    }
                }
            }
            int span = charCount - (posx2 - posx1 + 1) % charCount; 
            if (span < charCount)
            {
                int leftSpan = span / 2;  
                if (posx1 > leftSpan)
                {
                    posx1 = posx1 - leftSpan;
                }
                if (posx2 + span - leftSpan < bmp.Width)
                {
                    posx2 = posx2 + span - leftSpan;
                }
            }
            Rectangle cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            Bitmap newBmp = bmp.Clone(cloneRect, bmp.PixelFormat);
            return newBmp;
        }

        public static Bitmap ToValid(this Bitmap bmp, byte gray)
        {
            int posx1 = bmp.Width;
            int posy1 = bmp.Height;
            int posx2 = 0;
            int posy2 = 0;
            for (int y = 0; y < bmp.Height; y++) 
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    int pixelValue = bmp.GetPixel(x, y).R;
                    if (pixelValue < gray) 
                    {
                        if (posx1 > x)
                        {
                            posx1 = x;
                        }
                        if (posy1 > y)
                        {
                            posy1 = y;
                        }

                        if (posx2 < x)
                        {
                            posx2 = x;
                        }
                        if (posy2 < y)
                        {
                            posy2 = y;
                        }
                    }
                }
            }
            Rectangle cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            return bmp.Clone(cloneRect, bmp.PixelFormat);
        }

        public static Bitmap[] SplitAverage(this Bitmap bmp, int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0)
            {
                return null;
            }
            int singW = bmp.Width / rowNum;
            int singH = bmp.Height / colNum;
            Bitmap[] picArray = new Bitmap[rowNum * colNum];

            for (int i = 0; i < colNum; i++)
            {
                for (int j = 0; j < rowNum; j++)
                {
                    Rectangle cloneRect = new Rectangle(j * singW, i * singH, singW, singH);
                    picArray[i * rowNum + j] = bmp.Clone(cloneRect, bmp.PixelFormat); 
                }
            }
            return picArray;
        }

        public static string ToCodeString(this Bitmap bmp, byte gray, bool lineBreak = false)
        {
            string code = string.Empty;
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color piexl = bmp.GetPixel(x, y);
                    if (piexl.R < gray)
                    {
                        code += "1";
                    }
                    else
                    {
                        code += "0";
                    }
                }
                if (lineBreak)
                {
                    code += "\r\n";
                }
            }
            return code;
        }

        private static byte GetAverageColor(byte[,] source, int x, int y, int w, int h)
        {
            int result = source[x, y]
                + (x == 0 ? 255 : source[x - 1, y])
                + (x == 0 || y == 0 ? 255 : source[x - 1, y - 1])
                + (x == 0 || y == h - 1 ? 255 : source[x - 1, y + 1])
                + (y == 0 ? 255 : source[x, y - 1])
                + (y == h - 1 ? 255 : source[x, y + 1])
                + (x == w - 1 ? 255 : source[x + 1, y])
                + (x == w - 1 || y == 0 ? 255 : source[x + 1, y - 1])
                + (x == w - 1 || y == h - 1 ? 255 : source[x + 1, y + 1]);
            return (byte)(result / 9);
        }

        private static byte GetGrayValue(Color pixel)
        {
            return GetGrayValue(pixel.R, pixel.G, pixel.B);
        }

        private static byte GetGrayValue(byte red, byte green, byte blue)
        {
            return (byte)((red * 19595 + green * 38469 + blue * 7472) >> 16);
        }

        private static bool IsBlack(byte value)
        {
            return value == 0;
        }

        private static bool IsWhite(byte value)
        {
            return value == 255;
        }

        #endregion
    }
}