using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static Utils.DllUtils;

namespace Utils {
    public class RdpUtils {

        private static string Encrypt(string password) {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            DATA_BLOB dATA_BLOB = default;
            DATA_BLOB dATA_BLOB2 = default;
            DATA_BLOB dATA_BLOB3 = default;
            dATA_BLOB.cbData = bytes.Length;
            dATA_BLOB.pbData = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, dATA_BLOB.pbData, bytes.Length);
            dATA_BLOB3.cbData = 0;
            dATA_BLOB3.pbData = IntPtr.Zero;
            dATA_BLOB2.cbData = 0;
            dATA_BLOB2.pbData = IntPtr.Zero;
            CRYPTPROTECT_PROMPTSTRUCT cRYPTPROTECT_PROMPTSTRUCT = new CRYPTPROTECT_PROMPTSTRUCT {
                cbSize = Marshal.SizeOf(typeof(CRYPTPROTECT_PROMPTSTRUCT)),
                dwPromptFlags = 0,
                hwndApp = IntPtr.Zero,
                szPrompt = null
            };
            if (CryptProtectData(ref dATA_BLOB, "psw", ref dATA_BLOB3, IntPtr.Zero, ref cRYPTPROTECT_PROMPTSTRUCT, 1, ref dATA_BLOB2)) {
                if (IntPtr.Zero != dATA_BLOB.pbData) {
                    Marshal.FreeHGlobal(dATA_BLOB.pbData);
                }
                if (IntPtr.Zero != dATA_BLOB3.pbData) {
                    Marshal.FreeHGlobal(dATA_BLOB3.pbData);
                }
                byte[] array = new byte[dATA_BLOB2.cbData];
                Marshal.Copy(dATA_BLOB2.pbData, array, 0, dATA_BLOB2.cbData);
                return BitConverter.ToString(array).Replace("-", string.Empty);
            }
            return string.Empty;

        }
        /*
         * 创建rdp文件
         * filename 要创建的rdp文件名，可以绝对路径，也可以相对路径
         * address 服务器地址
         * username 用户名
         * password 密码
         * */
        public static void CreateProfile(string filename, string address, string username, string password, double height = 900, double width = 1440) {

            if (!File.Exists(filename)) {
                // 取得该文件的目录，如果文件名为相对路径，且只有文件名,那么路径赋值空格,当为空格的时候，不会对路径进行操作
                string directory = filename.IndexOf("\\") > -1 ? filename.Substring(0, filename.LastIndexOf("\\")) : "";
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
                using (StreamWriter streamWriter = new StreamWriter(filename, true)) {
                    streamWriter.WriteLine("allow desktop composition:i:0");
                    streamWriter.WriteLine("allow font smoothing:i:0");
                    streamWriter.WriteLine("alternate shell:s:");
                    streamWriter.WriteLine("audiocapturemode:i:0");
                    streamWriter.WriteLine("audiomode:i:0");//音频:0播放,2关闭
                    streamWriter.WriteLine("authentication level:i:2");
                    streamWriter.WriteLine("autoreconnection enabled:i:1");
                    streamWriter.WriteLine("bandwidthautodetect:i:1");
                    streamWriter.WriteLine("bitmapcachepersistenable:i:1");
                    streamWriter.WriteLine("compression:i:1");
                    streamWriter.WriteLine("connection type:i:7");
                    streamWriter.WriteLine($"desktopheight:i:{height}");
                    streamWriter.WriteLine($"desktopwidth:i:{width}");
                    streamWriter.WriteLine("disable cursor setting:i:0");
                    streamWriter.WriteLine("disable full window drag:i:1");
                    streamWriter.WriteLine("disable menu anims:i:1");
                    streamWriter.WriteLine("disable themes:i:0");
                    streamWriter.WriteLine("disable wallpaper:i:0");
                    streamWriter.WriteLine("displayconnectionbar:i:1");
                    streamWriter.WriteLine("drivestoredirect:s:");
                    streamWriter.WriteLine("enableworkspacereconnect:i:0");
                    streamWriter.WriteLine("full address:s:" + address);
                    streamWriter.WriteLine("gatewaybrokeringtype:i:0");
                    streamWriter.WriteLine("gatewaycredentialssource:i:4");
                    streamWriter.WriteLine("gatewayhostname:s:");
                    streamWriter.WriteLine("gatewayprofileusagemethod:i:0");
                    streamWriter.WriteLine("gatewayusagemethod:i:4");
                    streamWriter.WriteLine("kdcproxyname:s:");
                    streamWriter.WriteLine("keyboardhook:i:2");
                    streamWriter.WriteLine("negotiate security layer:i:1");
                    streamWriter.WriteLine("networkautodetect:i:1");
                    streamWriter.WriteLine("prompt for credentials:i:0");
                    streamWriter.WriteLine("promptcredentialonce:i:0");
                    streamWriter.WriteLine("rdgiskdcproxy:i:0");
                    streamWriter.WriteLine("redirectclipboard:i:1");//剪贴板:1打开,0关闭
                    streamWriter.WriteLine("redirectcomports:i:0");
                    streamWriter.WriteLine("redirectposdevices:i:0");
                    streamWriter.WriteLine("redirectprinters:i:1"); //打印机:1打开,0关闭
                    streamWriter.WriteLine("redirectsmartcards:i:1");
                    streamWriter.WriteLine("remoteapplicationmode:i:0");
                    streamWriter.WriteLine("screen mode id:i:2");
                    streamWriter.WriteLine("session bpp:i:32");
                    streamWriter.WriteLine("shell working directory:s:");
                    streamWriter.WriteLine("use multimon:i:0");
                    streamWriter.WriteLine("use redirection server name:i:0");
                    streamWriter.WriteLine("videoplaybackmode:i:1");
                    streamWriter.WriteLine("winposstr:s:0,3,0,0,800,600");
                    if (!string.IsNullOrEmpty(username)) {
                        streamWriter.WriteLine("username:s:" + username);
                    }
                    if (!string.IsNullOrEmpty(password)) {
                        streamWriter.WriteLine("password 51:b:" + Encrypt(password));
                    }
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }

        /// <summary>
        /// 修改服务rdp文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="address"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void UpdateProfile(string filename, string address, string username, string password) {
            if (File.Exists(filename)) {
                File.Delete(filename);
            }
            CreateProfile(filename, address, username, password);
        }

        /// <summary>
        /// 删除某个服务rdp文件
        /// </summary>
        /// <param name="v"></param>
        public static void DeleteProfiles(string filename) {
            if (File.Exists(filename)) {
                File.Delete(filename);
            }
        }
    }

}
