
using System.Security.Cryptography;
using System.Text;


namespace TuanZi.Secutiry
{
    public static class HashHelper
    {
        public static string GetMd5(string value, Encoding encoding = null)
        {
            value.CheckNotNull("value");
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = encoding.GetBytes(value);
            return GetMd5(bytes);
        }

        public static string GetMd5(byte[] bytes)
        {
            bytes.CheckNotNullOrEmpty("bytes");
            StringBuilder sb = new StringBuilder();
            MD5 hash = new MD5CryptoServiceProvider();
            bytes = hash.ComputeHash(bytes);
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        public static string GetSha1(string value, Encoding encoding = null)
        {
            value.CheckNotNullOrEmpty("value");

            StringBuilder sb = new StringBuilder();
            SHA1Managed hash = new SHA1Managed();
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = hash.ComputeHash(encoding.GetBytes(value));
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        public static string GetSha256(string value, Encoding encoding = null)
        {
            value.CheckNotNullOrEmpty("value");

            StringBuilder sb = new StringBuilder();
            SHA256Managed hash = new SHA256Managed();
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = hash.ComputeHash(encoding.GetBytes(value));
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        public static string GetSha512(string value, Encoding encoding = null)
        {
            value.CheckNotNullOrEmpty("value");

            StringBuilder sb = new StringBuilder();
            SHA512Managed hash = new SHA512Managed();
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = hash.ComputeHash(encoding.GetBytes(value));
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }
}