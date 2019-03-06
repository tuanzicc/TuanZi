
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace TuanZi.IO
{
    public class FileHelper
    {
        public static void CreateIfNotExists(string fileName)
        {
            if (File.Exists(fileName))
            {
                return;
            }

            string dir = Path.GetDirectoryName(fileName);
            if (dir != null)
            {
                DirectoryHelper.CreateIfNotExists(dir);
            }
            File.Create(fileName);
        }

        public static void DeleteIfExists(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }
            File.Delete(fileName);
        }

        public static void SetAttribute(string fileName, FileAttributes attribute, bool isSet)
        {
            FileInfo fi = new FileInfo(fileName);
            if (!fi.Exists)
            {
                throw new FileNotFoundException("File does not exists.", fileName);
            }
            if (isSet)
            {
                fi.Attributes = fi.Attributes | attribute;
            }
            else
            {
                fi.Attributes = fi.Attributes & ~attribute;
            }
        }

        public static string GetVersion(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(fileName);
                return fvi.FileVersion;
            }
            return null;
        }

        public static string GetFileMd5(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                const int bufferSize = 1024 * 1024;
                byte[] buffer = new byte[bufferSize];
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    md5.Initialize();
                    long offset = 0;
                    while (offset < fs.Length)
                    {
                        long readSize = bufferSize;
                        if (offset + readSize > fs.Length)
                        {
                            readSize = fs.Length - offset;
                        }
                        fs.Read(buffer, 0, (int)readSize);
                        if (offset + readSize < fs.Length)
                        {
                            md5.TransformBlock(buffer, 0, (int)readSize, buffer, 0);
                        }
                        else
                        {
                            md5.TransformFinalBlock(buffer, 0, (int)readSize);
                        }
                        offset += bufferSize;
                    }
                    fs.Close();
                    byte[] result = md5.Hash;
                    md5.Clear();
                    StringBuilder sb = new StringBuilder(32);
                    foreach (byte b in result)
                    {
                        sb.Append(b.ToString("X2"));
                    }
                    return sb.ToString();
                }
            }
        }

        public static Encoding GetEncoding(string fileName)
        {
            return GetEncoding(fileName, Encoding.Default);
        }

        public static Encoding GetEncoding(FileStream fs)
        {
            return GetEncoding(fs, Encoding.Default);
        }

        public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                return GetEncoding(fs, defaultEncoding);
            }
        }

        public static Encoding GetEncoding(FileStream fs, Encoding defaultEncoding)
        {
            Encoding targetEncoding = defaultEncoding;
            if (fs != null && fs.Length >= 2)
            {
                byte b1 = 0;
                byte b2 = 0;
                byte b3 = 0;
                byte b4 = 0;

                long oriPos = fs.Seek(0, SeekOrigin.Begin);
                fs.Seek(0, SeekOrigin.Begin);

                b1 = Convert.ToByte(fs.ReadByte());
                b2 = Convert.ToByte(fs.ReadByte());
                if (fs.Length > 2)
                {
                    b3 = Convert.ToByte(fs.ReadByte());
                }
                if (fs.Length > 3)
                {
                    b4 = Convert.ToByte(fs.ReadByte());
                }

                if (b1 == 0xFE && b2 == 0xFF)
                {
                    targetEncoding = Encoding.BigEndianUnicode;
                }
                if (b1 == 0xFF && b2 == 0xFE && b3 != 0xFF)
                {
                    targetEncoding = Encoding.Unicode;
                }
                if (b1 == 0xEF && b2 == 0xBB && b3 == 0xBF)
                {
                    targetEncoding = Encoding.UTF8;
                }

                fs.Seek(0, SeekOrigin.Begin);
            }
            return targetEncoding;
        }
    }
}