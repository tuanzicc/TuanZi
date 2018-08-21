

using System.Drawing;

namespace TuanZi.AspNetCore.Imaging
{
    public static class StringExtensions
    {
        /// <param name="s"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="background"></param>
        /// <param name="font"></param>
        /// <param name="brush"></param>
        /// <param name="layoutRectangle"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static System.Drawing.Image ToImage(this string s, int width, int height, Color background, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            var image = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(background);

                g.DrawString(s, font, brush, layoutRectangle, format);
            }

            return image;
        }
    }
}
