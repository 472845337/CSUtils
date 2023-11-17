using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Utils {
    public class AsdlUtils {

        public static void Disconnect() {
            OperateAsdl("-h");
        }

        public static void Connect() {
            OperateAsdl("-d ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">-d：连接，-h:断开</param>
        private static void OperateAsdl(string type) {
            RegistryKey UserKey = Registry.CurrentUser;
            RegistryKey Key = UserKey.OpenSubKey(@"RemoteAccess/Profile");
            string[] KeysList = Key.GetSubKeyNames();
            string Connection = KeysList[0];
            string WinDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string file = @"/rasphone.exe ";
            string FileName = WinDir + file;
            //2.在进程中调用，变量Connection为windows中的adsl连接的名称，在网上邻居－> 属性中可以看到，也就是上面取到的
            string args = type + "\"" + Connection + "\"";
            Process myProcess = new Process();
            ProcessStartInfo Adsl = new ProcessStartInfo {
                FileName = FileName
            };
            myProcess.StartInfo = Adsl;
            Adsl.Arguments = args;
            myProcess.Start();
        }
    }
}
