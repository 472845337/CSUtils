using System;

namespace Utils {
    public class NumberUtils {
        public static double Accuracy(double d, int digit) {
            int digitNumber = GetDigit(digit);
            return Math.Round(d * digitNumber, digit) / (double)digitNumber;
        }

        public static double Accuracy(float f, int digit) {
            int digitNumber = GetDigit(digit);
            return Math.Round(f * digitNumber, digit) / (float)digitNumber;
        }

        // 是否数字
        public static bool IsNumeric(string s, out double result) {
            bool bReturn = false;
            result = 0;
            try {
                if (!string.IsNullOrEmpty(s)) {
                    result = double.Parse(s);
                    bReturn = true;
                }
            } catch {

            }
            return bReturn;
        }

        //判断是否为整数
        public static bool IsInt(string s, out int result) {
            bool bReturn = false;
            result = 0;
            try {
                if (!string.IsNullOrEmpty(s)) {
                    result = int.Parse(s);
                    bReturn = true;
                }
            } catch {

            }
            return bReturn;
        }

        private static int GetDigit(int digit) {
            int a = 1;
            for (int i = 0; i < digit; i++) {
                a *= 10;
            }
            return a;
        }
    }
}
