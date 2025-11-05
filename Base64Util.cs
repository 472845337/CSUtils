using System;
using System.Text;

namespace Utils {
    public class Base64Util {
        // 字符串转换为 Base64
        public static string Encode(string text) {
            try {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                return Convert.ToBase64String(bytes);
            } catch {
                // 异常了
                return text;
            }
        }

        // Base64 转换为字符串
        public static string Decode(string base64String) {
            try {
                byte[] bytes = Convert.FromBase64String(base64String);
                return Encoding.UTF8.GetString(bytes);
            } catch {
                return base64String;
            }
        }
    }
}
