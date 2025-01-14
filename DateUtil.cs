using System;

namespace Utils {
    public class DateUtil {
        public static DateTime GetTime(string time) {
            DateTime dateTime = DateTime.Parse(time);
            return dateTime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">时间戳</param>
        /// <returns></returns>
        public static DateTime GetTime(long time) {
            DateTime dateTime = new DateTime(time);
            return dateTime;
        }

        public static string Format(DateTime dateTime, string format) {
            return dateTime.ToString(format);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="operateType">1 add,2 set</param>
        /// <param name="field">属性</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static DateTime OperateDateTime(DateTime dateTime, int operateType, int field, int value) {
            DateTime newDateTime = dateTime;
            if (1 == operateType) {
                // 增
                if (1 == field) {
                    // 年
                    newDateTime = dateTime.AddYears(value);
                } else if (2 == field) {
                    // 月
                    newDateTime = dateTime.AddMonths(value);
                } else if (3 == field) {
                    // 日
                    newDateTime = dateTime.AddDays(value);
                } else if (2 == field) {
                    // 时
                    newDateTime = dateTime.AddHours(value);
                } else if (2 == field) {
                    // 分
                    newDateTime = dateTime.AddMinutes(value);
                } else if (2 == field) {
                    // 秒
                    newDateTime = dateTime.AddSeconds(value);
                } else if (2 == field) {
                    // 毫秒
                    newDateTime = dateTime.AddMilliseconds(value);
                }
            } else if (2 == operateType) {
                // 设置
                if (1 == field) {
                    // 年
                    newDateTime = new DateTime(value, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
                } else if (2 == field) {
                    // 月
                    newDateTime = new DateTime(dateTime.Year, value, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
                } else if (3 == field) {
                    // 日
                    newDateTime = new DateTime(dateTime.Year, dateTime.Month, value, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
                } else if (2 == field) {
                    // 时
                    newDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, value, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
                } else if (2 == field) {
                    // 分
                    newDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, value, dateTime.Second, dateTime.Millisecond);
                } else if (2 == field) {
                    // 秒
                    newDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, value, dateTime.Millisecond);
                } else if (2 == field) {
                    // 毫秒
                    newDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, value);
                }
            }
            return newDateTime;
        }
    }
}
