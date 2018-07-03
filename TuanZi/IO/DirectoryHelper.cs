
using System;
using System.IO;
using TuanZi.Extensions;

namespace TuanZi.IO
{
    public static class DirectoryHelper
    {
        public static void CreateIfNotExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static void Copy(string sourcePath, string targetPath, string[] searchPatterns = null)
        {
            sourcePath.CheckNotNullOrEmpty("sourcePath");
            targetPath.CheckNotNullOrEmpty("targetPath");

            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException("The source directory does not exist when folders are recursively copied.");
            }
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            string[] dirs = Directory.GetDirectories(sourcePath);
            if (dirs.Length > 0)
            {
                foreach (string dir in dirs)
                {
                    Copy(dir, targetPath + dir.Substring(dir.LastIndexOf("\\", StringComparison.Ordinal)));
                }
            }
            if (searchPatterns != null && searchPatterns.Length > 0)
            {
                foreach (string searchPattern in searchPatterns)
                {
                    string[] files = Directory.GetFiles(sourcePath, searchPattern);
                    if (files.Length <= 0)
                    {
                        continue;
                    }
                    foreach (string file in files)
                    {
                        File.Copy(file, targetPath + file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal)));
                    }
                }
            }
            else
            {
                string[] files = Directory.GetFiles(sourcePath);
                if (files.Length <= 0)
                {
                    return;
                }
                foreach (string file in files)
                {
                    File.Copy(file, targetPath + file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal)));
                }
            }
        }

        public static bool Delete(string directory, bool isDeleteRoot = true)
        {
            directory.CheckNotNullOrEmpty("directory");

            bool flag = false;
            DirectoryInfo dirPathInfo = new DirectoryInfo(directory);
            if (dirPathInfo.Exists)
            {
                foreach (FileInfo fileInfo in dirPathInfo.GetFiles())
                {
                    fileInfo.Delete();
                }
                foreach (DirectoryInfo subDirectory in dirPathInfo.GetDirectories())
                {
                    Delete(subDirectory.FullName);
                }
                if (isDeleteRoot)
                {
                    dirPathInfo.Attributes = FileAttributes.Normal;
                    dirPathInfo.Delete();
                }
                flag = true;
            }
            return flag;
        }

        public static void SetAttributes(string directory, FileAttributes attribute, bool isSet)
        {
            directory.CheckNotNullOrEmpty("directory");
            DirectoryInfo di = new DirectoryInfo(directory);
            if (!di.Exists)
            {
                throw new DirectoryNotFoundException("Specified folder does not exist when setting directory attributes");
            }
            if (isSet)
            {
                di.Attributes = di.Attributes | attribute;
            }
            else
            {
                di.Attributes = di.Attributes & ~attribute;
            }
        }
    }
}