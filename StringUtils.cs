
using System;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class StringUtils
    {
        public static string TxtEncode(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace(Environment.NewLine, "\\r\\n").Replace("\n", "\\n").Replace("\r", "\\r");
            }
            return str;
        }

        public static string TxtDecode(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Replace("\\r\\n", Environment.NewLine).Replace("\\n", "\n").Replace("\\r", "\r");
            }
            return str;
        }

        public static string Trim(string str)
        {
            return str.Replace("\n", "").Replace("\t", "").Replace("\r", "").Trim();
        }


        public static string FormatSize(float byteSize)
        {
            string unit;
            var count = byteSize;
            if (byteSize > 536870912)
            {
                // GB
                unit = "GB";
                count = byteSize / 1024.0F / 1024.0F / 1024.0F;
            }
            else if (byteSize > 524288)
            {
                // MB
                unit = "MB";
                count = byteSize / 1024.0F / 1024.0F;
            }
            else if (byteSize > 512)
            {
                // KB
                unit = "KB";
                count = byteSize / 1024.0F;
            }
            else
            {
                unit = "B";
            }

            return string.Format("{0:###,##0.00}" + unit, count);
        }

        public static bool IsNumeric(string s, out double result)
        {
            bool bReturn = false;
            result = 0;
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    result = double.Parse(s);
                    bReturn = true;
                }
            }
            catch
            {

            }
            return bReturn;
        }
        //判断是否为正整数
        public static bool IsInt(string s, out int result)
        {
            bool bReturn = false;
            result = 0;
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    result = int.Parse(s);
                    bReturn = true;
                }
            }
            catch
            {

            }
            return bReturn;
        }

        public static bool IsByte(string s, out byte result)
        {
            bool bReturn = false;
            result = 0;
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    result = byte.Parse(s);
                    bReturn = true;
                }
            }
            catch
            {

            }
            return bReturn;
        }

        public static bool IsBool(string s, out bool result)
        {
            bool bReturn = false;
            result = false;
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    result = System.Convert.ToBoolean(s);
                    bReturn = true;
                }
            }
            catch
            {

            }
            return bReturn;
        }

        public static void TransferBool(string s, ref bool result, bool defaultValue)
        {
            result = defaultValue;
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    result = Convert.ToBoolean(s);
                }
            }
            catch
            {

            }
        }

        public static bool IsHtmlColor(string htmlColor)
        {
            var type = "^#[0-9a-fA-F]{6}$";
            var re = new Regex(type);
            if (!re.IsMatch(htmlColor))
            {
                type = "^[rR][gG][Bb][\\(]([\\s]*(2[0-4][0-9]|25[0-5]|[01]?[0-9][0-9]?)[\\s]*,){2}[\\s]*(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)[\\s]*[\\)]{1}$";
                re = new Regex(type);
                if (!re.IsMatch(htmlColor))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }
    }
}
