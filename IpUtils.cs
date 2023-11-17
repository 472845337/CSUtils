using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Utils {
    public static class IpUtils {
        /// <summary>
        /// 获取本地IP,默认IPv4，addressFamily可传AddressFamily.InterNetwork或AddressFamily.InterNetworkV6
        /// </summary>
        /// <param name="addressFamily">AddressFamily.InterNetwork:ipv4,AddressFamily.InterNetworkV6:ipv6</param>
        /// <returns></returns>
        public static string GetLocalIp(AddressFamily addressFamily = AddressFamily.InterNetwork) {
            var ip = "0.0.0.0";
            var hostName = Dns.GetHostName(); //得到主机名
            var ipEntry = Dns.GetHostEntry(hostName);
            foreach (var item in ipEntry.AddressList) {
                if (item.AddressFamily != addressFamily) continue;
                ip = item.ToString();
                break;
            }
            return ip;
        }
        /// <summary>
        /// 该方法获取到的IP，有的时候不一样
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPv4() {
            var ipv4 = string.Empty;
            foreach (var item in NetworkInterface.GetAllNetworkInterfaces()) {
                if (item.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 && item.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                foreach (var ip in item.GetIPProperties().UnicastAddresses) {
                    if (ip.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                    ipv4 = ip.Address.ToString();
                    break;
                }
            }
            return ipv4;
        }
    }
}
