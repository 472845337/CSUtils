using CliWrap;
using System;
using System.Text;

namespace Utils {
    public static class CliWrapUtils {

        /// <summary>
        /// 执行应用程式
        /// </summary>
        /// <param name="appPath">应用路径</param>
        /// <param name="directoryPath">起始位置</param>
        /// <param name="arguments">参数</param>
        /// <param name="callback">回执函数</param>
        /// <param name="errorCallback"></param>
        /// <param name="closedCallback"></param>
        /// <param name="errorClosedCallback"></param>
        public static async void ExecuteApp(string appPath, string directoryPath, string arguments, Action<string> callback = null, Action<string> errorCallback = null, Action closedCallback = null, Action errorClosedCallback = null) {
            ICli command = CreateCli(appPath, directoryPath, arguments, callback, errorCallback, closedCallback, errorClosedCallback);
            await command.ExecuteAsync();
        }

        /// <summary>
        /// 创建Cli对象
        /// 默认输出UTF8格式
        /// 不校验退出编码
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="directoryPath"></param>
        /// <param name="arguments"></param>
        /// <param name="callback"></param>
        /// <param name="errorCallBack"></param>
        /// <param name="closedCallback"></param>
        /// <param name="errorClosedCallback"></param>
        /// <returns></returns>
        public static ICli CreateCli(string filePath, string directoryPath = null, string arguments = null, Action<string> callback = null, Action<string> errorCallBack = null, Action closedCallback = null, Action errorClosedCallback = null) {
            ICli cli = Cli.Wrap(filePath).SetStandardOutputEncoding(Encoding.UTF8).EnableExitCodeValidation(false);

            if (!string.IsNullOrEmpty(directoryPath)) {
                cli.SetWorkingDirectory(directoryPath);
            }
            if (!string.IsNullOrEmpty(arguments)) {
                cli.SetArguments(arguments);
            }
            if (null != callback) {
                cli.SetStandardOutputCallback(callback);
            }
            if (null != errorCallBack) {
                cli.SetStandardErrorCallback(errorCallBack);
            }
            if (null != closedCallback) {
                cli.SetStandardOutputClosedCallback(closedCallback);
            }
            if (null != errorClosedCallback) {
                cli.SetStandardErrorClosedCallback(errorClosedCallback);
            }
            return cli;
        }
    }
}
