using System.IO;

namespace Utils {
    public class WinUtils {
        public static bool ShowFileProperties(string Filename) {
            DllUtils.SHELLEXECUTEINFO info = new DllUtils.SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = WinApi.SW_SHOW;
            info.fMask = WinApi.SEE_MASK_INVOKEIDLIST;
            return DllUtils.ShellExecuteEx(ref info);
        }

        public static DriveInfo GetDriveInfoByName(string driveName) {
            DriveInfo[] driveInfoArray = DriveInfo.GetDrives();
            foreach (DriveInfo single in driveInfoArray) {
                if (single.Name.Equals(driveName)) {
                    return single;
                }
            }
            return null;
        }
    }
}
