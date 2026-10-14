using System.IO;

namespace Utils {
    /// <summary>
    /// 表示表单文件参数（移植自 AppManager Pojo/FormFile）
    /// </summary>
    public class FormFile {
        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件内容
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Content-Type (可选，如 "image/jpeg")
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 从文件路径创建 FormFile，文件不存在返回null
        /// </summary>
        public static FormFile FromPath(string name, string filePath) {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) {
                return null;
            }

            return new FormFile {
                Name = name,
                FileName = Path.GetFileName(filePath),
                Content = File.ReadAllBytes(filePath),
                ContentType = GetMimeType(Path.GetExtension(filePath))
            };
        }

        /// <summary>
        /// 根据文件扩展名获取 MIME 类型
        /// </summary>
        private static string GetMimeType(string extension) {
            var ext = extension.ToLower();
            switch (ext) {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                case ".webp":
                    return "image/webp";
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".txt":
                    return "text/plain";
                case ".html":
                case ".htm":
                    return "text/html";
                case ".xml":
                    return "application/xml";
                case ".json":
                    return "application/json";
                case ".zip":
                    return "application/zip";
                case ".rar":
                    return "application/x-rar-compressed";
                case ".mp3":
                    return "audio/mpeg";
                case ".mp4":
                    return "video/mp4";
                case ".avi":
                    return "video/x-msvideo";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
