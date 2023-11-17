using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utils {
    public static class IniUtils {


        //类的构造函数，传递INI文件名  
        public static long IniWriteValue(string iniPath, string section, string key, string value) {
            // 如果文件不存在，创建文件
            string directoryPath = Path.GetDirectoryName(iniPath);
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
            return DllUtils.WritePrivateProfileString(section, key, value, iniPath);
        }

        //读取section中某个值  
        public static string IniReadValue(string iniPath, string section, string key) {
            var buffer = new byte[256];
            var len = DllUtils.GetPrivateProfileStringA(section, key, string.Empty, buffer, 256, iniPath);
            var result = Encoding.Default.GetString(buffer, 0, (int)len);
            return result;
        }
        // 封装的方法中，最有价值的是获取所有Sections和所有的Keys，网上关于这个的代码大部分是错误的，这里给出一个正确的方法：  
        /// 返回该配置文件中所有Section名称的集合  
        public static List<string> ReadSections(string iniPath) {
            var buffer = new byte[65535];
            var len = DllUtils.GetPrivateProfileStringA(null, null, null, buffer, buffer.Length, iniPath);
            var j = 0;
            var list = new List<string>();
            for (var i = 0; i < len; i++) {
                if (buffer[i] != 0) continue;
                list.Add(Encoding.Default.GetString(buffer, j, i - j));
                j = i + 1;
            }
            return list;
        }

        // 获取节点的所有KEY值  

        public static List<string> ReadKeys(string iniPath, string sectionName) {

            var buffer = new byte[5120];
            var rel = DllUtils.GetPrivateProfileStringA(sectionName, null, string.Empty, buffer, buffer.GetUpperBound(0), iniPath);

            var list = new List<string>();
            if (rel <= 0) return list;
            var iPos = 0;
            int iCnt;
            for (iCnt = 0; iCnt < rel; iCnt++) {
                if (buffer[iCnt] != 0x00) continue;
                var tmp = Encoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim();
                iPos = iCnt + 1;
                if (!string.IsNullOrEmpty(tmp)) {
                    list.Add(tmp);
                }
            }
            return list;
        }

        ///   <summary> 
        ///   删除这个项现有的字串。 
        ///   </summary> 
        ///   <param name="iniPath"></param>
        ///   <param name="sectionName"></param>
        ///   <param name="keyName"></param> 
        public static void DeleteKey(string iniPath, string sectionName, string keyName) {
            DllUtils.WritePrivateProfileString(sectionName, keyName, null, iniPath);
        }

        ///   <summary> 
        ///   删除这个小节的所有设置项。 
        ///   </summary> 
        ///   <param name="iniPath"> 要删除的小节名。这个字串不区分大小写。 </param>
        ///   <param name="section"></param> 
        public static void EraseSection(string iniPath, string section) {
            DllUtils.WritePrivateProfileString(section, null, null, iniPath);
        }
    }
}
