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
    /// 序列化，对象转为 JSON 字符串
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="indented"></param>
    /// <param name="dateTimeFormatter"></param>
    /// <returns></returns>
    public static string ToNJson(this object obj, bool indented = false, string dateTimeFormatter = NJsonConverterTo.DefaultDateTimeFormatter)
    {
        var setting = new JsonSerializerSettings();
        setting.DefaultJsonSerializerSettings(indented, dateTimeFormatter);
        return JsonConvert.SerializeObject(obj, setting);
    }

    /// <summary>
    /// 默认设置
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="indented"></param>
    /// <param name="dateTimeFormatter"></param>
    public static void DefaultJsonSerializerSettings(this JsonSerializerSettings setting, bool indented = false, string dateTimeFormatter = NJsonConverterTo.DefaultDateTimeFormatter)
    {
        setting.Formatting = indented ? Formatting.Indented : Formatting.None;
        setting.TypeNameHandling = TypeNameHandling.None;
        setting.DateFormatString = dateTimeFormatter;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        setting.Converters.Add(new NJsonConverterTo.IPAddressConverter());
    }

    /// <summary>
    /// 反序列化，JSON 字符串转为对象
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <returns>JObject对象</returns>
    public static JObject DeJObject(this string json)
    {
        return JObject.Parse(json);
    }

    /// <summary>
    /// 反序列化，JSON 字符串转为对象
    /// </summary>
    /// <param name="json">JSON字符串</param>
    /// <returns>JArray对象</returns>
    public static JArray DeJArray(this string json)
    {
        return JArray.Parse(json);
    }
}

#endif