using Business.Core.Cryptography;

namespace Business.Core.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Encrypt(this string textToEncrypt)
        {
            return CryptManager.Encrypt(textToEncrypt);
        }

        public static string Decrypt(this string textToDecrypt)
        {
            return CryptManager.Decrypt(textToDecrypt);
        }
    }
}
