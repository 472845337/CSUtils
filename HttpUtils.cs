using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
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
                var dataArray = null == data ? new byte[] { } : Encoding.UTF8.GetBytes(data);
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
            var request = HttpRequest(url, TypePost, data, contentType, timeout);
            //定义response为前面的request响应
            string responseFromServer;
            try {
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //定义response字符流
                using Stream dataStream = response.GetResponseStream();
                using StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();//读取所有
            } catch (Exception e) {
                throw e;
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
        public static int? GetPortFromUrl(string url) {
            try {
                Uri uri = new Uri(url);
                return uri.Port; // 如果未指定端口，会返回默认端口（http=80, https=443）
            } catch (UriFormatException) {
                return null; // URL格式无效
            }
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
            var request = HttpRequest(url, TypeGet, null, contentType, timeout);
            //定义response为前面的request响应
            string responseFromServer;
            try {
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //定义response字符流
                using Stream dataStream = response.GetResponseStream();
                using StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();//读取所有
            } catch (Exception e) {
                throw e;
            }

            return responseFromServer;
        }
        /// <summary>
        /// 和风天气请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string GetQRequest(string url, string contentType) {
            //定义request并设置request的路径
            var request = HttpRequest(url, TypeGet, null, contentType, 5 * 1000);
            //定义response为前面的request响应
            string result = null;
            try {
                using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //定义response字符流
                using Stream dataStream = response.GetResponseStream();
                // GZip解压
                result = GZipUtil.DecompressStream(dataStream);
            } catch (Exception e) {
                throw e;
            }
            return result;
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

        /// <summary>
        /// 创建Http请求（带自定义请求头和100-continue开关，供PostDog使用，移植自AppManager）
        /// </summary>
        public static HttpWebRequest HttpRequest(string url, string type, string data, string contentType, int timeout, Dictionary<string, string> head, bool expect100Continue) {
            // 这里设置了协议类型。把SSL验证的设置写到HttpRequect创建之前
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = true;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            // 配置请求头中如果配置了content-type，则使用请求头中的content-type
            bool headHasContentType = false;
            if (null != head && head.Count > 0) {
                foreach (var headParam in head) {
                    if (string.Equals("content-type", headParam.Key, StringComparison.OrdinalIgnoreCase)) {
                        headHasContentType = true;
                        request.ContentType = headParam.Value;
                    } else {
                        request.Headers.Add(headParam.Key, headParam.Value);
                    }
                }
            }
            if (!headHasContentType) {
                request.ContentType = string.IsNullOrEmpty(contentType) ? ContentTypeJson : contentType;
            }
            request.UserAgent = DEFAULT_USER_AGENT;
            request.Method = type;
            request.KeepAlive = false;
            request.Timeout = timeout;
            request.ServicePoint.Expect100Continue = expect100Continue; // 当为false表示判定是否接受客户端主体
                                                                        // 允许GET等请求携带请求体（raw方式GET发送使用）
            var requestType = request.GetType();
            var currentMethod = requestType.GetProperty("CurrentMethod", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(request);
            var methodType = currentMethod.GetType();
            methodType.GetField("ContentBodyNotAllowed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(currentMethod, false);
            var writer = request.GetRequestStream();
            var dataArray = null == data ? new byte[] { } : Encoding.UTF8.GetBytes(data);
            writer.Write(dataArray, 0, dataArray.Length);
            writer.Flush();
            return request;
        }

        /// <summary>
        /// 创建 multipart/form-data 类型的 Http 请求，支持文件上传（移植自AppManager）
        /// </summary>
        public static HttpWebRequest HttpRequestMultipart(string url, string type,
            Dictionary<string, List<string>> textParams, List<FormFile> files,
            int timeout, Dictionary<string, string> head, bool expect100Continue) {

            // 这里设置了协议类型。把SSL验证的设置写到HttpRequect创建之前
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = true;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            // 设置请求头（不包含 Content-Type，因为后面需要设置 boundary）
            if (null != head && head.Count > 0) {
                foreach (var headParam in head) {
                    if (!string.Equals("content-type", headParam.Key, StringComparison.OrdinalIgnoreCase)) {
                        request.Headers.Add(headParam.Key, headParam.Value);
                    }
                }
            }

            request.UserAgent = DEFAULT_USER_AGENT;
            request.Method = type;
            request.KeepAlive = false;
            request.Timeout = timeout;
            request.ServicePoint.Expect100Continue = expect100Continue;

            // 生成 boundary
            string boundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = $"multipart/form-data; boundary={boundary}";

            // 允许写入 POST 数据
            var requestType = request.GetType();
            var currentMethod = requestType.GetProperty("CurrentMethod", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(request);
            var methodType = currentMethod.GetType();
            methodType.GetField("ContentBodyNotAllowed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(currentMethod, false);

            // 构建 multipart 数据
            using (var stream = request.GetRequestStream()) {
                // 写入文本参数
                if (textParams != null) {
                    foreach (var param in textParams) {
                        foreach (var value in param.Value) {
                            WriteMultipartPart(stream, boundary, param.Key, value, null, null, null);
                        }
                    }
                }

                // 写入文件参数
                if (files != null) {
                    foreach (var file in files) {
                        WriteMultipartPart(stream, boundary, file.Name, null, file.FileName, file.Content, file.ContentType);
                    }
                }

                // 写入结束 boundary
                byte[] endBoundary = Encoding.UTF8.GetBytes($"--{boundary}--\r\n");
                stream.Write(endBoundary, 0, endBoundary.Length);
            }
            return request;
        }

        /// <summary>
        /// 写入 multipart 数据部分
        /// </summary>
        private static void WriteMultipartPart(Stream stream, string boundary, string name, string textValue,
            string fileName, byte[] fileContent, string contentType = null) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"--{boundary}\r\n");
            sb.Append($"Content-Disposition: form-data; name=\"{name}\"");

            if (!string.IsNullOrEmpty(fileName)) {
                sb.Append($"; filename=\"{fileName}\"");
                if (!string.IsNullOrEmpty(contentType)) {
                    sb.Append($"\r\nContent-Type: {contentType}");
                }
            }

            sb.Append("\r\n\r\n");

            // 写入头部
            byte[] headerBytes = Encoding.UTF8.GetBytes(sb.ToString());
            stream.Write(headerBytes, 0, headerBytes.Length);

            // 写入内容
            if (fileContent != null) {
                stream.Write(fileContent, 0, fileContent.Length);
            } else if (!string.IsNullOrEmpty(textValue)) {
                byte[] textBytes = Encoding.UTF8.GetBytes(textValue);
                stream.Write(textBytes, 0, textBytes.Length);
            }

            // 写入 \r\n
            byte[] crlf = Encoding.UTF8.GetBytes("\r\n");
            stream.Write(crlf, 0, crlf.Length);
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true; //总是接收
        }
    }
}
