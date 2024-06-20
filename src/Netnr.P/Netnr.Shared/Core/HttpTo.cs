#if Full || Core

using System;
using System.IO;
using System.Net;
using System.Text;

namespace Netnr;

/// <summary>
/// HTTP请求
/// </summary>
public partial class HttpTo
{

#if NET6_0_OR_GREATER

    /// <summary>
    /// 构建 HttpClientHandler
    /// </summary>
    /// <param name="ignoreCertificate">忽略证书错误</param>
    /// <returns></returns>
    public static HttpClientHandler BuildHandler(bool ignoreCertificate = true)
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All,
            // 设置是否允许自动重定向
            AllowAutoRedirect = true,
            // 设置最大自动重定向次数
            MaxAutomaticRedirections = 5
        };

        if (ignoreCertificate)
        {
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        }

        return handler;
    }

    /// <summary>
    /// 构建请求客户端
    /// </summary>
    /// <returns></returns>
    public static HttpClient BuildClient()
    {
        var client = new HttpClient(BuildHandler());
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("Netnr");
        client.Timeout = TimeSpan.FromSeconds(60);

        return client;
    }

    /// <summary>
    /// 构建参数 x-www-form-urlencoded
    /// </summary>
    /// <param name="data">参数 k1=v1&amp;k2=v2&amp;k2=v3</param>
    /// <param name="encoding">编码，默认 utf8</param>
    /// <returns></returns>
    public static StringContent BuildContentFormUrl(string data, Encoding encoding = null)
    {
        var content = new StringContent(data, encoding ?? Encoding.UTF8, "application/x-www-form-urlencoded");
        return content;
    }

    /// <summary>
    /// 构建参数 multipart/form-data （文件在构建对象再追加 StreamContent）
    /// </summary>
    /// <param name="dictFormData">键值参数</param>
    /// <param name="encoding">编码，默认 utf8</param>
    /// <returns></returns>
    public static MultipartFormDataContent BuildContentFormData(Dictionary<string, string> dictFormData, Encoding encoding = null)
    {
        var content = new MultipartFormDataContent();
        foreach (var key in dictFormData.Keys)
        {
            content.Add(new StringContent(dictFormData[key], encoding ?? Encoding.UTF8), key);
        }
        return content;
    }

    /// <summary>
    /// 构建参数 body text/plain
    /// </summary>
    /// <param name="data"></param>
    /// <param name="encoding">编码，默认 utf8</param>
    /// <returns></returns>
    public static StringContent BuildContentTextPlain(string data, Encoding encoding = null)
    {
        var content = new StringContent(data, encoding ?? Encoding.UTF8, "text/plain");
        return content;
    }

#endif

#if NETSTANDARD2_0

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
        request.UserAgent = "Netnr";
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
    public static StreamReader Stream(HttpWebRequest request, out HttpWebResponse response, string charset = "utf-8")
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
            var statusCode = (int)httpResponse.StatusCode;
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

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"StatusCode: {statusCode}\r\n{result}");
            Console.ResetColor();

            throw;
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

        var stream = Stream(request, out _, null);

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
        var stream = Stream(request, out _, charset);
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

#endif

}

#endif