
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Newtonsoft.Json;
using TuanZi.Collections;
using TuanZi.Secutiry;


namespace TuanZi
{
    public static class StringExtensions
    {
        #region pattern

        public static bool IsMatch(this string value, string pattern, bool isContains = true)
        {
            if (value == null)
            {
                return false;
            }
            return isContains
                ? Regex.IsMatch(value, pattern)
                : Regex.Match(value, pattern).Success;
        }

        public static string Match(this string value, string pattern)
        {
            if (value == null)
            {
                return null;
            }
            return Regex.Match(value, pattern).Value;
        }

        public static IEnumerable<string> Matches(this string value, string pattern)
        {
            if (value == null)
            {
                return new string[] { };
            }
            MatchCollection matches = Regex.Matches(value, pattern);
            return from Match match in matches select match.Value;
        }

        public static string MatchFirstNumber(this string value)
        {
            MatchCollection matches = Regex.Matches(value, @"\d+");
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            return matches[0].Value;
        }

        public static string MatchLastNumber(this string value)
        {
            MatchCollection matches = Regex.Matches(value, @"\d+");
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            return matches[matches.Count - 1].Value;
        }

        public static IEnumerable<string> MatchNumbers(this string value)
        {
            return Matches(value, @"\d+");
        }

        public static bool IsMatchNumber(this string value)
        {
            return IsMatch(value, @"\d");
        }

        public static bool IsMatchNumber(this string value, int length)
        {
            Regex regex = new Regex(@"^\d{" + length + "}$");
            return regex.IsMatch(value);
        }

        public static string Substring(this string source, string startString, params string[] endStrings)
        {
            if (source.IsMissing())
            {
                return string.Empty;
            }
            int startIndex = 0;
            if (!string.IsNullOrEmpty(startString))
            {
                startIndex = source.IndexOf(startString, StringComparison.OrdinalIgnoreCase);
                if (startIndex < 0)
                {
                    throw new InvalidOperationException(string.Format("Cannot find substring position '{0}'{0}' in source string", startString));
                }
                startIndex = startIndex + startString.Length;
            }
            int endIndex = source.Length;
            endStrings = endStrings.OrderByDescending(m => m.Length).ToArray();
            foreach (string endString in endStrings)
            {
                if (string.IsNullOrEmpty(endString))
                {
                    endIndex = source.Length;
                    break;
                }
                endIndex = source.IndexOf(endString, startIndex, StringComparison.OrdinalIgnoreCase);
                if (endIndex < 0 || endIndex < startIndex)
                {
                    continue;
                }
                break;
            }
            if (endIndex < 0 || endIndex < startIndex)
            {
                throw new InvalidOperationException(string.Format("Cannot find substring position '{0}'{0}' in source string", endStrings.ExpandAndToString()));
            }
            
            int length = endIndex - startIndex;
            return source.Substring(startIndex, length);
        }

        public static string Substring2(this string source, string startString, string endString)
        {
            return source.Substring2(startString, endString, false);
        }

        public static string Substring2(this string source, string startString, string endString, bool containsEmpty)
        {
            if (source.IsMissing())
            {
                return string.Empty;
            }
            string inner = containsEmpty ? "\\s\\S" : "\\S";
            string result = source.Match(string.Format("(?<={0})([{1}]+?)(?={2})", startString, inner, endString));
            return result.IsMissing() ? null : result;
        }

        public static bool IsEmail(this string value)
        {
            const string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            return value.IsMatch(pattern);
        }

        public static bool IsIpAddress(this string value)
        {
            const string pattern = @"^((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))$";
            return value.IsMatch(pattern);
        }

        public static bool IsNumeric(this string value)
        {
            const string pattern = @"^\-?[0-9]+$";
            return value.IsMatch(pattern);
        }

        public static bool IsUnicode(this string value)
        {
            const string pattern = @"^[\u4E00-\u9FA5\uE815-\uFA29]+$";
            return value.IsMatch(pattern);
        }

