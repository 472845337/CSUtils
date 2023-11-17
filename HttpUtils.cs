using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Utils {
    /// <summary>
    /// Http请求工具类
    /// Get和Post方法
    /// 
    /// </summary>
    public static class HttpUtils {
        public const string TypeGet = WebRequestMethods.Http.Get;
        public const string TypePost = WebRequestMethods.Http.Post;
        public const string Http = "http://";
        public const string Https = "https://";

        private const string DEFAULT_USER_AGENT = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        public const string ContentTypeFormData = "multipart/form-data";
        public const string ContentTypeFormUrlencoded = "application/x-www-form-urlencoded;charset=utf-8";
        public const string ContentTypeJson = "application/json";
        public const string ContentTypeText = "text/plain";
        public const string ContentTypeJavascript = "application/javascript";
        public const string ContentTypeHtml = "text/html";
        public const string ContentTypeXml = "application/xml";

        /// <summary>
        /// 创建Http请求，可用于后续的异步或同步操作
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type">post或get</param>
        /// <param name="data">Post时的数据，Get无效</param>
        /// <param name="contentType"></param>
        /// <param name="timeout">超时</param>
        /// <returns></returns>
        public static HttpWebRequest HttpRequest(string url, string type, string data, string contentType, int timeout) {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                // 这里设置了协议类型。把SSL验证的设置写到HttpRequect创建之前
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.CheckCertificateRevocationList = true;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = true;

                request = WebRequest.Create(url) as HttpWebRequest;
            } else {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            request.ContentType = string.IsNullOrEmpty(contentType) ? ContentTypeJson : contentType;
            request.UserAgent = DEFAULT_USER_AGENT;
            request.Method = type;
            request.KeepAlive = false;
            request.Timeout = timeout;
            // request.ReadWriteTimeout = timeout;
            if (WebRequestMethods.Http.Post.ToLower().Equals(type.ToLower())) {
                // post
                var writer = request.GetRequestStream();
                var dataArray = null == data ? new Byte[] { } : Encoding.UTF8.GetBytes(data);
                writer.Write(dataArray, 0, dataArray.Length);
                writer.Flush();
            }
            return request;
        }

        /// <summary>
        /// Http请求执行异步操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static System.Threading.Tasks.Task<WebResponse> MakeAsyncRequest(HttpWebRequest request) {
            return System.Threading.Tasks.Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse
            , null);
        }

        /// <summary>
        /// Post同步请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">参数数据</param>
        /// <param name="contentType">内容格式</param>
        /// <param name="timeout">超时时间/ms，当为System.Threading.Timeout.Infinite(-1)时表示不超时</param>
        /// <returns></returns>
        public static string PostRequest(string url, string data, string contentType, int timeout) {
            //定义request并设置request的路径
            var request = HttpRequest(url, HttpUtils.TypePost, data, contentType, timeout);
            //定义response为前面的request响应
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            string responseFromServer;
            try {
                response = (HttpWebResponse)request.GetResponse();
                //定义response字符流
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();//读取所有
            } catch (Exception e) {
                throw e;
            } finally {
                //关闭资源
                reader?.Dispose();
                dataStream?.Dispose();
                response?.Dispose();
            }

            return responseFromServer;
        }
        /// <summary>
        /// 使用默认超时的Post同步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string PostRequest(string url, string data, string contentType) {
            return PostRequest(url, data, contentType, 5 * 1000);
        }

        /// <summary>
        /// Get方式同步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <param name="timeout">超时时间/ms，请注意调用时应当*1000，当为System.Threading.Timeout.Infinite(-1)时为不超时</param>
        /// <returns></returns>
        public static string GetRequest(string url, string contentType, int timeout) {
            //定义request并设置request的路径
            var request = HttpRequest(url, HttpUtils.TypeGet, null, contentType, timeout);
            //定义response为前面的request响应
            HttpWebResponse response = null;
            Stream dataStream = null;
            StreamReader reader = null;
            string responseFromServer;
            try {

                response = (HttpWebResponse)request.GetResponse();
                //定义response字符流
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();//读取所有

            } catch (Exception e) {
                throw e;
            } finally {
                //关闭资源
                reader?.Dispose();
                dataStream?.Dispose();
                response?.Close();
            }

            return responseFromServer;
        }
        /// <summary>
        /// 默认超时时间为15秒的同步Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string GetRequest(string url, string contentType) {
            return GetRequest(url, contentType, 5 * 1000);
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true; //总是接收
        }
    }
}
