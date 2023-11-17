using IWshRuntimeLibrary;
using System;
using System.IO;

namespace Utils {
    public class FileUtils {
        public static void CreateShortCut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null) {
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            string shortcutPath = GetUnexistLinkName(directory, shortcutName);

            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.WindowStyle = 1;
            shortcut.Description = description;
            shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;
            shortcut.Save();
        }

        public static IWshShortcut ReadShortCut(string path) {
            if (System.IO.File.Exists(path)) {
                WshShell shell = new WshShell();
                IWshShortcut shortCut = (IWshShortcut)shell.CreateShortcut(path);
                return shortCut;
            } else {
                return null;
            }

        }

        private static string GetUnexistLinkName(string directory, string shortcutName) {
            string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
            int index = 1;
            while (System.IO.File.Exists(shortcutPath)) {
                shortcutPath = Path.Combine(directory, string.Format("{0} ({1}).lnk", shortcutName, index++));
            }
            return shortcutPath;
        }

        public static void CreateShortCutOnDesktop(string shortcutName, string targetPath, string description = null, string iconLocation = null) {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            CreateShortCut(desktop, shortcutName, targetPath, description, iconLocation);
        }
    }
}
