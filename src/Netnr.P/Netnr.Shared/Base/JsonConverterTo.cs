#if Full || Base

using System;
using System.Net;
using System.Reflection;
using System.Text.Encodings.Web;

namespace Netnr;

/// <summary>
/// 格式化
/// </summary>
public class JsonConverterTo
{
    /// <summary>
    /// 默认时间格式化
    /// </summary>
    public const string DefaultDateTimeFormatter = "yyyy-MM-dd HH:mm:ss.fff";

    /// <summary>
    /// STJ 时间格式化
    /// </summary>
    /// <remarks>
    /// 构造
    /// </remarks>
    /// <param name="formatter"></param>
    public class DateTimeJsonConverter(string formatter = DefaultDateTimeFormatter) : JsonConverter<DateTime>
    {
        /// <summary>
        /// 格式
        /// </summary>
        public string Formatter { get; set; } = formatter;

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Formatter));
        }
    }

    /// <summary>
    /// STJ DataTable 数据表格式化
    /// https://github.com/dotnet/docs/blob/main/docs/standard/serialization/system-text-json/snippets/how-to/csharp/RoundtripDataTable.cs
    /// </summary>
    public class DataTableJsonConverter : JsonConverter<DataTable>
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DataTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            JsonElement rootElement = jsonDoc.RootElement;
            DataTable dataTable = JsonElementToDataTable(rootElement);
            return dataTable;
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="jsonWriter"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter jsonWriter, DataTable value, JsonSerializerOptions options)
        {
            jsonWriter.WriteStartArray();
            foreach (DataRow dr in value.Rows)
            {
                jsonWriter.WriteStartObject();
                foreach (DataColumn col in value.Columns)
                {
                    string key = col.ColumnName.Trim();

                    Action<string> action = GetWriteAction(dr, col, jsonWriter);
                    action.Invoke(key);

                    static Action<string> GetWriteAction(DataRow row, DataColumn column, Utf8JsonWriter writer) => row[column] switch
                    {
                        // bool
                        bool value => key => writer.WriteBoolean(key, value),

                        // numbers
                        byte value => key => writer.WriteNumber(key, value),
                        sbyte value => key => writer.WriteNumber(key, value),
                        decimal value => key => writer.WriteNumber(key, value),
                        double value => key => writer.WriteNumber(key, value),
                        float value => key => writer.WriteNumber(key, value),
                        short value => key => writer.WriteNumber(key, value),
                        int value => key => writer.WriteNumber(key, value),
                        ushort value => key => writer.WriteNumber(key, value),
                        uint value => key => writer.WriteNumber(key, value),
                        ulong value => key => writer.WriteNumber(key, value),

                        // strings
                        DateTime value => key => writer.WriteString(key, value),
                        Guid value => key => writer.WriteString(key, value),

                        // more types
                        //warning: JavaScript Number.MAX_SAFE_INTEGER=9007199254740991 use string
                        long value => key => writer.WriteNumber(key, value),
                        byte[] value => key => writer.WriteBase64String(key, value),
                        DBNull value => writer.WriteNull,
                        IEnumerable<string> value => key => writer.WriteString(key, string.Join(",", value)),

                        _ => key => writer.WriteString(key, row[column].ToString())
                    };
                }
                jsonWriter.WriteEndObject();
            }
            jsonWriter.WriteEndArray();
        }
    }

    /// <summary>
    /// 转 DataSet
    /// </summary>
    /// <param name="dataRoot"></param>
    /// <returns></returns>
    public static DataSet JsonElementToDataSet(JsonElement dataRoot)
    {
        var dataSet = new DataSet();
        foreach (var item in dataRoot.EnumerateObject())
        {
            var dataTable = JsonElementToDataTable(item.Value);
            dataTable.TableName = item.Name;
            dataSet.Tables.Add(dataTable);
        }
        return dataSet;
    }

    /// <summary>
    /// 转 DataTable
    /// </summary>
    /// <param name="dataRoot"></param>
    /// <returns></returns>
    public static DataTable JsonElementToDataTable(JsonElement dataRoot)
    {
        var dataTable = new DataTable();
        bool firstPass = true;
        foreach (JsonElement element in dataRoot.EnumerateArray())
        {
            DataRow row = dataTable.NewRow();
            dataTable.Rows.Add(row);
            foreach (JsonProperty col in element.EnumerateObject())
            {
                if (firstPass)
                {
                    JsonElement colValue = col.Value;
                    dataTable.Columns.Add(new DataColumn(col.Name, ValueKindToType(colValue.ValueKind, colValue.ToString()!)));
                }
                row[col.Name] = JsonElementToTypedValue(col.Value);
            }
            firstPass = false;
        }

        return dataTable;
    }

    private static Type ValueKindToType(JsonValueKind valueKind, string value)
    {
        switch (valueKind)
        {
            case JsonValueKind.String:
                return typeof(string);
            case JsonValueKind.Number:
                if (long.TryParse(value, out _))
                {
                    return typeof(long);
                }
                else
                {
                    return typeof(double);
                }
            case JsonValueKind.True:
            case JsonValueKind.False:
                return typeof(bool);
            case JsonValueKind.Undefined:
                throw new NotSupportedException();
            case JsonValueKind.Object:
                return typeof(object);
            case JsonValueKind.Array:
                return typeof(Array);
            case JsonValueKind.Null:
                throw new NotSupportedException();
            default:
                return typeof(object);
        }
    }

    private static object JsonElementToTypedValue(JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Object:
            case JsonValueKind.Array:
                throw new NotSupportedException();
            case JsonValueKind.String:
                if (jsonElement.TryGetGuid(out Guid guidValue))
                {
                    return guidValue;
                }
                else
                {
                    if (jsonElement.TryGetDateTime(out DateTime datetime))
                    {
                        // If an offset was provided, use DateTimeOffset.
                        if (datetime.Kind == DateTimeKind.Local)
                        {
                            if (jsonElement.TryGetDateTimeOffset(out DateTimeOffset datetimeOffset))
                            {
                                return datetimeOffset;
                            }
                        }
                        return datetime;
                    }
                    return jsonElement.ToString();
                }
            case JsonValueKind.Number:
                if (jsonElement.TryGetInt64(out long longValue))
                {
                    return longValue;
                }
                else
                {
                    return jsonElement.GetDouble();
                }
            case JsonValueKind.True:
            case JsonValueKind.False:
                return jsonElement.GetBoolean();
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                return null;
            default:
                return jsonElement.ToString();
        }
    }

    /// <summary>
    /// STJ DataSet 数据集格式化
    /// </summary>
    public class DataSetJsonConverter : JsonConverter<DataSet>
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DataSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            JsonElement rootElement = jsonDoc.RootElement;
            DataSet dataSet = JsonElementToDataSet(rootElement);
            return dataSet;
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DataSet value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (DataTable table in value.Tables)
            {
                writer.WritePropertyName(table.TableName);
                JsonSerializer.Serialize(writer, table, options);
            }
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// IPAddress
    /// </summary>
    public class IPAddressJsonConverter : JsonConverter<IPAddress>
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IPAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return IPAddress.Parse(reader.GetString());
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    /// <summary>
    /// IPEndPoint
    /// </summary>
    public class IPEndPointJsonConverter : JsonConverter<IPEndPoint>
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IPEndPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            JsonElement rootElement = jsonDoc.RootElement;

            var address = IPAddress.Parse(rootElement.GetProperty("Address").ToString());
            var port = Convert.ToInt32(rootElement.GetProperty("port").ToString());

            return new IPEndPoint(address, port);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, IPEndPoint value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("AddressFamily", value.AddressFamily.ToString());
            writer.WriteString("Address", value.Address.ToString());
            writer.WriteNumber("Port", value.Port);
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// DirectoryInfo
    /// </summary>
    public class DirectoryInfoJsonConverter : JsonConverter<DirectoryInfo>
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DirectoryInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            JsonElement rootElement = jsonDoc.RootElement;

            var fullName = rootElement.GetProperty("FullName").ToString();
            return new DirectoryInfo(fullName);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DirectoryInfo value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Attributes", value.Attributes.ToString());
            writer.WriteString("CreationTime", value.CreationTime);
            writer.WriteString("CreationTimeUtc", value.CreationTimeUtc);
            writer.WriteBoolean("Exists", value.Exists);
            writer.WriteString("Extension", value.Extension);
            writer.WriteString("FullName", value.FullName);
            writer.WriteString("LastAccessTime", value.LastAccessTime);
            writer.WriteString("LastAccessTimeUtc", value.LastAccessTimeUtc);
            writer.WriteString("LastWriteTime", value.LastWriteTime);
            writer.WriteString("LastWriteTimeUtc", value.LastWriteTimeUtc);
            writer.WriteString("LinkTarget", value.LinkTarget);
            writer.WriteString("Name", value.Name);
            writer.WriteString("UnixFileMode", value.UnixFileMode.ToString());
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// IntPtr
    /// </summary>
    public class IntPtrJsonConverter : JsonConverter<IntPtr>
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IntPtr Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return IntPtr.Zero;
            return new IntPtr(reader.GetInt64());
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, IntPtr value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    /// <summary>
    /// Type
    /// </summary>
    public class TypeJsonConverter : JsonConverter<Type>
    {
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return null;
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.FullName);
        }
    }

    /// <summary>
    /// 构建 JSON 序列化选项
    /// </summary>
    /// <param name="formatter">时间格式</param>
    /// <param name="useStrict">严格模式</param>
    /// <returns></returns>
    public static JsonSerializerOptions JSOptions(string formatter = DefaultDateTimeFormatter, bool useStrict = false)
    {
        var options = new JsonSerializerOptions()
        {
            //包含字段 如元组 Tuple
            IncludeFields = true,
            //忽略大小写
            PropertyNameCaseInsensitive = !useStrict,
            //允许注释
            ReadCommentHandling = useStrict ? JsonCommentHandling.Disallow : JsonCommentHandling.Skip,
            //允许带引号的数字
            NumberHandling = useStrict ? JsonNumberHandling.Strict : JsonNumberHandling.AllowReadingFromString,
            //原样输出
            PropertyNamingPolicy = null,
            //编码
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            //忽略循环引用
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            //转换
            Converters =
            {
                new DateTimeJsonConverter(formatter),
                new JsonStringEnumConverter(),
                new DataTableJsonConverter(),
                new DataSetJsonConverter(),
                new IPAddressJsonConverter(),
                new IPEndPointJsonConverter(),
                new DirectoryInfoJsonConverter(),
                new IntPtrJsonConverter(),
                new TypeJsonConverter(),
            }
        };

        return options;
    }

    /// <summary>
    /// JsonNode 反序列化选项
    /// </summary>
    /// <returns></returns>
    public static JsonDocumentOptions JDOptions() => new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

}

#endif