using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NCFAzureDurableFunctions.Src.Services.Helpers
{
    public class EncryptionHelper
    {
        private readonly string _key;

        public EncryptionHelper(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
                throw new ArgumentException("Encryption key must be at least 32 characters long.");
            _key = key;
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key.Substring(0, 32));
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length); // prepend IV
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_key.Substring(0, 32));
            aes.IV = fullCipher[..16];

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(fullCipher[16..]);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}