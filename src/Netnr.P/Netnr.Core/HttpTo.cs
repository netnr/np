using System.IO;
using System.Net;
using System.Text;

namespace Netnr.Core
{
    /// <summary>
    /// HTTP请求
    /// </summary>
    public class HttpTo
    {
        /// <summary>
        /// HttpWebRequest对象
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="type">请求类型，默认GET</param>
        /// <param name="data">发送数据，非GET、DELETE请求</param>
        /// <param name="charset">编码，默认utf-8</param>
        /// <returns></returns>
        public static HttpWebRequest HWRequest(string url, string type = "GET", string data = null, string charset = "utf-8")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = type;
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;
            request.MaximumAutomaticRedirections = 4;
            request.Timeout = short.MaxValue * 3;//MS
            request.ContentType = "application/x-www-form-urlencoded";

            if (type != "GET" && type != "DELETE" && data != null)
            {
                //发送内容
                byte[] bytes = Encoding.GetEncoding(charset).GetBytes(data);
                request.ContentLength = Encoding.GetEncoding(charset).GetBytes(data).Length;
                Stream outputStream = request.GetRequestStream();
                outputStream.Write(bytes, 0, bytes.Length);
                outputStream.Close();
            }
            return request;
        }

        /// <summary>
        /// HTTP请求
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="charset">编码，默认utf-8</param>
        /// <returns></returns>
        public static string Url(HttpWebRequest request, string charset = "utf-8")
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            if (string.Compare(response.ContentEncoding, "gzip", true) >= 0)
                responseStream = new System.IO.Compression.GZipStream(responseStream, System.IO.Compression.CompressionMode.Decompress);

            using var sr = new StreamReader(responseStream, Encoding.GetEncoding(charset));
            var result = sr.ReadToEnd();
            return result;
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="charset">编码，默认utf-8</param>
        /// <returns></returns>
        public static string Get(string url, string charset = "utf-8")
        {
            var request = HWRequest(url, "GET", null, charset);
            return Url(request, charset);
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">发送数据</param>
        /// <param name="charset">编码，默认utf-8</param>
        /// <returns></returns>
        public static string Post(string url, string data, string charset = "utf-8")
        {
            var request = HWRequest(url, "POST", data, charset);
            return Url(request, charset);
        }
    }
}