using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TuanZi
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// Rounds and formats a decimal culture invariant
        /// </summary>
        /// <param name="value">The decimal</param>
        /// <param name="decimals">Rounding decimal number</param>
        /// <returns>Formated value</returns>
        public static string FormatInvariant(this decimal value, int decimals = 2)
        {
            return Math.Round(value, decimals).ToString("0.00", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Calculates the tax (percentage) from a gross and a net value.
        /// </summary>
        /// <param name="inclTax">Gross value</param>
        /// <param name="exclTax">Net value</param>
        /// <param name="decimals">Rounding decimal number</param>
        /// <returns>Tax percentage</returns>
        public static decimal ToTaxPercentage(this decimal inclTax, decimal exclTax, int? decimals = null)
        {
            if (exclTax == decimal.Zero)
                return decimal.Zero;

            var result = ((inclTax / exclTax) - 1.0M) * 100.0M;

            return (decimals.HasValue ? Math.Round(result, decimals.Value) : result);
        }



        public static string ToFileSizeString(this long bytes)
        {
            float size = (float)bytes;
            string[] fix = { "B", "KB", "MB", "GB", "TB", "PB", "??" };
            int pos = 0;
            while (size >= 1024f)
            {
                size /= 1024f;
                pos++;
            }
            if (pos >= fix.Length - 1)
                pos = fix.Length - 1;
            return Math.Round(size, 2) + " " + fix[pos];
        }

        public static string ToFileSizeString(this int byteSize)
        {
            return ((long)byteSize).ToFileSizeString();
        }


        public static double Round(this double obj, int decimals)
        {
            return double.Parse(Math.Round(obj, decimals).ToString());
        }
        public static decimal Round(this decimal obj, int decimals)
        {
            return decimal.Parse(Math.Round(obj, decimals).ToString());
        }
    }
}
