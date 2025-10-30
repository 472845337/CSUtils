using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utils {
    public class FileUtils {
        public static void CreateShortCut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null) {
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            string shortcutPath = GetUnexistLinkName(directory, shortcutName, ".lnk");

            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.WindowStyle = 1;
            shortcut.Description = description;
            shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;
            shortcut.Save();
        }

        public static void CreateUrlShortCut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null) {
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            string shortcutPath = GetUnexistLinkName(directory, shortcutName, ".url");

            WshShell shell = new WshShell();
            IWshURLShortcut shortcut = (IWshURLShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Save();
        }

        public static ShortCutUtil ReadShortCut(string path) {
            if (System.IO.File.Exists(path)) {
                WshShell shell = new WshShell();
                IWshShortcut shortCut = (IWshShortcut)shell.CreateShortcut(path);
                ShortCutUtil scu = new ShortCutUtil {
                    FullName = shortCut.FullName,
                    Arguments = shortCut.Arguments,
                    Description = shortCut.Description,
                    Hotkey = shortCut.Hotkey,
                    TargetPath = shortCut.TargetPath,
                    WindowStyle = shortCut.WindowStyle,
                    WorkingDirectory = shortCut.WorkingDirectory

                };
                return scu;
            } else {
                return null;
            }

        }

        public static UrlShortCut ReadUrlShortCut(string path) {
            if (System.IO.File.Exists(path)) {
                WshShell shell = new WshShell();
                IWshURLShortcut shortCut = (IWshURLShortcut)shell.CreateShortcut(path);
                UrlShortCut scu = new UrlShortCut {
                    FullName = shortCut.FullName,
                    Url = shortCut.TargetPath,
                };
                return scu;
            } else {
                return null;
            }

        }

        private static string GetUnexistLinkName(string directory, string shortcutName, string suffix) {
            if (string.IsNullOrEmpty(suffix)) {
                suffix = ".lnk";
            }
            string shortcutPath = Path.Combine(directory, string.Format("{0}{1}", shortcutName, suffix));
            int index = 1;
            while (System.IO.File.Exists(shortcutPath)) {
                shortcutPath = Path.Combine(directory, string.Format("{0} ({1}){2}", shortcutName, index++, suffix));
            }
            return shortcutPath;
        }

        public static void CreateShortCutOnDesktop(string shortcutName, string targetPath, string description = null, string iconLocation = null) {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            CreateShortCut(desktop, shortcutName, targetPath, description, iconLocation);
        }

        public class ShortCutUtil {
            public string FullName { get; set; }
            public string Arguments { get; set; }
            public string Description { get; set; }
            public string Hotkey { get; set; }
            public string IconLocation { get; set; }
            public string TargetPath { get; set; }
            public int WindowStyle { get; set; }
            public string WorkingDirectory { get; set; }
        }

        public class UrlShortCut {
            public string FullName { get; set; }
            public string Url { get; set; }
        }

        public static List<string> GetFileNames(string path, bool? includeSub, string include, string exclude) {
            var files = new List<string>();
            if (Directory.Exists(path)) {
                var root = new DirectoryInfo(path);
                foreach (var file in root.EnumerateFiles()) {
                    if (IsFileIncluded(file.Name, include, exclude)) {
                        files.Add(file.FullName);
                    }
                }
                if (null != includeSub && (bool)includeSub) {
                    foreach (var dir in root.EnumerateDirectories()) {
                        files.AddRange(GetFileNames(dir.FullName, includeSub, include, exclude));
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// 检查文件是否应该被包含在结果中
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="include">包含模式，多个用逗号隔开</param>
        /// <param name="exclude">排除模式，多个用逗号隔开</param>
        /// <returns>true表示包含，false表示排除</returns>
        private static bool IsFileIncluded(string fileName, string include, string exclude) {
            // 如果指定了包含规则，检查文件是否匹配任何包含模式
            if (!string.IsNullOrWhiteSpace(include)) {
                var includePatterns = include.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(p => p.Trim())
                                            .Where(p => !string.IsNullOrEmpty(p))
                                            .ToList();

                if (includePatterns.Any()) {
                    bool included = includePatterns.Any(pattern => IsFileNameMatch(fileName, pattern));
                    if (!included) {
                        return false; // 不匹配任何包含模式，排除
                    }
                }
            }

            // 如果指定了排除规则，检查文件是否匹配任何排除模式
            if (!string.IsNullOrWhiteSpace(exclude)) {
                var excludePatterns = exclude.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(p => p.Trim())
                                            .Where(p => !string.IsNullOrEmpty(p))
                                            .ToList();

                if (excludePatterns.Any(pattern => IsFileNameMatch(fileName, pattern))) {
                    return false; // 匹配排除模式，排除
                }
            }

            return true;
        }

        /// <summary>
        /// 检查文件名是否匹配模式
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="pattern">模式，支持 * 和 ? 通配符</param>
        /// <returns>true表示匹配</returns>
        private static bool IsFileNameMatch(string fileName, string pattern) {
            try {
                // 简单的通配符匹配
                if (pattern.Contains("*") || pattern.Contains("?")) {
                    // 将通配符模式转换为正则表达式
                    string regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
                        .Replace("\\*", ".*")
                        .Replace("\\?", ".") + "$";

                    return System.Text.RegularExpressions.Regex.IsMatch(fileName, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                } else {
                    // 精确匹配（忽略大小写）
                    return string.Equals(fileName, pattern, StringComparison.OrdinalIgnoreCase);
                }
            } catch {
                // 如果正则表达式出错，回退到简单匹配
                return fileName.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }
    }
}
