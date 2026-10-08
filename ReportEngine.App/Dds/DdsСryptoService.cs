using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.App.Dds
{
    public static class DdsСryptoService
    {

        public static (string cryptedText, string iv) Encrypt(string plainText, string key)
        {
            // Превращаем строку в 32-байтовый ключ через SHA-256
            byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = keyBytes;          // ровно 32 байта — AES-256
            aesAlg.GenerateIV();            // IV всегда 16 байт

            byte[] iv = aesAlg.IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            byte[] cipherBytes;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                cipherBytes = msEncrypt.ToArray();
            }

            return (Convert.ToBase64String(cipherBytes), Convert.ToBase64String(iv));
        }


        public static string Decrypt(string cryptedText, string key, string iv)
        {
            byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            byte[] cipherBytes = Convert.FromBase64String(cryptedText);
            byte[] ivBytes = Convert.FromBase64String(iv);

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = keyBytes;
            aesAlg.IV = ivBytes;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(cipherBytes);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();

        }
    }
}
