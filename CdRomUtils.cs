using System;

namespace Utils {
    public class CdRomUtils {
        public static void OpenCdRom() {
            DllUtils.MciSendString("set cdaudio door open", null, 0, IntPtr.Zero);//打开光驱
        }

        public static void CloseCdRom() {
            DllUtils.MciSendString("set cdaudio door closed", null, 0, IntPtr.Zero);//关闭光驱
        }
    }
}
