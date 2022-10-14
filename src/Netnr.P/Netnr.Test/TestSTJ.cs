using System.Text.Encodings.Web;
using System.Text.Unicode;
using Xunit;

namespace Netnr.Test
{
    /// <summary>
    /// System.Text.Json
    /// https://docs.microsoft.com/zh-cn/dotnet/standard/serialization/system-text-json-how-to
    /// </summary>
    public class TestSTJ
    {
        public string JsonText { get; set; } = "{\"site\":{\"title\":\"NET牛人\",\"domain\":\"https://www.netnr.com\",\"mirror\":\"https://netnr.zme.ink\",\"createtime\":\"2014.01.01\"},\"about\":{\"name\":\"netnr\",\"sex\":\"男\",\"injob\":\"2012.03.01\",\"live\":\"中国重庆\",\"mail\":\"netnr@netnr.com\",\"git\":[{\"name\":\"github\",\"url\":\"https://github.com/netnr\"},{\"name\":\"gitee\",\"url\":\"https://gitee.com/netnr\"}]},\"update\":\"2022.07.16\",\"version\":\"v.1.0.0\"}";

        /// <summary>
        /// STJ 读 JSON ，更高的性能
        /// </summary>
        [Fact]
        public void STJDocument()
        {
            var jsonElement = JsonDocument.Parse(JsonText).RootElement;

            Debug.WriteLine(jsonElement.GetProperty("update").ToString());

            var jsonObj = jsonElement.EnumerateObject();
            foreach (var item in jsonObj)
            {
                Debug.WriteLine(item.Name);
                Debug.WriteLine(item.Value);

                if (item.Name == "about")
                {
                    var jsonArr = item.Value.GetProperty("git").EnumerateArray();
                    foreach (var item2 in jsonArr)
                    {
                        Debug.WriteLine(item2.GetProperty("name"));
                        Debug.WriteLine(item2.GetProperty("url"));
                    }
                }
            }

            var joa = "{\"value\":[1,2]}".DeJson();
            Debug.WriteLine(joa.GetProperty("value")[1]);

            Debug.WriteLine(joa.GetValue("value"));
            Debug.WriteLine(joa.GetValue<int>("value3"));
            Debug.WriteLine(joa.GetValue<int?>("value3"));

            //Debug.WriteLine(joa.GetProperty("value2")); //error

            var vm = new ResultVM
            {
                Data = new
                {
                    jsonElement,
                    sub = new ResultVM()
                }
            };
            Debug.WriteLine(vm.ToJson());
            Debug.WriteLine(vm.ToJson().DeJson().GetProperty("data").GetValue<ResultVM>("sub"));

            var jsonFormat = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions()
            {
                WriteIndented = true, // 缩进
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) //编码
            });

            Debug.WriteLine(jsonFormat);
        }

        /// <summary>
        /// STJ 读写 JSON
        /// </summary>
        [Fact]
        public void STJNode()
        {
            var jsonNode = JsonNode.Parse(JsonText);
            Debug.WriteLine(jsonNode["update"].ToString());

            jsonNode["update"] = DateTime.Now;
            Debug.WriteLine(jsonNode["update"].ToString());

            foreach (var item in jsonNode.AsObject())
            {
                Debug.WriteLine(item.Key);
                Debug.WriteLine(item.Value);

                if (item.Key == "about")
                {
                    foreach (var item2 in item.Value["git"].AsArray())
                    {
                        Debug.WriteLine(item2["name"]);
                        Debug.WriteLine(item2["url"]);
                    }
                }
            }

            var jdo = new JsonDocumentOptions()
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            var jnp = JsonNode.Parse(JsonText, documentOptions: jdo);
            Assert.True(jnp != null);
        }

        [Fact]
        public void STJOther()
        {
            var cc = new ColorClass();
            Debug.WriteLine(JsonSerializer.Serialize(cc));

            var vm = new ResultVM
            {
                Data = new
                {
                    Abc = 1,
                    Bit = DateTime.Now,
                    Com = true,
                    ET = EnumType.Get,
                    coloc = new ColorClass()
                }
            };

            var options = new JsonSerializerOptions()
            {
                IncludeFields = true, //包含字段 如元组 Tuple
                PropertyNameCaseInsensitive = true, // 反序列化，不区分大小写，默认区分

                ReadCommentHandling = JsonCommentHandling.Skip, //允许注释
                AllowTrailingCommas = true, //允许末尾逗号
                //允许带引号的数字（读、写）
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,

                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, //忽略 null 值 

                WriteIndented = true, // 缩进
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) //编码
            };
            options.Converters.Add(new DateTimeJsonConverterTo()); //时间格式化
            options.Converters.Add(new JsonStringEnumConverter()); //枚举字符串

            var jsonFormat = JsonSerializer.Serialize(vm, options);
            Debug.WriteLine(jsonFormat);

            var vals = MultiValue();
            Debug.WriteLine(JsonSerializer.Serialize(vals, options));
        }

        private static (string v1, int v2, DateTime v3) MultiValue()
        {
            return new("1", 2, DateTime.Now);
        }

        /// <summary>
        /// 枚举，转为字符串
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum EnumType
        {
            Get,
            Head,
            Post,
            Put,
            Patch,
            Delete
        }

        public class ColorClass
        {
            /// <summary>
            /// 忽略属性
            /// </summary>
            [JsonIgnore]
            public string Red { get; set; } = "red";

            /// <summary>
            /// 指定属性名
            /// </summary>
            [JsonPropertyName("blue")]
            public string Blue { get; set; } = "blue";

            public int Order { get; set; } = 99;

            /// <summary>
            /// 指定顺序
            /// </summary>
            [JsonPropertyOrder(-5)]
            public string Orange { get; set; } = "orange";
        }

        /// <summary>
        /// STJ 时间格式化
        /// </summary>
        public class DateTimeJsonConverterTo : JsonConverter<DateTime>
        {
            public string Formatter { get; set; }

            public DateTimeJsonConverterTo(string formatter = "yyyy-MM-dd HH:mm:ss.fff")
            {
                Formatter = formatter;
            }

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.Parse(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(Formatter));
            }
        }
    }
}
