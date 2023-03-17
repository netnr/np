#if Full || NJson

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Netnr;

/// <summary>
/// 公共扩展
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// object 转 JSON 字符串（序列化）
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isSpace">缩进输出</param>
    /// <param name="DateTimeFormat">时间格式化</param>
    /// <returns></returns>
    public static string ToNJson(this object obj, bool isSpace = false, string DateTimeFormat = "yyyy-MM-dd HH:mm:ss")
    {
        Newtonsoft.Json.Converters.IsoDateTimeConverter dtFmt = new()
        {
            DateTimeFormat = DateTimeFormat
        };
        return JsonConvert.SerializeObject(obj, isSpace ? Formatting.Indented : Formatting.None, dtFmt);
    }

    /// <summary>
    /// 解析 JSON字符串 为JObject对象（反序列化）
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <returns>JObject对象</returns>
    public static JObject DeJObject(this string json)
    {
        return JObject.Parse(json);
    }

    /// <summary>
    /// 解析 JSON字符串 为JArray对象（反序列化）
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <returns>JArray对象</returns>
    public static JArray DeJArray(this string json)
    {
        return JArray.Parse(json);
    }
}

#endif