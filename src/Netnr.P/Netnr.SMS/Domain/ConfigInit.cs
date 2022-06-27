using Newtonsoft.Json.Linq;

namespace Netnr.SMS.Domain
{
    /// <summary>
    /// 配置初始化
    /// </summary>
    public class ConfigInit
    {
        public JObject ConfigJson { get; set; }

        public ConfigInit()
        {
            var configPath = Path.Combine(ProjectDir, "config.json");
            if (File.Exists(configPath))
            {
                ConfigJson = File.ReadAllText(configPath).ToJObject();
            }
        }

#if DEBUG
        public static string ProjectDir = AppContext.BaseDirectory.Split("bin")[0];
#else
        public static string ProjectDir = AppContext.BaseDirectory;
#endif

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="keyPath"></param>
        /// <returns></returns>
        public string GetValue(string keyPath) => GetValue<string>(keyPath);

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyPath"></param>
        /// <returns></returns>
        public T GetValue<T>(string keyPath)
        {
            var keys = keyPath.Split(':');
            JToken val = ConfigJson;
            foreach (var key in keys)
            {
                try
                {
                    val = val[key];
                }
                catch (Exception)
                {
                    break;
                }
            }

            return val.ToString().ToConvert<T>();
        }
    }

}
