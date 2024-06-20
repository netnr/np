#if Full || Core

using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Linq;
using System.Text;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Netnr;

/// <summary>
/// 公共扩展
/// </summary>
public static partial class Extensions
{
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
    /// 反序列化，XML 字符串转为对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static T DeXml<T>(this string xml)
    {
        if (string.IsNullOrWhiteSpace(xml)) return default;

        using var sr = new StringReader(xml);
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(sr);
    }

    /// <summary>
    /// 反序列化，XML 字符串转为文档
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static XmlDocument DeXml(this string xml)
    {
        var xmldoc = new XmlDocument();
        xmldoc.LoadXml(xml);
        return xmldoc;
    }

    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="xpath"></param>
    /// <returns></returns>
    public static string GetValue(this XmlDocument doc, string xpath)
    {
        return doc.SelectSingleNode(xpath)?.InnerText;
    }

    /// <summary>
    /// 字符串 JSON转义
    /// </summary>
    /// <param name="s">字符串</param>
    /// <returns></returns>
    public static string OfJson(this string s)
    {
        StringBuilder sb = new();
        for (int i = 0; i < s.Length; i++)
        {
            char c = s.ToCharArray()[i];
            switch (c)
            {
                case '\"':
                    sb.Append("\\\""); break;
                case '\\':
                    sb.Append("\\\\"); break;
                case '/':
                    sb.Append("\\/"); break;
                case '\b':
                    sb.Append("\\b"); break;
                case '\f':
                    sb.Append("\\f"); break;
                case '\n':
                    sb.Append("\\n"); break;
                case '\r':
                    sb.Append("\\r"); break;
                case '\t':
                    sb.Append("\\t"); break;
                default:
                    sb.Append(c); break;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// SQL单引号转义
    /// </summary>
    /// <param name="s">字符串</param>
    /// <returns></returns>
    public static string OfSql(this string s)
    {
        return s.Replace("'", "''");
    }

    /// <summary>
    /// 实体转表
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="list">对象</param>
    /// <returns></returns>
    public static DataTable ToDataTable<T>(this IList<T> list)
    {
        Type elementType = typeof(T);
        var t = new DataTable();
        elementType.GetProperties().ForEach(propInfo => t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType));
        foreach (T item in list)
        {
            var row = t.NewRow();
            elementType.GetProperties().ForEach(propInfo => row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value);
            t.Rows.Add(row);
        }
        return t;
    }

    /// <summary>
    /// 表转为实体
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="table">表</param>
    /// <returns></returns>
    public static List<T> ToModel<T>(this DataTable table) where T : class, new()
    {
        var list = new List<T>();
        foreach (DataRow dr in table.Rows)
        {
            var model = new T();
            var pis = model.GetType().GetProperties();
            foreach (DataColumn dc in dr.Table.Columns)
            {
                object drValue = dr[dc.ColumnName];

                var pi = pis.FirstOrDefault(x => x.Name.Equals(dc.ColumnName, StringComparison.OrdinalIgnoreCase));
                if (pi != null)
                {
                    Type type = pi.PropertyType;
                    if (pi.PropertyType.FullName.Contains("System.Nullable"))
                    {
                        type = Type.GetType("System." + pi.PropertyType.FullName.Split(',')[0].Split('.')[2]);
                    }

                    if (pi != null && pi.CanWrite && (drValue != null && drValue is not DBNull))
                    {
                        try
                        {
                            drValue = Convert.ChangeType(drValue, type);
                            pi.SetValue(model, drValue, null);
                        }
                        catch (Exception) { }
                    }
                }
            }
            list.Add(model);
        }
        return list;
    }

    /// <summary>
    /// 异常转为树
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="messageOnly">仅消息</param>
    /// <param name="limitStackTrace">截断堆栈跟踪长度，默认-1不截断</param>
    /// <returns></returns>
    public static Dictionary<string, object> ToTree(this Exception ex, bool messageOnly = false, int limitStackTrace = -1)
    {
        var result = new Dictionary<string, object>
        {
            { "Message", ex.Message },
        };
        if (!messageOnly)
        {
            result.Add("HResult", ex.HResult);
            var stackTrace = ex.StackTrace;
            if (stackTrace != null && limitStackTrace > 1 && stackTrace.Length > limitStackTrace + 4)
            {
                stackTrace = stackTrace.Substring(0, limitStackTrace) + " ...";
            }
            result.Add("StackTrace", stackTrace);
            result.Add("Source", ex.Source);
        }
        if (ex.Data.Count > 0)
        {
            result.Add("Data", ex.Data);
        }
        if (ex.InnerException != null)
        {
            result.Add("InnerException", ex.InnerException.ToTree(messageOnly, limitStackTrace));
        }

        return result;
    }

    /// <summary>
    /// 异常转为文本
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static string ToText(this Exception ex)
    {
        var list = new List<Exception>() { ex };

        var innerException = ex;
        while (innerException.InnerException != null)
        {
            innerException = innerException.InnerException;
            list.Add(innerException);
        }

        var result = string.Join("\r\n", list.Select(e =>
        {
            var message = $"## Message\r\n{e.Message}";
            return string.IsNullOrEmpty(e.StackTrace) ? message : $"{message}\r\n## StackTrace\r\n{e.StackTrace}";
        }));

        return result;
    }

    /// <summary>
    /// URL 编码
    /// </summary>
    /// <param name="value">内容</param>
    /// <returns></returns>
    public static string ToUrlEncode(this string value) => Uri.EscapeDataString(value);

    /// <summary>
    /// URL 解码
    /// </summary>
    /// <param name="value">内容</param>
    /// <returns></returns>
    public static string ToUrlDecode(this string value) => Uri.UnescapeDataString(value);

    /// <summary>
    /// HTML 编码
    /// </summary>
    /// <param name="value">内容</param>
    /// <returns></returns>
    public static string ToHtmlEncode(this string value) => WebUtility.HtmlEncode(value);

    /// <summary>
    /// HTML 解码
    /// </summary>
    /// <param name="value">内容</param>
    /// <returns></returns>
    public static string ToHtmlDecode(this string value) => WebUtility.HtmlDecode(value);

    /// <summary>
    /// 等待获取异步结果（转同步）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    public static T ToResult<T>(this Task<T> task) => task.GetAwaiter().GetResult();

#if NET48 || NETSTANDARD2_0
    /// <summary>
    /// 按多个字符串分割
    /// </summary>
    /// <param name="value"></param>
    /// <param name="separator">分隔符</param>
    /// <param name="options">配置</param>
    /// <returns></returns>
    public static string[] Split(this string value, string separator, StringSplitOptions options = StringSplitOptions.None)
    {
        var result = value.Split(new[] { separator }, options);
        return result;
    }
#endif

    /// <summary>
    /// 转 Byte
    /// </summary>
    /// <param name="value">内容</param>
    /// <param name="encoding">默认 UTF8</param>
    /// <returns></returns>
    public static byte[] ToByte(this string value, Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return encoding.GetBytes(value);
    }

    /// <summary>
    /// Byte 转
    /// </summary>
    /// <param name="value">内容</param>
    /// <param name="encoding">默认 UTF8</param>
    /// <returns></returns>
    public static string ToText(this byte[] value, Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return encoding.GetString(value);
    }

    /// <summary>
    /// Stream 转
    /// </summary>
    /// <param name="stream">流</param>
    /// <returns></returns>
    public static string ToText(this Stream stream)
    {
        using var reader = new StreamReader(stream);
        var result = reader.ReadToEnd();
        return result;
    }

    /// <summary>
    /// 文本转流（默认位置0）
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Stream ToStream(this string value)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(value);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }

    /// <summary>
    /// Byte 转流（默认位置0）
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static Stream ToStream(this byte[] bytes) => new MemoryStream(bytes) { Position = 0 };

    /// <summary>
    /// 复制流带进度
    /// </summary>
    /// <param name="source">源</param>
    /// <param name="target">写入流</param>
    /// <param name="progress">进度，单位：byte</param>
    /// <param name="speed">限速 单位：byte</param>
    /// <returns></returns>
    public static async Task CopyToAsync(this Stream source, Stream target, Action<long> progress = null, long speed = 0)
    {
        int readLength;
        int blen = 8192;
        var buffer = new byte[blen];
        long receiveLength = 0;

        var speedSw = Stopwatch.StartNew();
        long speedReceiveLength = 0;

        while ((readLength = await source.ReadAsync(buffer, 0, blen).ConfigureAwait(false)) != 0)
        {
            await target.WriteAsync(buffer, 0, readLength).ConfigureAwait(false);
            receiveLength += readLength;
            progress?.Invoke(receiveLength);

            // 限速
            speedReceiveLength += readLength;
            if (speed >= blen && speedReceiveLength >= speed * 0.1)
            {
                speedReceiveLength = 0;

                var wait = 100 - (int)speedSw.ElapsedMilliseconds;
                if (wait > 0)
                {
                    await Task.Delay(wait);
                }
                speedSw.Restart();
            }
        }

        speedSw.Stop();
    }

#if NET6_0_OR_GREATER

    /// <summary>
    /// 下载带进度
    /// </summary>
    /// <param name="client"></param>
    /// <param name="url"></param>
    /// <param name="saveStream"></param>
    /// <param name="receiveAndTotalLength"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static async Task DownloadAsync(this HttpClient client, string url, Stream saveStream, Action<long, long?> receiveAndTotalLength = null, long speed = 0)
    {
        using var response = await client.GetAsync(url).ConfigureAwait(false);
        using var download = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        var contentLength = response.Content.Headers.ContentLength; //total
        if (receiveAndTotalLength == null)
        {
            await download.CopyToAsync(saveStream);
        }
        else
        {
            await download.CopyToAsync(saveStream, progress: (p) =>
            {
                receiveAndTotalLength.Invoke(p, contentLength);
            }, speed);
        }
        //将缓冲区中的数据刷新到磁盘，但并不保证数据已经写入磁盘的物理存储
        await saveStream.FlushAsync();
    }

    /// <summary>
    /// 下载带进度
    /// </summary>
    /// <param name="client"></param>
    /// <param name="url"></param>
    /// <param name="savePath">保存物理路径，文件夹需存在</param>
    /// <param name="receiveAndTotalLength">(rlen,tlen)=>{...}</param>
    /// <param name="speed">限速</param>
    /// <returns></returns>
    public static async Task DownloadAsync(this HttpClient client, string url, string savePath, Action<long, long?> receiveAndTotalLength = null, long speed = 0)
    {
        using var saveStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await DownloadAsync(client, url, saveStream, receiveAndTotalLength, speed);
    }

#endif

    /// <summary>
    /// Base64 编码
    /// </summary>
    /// <param name="value">内容</param>
    /// <param name="encoding">默认 UTF8</param>
    /// <returns></returns>
    public static string ToBase64Encode(this string value, Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return Convert.ToBase64String(encoding.GetBytes(value));
    }

    /// <summary>
    /// Url 安全替换
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToUrlSafe(this string value)
    {
        return value.Replace('+', '-').Replace('/', '_');
    }

    /// <summary>
    /// Base64 解码
    /// </summary>
    /// <param name="value"></param>
    /// <param name="encoding">默认 UTF8</param>
    /// <returns></returns>
    public static string ToBase64Decode(this string value, Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return encoding.GetString(Convert.FromBase64String(value));
    }

    /// <summary>
    /// 获取对象属性
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="full">完整的，包含集合</param>
    /// <returns></returns>
    public static Dictionary<string, object> ToProps(this object obj, bool full = true)
    {
        var dict = new Dictionary<string, object>();

        var pis = obj.GetType().GetProperties();
        foreach (var pi in pis)
        {
            try
            {
                if (typeof(ICollection).IsAssignableFrom(pi.PropertyType))
                {
                    if (full)
                    {
                        var subCollection = (ICollection)pi.GetValue(obj, null);
                        var subResult = new List<object>();
                        foreach (var val in subCollection)
                        {
                            subResult.Add(val.ToProps());
                        }
                        dict.Add(pi.Name, subResult);
                    }
                }
                else
                {
                    var val = pi.GetValue(obj, null);
                    dict.Add(pi.Name, val);
                }
            }
            catch (Exception) { }
        }

        return dict;
    }

    /// <summary>
    /// 深拷贝
    /// </summary>
    /// <param name="target">需要赋值的对象</param>
    /// <param name="source">（读取）源对象</param>
    public static T ToDeepCopy<T>(this T target, object source) where T : class
    {
        var targetPis = target.GetType().GetProperties();
        var sourcePis = source.GetType().GetProperties();

        foreach (var sourcePi in sourcePis)
        {
            foreach (var targetPi in targetPis)
            {
                if (targetPi.Name == sourcePi.Name)
                {
                    var sourcePiVal = sourcePi.GetValue(source, null);
                    if (targetPi.PropertyType.IsAssignableFrom(sourcePi.PropertyType) && sourcePi.SetMethod != null)
                    {
                        targetPi.SetValue(target, sourcePiVal, null);
                    }
                    else
                    {
                        targetPi.ToDeepCopy(sourcePiVal);
                    }
                    break;
                }
            }
        }

        return target;
    }

    /// <summary>
    /// 值类型转换
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static T ToConvert<T>(this string value) => (T)ToConvert(value, typeof(T));

    /// <summary>
    /// 值类型转换
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static object ToConvert(this string value, Type type)
    {
        if (type == typeof(object))
        {
            return value;
        }

        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return ToConvert(value, Nullable.GetUnderlyingType(type));
        }

        var converter = TypeDescriptor.GetConverter(type);
        if (converter.CanConvertFrom(typeof(string)))
        {
            return converter.ConvertFromInvariantString(value);
        }

        return null;
    }

