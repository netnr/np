using Newtonsoft.Json.Linq;

namespace Netnr.DataX.Domain
{
    /// <summary>
    /// 配置初始化
    /// </summary>
    public class ConfigInit
    {
        public ConfigInit()
        {

#if DEBUG

            DXPath = Path.Combine(AppContext.BaseDirectory.Split("bin")[0], "ud");
#else
            DXPath = Path.Combine(AppContext.BaseDirectory, "ud");
#endif

            var content = File.ReadAllText(Path.Combine(DXPath, "config.json"));
            ConfigObj = content.ToEntity<ConfigDomain>();

            Silence = content.ToJObject()[nameof(Silence)] as JObject;

            if (!Directory.Exists(DXHub))
            {
                Directory.CreateDirectory(DXHub);
            }
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
                return Path.Combine(DXPath, "hub");
            }
        }

        public ConfigDomain ConfigObj { get; set; }

        public JObject Silence { get; set; }
    }

}
