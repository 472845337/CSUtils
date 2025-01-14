
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utils {
    public class AesUtils {
        private static readonly byte[] Key = Convert.FromBase64String("Yl+wNn/Be259rsps4D9DkkjK6FcrHPOUsBNVZF39Puo=");
        private static readonly byte[] Iv = Convert.FromBase64String("fuDv69y3hkjoKs6ncA7Xvg==");

       /// <summary>
       /// AES加密字符串
       /// </summary>
       /// <param name="key">密钥</param>
       /// <param name="iv">向量</param>
       /// <param name="content">待加密内容</param>
       /// <returns>加密结果</returns>
        public static string Encrypt(byte[] key, byte[] iv, string content) {
            byte[] textBytes = Encoding.UTF8.GetBytes(content);
            using Aes aes = Aes.Create();
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            using ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
            return Convert.ToBase64String(encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length));
        }

        /// <summary>
        /// 使用默认的密钥和向量加密字符串
        /// </summary>
        /// <param name="content">待加密内容</param>
        /// <returns>加密结果</returns>
        public static string Encrypt(string content) {
            return Encrypt(Key, Iv, content);
        }

        /// <summary>
        /// AES解密字符串
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <param name="content">待解密内容</param>
        /// <returns>解密结果</returns>
        public static string Decrypt(byte[] key, byte[] iv, string content) {
            var cipherText = Convert.FromBase64String(content);
            using Aes aes = Aes.Create();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            using ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
            using MemoryStream msDecrypt = new MemoryStream(cipherText);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }

        /// <summary>
        /// 使用默认密钥和向量解密
        /// </summary>
        /// <param name="content">待解密字符串</param>
        /// <returns>解密结果</returns>
        public static string DecryptContent(string content) {
            return Decrypt(Key, Iv, content);
        }
    }
}
