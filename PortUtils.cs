using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace Utils {
    public static class PortUtils {
        public static bool PortInUse(short port) {
            try {
                // 查看UDP端口是否被占用
                var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                var ipEndUdpPoints = ipProperties.GetActiveUdpListeners();
                if (ipEndUdpPoints.Any(ipEndUdpPoint => ipEndUdpPoint.Port == port)) {
                    return true;
                }

                // 查看TCP端口是否被占用
                var ipEndTcpPoints = ipProperties.GetActiveTcpListeners();
                return ipEndTcpPoints.Any(endPoint => endPoint.Port == port);
            } catch (Exception) {
                return false;
            }
        }
    }
}
