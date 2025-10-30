using System;

namespace Utils {
    public class ConvertUtils {
        public static bool ToBool(string value, bool defaultValue = false) {
            bool result = defaultValue;
            if (!string.IsNullOrEmpty(value)) {
                try {
                    result = Convert.ToBoolean(value);
                } catch (Exception) { }
            }
            return result;
        }

        // 转成double数值类型
        public static double ToNum(string s, double defaultValue = 0) {
            double result = defaultValue;
            try {
                if (!string.IsNullOrEmpty(s)) {
                    result = double.Parse(s);
                }
            } catch {

            }
            return result;
        }

        // 转为整数
        public static int ToInt(string s, int defaultValue = 0) {
            int result = defaultValue;
            try {
                if (!string.IsNullOrEmpty(s)) {
                    result = int.Parse(s);
                }
            } catch {

            }
            return result;
        }
    }
}
