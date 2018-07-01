using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

using TuanZi.Collections;
using TuanZi.Extensions;


namespace TuanZi.Drawing
{
    public class ValidateCoder
    {
        private static readonly Random Random = new Random();

        public ValidateCoder()
        {
            FontNames = new List<string> { "Arial", "Batang", "Buxton Sketch", "David", "SketchFlow Print" };
            FontNamesForHanzi = new List<string> { "宋体", "幼圆", "楷体", "仿宋", "隶书", "黑体" };
            FontSize = 20;
            FontWidth = FontSize;
            BgColor = Color.FromArgb(240, 240, 240);
            RandomPointPercent = 0;
        }

       

        public List<string> FontNames { get; set; }

        public List<string> FontNamesForHanzi { get; set; }

        public int FontSize { get; set; }

        public int FontWidth { get; set; }

        public int Height { get; set; }

        public Color BgColor { get; set; }

        public bool HasBorder { get; set; }

        public bool RandomPosition { get; set; }

        public bool RandomColor { get; set; }

        public bool RandomItalic { get; set; }

        public double RandomPointPercent { get; set; }

        public int RandomLineCount { get; set; }


        public string GetCode(int length, ValidateCodeType codeType = ValidateCodeType.NumberAndLetter)
        {
            length.CheckGreaterThan("length", 0);

            switch (codeType)
            {
                case ValidateCodeType.Number:
                    return GetRandomNums(length);
                case ValidateCodeType.Hanzi:
                    return GetRandomHanzis(length);
                default:
                    return GetRandomNumsAndLetters(length);
            }
        }

        public Bitmap CreateImage(string code, ValidateCodeType codeType)
        {
            code.CheckNotNullOrEmpty("code");

            int width = FontWidth * code.Length + FontWidth;
            int height = FontSize + FontSize / 2;
            const int flag = 255 / 2;
            bool isBgLight = (BgColor.R + BgColor.G + BgColor.B) / 3 > flag;
            Bitmap image = new Bitmap(width, height);
            Graphics grap = Graphics.FromImage(image);
            grap.Clear(BgColor);
            Brush brush = new SolidBrush(Color.FromArgb(255 - BgColor.R, 255 - BgColor.G, 255 - BgColor.B));
            int x, y = 3;
            if (HasBorder)
            {
                grap.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
            }

            Random rnd = Random;

            for (int i = 0; i < RandomLineCount; i++)
            {
                x = rnd.Next(image.Width);
                y = rnd.Next(image.Height);
                int m = rnd.Next(image.Width);
                int n = rnd.Next(image.Height);
                Color lineColor = !RandomColor
                    ? Color.FromArgb(90, 90, 90)
                    : isBgLight
                        ? Color.FromArgb(rnd.Next(130, 200), rnd.Next(130, 200), rnd.Next(130, 200))
                        : Color.FromArgb(rnd.Next(70, 150), rnd.Next(70, 150), rnd.Next(70, 150));
                Pen pen = new Pen(lineColor, 2);
                grap.DrawLine(pen, x, y, m, n);
            }

            for (int i = 0; i < (int)(image.Width * image.Height * RandomPointPercent / 100); i++)
            {
                x = rnd.Next(image.Width);
                y = rnd.Next(image.Height);
                Color pointColor = isBgLight
                    ? Color.FromArgb(rnd.Next(30, 80), rnd.Next(30, 80), rnd.Next(30, 80))
                    : Color.FromArgb(rnd.Next(150, 200), rnd.Next(150, 200), rnd.Next(150, 200));
                image.SetPixel(x, y, pointColor);
            }

            for (int i = 0; i < code.Length; i++)
            {
                rnd = Random;
                x = FontWidth / 4 + FontWidth * i;
                if (RandomPosition)
                {
                    x = rnd.Next(FontWidth / 4) + FontWidth * i;
                    y = rnd.Next(image.Height / 5);
                }
                PointF point = new PointF(x, y);
                if (RandomColor)
                {
                    int r, g, b;
                    if (!isBgLight)
                    {
                        r = rnd.Next(255 - BgColor.R);
                        g = rnd.Next(255 - BgColor.G);
                        b = rnd.Next(255 - BgColor.B);
                        if ((r + g + b) / 3 < flag)
                        {
                            r = 255 - r;
                            g = 255 - g;
                            b = 255 - b;
                        }
                    }
                    else
                    {
                        r = rnd.Next(BgColor.R);
                        g = rnd.Next(BgColor.G);
                        b = rnd.Next(BgColor.B);
                        if ((r + g + b) / 3 > flag)
                        {
                            r = 255 - r;
                            g = 255 - g;
                            b = 255 - b;
                        }
                    }
                    brush = new SolidBrush(Color.FromArgb(r, g, b));
                }
                string fontName = codeType == ValidateCodeType.Hanzi
                    ? FontNamesForHanzi[rnd.Next(FontNamesForHanzi.Count)]
                    : FontNames[rnd.Next(FontNames.Count)];
                Font font = new Font(fontName, FontSize, FontStyle.Bold);
                if (RandomItalic)
                {
                    grap.TranslateTransform(0, 0);
                    Matrix transform = grap.Transform;
                    transform.Shear(Convert.ToSingle(rnd.Next(2, 9) / 10d - 0.5), 0.001f);
                    grap.Transform = transform;
                }
                grap.DrawString(code.Substring(i, 1), font, brush, point);
                grap.ResetTransform();
            }

            return image;
        }

