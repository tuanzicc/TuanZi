using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using TuanZi.Extensions;

namespace TuanZi.Secutiry
{
    public class AesHelper
    {
        private readonly bool _needIV;

        public AesHelper(bool needIV = false)
            : this(GetRandomKey(), needIV)
        { }

        public AesHelper(string key, bool needIV = false)
        {
            Key = key;
            _needIV = needIV;
        }

        public string Key { get; }

        #region Instance Methods

        public byte[] Encrypt(byte[] decodeBytes)
        {
            return Encrypt(decodeBytes, Key, _needIV);
        }

        public byte[] Decrypt(byte[] encodeBytes)
        {
            return Decrypt(encodeBytes, Key, _needIV);
        }

        public string Encrypt(string source)
        {
            return Encrypt(source, Key, _needIV);
        }

        public string Decrypt(string source)
        {
            return Decrypt(source, Key, _needIV);
        }

        public void EncryptFile(string sourceFile, string targetFile)
        {
            EncryptFile(sourceFile, targetFile, Key, _needIV);
        }

        public void DecryptFile(string sourceFile, string targetFile)
        {
            DecryptFile(sourceFile, targetFile, Key, _needIV);
        }

        #endregion

        #region Static Methods

        public static byte[] Encrypt(byte[] decodeBytes, string key, bool needIV = false)
        {
            decodeBytes.CheckNotNull("decodeBytes");
            using (Aes aes = Aes.Create())
            {
                aes.Key = CheckKey(key);
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.ECB;
                byte[] ivBytes = { };
                if (needIV)
                {
                    aes.Mode = CipherMode.CBC;
                    aes.GenerateIV();
                    ivBytes = aes.IV;
                }
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] encodeBytes = encryptor.TransformFinalBlock(decodeBytes, 0, decodeBytes.Length);
                    aes.Clear();
                    return needIV ? ivBytes.Concat(encodeBytes).ToArray() : encodeBytes;
                }
            }
        }

        public static byte[] Decrypt(byte[] encodeBytes, string key, bool needIV = false)
        {
            encodeBytes.CheckNotNull("source");
            using (Aes aes = Aes.Create())
            {
                aes.Key = CheckKey(key);
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.ECB;
                if (needIV)
                {
                    aes.Mode = CipherMode.CBC;
                    const int ivLength = 16;
                    byte[] ivBytes = new byte[ivLength], newEncodeBytes = new byte[encodeBytes.Length - ivLength];
                    Array.Copy(encodeBytes, 0, ivBytes, 0, ivLength);
                    aes.IV = ivBytes;
                    Array.Copy(encodeBytes, ivLength, newEncodeBytes, 0, newEncodeBytes.Length);
                    encodeBytes = newEncodeBytes;
                }
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decodeBytes = decryptor.TransformFinalBlock(encodeBytes, 0, encodeBytes.Length);
                    aes.Clear();
                    return decodeBytes;
                }
            }
        }

        public static string Encrypt(string source, string key, bool needIV = false)
        {
            source.CheckNotNull("source");

            byte[] decodeBytes = source.ToBytes();
            byte[] encodeBytes = Encrypt(decodeBytes, key, needIV);
            return Convert.ToBase64String(encodeBytes, 0, encodeBytes.Length);
        }

        public static string Decrypt(string source, string key, bool needIV = false)
        {
            source.CheckNotNull("source");

            byte[] encodeBytes = Convert.FromBase64String(source);
            byte[] decodeBytes = Decrypt(encodeBytes, key, needIV);
            return decodeBytes.ToString2();
        }

        public static void EncryptFile(string sourceFile, string targetFile, string key, bool needIV = false)
        {
            sourceFile.CheckFileExists("sourceFile");
            targetFile.CheckNotNullOrEmpty("targetFile");

            using (FileStream ifs = new FileStream(sourceFile, FileMode.Open, FileAccess.Read),
                    ofs = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
            {
                long length = ifs.Length;
                byte[] decodeBytes = new byte[length];
                ifs.Read(decodeBytes, 0, decodeBytes.Length);
                byte[] encodeBytes = Encrypt(decodeBytes, key, needIV);
                ofs.Write(encodeBytes, 0, encodeBytes.Length);
            }
        }

        public static void DecryptFile(string sourceFile, string targetFile, string key, bool needIV = false)
        {
            sourceFile.CheckFileExists("sourceFile");
            targetFile.CheckNotNullOrEmpty("targetFile");

            using (FileStream ifs = new FileStream(sourceFile, FileMode.Open, FileAccess.Read),
                    ofs = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
            {
                long length = ifs.Length;
                byte[] encodeBytes = new byte[length];
                ifs.Read(encodeBytes, 0, encodeBytes.Length);
                byte[] decodeBytes = Decrypt(encodeBytes, key, needIV);
                ofs.Write(decodeBytes, 0, decodeBytes.Length);
            }
        }

        public static string GetRandomKey()
        {
            using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider())
            {
                provider.GenerateKey();
                Console.WriteLine(provider.Key.Length);
                return Convert.ToBase64String(provider.Key);
            }
        }

        private static byte[] CheckKey(string key)
        {
            key.CheckNotNull("key");
            byte[] bytes, keyBytes = new byte[32];
            try
            {
                bytes = Convert.FromBase64String(key);
            }
            catch (FormatException)
            {
                bytes = key.ToBytes();
            }
            if (bytes.Length < 32)
            {
                Array.Copy(bytes, 0, keyBytes, 0, bytes.Length);
            }
            else if (bytes.Length > 32)
            {
                Array.Copy(bytes, 0, keyBytes, 0, 32);
            }
            else
            {
                keyBytes = bytes;
            }
            return keyBytes;
        }

        #endregion
    }
}