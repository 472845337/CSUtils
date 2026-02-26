using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utils {
    public class IniParserUtils {
        private static readonly IniParser.FileIniDataParser iniParser = new IniParser.FileIniDataParser();
        private static readonly Dictionary<string, IniData> iniDataDic = new Dictionary<string, IniData>();

        /// <summary>
        /// 1.自动创建文件
        /// 2.文件内容异常，自动去除异常行，替换为正确的内容
        /// 3.ini数据放到字典中
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IniData GetIniData(string filePath) {
            #region 如果文件不存在，创建文件
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
            if (!File.Exists(filePath)) {
                StreamWriter sw = File.CreateText(filePath);
                sw.Flush();
                sw.Close();
            }
            #endregion

            if (!iniDataDic.TryGetValue(filePath, out IniData _iniData)) {
                try {
                    _iniData = iniParser.ReadFile(filePath, new UTF8Encoding(false));
                } catch (Exception) {
                    IniFileCheck(filePath);
                    _iniData = iniParser.ReadFile(filePath, new UTF8Encoding(false));
                }
                iniDataDic.Add(filePath, _iniData);
            }
            return _iniData;
        }

        /// <summary>
        /// INI文件正确性校验并去掉非法行
        /// </summary>
        /// <param name="filePath"></param>
        private static void IniFileCheck(string filePath) {
            // 读取原始文件，过滤掉无效行
            var allLines = File.ReadAllLines(filePath, new UTF8Encoding(false));
            var validLines = new List<string>();
            bool checkValid = true;
            foreach (string line in allLines) {
                bool isRight = IsValidIniLine(line);
                if (!isRight) {
                    // 存在异常行
                    checkValid = false;
                } else {
                    validLines.Add(line);
                }
            }
            if (!checkValid) {
                // 文件有格式错误，将正确的文件内容写入到文件中
                File.WriteAllLines(filePath, validLines.ToArray(), new UTF8Encoding(false));
            }
        }

        private static bool IsValidIniLine(string line) {
            if (string.IsNullOrWhiteSpace(line))
                return true; // 空行是允许的

            line = line.Trim();

            // 有效的行包括：
            // - 节声明: [section]
            // - 键值对: key=value
            // - 注释: ;comment 或 #comment
            // - 空行

            return line.StartsWith("[") && line.EndsWith("]") ||  // 节
                   line.Contains("=") ||                         // 键值对
                   line.StartsWith(";") ||                       // 注释
                   line.StartsWith("#") ||                       // 注释
                   string.IsNullOrWhiteSpace(line);              // 空行
        }

        public static void SaveIniData(string filePath, string section, string key, string value) {
            IniData iniData = new IniData();
            iniData[section][key] = value;
            SaveIniData(filePath, iniData);
        }

        public static void SaveIniData(string filePath, IniData iniData) {
            if (null == iniData || iniData.Sections.Count == 0) {
                return;
            }
            if (!iniDataDic.TryGetValue(filePath, out _)) {
                GetIniData(filePath);
            }
            iniDataDic[filePath].Merge(iniData);
            iniParser.WriteFile(filePath, iniDataDic[filePath], new UTF8Encoding(false));
        }

        public static void EraseSection(string filePath, string section) {
            IniData iniData = GetIniData(filePath);
            if (null != iniData &&
                iniData.Sections.ContainsSection(section)) {
                iniData.Sections.RemoveSection(section);
                SaveIniData(filePath, iniData);
            }
        }

        /// <summary>
        /// Config配置放入IniData中
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="section">INI的section头</param>
        /// <param name="key">INI的关键字</param>
        /// <param name="from">原值</param>
        /// <param name="to">新值</param>
        public static void ConfigIniData<T>(IniData iniData, string section, string key, ref T from, T to) {
            bool isChange = false;
            if (from is bool fromBool && to is bool toBool) {
                if (fromBool != toBool) {
                    isChange = true;
                }
            } else if (from is string fromStr && to is string toStr) {
                if (!fromStr.Equals(toStr)) {
                    isChange = true;
                }
            } else if (from is int fromInt && to is int toInt) {
                if (fromInt != toInt) {
                    isChange = true;
                }
            } else if (from is uint fromUint && to is uint toUint) {
                if (fromUint != toUint) {
                    isChange = true;
                }
            } else if (from is double fromDouble && to is double toDouble) {
                if (fromDouble != toDouble) {
                    isChange = true;
                }
            } else if (null == from && null != to) {
                isChange = true;
            } else if (null != from && null == to) {
                isChange = true;
            }
            if (isChange) {
                iniData[section][key] = Convert.ToString(to);
                from = to;
            }
        }
    }
}
