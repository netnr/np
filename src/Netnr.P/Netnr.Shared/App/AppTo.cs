#if Full || App

using Microsoft.Extensions.Configuration;

namespace Netnr
{
    public class AppTo
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static EnumTo.TypeDB TDB { get; set; }

        /// <summary>
        /// 内部访问（项目根路径）
        /// </summary>
        public static string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();

        /// <summary>
        /// web外部访问（wwwroot）
        /// </summary>
        public static string WebRootPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        private static IConfiguration Config { get; set; }

        public static IConfiguration Configuration
        {
            get
            {
                if (Config == null)
                {
                    var builder = new ConfigurationBuilder();
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                    builder.AddJsonFile(path, false, true);

                    Config = builder.Build();
                }
                return Config;
            }
        }

        /// <summary>
        /// 获取值（冒号拼接层级）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string key) => Configuration[key];

        /// <summary>
        /// 获取值（冒号拼接层级）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetValue<T>(string key) => Configuration.GetValue<T>(key);
    }
}

#endif