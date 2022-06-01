using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Netnr.UAParser
{
    /// <summary>
    /// 实体
    /// </summary>
    public class Entitys
    {
        /// <summary>
        /// 客户端
        /// </summary>
        public List<ClientEntity> ListClient { get; set; } = new List<ClientEntity>();
        /// <summary>
        /// 设备
        /// </summary>
        public List<DeviceEntity> ListDevice { get; set; } = new List<DeviceEntity>();
        /// <summary>
        /// 系统
        /// </summary>
        public List<OSEntity> ListOS { get; set; } = new List<OSEntity>();
        /// <summary>
        /// 爬虫
        /// </summary>
        public List<BotEntity> ListBot { get; set; } = new List<BotEntity>();

        /// <summary>
        /// 客户端
        /// </summary>
        public class ClientEntity
        {
            /// <summary>
            /// 类型
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// 正则
            /// </summary>
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
        public class DeviceEntity
        {
            /// <summary>
            /// 类型
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// 正则
            /// </summary>
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
        public class OSEntity
        {
            /// <summary>
            /// 正则
            /// </summary>
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
        public class BotEntity
        {
            /// <summary>
            /// 正则
            /// </summary>
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
}
