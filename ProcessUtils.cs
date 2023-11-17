using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Utils {
    public static class ProcessUtils {

        /// <summary>
        /// 获取进程名
        /// 
        /// </summary>
        /// <param name="processName">进程名</param>
        /// <param name="processPath"></param>
        /// <returns></returns>
        public static List<Process> GetProcessByName(string processName, string processPath) {
            if (processName.ToLower().EndsWith(".exe")) {
                processName = processName.Substring(0, processName.Length - 4);
            }
            List<Process> processList = new List<Process>();
            Process[] processArray = Process.GetProcessesByName(processName);
            if (!string.IsNullOrEmpty(processPath)) {
                // 路径的判断
                foreach (Process process in processArray) {
                    // MainModule会引起64位和32位的兼容问题，使用api来获取进程
                    //if (process.MainModule.FileName.Equals(processPath)) {
                    //    processList.Add(process);
                    //}

                    var processHandler = DllUtils.OpenProcess(0x0400 | 0x0010, false, (uint)process.Id);
                    if (processHandler == IntPtr.Zero) {
                        return null;
                    }

                    const int lengthSb = 4000;

                    var sb = new StringBuilder(lengthSb);

                    if (DllUtils.GetModuleFileNameEx(processHandler, IntPtr.Zero, sb, lengthSb)) {
                        if (sb.ToString().Equals(processPath)) {
                            processList.Add(process);
                        }

                    }
                    DllUtils.CloseHandle(processHandler);

                }
            } else {
                processList.AddRange(processArray);
            }
            return processList;
        }

        public static string GetProcessName(string processPath) {
            FileInfo fileInfo = new FileInfo(processPath);
            if (fileInfo.Name.Contains(".")) {
                return fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("."));
            } else {
                return fileInfo.Name;
            }
        }
    }
}