        public Bitmap CreateImage(int length, out string code, ValidateCodeType codeType = ValidateCodeType.NumberAndLetter)
        {
            length.CheckGreaterThan("length", 0);

            length = length < 1 ? 1 : length;
            switch (codeType)
            {
                case ValidateCodeType.Number:
                    code = GetRandomNums(length);
                    break;
                case ValidateCodeType.Hanzi:
                    code = GetRandomHanzis(length);
                    break;
                default:
                    code = GetRandomNumsAndLetters(length);
                    break;
            }
            if (code.Length > length)
            {
                code = code.Substring(0, length);
            }
            return CreateImage(code, codeType);
        }



        private static string GetRandomNums(int length)
        {
            int[] ints = new int[length];
            for (int i = 0; i < length; i++)
            {
                ints[i] = Random.Next(0, 9);
            }
            return ints.ExpandAndToString("");
        }

        private static string GetRandomNumsAndLetters(int length)
        {
            const string allChar = "2,3,4,5,6,7,8,9," +
                "A,B,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,T,U,V,W,X,Y,Z," +
                "a,b,c,d,e,f,g,h,k,m,n,p,q,r,s,t,u,v,w,x,y,z";
            string[] allChars = allChar.Split(',');
            List<string> result = new List<string>();
            while (result.Count < length)
            {
                int index = Random.Next(allChars.Length);
                string c = allChars[index];
                result.Add(c);
            }
            return result.ExpandAndToString("");
        }

        private static string GetRandomHanzis(int length)
        {
            string[] baseStrs = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f".Split(',');
            Encoding encoding = Encoding.GetEncoding("GB2312");
            string result = null;

            for (int i = 0; i < length; i++)
            {
                Random rnd = Random;
                int index1 = rnd.Next(11, 14);
                string str1 = baseStrs[index1];

                int index2 = index1 == 13 ? rnd.Next(0, 7) : rnd.Next(0, 16);
                string str2 = baseStrs[index2];

                int index3 = rnd.Next(10, 16);
                string str3 = baseStrs[index3];

                int index4 = index3 == 10 ? rnd.Next(1, 16) : (index3 == 15 ? rnd.Next(0, 15) : rnd.Next(0, 16));
                string str4 = baseStrs[index4];

                byte b1 = Convert.ToByte(str1 + str2, 16);
                byte b2 = Convert.ToByte(str3 + str4, 16);
                byte[] bs = { b1, b2 };

                result += encoding.GetString(bs);
            }
            return result;
        }
        
    }
}