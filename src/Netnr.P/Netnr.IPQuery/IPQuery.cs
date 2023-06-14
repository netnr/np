using System.IO;
using System.Net;
using System.Linq;

namespace Netnr;

/// <summary>
/// 结果
/// </summary>
public class IPQueryResult
{
    /// <summary>
    /// 地址
    /// </summary>
    public string Addr { get; set; }
    /// <summary>
    /// ISP
    /// </summary>
    public string ISP { get; set; }
    /// <summary>
    /// 是 IPv6
    /// </summary>
    public bool IsIPv6 { get; set; }
}

/// <summary>
/// IP 查询
/// </summary>
public class IPQuery
{
    /// <summary>
    /// IPv4 数据库
    /// </summary>
    public string DataForIPv4 { get; set; }

    /// <summary>
    /// IPv6 数据库
    /// </summary>
    public string DataForIPv6 { get; set; }

    /// <summary>
    /// IPv4
    /// </summary>
    public IPv4Query V4Query { get; set; }

    /// <summary>
    /// IPv6
    /// </summary>
    public IPv6Query V6Query { get; set; }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="dataForIPv4"></param>
    /// <param name="dataForIPv6"></param>
    public IPQuery(string dataForIPv4 = null, string dataForIPv6 = null)
    {
        if (!string.IsNullOrWhiteSpace(dataForIPv4) && File.Exists(dataForIPv4))
        {
            DataForIPv4 = dataForIPv4;
        }

        if (!string.IsNullOrWhiteSpace(dataForIPv6) && File.Exists(dataForIPv6))
        {
            DataForIPv6 = dataForIPv6;
        }

        V4Query = new IPv4Query(DataForIPv4);
        V6Query = new IPv6Query(DataForIPv6);
    }

    /// <summary>
    /// 搜索
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public IPQueryResult Search(string ip)
    {
        var result = new IPQueryResult();

        if (IPAddress.TryParse(ip, out IPAddress addr))
        {
            result.IsIPv6 = addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;

            var qr = result.IsIPv6 ? V6Query.Search(ip) : V4Query.Search(ip);
            if (qr != null)
            {
                if (result.IsIPv6)
                {
                    result.Addr = qr.Addr;
                    result.ISP = qr.ISP;
                }
                else
                {
                    result.Addr = TryParseAddress(qr.Addr);
                    result.ISP = qr.ISP.Trim() == "CZ88.NET" ? "" : qr.ISP;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 尝试解析地址
    /// </summary>
    /// <param name="addr"></param>
    /// <returns></returns>
    public string TryParseAddress(string addr)
    {
        var pcs1 = new[]
        {
            "北京市","天津市","河北省","山西省","内蒙古","辽宁省","吉林省","黑龙江省","上海市","江苏省","浙江省","安徽省","福建省","江西省","山东省","河南省","湖北省","湖南省","广东省","广西","海南省","重庆市","四川省","贵州省","云南省","西藏","陕西省","甘肃省","青海省","宁夏","新疆","台湾省","香港","澳门"
        };

        var isc1 = pcs1.FirstOrDefault(addr.StartsWith);
        if (isc1 != null)
        {
            var city = addr.Substring(isc1.Length);
            if (city == "")
            {
                addr = $"中国\t{isc1}";
            }
            else
            {
                addr = $"中国\t{isc1}\t{city}";
            }
        }
        else if (new[] { "北京", "北京省" }.Contains(addr))
        {
            addr = "中国\t北京市";
        }
        else if (new[] { "日本东京", "泰国曼谷" }.Contains(addr))
        {
            addr = $"{addr.Substring(0, 2)}\t{addr.Substring(2)}";
        }
        else if (addr == "甘肃")
        {
            addr = "中国\t甘肃省";
        }
        else if (addr == "黑龙江齐齐哈尔市")
        {
            addr = "中国\t黑龙江省\t齐齐哈尔市";
        }
        else if (addr == "重庆工学院")
        {
            addr = "中国\t重庆市\t工学院";
        }

        return addr;
    }
}
