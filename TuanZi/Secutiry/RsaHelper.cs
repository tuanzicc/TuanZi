using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using TuanZi.Extensions;

namespace TuanZi.Secutiry
{
    public class RsaHelper
    {
        public RsaHelper()
        {
            RSA rsa = RSA.Create();
            PublicKey = rsa.ToXmlString2(false);
            PrivateKey = rsa.ToXmlString2(true);
        }

        public string PublicKey { get; }

        public string PrivateKey { get; }
        
        #region Instance Methods

        public byte[] Encrypt(byte[] source)
        {
            source.CheckNotNull("source");
            return Encrypt(source, PublicKey);
        }

        public byte[] Decrypt(byte[] source)
        {
            source.CheckNotNull("source");
            return Decrypt(source, PrivateKey);
        }

        public byte[] SignData(byte[] source)
        {
            source.CheckNotNull("source");

            return SignData(source, PrivateKey);
        }

        public bool VerifyData(byte[] source, byte[] signData)
        {
            source.CheckNotNull("source");
            signData.CheckNotNull("signData");

            return VerifyData(source, signData, PublicKey);
        }

        public string Encrypt(string source)
        {
            source.CheckNotNull("source");
            return Encrypt(source, PublicKey);
        }

        public string Decrypt(string source)
        {
            source.CheckNotNullOrEmpty("source");
            return Decrypt(source, PrivateKey);
        }

        public string SignData(string source)
        {
            source.CheckNotNull("source");

            return SignData(source, PrivateKey);
        }

        public bool VerifyData(string source, string signData)
        {
            source.CheckNotNull("source");
            signData.CheckNotNullOrEmpty("signData");

            return VerifyData(source, signData, PublicKey);
        }

        #endregion

        #region Static Methods

        public static byte[] Encrypt(byte[] source, string publicKey)
        {
            source.CheckNotNull("source");
            publicKey.CheckNotNullOrEmpty("publicKey");

            RSA rsa = RSA.Create();
            rsa.FromXmlString2(publicKey);
            return rsa.Encrypt(source, RSAEncryptionPadding.Pkcs1);
        }

        public static byte[] Decrypt(byte[] source, string privateKey)
        {
            source.CheckNotNull("source");
            privateKey.CheckNotNullOrEmpty("privateKey");

            RSA rsa = RSA.Create();
            rsa.FromXmlString2(privateKey);
            return rsa.Decrypt(source, RSAEncryptionPadding.Pkcs1);
        }

        public static byte[] SignData(byte[] source, string privateKey)
        {
            source.CheckNotNull("source");
            privateKey.CheckNotNullOrEmpty("privateKey");

            RSA rsa = RSA.Create();
            rsa.FromXmlString2(privateKey);
            return rsa.SignData(source, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }

        public static bool VerifyData(byte[] source, byte[] signData, string publicKey)
        {
            source.CheckNotNull("source");
            signData.CheckNotNull("signData");

            RSA rsa = RSA.Create();
            rsa.FromXmlString2(publicKey);
            return rsa.VerifyData(source, signData, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }

        public static string Encrypt(string source, string publicKey)
        {
            source.CheckNotNull("source");
            publicKey.CheckNotNullOrEmpty("publicKey");

            byte[] bytes = Encoding.UTF8.GetBytes(source);
            bytes = Encrypt(bytes, publicKey);
            return Convert.ToBase64String(bytes);
        }

        public static string Decrypt(string source, string privateKey)
        {
            source.CheckNotNullOrEmpty("source");
            privateKey.CheckNotNullOrEmpty("privateKey");

            byte[] bytes = Convert.FromBase64String(source);
            bytes = Decrypt(bytes, privateKey);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string SignData(string source, string privateKey)
        {
            source.CheckNotNull("source");

            byte[] bytes = Encoding.UTF8.GetBytes(source);
            byte[] signBytes = SignData(bytes, privateKey);
            return Convert.ToBase64String(signBytes);
        }

        public static bool VerifyData(string source, string signData, string publicKey)
        {
            source.CheckNotNull("source");
            signData.CheckNotNullOrEmpty("signData");

            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            byte[] signBytes = Convert.FromBase64String(signData);
            return VerifyData(sourceBytes, signBytes, publicKey);
        }

        #endregion
    }


    internal static class RSAKeyExtensions
    {
        #region XML

        public static void FromXmlString2(this RSA rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        public static string ToXmlString2(this RSA rsa, bool includePrivateParameters)
        {
            RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                  parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
                  parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
                  parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
                  parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
                  parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
                  parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
                  parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
                  parameters.D != null ? Convert.ToBase64String(parameters.D) : null);
        }

        #endregion
    }
}