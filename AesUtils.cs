
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utils {
    public static class AesUtils {
        private static readonly byte[] Key = Convert.FromBase64String("Yl+wNn/Be259rsps4D9DkkjK6FcrHPOUsBNVZF39Puo=");
        private static readonly byte[] Iv = Convert.FromBase64String("fuDv69y3hkjoKs6ncA7Xvg==");
        public static string EntryptContent(string content) {
            byte[] textBytes = Encoding.UTF8.GetBytes(content);
            Aes aes = null;
            ICryptoTransform encryptor = null;
            try {
                aes = Aes.Create();
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                encryptor = aes.CreateEncryptor(Key, Iv);
                return Convert.ToBase64String(encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length));
            } catch (Exception e) {
                throw e;
            } finally {
                if (null != encryptor) {
                    encryptor.Dispose();
                }
                if (null != aes) {
                    aes.Dispose();
                }
            }
        }

        public static string DecryptContent(string content) {
            Aes aes = null;
            ICryptoTransform decryptor = null;
            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;
            try {
                aes = Aes.Create();
                aes.BlockSize = 128;
                aes.KeySize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                decryptor = aes.CreateDecryptor(Key, Iv);
                var cipherText = Convert.FromBase64String(content);
                msDecrypt = new MemoryStream(cipherText);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);
                return srDecrypt.ReadToEnd();
            } catch (Exception e) {
                throw e;
            } finally {
                if (null != srDecrypt) {
                    srDecrypt.Dispose();
                }
                if (null != csDecrypt) {
                    csDecrypt.Dispose();
                }
                if (null != msDecrypt) {
                    msDecrypt.Dispose();
                }
                if (null != decryptor) {
                    decryptor.Dispose();
                }
                if (null != aes) {
                    aes.Dispose();
                }
            }
        }
    }
}
