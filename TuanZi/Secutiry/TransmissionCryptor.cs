
using System;
using System.Security.Cryptography;
using System.Text;
using TuanZi.Collections;
using TuanZi.Extensions;
using TuanZi.Properties;


namespace TuanZi.Secutiry
{
    public class TransmissionCryptor
    {
        private readonly string _ownPrivateKey;
        private readonly string _facePublicKey;
        private static readonly string Separator = Convert.ToBase64String(Encoding.UTF8.GetBytes("#@|Tuan|@#"));

        public TransmissionCryptor(string ownPrivateKey, string facePublicKey)
        {
            ownPrivateKey.CheckNotNull("ownPrivateKey");
            facePublicKey.CheckNotNull("facePublicKey");

            _ownPrivateKey = ownPrivateKey;
            _facePublicKey = facePublicKey;
        }

        public string DecryptAndVerifyData(string data)
        {
            data.CheckNotNullOrEmpty("data");

            string[] separators = { Separator };
            string[] datas = data.Split(separators, StringSplitOptions.None);
            byte[] keyBytes = RsaHelper.Decrypt(Convert.FromBase64String(datas[0]), _ownPrivateKey);
            string key = keyBytes.ToString2();
            data = new AesHelper(key, true).Decrypt(datas[1]);
            datas = data.Split(separators, StringSplitOptions.None);
            data = datas[0];
            if (RsaHelper.VerifyData(data, datas[1], _facePublicKey))
            {
                return data;
            }
            throw new CryptographicException("Encrypted data failed to verify when decrypted");
        }

        public string EncryptData(string data)
        {
            data.CheckNotNull("data");

            string signData = RsaHelper.SignData(data, _ownPrivateKey);
            data = new[] { data, signData }.ExpandAndToString(Separator);
            AesHelper aes = new AesHelper(true);
            data = aes.Encrypt(data);
            byte[] keyBytes = aes.Key.ToBytes();
            string enDesKey = Convert.ToBase64String(RsaHelper.Encrypt(keyBytes, _facePublicKey));
            return new[] { enDesKey, data }.ExpandAndToString(Separator);
        }
    }
}