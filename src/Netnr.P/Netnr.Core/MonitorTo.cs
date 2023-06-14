using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.Security;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Netnr;

/// <summary>
/// 监测
/// </summary>
public partial class MonitorTo
{
    /// <summary>
    /// 监测结果对象
    /// </summary>
    public class MonitorModel
    {
        private readonly Stopwatch sw = new();

        /// <summary>
        /// 构造
        /// </summary>
        public MonitorModel()
        {
            sw.Start();
        }

        /// <summary>
        /// 代码，默认0，成功200，异常-1
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 耗时，单位：毫秒
        /// </summary>
        public long TimeCost
        {
            get
            {
                sw.Stop();
                return sw.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// 日志，消息
        /// </summary>
        public List<string> Logs { get; set; } = new List<string>();

        /// <summary>
        /// 异常包装
        /// </summary>
        /// <param name="ex"></param>
        public void Set(Exception ex)
        {
            Code = -1;
            var exp = ex;
            do
            {
                Console.WriteLine(exp.Message);
                Logs.Add(exp.Message);
                exp = exp.InnerException;
            } while (exp != null);
        }
    }

    /// <summary>
    /// SSL 解析
    /// </summary>
    public class MonitorSSLModel
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 验证链时间
        /// </summary>
        public DateTime VerificationTime { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public SslPolicyErrors PolicyError { get; set; }

        /// <summary>
        /// 加密算法（类型）
        /// </summary>
        public string AlgorithmType { get; set; }

        /// <summary>
        /// 加密算法（大小）
        /// </summary>
        public int AlgorithmSize { get; set; }

        /// <summary>
        /// 签名算法
        /// </summary>
        public string SignatureAlgorithm { get; set; }

        /// <summary>
        /// 颁发给
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 指纹 SHA1
        /// </summary>
        public string Thumbprint { get; set; }

        /// <summary>
        /// 指纹 SHA256
        /// </summary>
        public string Thumbprint256 { get; set; }

        /// <summary>
        /// 有效期（开始）
        /// </summary>
        public DateTime NotBefore { get; set; }

        /// <summary>
        /// 有效期（结束）
        /// </summary>
        public DateTime NotAfter { get; set; }

        /// <summary>
        /// 有效期（可用天数）
        /// </summary>
        public int AvailableDay { get; set; }

        /// <summary>
        /// 吊销状态
        /// </summary>
        public X509ChainStatus[] ChainStatus { get; set; }

        /// <summary>
        /// 使用者可选名称
        /// </summary>
        public string AlternativeName { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNumber { get; set; }
    }

    /// <summary>
    /// HTTP
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="method"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static async Task<MonitorModel> HTTP(string uri, HttpMethod method, List<KeyValuePair<string, string>> headers = null)
    {
        var model = new MonitorModel();

        //https://stackoverflow.com/questions/12553277
        var handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors != default)
                {
                    model.Logs.Add($"{sslPolicyErrors}");
                }

                return true;
            }
        };
        using var client = new HttpClient(handler);

        try
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(uri),
                Method = method ?? HttpMethod.Get,
            };
            if (headers?.Count > 0)
            {
                headers.ForEach(h => request.Headers.Add(h.Key, h.Value));
            }

            var respMessage = await client.SendAsync(request).ConfigureAwait(false);
            model.Code = (int)respMessage.StatusCode;

