using System;
using System.IO;
using System.Net;
using System.Text;

namespace Netnr;

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
    /// <returns></returns>
    public static HttpWebRequest HWRequest(string url, string type = "GET", byte[] data = null)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        request.Method = type;
        request.KeepAlive = true;
        request.AllowAutoRedirect = true;
        request.MaximumAutomaticRedirections = 4;
        request.Timeout = short.MaxValue * 3;//MS
        request.ContentType = "application/x-www-form-urlencoded";

        //发送内容
        if (type != "GET" && data != null)
        {
            request.ContentLength = data.Length;
            Stream outputStream = request.GetRequestStream();
            outputStream.Write(data, 0, data.Length);
            outputStream.Close();
        }

        return request;
    }

    /// <summary>
    /// HTTP请求
    /// </summary>
    /// <param name="request">HttpWebRequest对象</param>
    /// <param name="charset">编码，默认utf-8</param>
    /// <param name="response">输出</param>
    /// <returns></returns>
    public static StreamReader Stream(HttpWebRequest request, ref HttpWebResponse response, string charset = "utf-8")
    {
        try
        {
            response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            if (string.Compare(response.ContentEncoding, "gzip", true) >= 0)
                responseStream = new System.IO.Compression.GZipStream(responseStream, System.IO.Compression.CompressionMode.Decompress);

            return string.IsNullOrEmpty(charset) ?
                new StreamReader(responseStream) : new StreamReader(responseStream, Encoding.GetEncoding(charset));
        }
        catch (WebException e)
        {
            var httpResponse = (HttpWebResponse)e.Response;
            var errCode = (int)httpResponse.StatusCode;
            var result = string.Empty;
            if (e.Response == null)
            {
                result = e.Response.ToString();
            }
            else
            {
                using Stream stream = e.Response.GetResponseStream();
                using var reader = new StreamReader(stream);
                result = reader.ReadToEnd();
            }
            Console.WriteLine($"Error Code: {errCode}");
            Console.WriteLine(result);

            throw e;
        }
    }

    /// <summary>
    /// HTTP请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="fullFilePath">存储完整路径</param>
    /// <returns></returns>
    public static void DownloadSave(string url, string fullFilePath)
    {
        var request = HWRequest(url);

        HttpWebResponse response = null;
        var stream = Stream(request, ref response, null);

        var fileDir = Path.GetDirectoryName(fullFilePath);
        if (!Directory.Exists(fileDir))
        {
            Directory.CreateDirectory(fileDir);
        }

        using var fs = File.Create(fullFilePath);
        stream.BaseStream.CopyTo(fs);
        fs.Flush();
    }

    /// <summary>
    /// HTTP请求
    /// </summary>
    /// <param name="request">HttpWebRequest对象</param>
    /// <param name="charset">编码，默认utf-8</param>
    /// <returns></returns>
    public static string Url(HttpWebRequest request, string charset = "utf-8")
    {
        HttpWebResponse response = null;
        var stream = Stream(request, ref response, charset);
        return stream.ReadToEnd();
    }

    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="charset">编码，默认utf-8</param>
    /// <returns></returns>
    public static string Get(string url, string charset = "utf-8")
    {
        var request = HWRequest(url, "GET", null);
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
        var request = HWRequest(url, "POST", Encoding.GetEncoding(charset).GetBytes(data));
        return Url(request, charset);
    }

    /// <summary>
    /// POST请求
    /// </summary>
    /// <param name="url">地址</param>
    /// <param name="bytes">发送数据</param>
    /// <param name="charset">编码，默认utf-8</param>
    /// <returns></returns>
    public static string Post(string url, byte[] bytes, string charset = "utf-8")
    {
        var request = HWRequest(url, "POST", bytes);
        return Url(request, charset);
    }
}