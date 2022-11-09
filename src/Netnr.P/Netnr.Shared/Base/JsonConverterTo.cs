#if Full || Base

using System.Text.Unicode;
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
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// 格式
        /// </summary>
        public string Formatter { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="formatter"></param>
        public DateTimeJsonConverter(string formatter = DefaultDateTimeFormatter)
        {
            Formatter = formatter;
        }

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
    /// https://github.com/dotnet/docs/blob/main/docs/standard/serialization/system-text-json/snippets/system-text-json-how-to/csharp/RoundtripDataTable.cs
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
            DataTable dataTable = AsDataTable(rootElement);
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
                        ushort value => key => writer.WriteNumber(key, value),
                        int value => key => writer.WriteNumber(key, value),
                        uint value => key => writer.WriteNumber(key, value),

                        //warning: JavaScript Number.MAX_SAFE_INTEGER=9007199254740991 use string
                        long value => key => writer.WriteNumber(key, value),
                        ulong value => key => writer.WriteNumber(key, value),

                        // strings
                        DateTime value => key => writer.WriteString(key, value),
                        Guid value => key => writer.WriteString(key, value),

                        // binary
                        byte[] value => key => writer.WriteBase64String(key, value),

                        // null
                        DBNull value => key => writer.WriteNull(key),

                        _ => key => writer.WriteString(key, row[column].ToString())
                    };
                }
                jsonWriter.WriteEndObject();
            }
            jsonWriter.WriteEndArray();
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
            DataSet dataSet = AsDataSet(rootElement);
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
    /// 转 DataTable
    /// </summary>
    /// <param name="dataRoot"></param>
    /// <returns></returns>
    public static DataTable AsDataTable(JsonElement dataRoot)
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
                    dataTable.Columns.Add(new DataColumn(col.Name, ValueKindAsType(colValue.ValueKind, colValue.ToString()!)));
                }
                row[col.Name] = AsTypedValue(col.Value);
            }
            firstPass = false;
        }

        return dataTable;
    }

    /// <summary>
    /// 转 DataSet
    /// </summary>
    /// <param name="dataRoot"></param>
    /// <returns></returns>
    public static DataSet AsDataSet(JsonElement dataRoot)
    {
        var dataSet = new DataSet();
        foreach (var item in dataRoot.EnumerateObject())
        {
            var dataTable = AsDataTable(item.Value);
            dataTable.TableName = item.Name;
            dataSet.Tables.Add(dataTable);
        }
        return dataSet;
    }

    private static Type ValueKindAsType(JsonValueKind valueKind, string value)
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
                return typeof(System.Array);
            case JsonValueKind.Null:
                throw new NotSupportedException();
            default:
                return typeof(object);
        }
    }

    private static object AsTypedValue(JsonElement jsonElement)
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
            //格式化
            Converters = {
                new DateTimeJsonConverter(formatter), //时间格式化
                new JsonStringEnumConverter(), //枚举字符串
                new DataTableJsonConverter(), //数据表格式化
                new DataSetJsonConverter() //数据集格式化
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