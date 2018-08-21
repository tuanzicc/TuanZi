
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

using Newtonsoft.Json;
using TuanZi.Collections;
using TuanZi.Data;
using TuanZi.Secutiry;


namespace TuanZi.Extensions
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

        public static string Cut(this string source, int length, string endStr = "...")
        {
            if (source.IsNullOrEmpty())
                return source;
            string temp = source;
            if (source.Length <= length)
            {
                return temp;
            }
            temp = temp.Substring(0, length);
            if (length < source.Length)
            {
                return temp + endStr;
            }
            return temp;
        }

        public static bool IsEmail(this string value)
        {
            return !String.IsNullOrEmpty(value) && RegularExpressions.IsEmail.IsMatch(value.Trim());
        }

        public static bool IsIpAddress(this string value)
        {
            const string pattern = @"^((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))$";
            return value.IsMatch(pattern);
        }

        public static bool IsNumeric(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return false;

            return !RegularExpressions.IsNotNumber.IsMatch(value) &&
                   !RegularExpressions.HasTwoDot.IsMatch(value) &&
                   !RegularExpressions.HasTwoMinus.IsMatch(value) &&
                   RegularExpressions.IsNumeric.IsMatch(value);
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

        [DebuggerStepThrough]
        public static bool IsWebUrl(this string value)
        {
            return !String.IsNullOrEmpty(value) && RegularExpressions.IsWebUrl.IsMatch(value.Trim());
        }

        [DebuggerStepThrough]
        public static string EnsureNumericOnly(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            return new String(str.Where(c => Char.IsDigit(c)).ToArray());
        }

        [DebuggerStepThrough]
        public static bool IsAlpha(this string value)
        {
            return RegularExpressions.IsAlpha.IsMatch(value);
        }

        [DebuggerStepThrough]
        public static bool IsAlphaNumeric(this string value)
        {
            return RegularExpressions.IsAlphaNumeric.IsMatch(value);
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

        public static bool IsPhoneNumber(this string value)
        {
            var number = value.ToDigitsOnly();
            return number.Length > 9;
            //string pattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            //return value.IsMatch(pattern);
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
        public static bool IsCaseSensitiveEqual(this string value, string comparing)
        {
            return string.CompareOrdinal(value, comparing) == 0;
        }

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="value">The string to check equality.</param>
        /// <param name="comparing">The comparing with string.</param>
        /// <returns>
        /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsCaseInsensitiveEqual(this string value, string comparing)
        {
            return string.Compare(value, comparing, StringComparison.OrdinalIgnoreCase) == 0;
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

        public static object FromJsonString(this string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
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

        public static string ToDigitsOnly(this string input)
        {
            return new Regex(@"[^\d]").Replace(input, string.Empty);
        }

        public static string ToTitleCase(this string value)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value);
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

        public static string ToPlainString(this string str, bool decode = true)
        {
            if (str == null)
                return null;
            var reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            var stripped = reg.Replace(str, string.Empty);
            return (decode ? HttpUtility.HtmlDecode(stripped) : stripped).Trim();

        }

        public static string ToUnicodeString(this string source)
        {
            Regex regex = new Regex(@"[^\u0000-\u00ff]");
            return regex.Replace(source, m => string.Format(@"\u{0:x4}", (short)m.Value[0]));
        }
        public static T ToEnum<T>(this string value, T defaultValue)
        {
            if (!value.HasValue())
            {
                return defaultValue;
            }
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }

        public static string ToSafe(this string value, string defaultValue = null)
        {
            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }
            return (defaultValue ?? String.Empty);
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

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
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


        // <summary>
        /// Mask by replacing characters with asterisks.
        /// </summary>
        /// <param name="value">The string</param>
        /// <param name="length">Number of characters to leave untouched.</param>
        /// <returns>The mask string</returns>
        [DebuggerStepThrough]
        public static string Mask(this string value, int length)
        {
            if (value.HasValue())
                return value.Substring(0, length) + new String('*', value.Length - length);
            return value;
        }

        public static string Mask(this string source, int start, int count)
        {
            return source.Mask('*', start, count);
        }

        public static string Mask(this string source, char maskValue, int start, int count)
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

        [DebuggerStepThrough]
        public static string Truncate(this string value, int maxLength, string suffix = "")
        {
            suffix.CheckNotNull(nameof(suffix));
            maxLength.CheckGreaterThan(nameof(maxLength), -1);

            int subStringLength = maxLength - suffix.Length;

            if (subStringLength <= 0)
                throw new Exception("Length of suffix string is greater or equal to maximumLength");

            if (value != null && value.Length > maxLength)
            {
                string truncatedString = value.Substring(0, subStringLength);
                // in case the last character is a space
                truncatedString = truncatedString.Trim();
                truncatedString += suffix;

                return truncatedString;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Ensure that a string starts with a string.
        /// </summary>
        /// <param name="value">The target string</param>
        /// <param name="startsWith">The string the target string should start with</param>
        /// <returns>The resulting string</returns>
        [DebuggerStepThrough]
        public static string EnsureStartsWith(this string value, string startsWith)
        {
            value.CheckNotNull(nameof(value));
            startsWith.CheckNotNull(nameof(startsWith));

            return value.StartsWith(startsWith) ? value : (startsWith + value);
        }

        /// <summary>
        /// Ensures the target string ends with the specified string.
        /// </summary>
        /// <param name="endWith">The target.</param>
        /// <param name="value">The value.</param>
        /// <returns>The target string with the value string at the end.</returns>
        [DebuggerStepThrough]
        public static string EnsureEndsWith(this string value, string endWith)
        {
            value.CheckNotNull(nameof(value));
            endWith.CheckNotNull(nameof(endWith));

            if (value.Length >= endWith.Length)
            {
                if (string.Compare(value, value.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    return value;

                string trimmedString = value.TrimEnd(null);

                if (string.Compare(trimmedString, trimmedString.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    return value;
            }

            return value + endWith;
        }
        [DebuggerStepThrough]
        public static string ToValidFileName(this string input, string replacement = "-")
        {
            return input.ToValidPathInternal(false, replacement);
        }

        [DebuggerStepThrough]
        public static string ToValidPath(this string input, string replacement = "-")
        {
            return input.ToValidPathInternal(true, replacement);
        }

        private static string ToValidPathInternal(this string input, bool isPath, string replacement)
        {
            var result = input.ToSafe();

            char[] invalidChars = isPath ? Path.GetInvalidPathChars() : Path.GetInvalidFileNameChars();

            foreach (var c in invalidChars)
            {
                result = result.Replace(c.ToString(), replacement ?? "-");
            }

            return result;
        }

        [DebuggerStepThrough]
        public static int[] ToIntArray(this string s)
        {
            return Array.ConvertAll(s.SplitSafe(","), v => int.Parse(v.Trim()));
        }

        [DebuggerStepThrough]
        public static bool ToIntArrayContains(this string s, int value, bool defaultValue)
        {
            if (s == null)
                return defaultValue;
            var arr = s.ToIntArray();
            if (arr == null || arr.Count() <= 0)
                return defaultValue;
            return arr.Contains(value);
        }

        [DebuggerStepThrough]
        public static string RemoveInvalidXmlChars(this string s)
        {
            if (s.IsEmpty())
                return s;

            return Regex.Replace(s, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", "", RegexOptions.Compiled);
        }

        [DebuggerStepThrough]
        public static string ReplaceCsvChars(this string s)
        {
            if (s.IsEmpty())
            {
                return "";
            }

            s = s.Replace(';', ',');
            s = s.Replace('\r', ' ');
            s = s.Replace('\n', ' ');
            return s.Replace("'", "");
        }

        [DebuggerStepThrough]
        public static string RemoveHtml(this string value)
        {
            return RemoveHtmlInternal(value, null);
        }

        public static string RemoveHtml(this string value, ICollection<string> removeTags)
        {
            return RemoveHtmlInternal(value, removeTags);
        }

        private static string RemoveHtmlInternal(string s, ICollection<string> removeTags)
        {
            List<string> removeTagsUpper = null;
            if (removeTags != null)
            {
                removeTagsUpper = new List<string>(removeTags.Count);

                foreach (string tag in removeTags)
                {
                    removeTagsUpper.Add(tag.ToUpperInvariant());
                }
            }

            return RegularExpressions.RemoveHTML.Replace(s, delegate (Match match)
            {
                string tag = match.Groups["tag"].Value.ToUpperInvariant();

                if (removeTagsUpper == null)
                    return string.Empty;
                else if (removeTagsUpper.Contains(tag))
                    return string.Empty;
                else
                    return match.Value;
            });
        }

        /// <summary>
        /// Replaces pascal casing with spaces. For example "CustomerId" would become "Customer Id".
        /// Strings that already contain spaces are ignored.
        /// </summary>
        /// <param name="value">String to split</param>
        /// <returns>The string after being split</returns>
        [DebuggerStepThrough]
        public static string SplitPascalCase(this string value)
        {
            //return Regex.Replace(input, "([A-Z][a-z])", " $1", RegexOptions.Compiled).Trim();
            StringBuilder sb = new StringBuilder();
            char[] ca = value.ToCharArray();
            sb.Append(ca[0]);
            for (int i = 1; i < ca.Length - 1; i++)
            {
                char c = ca[i];
                if (char.IsUpper(c) && (char.IsLower(ca[i + 1]) || char.IsLower(ca[i - 1])))
                {
                    sb.Append(" ");
                }
                sb.Append(c);
            }
            if (ca.Length > 1)
            {
                sb.Append(ca[ca.Length - 1]);
            }

            return sb.ToString();
        }

        [DebuggerStepThrough]
        public static string[] SplitSafe(this string value, string separator)
        {
            if (string.IsNullOrEmpty(value))
                return new string[0];
            return value.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>Splits a string into two strings</summary>
        /// <returns>true: success, false: failure</returns>
        [DebuggerStepThrough]
        [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
        public static bool SplitToPair(this string value, out string strLeft, out string strRight, string delimiter)
        {
            int idx = -1;
            if (value.IsEmpty() || delimiter.IsEmpty() || (idx = value.IndexOf(delimiter)) == -1)
            {
                strLeft = value;
                strRight = "";
                return false;
            }
            strLeft = value.Substring(0, idx);
            strRight = value.Substring(idx + delimiter.Length);
            return true;
        }

        [DebuggerStepThrough]
        public static string EncodeJsString(this string value)
        {
            return EncodeJsString(value, '"', true);
        }

        [DebuggerStepThrough]
        public static string EncodeJsString(this string value, char delimiter, bool appendDelimiters)
        {
            StringBuilder sb = new StringBuilder(value != null ? value.Length : 16);
            using (StringWriter w = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                EncodeJsString(w, value, delimiter, appendDelimiters);
                return w.ToString();
            }
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string enclosedIn)
        {
            return value.IsEnclosedIn(enclosedIn, StringComparison.CurrentCulture);
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string enclosedIn, StringComparison comparisonType)
        {
            if (String.IsNullOrEmpty(enclosedIn))
                return false;

            if (enclosedIn.Length == 1)
                return value.IsEnclosedIn(enclosedIn, enclosedIn, comparisonType);

            if (enclosedIn.Length % 2 == 0)
            {
                int len = enclosedIn.Length / 2;
                return value.IsEnclosedIn(
                    enclosedIn.Substring(0, len),
                    enclosedIn.Substring(len, len),
                    comparisonType);

            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string start, string end)
        {
            return value.IsEnclosedIn(start, end, StringComparison.CurrentCulture);
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string start, string end, StringComparison comparisonType)
        {
            return value.StartsWith(start, comparisonType) && value.EndsWith(end, comparisonType);
        }

        public static string RemoveEncloser(this string value, string encloser)
        {
            return value.RemoveEncloser(encloser, StringComparison.CurrentCulture);
        }

        public static string RemoveEncloser(this string value, string encloser, StringComparison comparisonType)
        {
            if (value.IsEnclosedIn(encloser, comparisonType))
            {
                int len = encloser.Length / 2;
                return value.Substring(
                    len,
                    value.Length - (len * 2));
            }

            return value;
        }

        public static string RemoveEncloser(this string value, string start, string end)
        {
            return value.RemoveEncloser(start, end, StringComparison.CurrentCulture);
        }

        public static string RemoveEncloser(this string value, string start, string end, StringComparison comparisonType)
        {
            if (value.IsEnclosedIn(start, end, comparisonType))
                return value.Substring(
                    start.Length,
                    value.Length - (start.Length + end.Length));

            return value;
        }

        /// <summary>Debug.WriteLine</summary>
        [DebuggerStepThrough]
        public static void Dump(this string value, bool appendMarks = false)
        {
            Debug.WriteLine(value);
            Debug.WriteLineIf(appendMarks, "------------------------------------------------");
        }
        public static string ToBr(this string str)
        {
            if (str.IsEmpty())
                return str;
            str = Regex.Replace(str, @"\r\n?|\n", "<br/>");
            return str;
        }

        /// <summary>Smart way to create a HTML attribute with a leading space.</summary>
        /// <param name="value">Name of the attribute.</param>
        /// <param name="name"></param>
        /// <param name="htmlEncode"></param>
        [SuppressMessage("ReSharper", "StringCompareIsCultureSpecific.3")]
        public static string ToAttribute(this string value, string name, bool htmlEncode = true)
        {
            if (name.IsEmpty())
                return "";

            if (value == "" && name != "value" && !name.StartsWith("data"))
                return "";

            if (name == "maxlength" && (value == "" || value == "0"))
                return "";

            if (name == "checked" || name == "disabled" || name == "multiple")
            {
                if (value == "" || string.Compare(value, "false", true) == 0)
                    return "";
                value = (string.Compare(value, "true", true) == 0 ? name : value);
            }

            if (name.StartsWith("data"))
                name = name.Insert(4, "-");

            return string.Format(" {0}=\"{1}\"", name, htmlEncode ? HttpUtility.HtmlEncode(value) : value);
        }

        /// <summary>Appends grow and uses delimiter if the string is not empty.</summary>
        [DebuggerStepThrough]
        public static string Grow(this string value, string grow, string delimiter)
        {
            if (string.IsNullOrEmpty(value))
                return (string.IsNullOrEmpty(grow) ? "" : grow);

            if (string.IsNullOrEmpty(grow))
                return (string.IsNullOrEmpty(value) ? "" : value);

            return string.Format("{0}{1}{2}", value, delimiter, grow);
        }

        /// <summary>Returns n/a if string is empty else self.</summary>
        [DebuggerStepThrough]
        public static string NaIfEmpty(this string value)
        {
            return (value.HasValue() ? value : "n/a");
        }

        /// <summary>Replaces substring with position x1 to x2 by replaceBy.</summary>
        [DebuggerStepThrough]
        public static string Replace(this string value, int x1, int x2, string replaceBy = null)
        {
            if (value.HasValue() && x1 > 0 && x2 > x1 && x2 < value.Length)
            {
                return value.Substring(0, x1) + (replaceBy == null ? "" : replaceBy) + value.Substring(x2 + 1);
            }
            return value;
        }

        [DebuggerStepThrough]
        public static string Replace(this string value, string oldValue, string newValue, StringComparison comparisonType)
        {
            try
            {
                int startIndex = 0;
                while (true)
                {
                    startIndex = value.IndexOf(oldValue, startIndex, comparisonType);
                    if (startIndex == -1)
                        break;

                    value = value.Substring(0, startIndex) + newValue + value.Substring(startIndex + oldValue.Length);

                    startIndex += newValue.Length;
                }
            }
            catch (Exception exc)
            {
                exc.Dump();
            }
            return value;
        }

        [DebuggerStepThrough]
        public static string TrimSafe(this string value)
        {
            return (value.HasValue() ? value.Trim() : value);
        }

        [DebuggerStepThrough]
        public static string Prettify(this string value, bool allowSpace = false, char[] allowChars = null)
        {
            string res = "";
            try
            {
                if (value.HasValue())
                {
                    StringBuilder sb = new StringBuilder();
                    bool space = false;
                    char ch;

                    for (int i = 0; i < value.Length; ++i)
                    {
                        ch = value[i];

                        if (ch == ' ' || ch == '-')
                        {
                            if (allowSpace && ch == ' ')
                                sb.Append(' ');
                            else if (!space)
                                sb.Append('-');
                            space = true;
                            continue;
                        }

                        space = false;

                        if ((ch >= 48 && ch <= 57) || (ch >= 65 && ch <= 90) || (ch >= 97 && ch <= 122))
                        {
                            sb.Append(ch);
                            continue;
                        }

                        if (allowChars != null && allowChars.Contains(ch))
                        {
                            sb.Append(ch);
                            continue;
                        }

                        switch (ch)
                        {
                            case '_': sb.Append(ch); break;

                            case 'ä': sb.Append("ae"); break;
                            case 'ö': sb.Append("oe"); break;
                            case 'ü': sb.Append("ue"); break;
                            case 'ß': sb.Append("ss"); break;
                            case 'Ä': sb.Append("AE"); break;
                            case 'Ö': sb.Append("OE"); break;
                            case 'Ü': sb.Append("UE"); break;

                            case 'é':
                            case 'è':
                            case 'ê': sb.Append('e'); break;
                            case 'á':
                            case 'à':
                            case 'â': sb.Append('a'); break;
                            case 'ú':
                            case 'ù':
                            case 'û': sb.Append('u'); break;
                            case 'ó':
                            case 'ò':
                            case 'ô': sb.Append('o'); break;
                        }   // switch
                    }   // for

                    if (sb.Length > 0)
                    {
                        res = sb.ToString().Trim(new char[] { ' ', '-' });

                        Regex pat = new Regex(@"(-{2,})");      // remove double SpaceChar
                        res = pat.Replace(res, "-");
                        res = res.Replace("__", "_");
                    }
                }
            }
            catch (Exception exp)
            {
                exp.Dump();
            }
            return (res.Length > 0 ? res : "null");
        }



        #endregion


        #region Helper

        private static void EncodeJsChar(TextWriter writer, char c, char delimiter)
        {
            switch (c)
            {
                case '\t':
                    writer.Write(@"\t");
                    break;
                case '\n':
                    writer.Write(@"\n");
                    break;
                case '\r':
                    writer.Write(@"\r");
                    break;
                case '\f':
                    writer.Write(@"\f");
                    break;
                case '\b':
                    writer.Write(@"\b");
                    break;
                case '\\':
                    writer.Write(@"\\");
                    break;
                //case '<':
                //case '>':
                //case '\'':
                //  StringUtils.WriteCharAsUnicode(writer, c);
                //  break;
                case '\'':
                    // only escape if this charater is being used as the delimiter
                    writer.Write((delimiter == '\'') ? @"\'" : @"'");
                    break;
                case '"':
                    // only escape if this charater is being used as the delimiter
                    writer.Write((delimiter == '"') ? "\\\"" : @"""");
                    break;
                default:
                    if (c > '\u001f')
                        writer.Write(c);
                    else
                        WriteCharAsUnicode(c, writer);
                    break;
            }
        }

        private static void EncodeJsString(TextWriter writer, string value, char delimiter, bool appendDelimiters)
        {
            // leading delimiter
            if (appendDelimiters)
                writer.Write(delimiter);

            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    EncodeJsChar(writer, value[i], delimiter);
                }
            }

            // trailing delimiter
            if (appendDelimiters)
                writer.Write(delimiter);
        }

        #endregion

        [DebuggerStepThrough]
        public static string ToUnicode(this char c)
        {
            using (StringWriter w = new StringWriter(CultureInfo.InvariantCulture))
            {
                WriteCharAsUnicode(c, w);
                return w.ToString();
            }
        }

        internal static void WriteCharAsUnicode(char c, TextWriter writer)
        {
            writer.CheckNotNull(nameof(writer));

            char h1 = ((c >> 12) & '\x000f').ToHex();
            char h2 = ((c >> 8) & '\x000f').ToHex();
            char h3 = ((c >> 4) & '\x000f').ToHex();
            char h4 = (c & '\x000f').ToHex();

            writer.Write('\\');
            writer.Write('u');
            writer.Write(h1);
            writer.Write(h2);
            writer.Write(h3);
            writer.Write(h4);
        }
    }
}