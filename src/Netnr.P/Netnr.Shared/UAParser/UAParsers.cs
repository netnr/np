#if Full || UAParser

using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// 解析、构造
/// </summary>
/// <param name="userAgent"></param>
public class UAParsers(string userAgent)
{
    /// <summary>
    /// 正则
    /// </summary>
    public static UAModels Regexes { get; set; } = UARegexes.DefaultUAModels;

    /// <summary>
    /// User-Agent
    /// </summary>
    public string UserAgent { get; set; } = userAgent;

    /// <summary>
    /// 客户端
    /// </summary>
    /// <returns></returns>
    public UAModels.ClientModel GetClient()
    {
        UAModels.ClientModel result = null;
        if (UserAgent.Length > 1 && UserAgent.Length < 1000)
        {
            foreach (var item in Regexes.ListClient)
            {
                item.Rgx ??= new Regex(item.Regex, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

                var match = item.Rgx.Match(UserAgent);
                if (match.Success)
                {
                    if (match.Groups.Count > 1 && item.Version?.Contains("$1") == true)
                    {
                        item.Version = item.Version.Replace("$1", match.Groups[1].ToString());
                    }
                    if (match.Groups.Count > 2 && item.Version?.Contains("$2") == true)
                    {
                        item.Version = item.Version.Replace("$2", match.Groups[2].ToString());
                    }
                    result = item;
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 设备
    /// </summary>
    /// <returns></returns>
    public UAModels.DeviceModel GetDevice()
    {
        UAModels.DeviceModel result = null;
        if (UserAgent.Length > 1 && UserAgent.Length < 1000)
        {
            foreach (var item in Regexes.ListDevice)
            {
                item.Rgx ??= new Regex(item.Regex, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

                var match = item.Rgx.Match(UserAgent);
                if (match.Success)
                {
                    result = item;
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 系统
    /// </summary>
    /// <returns></returns>
    public UAModels.OSModel GetOS()
    {
        UAModels.OSModel result = null;
        if (UserAgent.Length > 1 && UserAgent.Length < 1000)
        {
            foreach (var item in Regexes.ListOS)
            {
                item.Rgx ??= new Regex(item.Regex, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

                var match = item.Rgx.Match(UserAgent);
                if (match.Success)
                {
                    if (match.Groups.Count > 1)
                    {
                        if (item.Name.Contains("$1"))
                        {
                            item.Name = item.Name.Replace("$1", match.Groups[1].ToString());
                        }
                        if (item.Version?.Contains("$1") == true)
                        {
                            item.Version = item.Version.Replace("$1", match.Groups[1].ToString());
                        }
                    }
                    if (match.Groups.Count > 2 && item.Version?.Contains("$2") == true)
                    {
                        item.Version = item.Version.Replace("$2", match.Groups[2].ToString());
                    }
                    if (match.Groups.Count > 3 && item.Version?.Contains("$3") == true)
                    {
                        item.Version = item.Version.Replace("$3", match.Groups[2].ToString());
                    }
                    result = item;
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 爬虫
    /// </summary>
    /// <returns></returns>
    public UAModels.BotModel GetBot()
    {
        UAModels.BotModel result = null;
        if (UserAgent.Length > 1 && UserAgent.Length < 1000)
        {
            foreach (var item in Regexes.ListBot)
            {
                item.Rgx ??= new Regex(item.Regex, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

                var match = item.Rgx.Match(UserAgent);
                if (match.Success)
                {
                    result = item;
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 序列化，对象转为 XML 字符串
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">对象</param>
    /// <param name="indent">缩进</param>
    /// <returns></returns>
    public static string ToXml<T>(T obj, bool indent = false)
    {
        if (obj == null) return null;

        using var sw = new StringWriter();
        using var xtw = new XmlTextWriter(sw)
        {
            Formatting = indent ? Formatting.Indented : Formatting.None
        };

        var xs = new XmlSerializer(typeof(T));
        xs.Serialize(xtw, obj);

        return sw.ToString();
    }

    /// <summary>
    /// XML 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static T DeXml<T>(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml)) return default;

        using var sr = new StringReader(xml);
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(sr);
    }
}

#endif