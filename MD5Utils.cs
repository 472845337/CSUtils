using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Utils {
    public static class Md5Utils {
        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Md5Encrypt16(string password) {
            using var md5 = new MD5CryptoServiceProvider();
            var t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(password)), 4, 8);
            t2 = t2.Replace("-", string.Empty);
            return t2;
        }
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Md5Encrypt32(string password) {
            var cl = password;
            var pwd = string.Empty;
            using var md5 = MD5.Create(); //实例化一个md5对像
                                          // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            var s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            return s.Aggregate(pwd, (current, t) => current + t.ToString("X"));
        }

        public static string Md5Encrypt64(string password) {
            var cl = password;
            using var md5 = MD5.Create(); //实例化一个md5对像
                                          // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            var s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            return Convert.ToBase64String(s);
        }
    }
}