    /// <summary>
    /// 字符串转枚举
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T DeEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    /// <summary>
    /// 根据 Guid 获取唯一数字
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static long ToLong(this Guid guid)
    {
        var bytes = guid.ToByteArray();
        return BitConverter.ToInt64(bytes, 0);
    }

    /// <summary>
    /// 根据 Guid 获取唯一数字字符串
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static string ToLongString(this Guid guid)
    {
        var bytes = guid.ToByteArray();
        return BitConverter.ToInt64(bytes, 0).ToString();
    }

    /// <summary>
    /// 将Datetime转换成时间戳，10位：秒 或 13位：毫秒
    /// </summary>
    /// <param name="datetime"></param>
    /// <param name="isms">毫秒，默认false为秒，设为true，返回13位，毫秒</param>
    /// <returns></returns>
    public static long ToTimestamp(this DateTime datetime, bool isms = false)
    {
        var t = datetime.ToUniversalTime().Ticks - 621355968000000000;
        var tc = t / (isms ? 10000 : 10000000);
        return tc;
    }

    /// <summary>
    /// 将Datetime转换成从UTC开始计算的总天数
    /// </summary>
    /// <param name="datetime"></param>
    /// <returns></returns>
    public static int ToUtcTotalDays(this DateTime datetime)
    {
        var d = datetime.ToTimestamp() * 1.0 / 3600 / 24;
        return (int)Math.Ceiling(d);
    }

    /// <summary>
    /// 集合添加项（返回新的集合）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="newItem"></param>
    /// <returns></returns>
    public static IEnumerable<T> Add<T>(this IEnumerable<T> items, T newItem)
    {
        foreach (var item in items)
        {
            yield return item;
        }
        yield return newItem;
    }

    /// <summary>
    /// 遍历
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        if (action != null)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }

    /// <summary>
    /// 拓展批量添加
    /// </summary>
    /// <param name="oc"></param>
    /// <param name="list"></param>
    public static void AddRange(this ObservableCollection<object> oc, IEnumerable<object> list)
    {
        foreach (var item in list)
        {
            oc.Add(item);
        }
    }
}

#endif