using System.IO;
using System.Net;
using System.Text;

namespace Utils {
    public class RequestState {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;

        public StringBuilder RequestData {
            get;
            set;
        }

        public byte[] BufferRead {
            get;
            set;
        }

        public HttpWebRequest Request {
            get;
            set;
        }

        public HttpWebResponse Response {
            get;
            set;
        }

        public Stream StreamResponse {
            get;
            set;
        }

        public string AbortMsg { get; set; }

        public object Obj { get; set; }

        public RequestState() {
            BufferRead = new byte[BUFFER_SIZE];
            RequestData = new StringBuilder();
            Request = null;
            StreamResponse = null;
        }

        public void Flush() {
            if (null != Response) {
                Response.Close();
                StreamResponse.Close();
            }

            Request?.Abort();
            RequestData?.Clear();
            AbortMsg = string.Empty;
            Obj = null;
        }
    }
}