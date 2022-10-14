#if Full || XOps

using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;

namespace Netnr;

/// <summary>
/// 监控
/// </summary>
public class MonitorTo
{
    /// <summary>
    /// 监听结果对象
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
    /// HTTP
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="method"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    public static async Task<MonitorModel> HTTP(string uri, HttpMethod method, List<KeyValuePair<string, string>> headers = null)
    {
        var model = new MonitorModel();

        var client = new HttpClient();
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

            var result = await client.SendAsync(request);
            model.Code = (int)result.StatusCode;
        }
        catch (Exception ex)
        {
            model.Set(ex);
        }
        client.Dispose();

        return model;
    }

    /// <summary>
    /// SSL
    /// </summary>
    /// <param name="host"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static MonitorModel SSL(string host, int port = 443)
    {
        var model = new MonitorModel();

        try
        {
            var client = new TcpClient(host, port);

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

                var now = DateTime.Now;
                var co = ((X509Certificate2)certificate);

                var ExpiredDay = (int)((co.NotAfter - now).TotalDays);
                model.Data = new
                {
                    co.NotBefore,
                    co.NotAfter,
                    ExpiredDay
                };
                //有效且无错误信息为 200，否则为 400
                model.Code = ExpiredDay > 0 && model.Logs.Count == 0 ? 200 : 400;
                model.Logs.Add($"有效期: {ExpiredDay} 天，{co.NotBefore:yyyy-MM-dd HH:mm} 至 {co.NotAfter:yyyy-MM-dd HH:mm}");

                return true;
            }), null);

            sslStream.AuthenticateAsClient(host);
        }
        catch (Exception ex)
        {
            model.Set(ex);
        }

        return model;
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
            var has = await Dns.GetHostAddressesAsync(host);
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

        var client = new TcpClient();
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
        client.Dispose();

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

#endif