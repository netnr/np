#if Full || Base

namespace Netnr;

/// <summary>
/// 公共扩展
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 序列化，对象转为 JSON 字符串
    /// </summary>
    /// <param name="obj">对象</param>
    /// <param name="indented">缩进输出</param>
    /// <param name="dateTimeFormatter">时间格式化</param>
    /// <returns></returns>
    public static string ToJson(this object obj, bool indented = false, string dateTimeFormatter = JsonConverterTo.DefaultDateTimeFormatter)
    {
        if (obj == null)
        {
            return "null";
        }

        var options = JsonConverterTo.JSOptions(dateTimeFormatter);
        options.WriteIndented = indented;

        var result = JsonSerializer.Serialize(obj, options);
        return result;
    }

    /// <summary>
    /// 异常
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="indented"></param>
    /// <returns></returns>
    public static string ToJson(this Exception ex, bool indented = false)
    {
        return ex.ToTree().ToJson(indented);
    }

    /// <summary>
    /// [只读]反序列化，JSON 字符串转为对象
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static JsonElement DeJson(this string json) => JsonDocument.Parse(json, JsonConverterTo.JDOptions()).RootElement;

    /// <summary>
    /// [读写]反序列化，JSON 字符串转为对象
    /// </summary>
    /// <param name="json">JSON 字符串</param>
    /// <returns>对象</returns>
    public static JsonNode DeJsonNode(this string json) => JsonNode.Parse(json, documentOptions: JsonConverterTo.JDOptions());

    /// <summary>
    /// 反序列化，JSON 字符串转为类型
    /// </summary>
    /// <typeparam name="T">实体泛型</typeparam>
    /// <param name="json">JSON 字符串</param>
    /// <param name="formatter"></param>
    /// <param name="useStrict">严格模式</param>
    public static T DeJson<T>(this string json, string formatter = JsonConverterTo.DefaultDateTimeFormatter, bool useStrict = false)
        => JsonSerializer.Deserialize<T>(json, JsonConverterTo.JSOptions(formatter, useStrict));

    /// <summary>
    /// 获取值（无效类型返回默认值）
    /// </summary>
    /// <typeparam name="T">基本数据类型，不是类</typeparam>
    /// <param name="ele"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T GetValue<T>(this JsonElement ele, string name)
    {
        var val = GetValue(ele, name);
        if (!string.IsNullOrEmpty(val))
        {
            try
            {
                return val.ToConvert<T>();
            }
            catch (Exception) { }
        }
        return default;
    }

    /// <summary>
    /// 获取值（无效返回 null）
    /// </summary>
    /// <param name="ele"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetValue(this JsonElement ele, string name)
    {
        if (ele.TryGetProperty(name, out JsonElement val))
        {
            return val.ToString();
        }
        return null;
    }

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="ele"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetValue(this JsonElement? ele, string name)
    {
        if (ele != null && ele.Value.TryGetProperty(name, out JsonElement val))
        {
            return val.ToString();
        }
        return null;
    }
}

#endif