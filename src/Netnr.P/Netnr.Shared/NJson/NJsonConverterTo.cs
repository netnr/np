#if Full || NJson

using Newtonsoft.Json;

namespace Netnr;

/// <summary>
/// 格式化
/// </summary>
public class NJsonConverterTo
{
    public const string DefaultDateTimeFormatter = "yyyy-MM-dd HH:mm:ss.fff";

    public class IPAddressConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            string ipAddressString = (string)reader.Value;
            var ipAddress = IPAddress.Parse(ipAddressString);
            return ipAddress;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }

}

#endif