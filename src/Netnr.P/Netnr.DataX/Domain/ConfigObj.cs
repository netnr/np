using Newtonsoft.Json.Linq;
using Netnr.Core;

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

            Config = FileTo.ReadText(PathTo.Combine(DXPath, "config.json")).ToJObject();

            var dcsFiles = Directory.GetFiles(DXHub, "*.cs");
            foreach (var path in dcsFiles)
            {
                Compile.Add(Path.GetFileName(path), FileTo.ReadText(path));
            }

            var listConns = new List<DbConnObj>();
            var dbconns = Config["db-conn"] as JArray;
            for (int i = 0; i < dbconns.Count; i++)
            {
                var dbc = dbconns[i];
                var obj = new DbConnObj()
                {
                    TDB = Enum.Parse<SharedEnum.TypeDB>(dbc["type"].ToString(), true),
                    Conn = dbc["conn"].ToString(),
                    Remark = dbc["remark"].ToString()
                };

                if ((obj.TDB == SharedEnum.TypeDB.MySQL || obj.TDB == SharedEnum.TypeDB.MariaDB) && !obj.Conn.Contains("AllowLoadLocalInfile"))
                {
                    obj.Conn = obj.Conn.TrimEnd(';') + ";AllowLoadLocalInfile=true";
                }

                listConns.Add(obj);
            }
            DbConns = listConns;

            MappingTableFullMatch = Config["mapping-table-full-match"].Value<bool>();
            MappingColumnFullMatch = Config["mapping-column-full-match"].Value<bool>();
        }

        /// <summary>
        /// DX 根路径
        /// </summary>
        public string DXPath { get; set; }

        /// <summary>
        /// DX 枢纽
        /// </summary>
        public string DXHub
        {
            get
            {
                return PathTo.Combine(DXPath, "hub");
            }
        }

        /// <summary>
        /// 配置信息
        /// </summary>
        public JObject Config { get; set; }
        /// <summary>
        /// 动态编译对象
        /// </summary>
        public Dictionary<string, string> Compile { get; set; } = new();
        /// <summary>
        /// 表映射完整匹配
        /// </summary>
        public bool MappingTableFullMatch { get; set; }
        /// <summary>
        /// 列映射完整匹配
        /// </summary>
        public bool MappingColumnFullMatch { get; set; }
        /// <summary>
        /// 连接信息
        /// </summary>
        public List<DbConnObj> DbConns = new();
    }

}
