using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Netnr.Core;
using Netnr.SharedDataKit;

namespace Netnr.DataX.Domain
{
    /// <summary>
    /// 配置
    /// </summary>
    public class ConfigObj
    {
        public ConfigObj()
        {

#if DEBUG
            DXPath = PathTo.Combine(AppContext.BaseDirectory.Split("bin")[0], "ud");
#else
            DXPath = PathTo.Combine(AppContext.BaseDirectory, "ud");
#endif

            Init = FileTo.ReadText(PathTo.Combine(DXPath, "init.json")).ToJObject();

            //已设置配置
            if (!string.IsNullOrWhiteSpace(ConfigName))
            {
                ConfigFolder = ConfigName.Split('.')[0];
                Config = FileTo.ReadText(PathTo.Combine(DXPath, ConfigName)).ToJObject();

                var dcsPath = PathTo.Combine(DXPath, ConfigFolder);
                if (Directory.Exists(dcsPath))
                {
                    var dcsFiles = Directory.GetFiles(PathTo.Combine(DXPath, ConfigFolder), "*.cs");
                    foreach (var path in dcsFiles)
                    {
                        Compile.Add(Path.GetFileName(path), FileTo.ReadText(path));
                    }
                }

                OdTypeDB = DataKitAidTo.GetTypeDB(Config["od-type"].ToString());
                OdConn = Config["od-conn"].ToString();
                NdTypeDB = DataKitAidTo.GetTypeDB(Config["nd-type"].ToString());
                NdConn = Config["nd-conn"].ToString();

                MappingFullMatchTable = Init["table-mapping-full-match"].Value<bool>();
                MappingFullMatchColumn = Init["column-mapping-full-match"].Value<bool>();

                MappingTableName = Init["table-mapping-name"].ToString();
                MappingColumnName = Init["column-mapping-name"].ToString();

                var mapTableJson = FileTo.ReadText(PathTo.Combine(DXPath, ConfigFolder, MappingTableName));
                if (!string.IsNullOrWhiteSpace(mapTableJson))
                {
                    MappingTable = mapTableJson.ToJObject();
                }
                var mapColumnJson = FileTo.ReadText(PathTo.Combine(DXPath, ConfigFolder, MappingColumnName));
                if (!string.IsNullOrWhiteSpace(mapColumnJson))
                {
                    MappingColumn = mapColumnJson.ToJObject();
                }
            }
        }

        /// <summary>
        /// DX 根路径
        /// </summary>
        public string DXPath { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        public JObject Init { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public JObject Config { get; set; }
        /// <summary>
        /// 动态编译对象
        /// </summary>
        public Dictionary<string, string> Compile { get; set; } = new();
        /// <summary>
        /// 配置名
        /// </summary>
        public string ConfigName
        {
            get
            {
                return Application.DXService.ConfigName;
            }
        }
        /// <summary>
        /// 配置文件夹
        /// </summary>
        public string ConfigFolder { get; set; }
        /// <summary>
        /// 原数据库类型
        /// </summary>
        public SharedEnum.TypeDB OdTypeDB { get; set; }
        /// <summary>
        /// 原数据库连接
        /// </summary>
        public string OdConn { get; set; }
        /// <summary>
        /// 新数据库类型
        /// </summary>
        public SharedEnum.TypeDB NdTypeDB { get; set; }
        /// <summary>
        /// 新数据库连接
        /// </summary>
        public string NdConn { get; set; }
        /// <summary>
        /// 表映射完整匹配
        /// </summary>
        public bool MappingFullMatchTable { get; set; }
        /// <summary>
        /// 列映射完整匹配
        /// </summary>
        public bool MappingFullMatchColumn { get; set; }
        /// <summary>
        /// 表映射文件名
        /// </summary>
        public string MappingTableName { get; set; }
        /// <summary>
        /// 列映射文件名
        /// </summary>
        public string MappingColumnName { get; set; }
        /// <summary>
        /// 表映射
        /// </summary>
        public JObject MappingTable { get; set; }
        /// <summary>
        /// 列映射
        /// </summary>
        public JObject MappingColumn { get; set; }
    }

}
