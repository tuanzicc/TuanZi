

using System;
using System.Linq;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace TuanZi.AspNetCore.Imaging
{
    public static class ImageExtensions
    {
       
        public static bool HasTransparentPixel(this Image i)
        {
            using (Bitmap bitmap = new Bitmap(i))
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        if (bitmap.GetPixel(x, y).A < 255)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        public static Image Crop(this Image i, int width, int height)
        {
            return i.Crop(width, height, InterpolationMode.Bilinear);
        }

        public static Image Crop(this Image i, int width, int height, InterpolationMode interpolationMode)
        {
            if (width <= 0)
                width = height;
            if (height <= 0)
                height = width;

            if (width + height <= 0)
                return (Image)i.Clone();

            return i.Resize(width, height, GetMode(i.Width, i.Height, width, height, true), interpolationMode);
        }

        public static Image ResizeMaxW(this Image i, int maxWidth)
        {
            if (i.Width > maxWidth)
            {

                
                return i.ResizeW(maxWidth, InterpolationMode.Bilinear);
            }
            else
            {
                return (Image)i.Clone();
            }
        }

        public static Image ResizeW(this Image i, int width)
        {
            return i.ResizeW(width, InterpolationMode.Bilinear);
        }

        public static Image ResizeW(this Image i, int width, InterpolationMode interpolationMode)
        {
            return i.ResizeW(width, Color.Empty, interpolationMode);
        }

        public static Image ResizeW(this Image i, int width, Color background)
        {
            return i.ResizeW(width, background, InterpolationMode.Bilinear);
        }

        public static Image ResizeW(this Image i, int width, Color background, InterpolationMode interpolationMode)
        {
            return i.Resize(width, (int)((float)width / i.Width * i.Height), background, interpolationMode);
        }

        public static Image ResizeMaxH(this Image i, int maxHeight)
        {
            if (i.Height > maxHeight)
            {
                return i.ResizeH(maxHeight, InterpolationMode.Bilinear);
            }
            else
            {
                return (Image)i.Clone();
            }
        }

        public static Image ResizeH(this Image i, int height)
        {
            return i.ResizeH(height, InterpolationMode.Bilinear);
        }

        public static Image ResizeH(this Image i, int height, InterpolationMode interpolationMode)
        {
            return i.ResizeH(height, Color.Empty, interpolationMode);
        }

        public static Image ResizeH(this Image i, int height, Color background)
        {
            return i.ResizeH(height, background, InterpolationMode.Bilinear);
        }

        public static Image ResizeH(this Image i, int height, Color background, InterpolationMode interpolationMode)
        {
            return i.Resize((int)((float)height / i.Height * i.Width), height, background, interpolationMode);
        }

        public static Image Resize(this Image i, int width, int height)
        {
            return i.Resize(width, height, InterpolationMode.Bilinear);
        }

      
        public static Image Resize(this Image i, int width, int height, InterpolationMode interpolationMode)
        {
            return i.Resize(width, height, Color.Empty, interpolationMode);
        }

        public static Image Resize(this Image i, int width, int height, Color background)
        {
            return i.Resize(width, height, background, InterpolationMode.Bilinear);
        }

        public static Image Resize(this Image i, int width, int height, ResizeMode mode)
        {
            return i.Resize(width, height, Color.Empty, mode);
        }

        public static Image Resize(this Image i, int width, int height, Color background, InterpolationMode interpolationMode)
        {

            return i.Resize(width, height, background, GetMode(i.Width, i.Height, width, height, false), interpolationMode);
        }

        public static Image Resize(this Image i, int width, int height, ResizeMode mode, InterpolationMode interpolationMode)
        {
            return i.Resize(width, height, Color.Empty, mode, interpolationMode);
        }

        public static Image Resize(this Image i, int width, int height, Color background, ResizeMode mode)
        {
            return i.Resize(width, height, background, mode, InterpolationMode.Bilinear);
        }

        public static Image Resize(this Image i, int width, int height, Color background, ResizeMode mode, InterpolationMode interpolationMode)
        {
            if (width <= 0 && height <= 0)
            {
                width = i.Width;
                height = i.Height;
            }

            if (width > i.Width)
            {
                width = i.Width;
            }

            if (height > i.Height)
            {
                height = i.Height;
            }

            if (i.Width == width && i.Height == height)
            {
                return (Image)i.Clone();
            }

            if (((float)i.Width / i.Height == (float)width / height))
            {
                return i.Zoom(width, height, interpolationMode);
            }
            else
            {
                Size size = CalculateSize(i.Width, i.Height, width, height, mode);

                using (Image cropOrFill = i.CropOrFill(size.Width, size.Height, background, interpolationMode))
                {
                    return cropOrFill.Zoom(width, height, interpolationMode);
                }
            }
        }


        public static MemoryStream Save(this Image i, ImageFormat format, int value)
        {
            MemoryStream stream = new MemoryStream();

            try
            {
                i.Save(stream, format.ToImageCodecInfo(), value.ToEncoderParameters());
            }
            catch (ArgumentNullException)
            {
                throw new InvalidOperationException("Unsupported image format");
            }

            stream.Position = 0;

            return stream;
        }

        public static void Save(this Image i, string path, int value)
        {
            try
            {
                i.Save(path, path.ToImageCodecInfo(), value.ToEncoderParameters());
            }
            catch (ArgumentNullException)
            {
                throw new InvalidOperationException("Unsupported image format");
            }
        }
        public static Image Zoom(this Image i, int width, int height, InterpolationMode mode)
        {
            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = mode;

                g.Clear(Color.Empty);

                g.DrawImage(i, new Rectangle(0, 0, width, height), new Rectangle(0, 0, i.Width, i.Height), GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        private static ResizeMode GetMode(int oldWidth, int oldHeight, int newWidth, int newHeight, bool crop)
        {
            float oldScale = (float)oldWidth / oldHeight;

            float newScale = (float)newWidth / newHeight;

            if (crop)
            {
                return oldScale < newScale ? ResizeMode.WidthFirst : ResizeMode.HeightFirst;
            }
            else
            {
                return oldScale > newScale ? ResizeMode.WidthFirst : ResizeMode.HeightFirst;
            }
        }

        private static Image CropOrFill(this Image image, int width, int height, Color background, InterpolationMode mode)
        {
            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = mode;

                int srcX = GetSrc(image.Width, width);
                int srcY = GetSrc(image.Height, height);

                g.Clear(background);

                g.DrawImage(image, new Rectangle(0, 0, width, height), srcX, srcY, width, height, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        private static int GetSrc(int oldLength, int newLength)
        {
            return (int)((float)(oldLength - newLength) / 2);
        }

        private static Size CalculateSize(int oldWidth, int oldHeight, int newWidth, int newHeight, ResizeMode mode)
        {
            Size size = new Size();

            float scale = (float)newWidth / newHeight;

            if (mode == ResizeMode.WidthFirst)
            {
                size.Width = oldWidth;
                size.Height = (int)(oldWidth / scale);
            }
            else
            {
                size.Height = oldHeight;
                size.Width = (int)(oldHeight * scale);
            }

            return size;
        }

        private static EncoderParameters ToEncoderParameters(this int value)
        {
            EncoderParameters encoderParameters = new EncoderParameters();

            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, value);

            return encoderParameters;
        }

        private static ImageCodecInfo ToImageCodecInfo(this string path)
        {
            string ext = Path.GetExtension(path).ToUpper();

            if (!string.IsNullOrEmpty(ext))
            {
                ImageCodecInfo[] infos = ImageCodecInfo.GetImageEncoders();

                foreach (ImageCodecInfo info in infos)
                {
                    if (info.FilenameExtension.Split(';').Contains("*" + ext))
                    {
                        return info;
                    }
                }
            }

            throw new ArgumentException();
        }

        private static ImageCodecInfo ToImageCodecInfo(this ImageFormat format)
        {
            var info = ImageCodecInfo.GetImageEncoders().FirstOrDefault(m => m.FilenameExtension.Contains(format.ToString()));

            return info;
        }

        private const int exifOrientationID = 0x112; //274

        public static Image AutoRotate(this Image img)
        {
            if (!img.PropertyIdList.Contains(exifOrientationID))
                return img;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);

            return img;
        }
    }


    public enum ResizeMode
    {
        WidthFirst,
        HeightFirst
    }
}
