
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
    }
}