        public static bool IsUrl(this string value)
        {
            try
            {
                if (value.IsNullOrEmpty() || value.Contains(' '))
                {
                    return false;
                }
                Uri uri = new Uri(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsIdentityCardId(this string value)
        {
            if (value.Length != 15 && value.Length != 18)
            {
                return false;
            }
            Regex regex;
            string[] array;
            DateTime time;
            if (value.Length == 15)
            {
                regex = new Regex(@"^(\d{6})(\d{2})(\d{2})(\d{2})(\d{3})_");
                if (!regex.Match(value).Success)
                {
                    return false;
                }
                array = regex.Split(value);
                return DateTime.TryParse(string.Format("{0}-{1}-{2}", "19" + array[2], array[3], array[4]), out time);
            }
            regex = new Regex(@"^(\d{6})(\d{4})(\d{2})(\d{2})(\d{3})([0-9Xx])$");
            if (!regex.Match(value).Success)
            {
                return false;
            }
            array = regex.Split(value);
            if (!DateTime.TryParse(string.Format("{0}-{1}-{2}", array[2], array[3], array[4]), out time))
            {
                return false;
            }
            string[] chars = value.ToCharArray().Select(m => m.ToString()).ToArray();
            int[] weights = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                int num = int.Parse(chars[i]);
                sum = sum + num * weights[i];
            }
            int mod = sum % 11;
            string vCode = "10X98765432";
            string last = vCode.ToCharArray().ElementAt(mod).ToString();
            return chars.Last().ToUpper() == last;
        }

        public static bool IsMobileNumber(this string value, bool isRestrict = false)
        {
            string pattern = isRestrict ? @"^[1][3-8]\d{9}$" : @"^[1]\d{10}$";
            return value.IsMatch(pattern);
        }

        public static bool IsBase64(this string value)
        {
            try
            {
                byte[] converted = Convert.FromBase64String(value);
                return value.EndsWith("=");
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region others

        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        [DebuggerStepThrough]
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static string FormatWith(this string format, params object[] args)
        {
            format.CheckNotNull("format");
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        public static string ReverseString(this string value)
        {
            value.CheckNotNull("value");
            return new string(value.Reverse().ToArray());
        }

        public static bool IsImageFile(this string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }
            byte[] filedata = File.ReadAllBytes(filename);
            if (filedata.Length == 0)
            {
                return false;
            }
            ushort code = BitConverter.ToUInt16(filedata, 0);
            switch (code)
            {
                case 0x4D42: 
                case 0xD8FF: 
                case 0x4947: 
                case 0x5089: 
                    return true;
                default:
                    return false;
            }
        }

        public static string[] Split(this string value, string strSplit, bool removeEmptyEntries = false)
        {
            return value.Split(new[] { strSplit }, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static string ToMd5Hash(this string value)
        {
            return HashHelper.GetMd5(value);
        }

        public static int TextLength(this string value)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] bytes = ascii.GetBytes(value);
            foreach (byte b in bytes)
            {
                if (b == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }
            return tempLen;
        }

        public static T FromJsonString<T>(this string json)
        {
            json.CheckNotNull("json");
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string AddUrlQuery(this string url, params string[] queries)
        {
            foreach (string query in queries)
            {
                if (!url.Contains("?"))
                {
                    url += "?";
                }
                else if (!url.EndsWith("&"))
                {
                    url += "&";
                }

                url = url + query;
            }
            return url;
        }

        public static string GetUrlQuery(this string url, string key)
        {
            Uri uri = new Uri(url);
            string query = uri.Query;
            if (query.IsNullOrEmpty())
            {
                return string.Empty;
            }
            query = query.TrimStart('?');
            var dict = (from m in query.Split("&", true)
                        let strs = m.Split("=")
                        select new KeyValuePair<string, string>(strs[0], strs[1]))
                .ToDictionary(m => m.Key, m => m.Value);
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            return string.Empty;
        }

        public static string AddHashFragment(this string url, string query)
        {
            if (!url.Contains("#"))
            {
                url += "#";
            }

            return url + query;
        }

        public static byte[] ToBytes(this string value, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes(value);
        }

        public static string ToString2(this byte[] bytes, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetString(bytes);
        }

        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string ToBase64String(this string source, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return Convert.ToBase64String(encoding.GetBytes(source));
        }

        public static string FromBase64String(this string base64String, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = Convert.FromBase64String(base64String);
            return encoding.GetString(bytes);
        }

        public static string ToUrlDecode(this string source)
        {
            return HttpUtility.UrlDecode(source);
        }

        public static string ToUrlEncode(this string source)
        {
            return HttpUtility.UrlEncode(source);
        }

        public static string ToHtmlDecode(this string source)
        {
            return HttpUtility.HtmlDecode(source);
        }

        public static string ToHtmlEncode(this string source)
        {
            return HttpUtility.HtmlEncode(source);
        }

        public static string ToHexString(this string source, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = encoding.GetBytes(source);
            return bytes.ToHexString();
        }

        public static string FromHexString(this string hexString, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = hexString.ToHexBytes();
            return encoding.GetString(bytes);
        }

        public static string ToHexString(this byte[] bytes)
        {
            return bytes.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }

        public static byte[] ToHexBytes(this string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                hexString = hexString ?? "";
            }
            byte[] bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
        
        public static string ToUnicodeString(this string source)
        {
            Regex regex = new Regex(@"[^\u0000-\u00ff]");
            return regex.Replace(source, m => string.Format(@"\u{0:x4}", (short)m.Value[0]));
        }

        public static string FromUnicodeString(this string source)
        {
            Regex regex = new Regex(@"\\u([0-9a-fA-F]{4})", RegexOptions.Compiled);
            return regex.Replace(source,
                m =>
                {
                    short s;
                    if (short.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InstalledUICulture, out s))
                    {
                        return "" + (char)s;
                    }
                    return m.Value;
                });
        }

        public static int LevenshteinDistance(this string source, string target, out double similarity, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(source))
            {
                if (string.IsNullOrEmpty(target))
                {
                    similarity = 1;
                    return 0;
                }
                similarity = 0;
                return target.Length;
            }
            if (string.IsNullOrEmpty(target))
            {
                similarity = 0;
                return source.Length;
            }

            string from, to;
            if (ignoreCase)
            {
                from = source;
                to = target;
            }
            else
            {
                from = source.ToLower();
                to = source.ToLower();
            }

            int m = from.Length, n = to.Length;
            int[,] mn = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++)
            {
                mn[i, 0] = i;
            }
            for (int j = 1; j <= n; j++)
            {
                mn[0, j] = j;
            }
            for (int i = 1; i <= m; i++)
            {
                char c = from[i - 1];
                for (int j = 1; j <= n; j++)
                {
                    if (c == to[j - 1])
                    {
                        mn[i, j] = mn[i - 1, j - 1];
                    }
                    else
                    {
                        mn[i, j] = Math.Min(mn[i - 1, j - 1], Math.Min(mn[i - 1, j], mn[i, j - 1])) + 1;
                    }
                }
            }

            int maxLength = Math.Max(m, n);
            similarity = (double)(maxLength - mn[m, n]) / maxLength;
            return mn[m, n];
        }

        public static double GetSimilarityWith(this string source, string target, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(target))
            {
                return 1;
            }
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                return 0;
            }
            const double kq = 2, kr = 1, ks = 1;
            char[] sourceChars = source.ToCharArray(), targetChars = target.ToCharArray();

            int q = sourceChars.Intersect(targetChars).Count(), s = sourceChars.Length - q, r = targetChars.Length - q;
            return kq * q / (kq * q + kr * r + ks * s);
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }


        public static Guid ToGuid(this string value)
        {
            Guid gid;
            if (value.IsEmpty() || !Guid.TryParse(value.ToString(), out gid))
                return Guid.Empty;
            return gid;
        }

        #endregion



        #region Encrypt & Decrypt

       public static string EncryptAes(this string source, object key, bool needIV = false)
        {
            if (source.IsNullOrEmpty())
                return source;
            try
            {
                return AesHelper.Encrypt(source, key.ToStringSafe(), needIV);
            }
            catch
            {
                return source;
            }
        }

        public static string DecryptAes(this string source, object key, bool needIV = false)
        {
            if (source.IsNullOrEmpty())
                return source;
            try
            {
                return AesHelper.Decrypt(source, key.ToStringSafe(), needIV);
            }
            catch
            {
                return source;
            }

        }


        public static string Masked(this string source, int start, int count)
        {
            return source.Masked('x', start, count);
        }

        public static string Masked(this string source, char maskValue, int start, int count)
        {
            if (source.IsNullOrEmpty())
                return source;

            if (count > source.Length || count<0)
                count = source.Length-1;

            var firstPart = source.Substring(0, start);
            var lastPart = source.Substring(start + count);
            var middlePart = new string(maskValue, count);

            return firstPart + middlePart + lastPart;

        }

        #endregion
    }
}