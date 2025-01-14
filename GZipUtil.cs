using System.IO;
using System.IO.Compression;
using System.Text;

namespace Utils {
    public class GZipUtil {
        public static string DecompressStream(Stream stream) {
            using MemoryStream decompressedStream = new MemoryStream();
            using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress)) {
                decompressionStream.CopyTo(decompressedStream);
            }
            return Encoding.UTF8.GetString(decompressedStream.ToArray());
        }
    }
}