            var result = await respMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            model.Data = result;
        }
        catch (Exception ex)
        {
            model.Set(ex);
        }

        return model;
    }

    /// <summary>
    /// SSL
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static MonitorModel SSL(Uri uri)
    {
        var model = new MonitorModel();

        try
        {
            var client = new TcpClient(uri.Host, uri.Port);

            var sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                {
                    model.Logs.Add($"{sslPolicyErrors} 证书不可用");
                }
                else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    model.Logs.Add($"{sslPolicyErrors} 证书名称不匹配");
                }
                else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    model.Logs.Add($"{sslPolicyErrors}");
                }

                var cert = ((X509Certificate2)certificate);
                var listResult = CertificateInformation(cert, chain);
                model.Data = listResult;

                if (model.Logs.Count == 0 && listResult.First().AvailableDay > 0)
                {
                    model.Code = 200;
                }
                else
                {
                    model.Code = 400;
                }

                return true;
            }), null);

            sslStream.AuthenticateAsClient(uri.Host);
        }
        catch (Exception ex)
        {
            model.Set(ex);
        }

        return model;
    }

    /// <summary>
    /// 证书信息
    /// </summary>
    /// <param name="cert"></param>
    /// <param name="chain"></param>
    public static MonitorSSLModel CertificateInformationItem(X509Certificate2 cert, X509Chain chain)
    {
        var model = new MonitorSSLModel
        {
            VerificationTime = chain.ChainPolicy.VerificationTime,
            AlgorithmType = cert.PublicKey.Key.SignatureAlgorithm,
            AlgorithmSize = cert.PublicKey.Key.KeySize,
            Subject = cert.Subject,
            Issuer = cert.Issuer,
            SignatureAlgorithm = cert.SignatureAlgorithm.FriendlyName,
            Thumbprint = cert.Thumbprint,
            Thumbprint256 = CalcTo.HashString(CalcTo.HashType.SHA256, cert.GetRawCertData().ToStream()).ToUpper(),
            NotBefore = cert.NotBefore,
            NotAfter = cert.NotAfter,
            AvailableDay = (int)(cert.NotAfter - chain.ChainPolicy.VerificationTime).TotalDays,
            ChainStatus = chain.ChainStatus,
            Version = cert.Version,
            SerialNumber = cert.SerialNumber
        };

        foreach (var item in cert.Extensions)
        {
            //使用者可选名称
            if (item.Oid.Value == "2.5.29.17")
            {
                model.AlternativeName = Regex.Replace(item.RawData.ToText(Encoding.GetEncoding("iso-8859-1")), @"\p{C}+", " ");
                model.AlternativeName = string.Join(" ", model.AlternativeName.Split(' ').Where(x => x.Length > 3).Select(RemoveSpecialCharacters));

                break;
            }
        }

        return model;
    }

    /// <summary>
    /// 证书信息
    /// </summary>
    /// <param name="certificate"></param>
    /// <param name="chain"></param>
    public static List<MonitorSSLModel> CertificateInformation(X509Certificate2 certificate, X509Chain chain = null)
    {
        var list = new List<MonitorSSLModel>();

        if (chain == null)
        {
            chain = new X509Chain();
            chain.Build(certificate);
        }

        for (int i = 0; i < chain.ChainElements.Count; i++)
        {
            var itemCert = chain.ChainElements[i].Certificate;

            var model = CertificateInformationItem(itemCert, chain);
            model.Index = i;

            list.Add(model);
        }

        return list;
    }

    /// <summary>
    /// 移除乱码
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string RemoveSpecialCharacters(string content)
    {
        var sb = new StringBuilder();
        foreach (char c in content)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= 0x20000 && c <= 0xFA2D) || "-_.*@".Contains(c))
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// DNS
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static async Task<MonitorModel> DNS(string host)
    {
        var model = new MonitorModel();
        try
        {
            var has = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);
            model.Code = 200;
            model.Data = has;
        }
        catch (Exception ex)
        {
            model.Set(ex);
        }

        return model;
    }

    /// <summary>
    /// TCP-Port
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static MonitorModel TCPort(string host, int port)
    {
        var model = new MonitorModel();

        using var client = new TcpClient();
        try
        {
            var result = client.BeginConnect(host, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));
            model.Code = success ? 200 : 400;
        }
        catch (Exception ex)
        {
            model.Set(ex);
        }

        return model;
    }

    /// <summary>
    /// PING
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static MonitorModel PING(string host)
    {
        var model = new MonitorModel();

        var pinger = new Ping();
        try
        {
            var reply = pinger.Send(host);
            model.Code = reply.Status == IPStatus.Success ? 200 : 400;
        }
        catch (Exception ex)
        {
            model.Set(ex);
        }
        pinger.Dispose();

        return model;
    }
}