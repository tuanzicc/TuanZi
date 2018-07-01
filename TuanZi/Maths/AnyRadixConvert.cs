using System;
using System.Globalization;
using System.Linq;
using TuanZi.Extensions;
using TuanZi.Properties;


namespace TuanZi.Maths
{
    public static class AnyRadixConvert
    {
        private const string BaseChar = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static ulong X2H(string value, int fromRadix)
        {
            value.CheckNotNullOrEmpty("value");
            fromRadix.CheckBetween("fromRadix", 2, 62, true, true);

            value = value.Trim();
            string baseChar = BaseChar.Substring(0, fromRadix);
            ulong result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char @char = value[i];
                if (!baseChar.Contains(@char))
                {
                    throw new ArgumentException(string.Format(Resources.AnyRadixConvert_CharacterIsNotValid, @char, fromRadix));
                }
                result += (ulong)baseChar.IndexOf(@char) * (ulong)Math.Pow(baseChar.Length, value.Length - i - 1);
            }
            return result;
        }

        public static string H2X(ulong value, int toRadix)
        {
            toRadix.CheckBetween("fromRadix", 2, 62, true, true);
            if (value == 0)
            {
                return "0";
            }
            string baseChar = BaseChar.Substring(0, toRadix);
            string result = string.Empty;
            while (value > 0)
            {
                int index = (int)(value % (ulong)baseChar.Length);
                result = baseChar[index] + result;
                value = value / (ulong)baseChar.Length;
            }
            return result;
        }

        public static string X2X(string value, int fromRadix, int toRadix)
        {
            value.CheckNotNullOrEmpty("value");
            fromRadix.CheckBetween("fromRadix", 2, 62, true, true);
            toRadix.CheckBetween("toRadix", 2, 62, true, true);

            ulong num = X2H(value, fromRadix);
            return H2X(num, toRadix);
        }

        public static string _10To16(int value)
        {
            value.CheckGreaterThan("value", 0, true);
            string str = X2X(value.ToString(CultureInfo.InvariantCulture), 10, 16);
            return str.IsNullOrEmpty() ? "0" : str[0] == '0' ? str : '0' + str;
        }

        public static int _16To10(string value)
        {
            value = value.ToUpper();
            return X2X(value, 16, 10).CastTo<int>();
        }
    }
}