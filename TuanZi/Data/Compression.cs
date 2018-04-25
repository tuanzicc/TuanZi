
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace TuanZi.Data
{
    public static class Compression
    {
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (MemoryStream tmpMs = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
                    zip.CopyTo(tmpMs);
                    zip.Close();
                }
                return tmpMs.ToArray();
            }
        }

        public static string Compress(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            bytes = Compress(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string Decompress(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            byte[] bytes = Convert.FromBase64String(value);
            bytes = Decompress(bytes);
            return Encoding.UTF8.GetString(bytes);
        }

        public static void Zip(string sourceDir, string zipFile)
        {
            ZipFile.CreateFromDirectory(sourceDir, zipFile);
        }

        public static void UnZip(string zipFile, string targetDir)
        {
            ZipFile.ExtractToDirectory(zipFile, targetDir);
        }
    }
}