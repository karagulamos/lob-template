using System;
using System.Security.Cryptography;
using System.Text;
using Business.Core.Common.Exceptions;

namespace Business.Core.Cryptography
{
    internal static class CryptManager
    {
        private const string EncryptionKey = "X-BUSINESSABCDEFGH123456780";

        public static string Encrypt(string unencryptedText)
        {
            if (string.IsNullOrEmpty(unencryptedText))
                throw new BusinessGenericException("You cannot encrypt an empty text", "XRPT001");

            var hashmd5 = new MD5CryptoServiceProvider();
            var hashedKeyBytes = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));

            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = hashedKeyBytes,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var unencryptedBytes = Encoding.UTF8.GetBytes(unencryptedText);
            var encryptedBytes = tdes.CreateEncryptor()
                                     .TransformFinalBlock(unencryptedBytes, 0, unencryptedBytes.Length);

            tdes.Clear();

            return Convert.ToBase64String(encryptedBytes, 0, encryptedBytes.Length);
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                throw new BusinessGenericException("You cannot decrypt an empty text", "XRPT002");

            var hashmd5 = new MD5CryptoServiceProvider();
            var hashedKeyBytes = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));

            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = hashedKeyBytes,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptedBytes = tdes.CreateDecryptor()
                                     .TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            tdes.Clear();

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
