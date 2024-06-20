#if Full || Web

namespace Netnr
{
    /// <summary>
    /// 客户端信息
    /// </summary>
    public class ClientInfoTo
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="context">上下文</param>
        public ClientInfoTo(HttpContext context)
        {
            Headers = context.Request.HttpContext.Request.Headers;

            var address = context.Request.HttpContext.Connection.RemoteIpAddress;
            if (address.IsIPv4MappedToIPv6)
            {
                address = address.MapToIPv4();
            }
            IP = address.ToString();

            //X-Forwarded-For: <client>, <proxy1>, <proxy2>
            var xffKey = "X-Forwarded-For";
            if (Headers.TryGetValue(xffKey, out Microsoft.Extensions.Primitives.StringValues value))
            {
                var xffVal = value.ToString();
                if (!string.IsNullOrWhiteSpace(xffVal))
                {
                    var ips = xffVal.Split(',').ToList();
                    IP = ips.First();
                    address = IPAddress.Parse(IP);

                    ips.RemoveAt(0);
                    ProxyIP.AddRange(ips);
                }
            }

            IsIPv6 = address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
        }

        /// <summary>
        /// 请求头
        /// </summary>
        public IHeaderDictionary Headers { get; set; }

        /// <summary>
        /// 客户 IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 是 IPv6
        /// </summary>
        public bool IsIPv6 { get; set; }

        /// <summary>
        /// 代理 IP
        /// </summary>
        public List<string> ProxyIP { get; set; } = [];
    }
}

#endif