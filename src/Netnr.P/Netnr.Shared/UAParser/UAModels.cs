#if Full || UAParser

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Netnr;

/// <summary>
/// 实体
/// </summary>
public class UAModels
{
    /// <summary>
    /// 客户端
    /// </summary>
    public List<ClientModel> ListClient { get; set; } = [];
    /// <summary>
    /// 设备
    /// </summary>
    public List<DeviceModel> ListDevice { get; set; } = [];
    /// <summary>
    /// 系统
    /// </summary>
    public List<OSModel> ListOS { get; set; } = [];
    /// <summary>
    /// 爬虫
    /// </summary>
    public List<BotModel> ListBot { get; set; } = [];

    /// <summary>
    /// 客户端
    /// </summary>
    public class ClientModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        [JsonPropertyName("T")]
        public string Type { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        [JsonIgnore]
        public Regex Rgx { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        [JsonPropertyName("R")]
        public string Regex { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [JsonPropertyName("N")]
        public string Name { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        [JsonPropertyName("V")]
        public string Version { get; set; }
        /// <summary>
        /// 引擎
        /// </summary>
        [JsonPropertyName("E")]
        public string Engine { get; set; }
    }

    /// <summary>
    /// 设备
    /// </summary>
    public class DeviceModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        [JsonPropertyName("T")]
        public string Type { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        [JsonIgnore]
        public Regex Rgx { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        [JsonPropertyName("R")]
        public string Regex { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        [JsonPropertyName("B")]
        public string Brand { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        [JsonPropertyName("D")]
        public string Device { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        [JsonPropertyName("M")]
        public string Model { get; set; }
    }

    /// <summary>
    /// 系统
    /// </summary>
    public class OSModel
    {
        /// <summary>
        /// 正则
        /// </summary>
        [JsonIgnore]
        public Regex Rgx { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        [JsonPropertyName("R")]
        public string Regex { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [JsonPropertyName("N")]
        public string Name { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        [JsonPropertyName("V")]
        public string Version { get; set; }
    }

    /// <summary>
    /// 爬虫
    /// </summary>
    public class BotModel
    {
        /// <summary>
        /// 正则
        /// </summary>
        [JsonIgnore]
        public Regex Rgx { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        [JsonPropertyName("R")]
        public string Regex { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [JsonPropertyName("N")]
        public string Name { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        [JsonPropertyName("C")]
        public string Category { get; set; }
        /// <summary>
        /// 产品
        /// </summary>
        [JsonPropertyName("P")]
        public string Producer { get; set; }
    }
}

#endif