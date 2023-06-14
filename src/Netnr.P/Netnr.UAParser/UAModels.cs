using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Netnr;

/// <summary>
/// 实体
/// </summary>
public class UAModels
{
    /// <summary>
    /// 客户端
    /// </summary>
    public List<ClientModel> ListClient { get; set; } = new List<ClientModel>();
    /// <summary>
    /// 设备
    /// </summary>
    public List<DeviceModel> ListDevice { get; set; } = new List<DeviceModel>();
    /// <summary>
    /// 系统
    /// </summary>
    public List<OSModel> ListOS { get; set; } = new List<OSModel>();
    /// <summary>
    /// 爬虫
    /// </summary>
    public List<BotModel> ListBot { get; set; } = new List<BotModel>();

    /// <summary>
    /// 客户端
    /// </summary>
    public class ClientModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        [XmlIgnore]
        public Regex R { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        public string Regex { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 引擎
        /// </summary>
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
        public string Type { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        [XmlIgnore]
        public Regex R { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        public string Regex { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
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
        [XmlIgnore]
        public Regex R { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        public string Regex { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
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
        [XmlIgnore]
        public Regex R { get; set; }
        /// <summary>
        /// 正则
        /// </summary>
        public string Regex { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 产品
        /// </summary>
        public string Producer { get; set; }
    }
}
