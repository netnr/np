using System;
using System.IO;
using System.Runtime.Caching;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Netnr.UAParser
{
    /// <summary>
    /// 解析
    /// </summary>
    public class Parsers
    {
        private static Entitys _Regexes;
        /// <summary>
        /// 正则
        /// </summary>
        public static Entitys Regexes
        {
            get
            {
                if (_Regexes == null)
                {
                    var xmlContent = NodeConvert(Properties.Resources.regexes, false);
                    _Regexes = ToEntity<Entitys>(xmlContent);
                }

                _Regexes.ListClient.ForEach(x => x.R = new Regex(x.Regex, RegexOptions.IgnoreCase));
                _Regexes.ListDevice.ForEach(x => x.R = new Regex(x.Regex, RegexOptions.IgnoreCase));
                _Regexes.ListOS.ForEach(x => x.R = new Regex(x.Regex, RegexOptions.IgnoreCase));
                _Regexes.ListBot.ForEach(x => x.R = new Regex(x.Regex, RegexOptions.IgnoreCase));

                return _Regexes;
            }
            set
            {
                _Regexes = value;
            }
        }

        /// <summary>
        /// 节点转换
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <param name="isMini">精简/还原</param>
        /// <returns></returns>
        public static string NodeConvert(string xmlContent, bool isMini)
        {
            //节点转换
            var nodeMap = "CE:ClientEntity,DE:DeviceEntity,BE:BotEntity,OE:OSEntity,T:Type,R:Regex,N:Name,V:Version,E:Engine,C:Category,P:Producer".Split(',');
            if (isMini)
            {
                foreach (var nodeItem in nodeMap)
                {
                    var kv = nodeItem.Split(':');
                    xmlContent = xmlContent.Replace($"<{kv[1]}>", $"<{kv[0]}>").Replace($"</{kv[1]}>", $"</{kv[0]}>");
                }
                xmlContent = xmlContent.Replace("<Version />", "<V />").Replace("<Producer />", "<P />");
            }
            else
            {
                foreach (var nodeItem in nodeMap)
                {
                    var kv = nodeItem.Split(':');
                    xmlContent = xmlContent.Replace($"<{kv[0]}>", $"<{kv[1]}>").Replace($"</{kv[0]}>", $"</{kv[1]}>");
                }
                xmlContent = xmlContent.Replace("<V />", "<Version />").Replace("<P />", "<Producer />");
            }

            return xmlContent;
        }

        /// <summary>
        /// 缓存
        /// </summary>
        public static MemoryCache memoryCache = MemoryCache.Default;

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="second">过期时间，默认 1800 秒</param>
        /// <param name="sliding">相对时间过期</param>
        public static void SetCache(string key, object value, int second = 1800, bool sliding = true)
        {
            var cip = new CacheItemPolicy();

            if (sliding)
            {
                cip.SlidingExpiration = TimeSpan.FromSeconds(second);
            }
            else
            {
                cip.AbsoluteExpiration = DateTime.Now.AddSeconds(second);
            }

            memoryCache.Set(key, value, cip);
        }

        /// <summary>
        /// User-Agent
        /// </summary>
        public string UserAgent;
        /// <summary>
        /// 解析，构造函数
        /// </summary>
        /// <param name="userAgent"></param>
        public Parsers(string userAgent)
        {
            UserAgent = userAgent;
        }

        /// <summary>
        /// 客户端
        /// </summary>
        /// <returns></returns>
        public Entitys.ClientEntity GetClient()
        {
            var ckey = $"ua:client:{UserAgent.GetHashCode()}";
            if (memoryCache.Contains(ckey))
            {
                return memoryCache.Get(ckey) as Entitys.ClientEntity;
            }

            Entitys.ClientEntity result = null;
            foreach (var item in Regexes.ListClient)
            {
                var match = item.R.Match(UserAgent);
                if (match.Success)
                {
                    if (match.Groups.Count > 1 && item.Version.Contains("$1"))
                    {
                        item.Version = item.Version.Replace("$1", match.Groups[1].ToString());
                    }
                    if (match.Groups.Count > 2 && item.Version.Contains("$2"))
                    {
                        item.Version = item.Version.Replace("$2", match.Groups[2].ToString());
                    }
                    result = item;
                    break;
                }
            }
            SetCache(ckey, result == null ? true : result);

            return result;
        }

        /// <summary>
        /// 设备
        /// </summary>
        /// <returns></returns>
        public Entitys.DeviceEntity GetDevice()
        {
            var ckey = $"ua:device:{UserAgent.GetHashCode()}";
            if (memoryCache.Contains(ckey))
            {
                return memoryCache.Get(ckey) as Entitys.DeviceEntity;
            }

            Entitys.DeviceEntity result = null;
            foreach (var item in Regexes.ListDevice)
            {
                var match = item.R.Match(UserAgent);
                if (match.Success)
                {
                    result = item;
                    break;
                }
            }
            SetCache(ckey, result == null ? true : result);

            return result;
        }

        /// <summary>
        /// 系统
        /// </summary>
        /// <returns></returns>
        public Entitys.OSEntity GetOS()
        {
            var ckey = $"ua:os:{UserAgent.GetHashCode()}";
            if (memoryCache.Contains(ckey))
            {
                return memoryCache.Get(ckey) as Entitys.OSEntity;
            }

            Entitys.OSEntity result = null;
            foreach (var item in Regexes.ListOS)
            {
                var match = item.R.Match(UserAgent);
                if (match.Success)
                {
                    if (match.Groups.Count > 1)
                    {
                        if (item.Name.Contains("$1"))
                        {
                            item.Name = item.Name.Replace("$1", match.Groups[1].ToString());
                        }
                        if (item.Version.Contains("$1"))
                        {
                            item.Version = item.Version.Replace("$1", match.Groups[1].ToString());
                        }
                    }
                    if (match.Groups.Count > 2 && item.Version.Contains("$2"))
                    {
                        item.Version = item.Version.Replace("$2", match.Groups[2].ToString());
                    }
                    if (match.Groups.Count > 3 && item.Version.Contains("$3"))
                    {
                        item.Version = item.Version.Replace("$3", match.Groups[2].ToString());
                    }
                    result = item;
                    break;
                }
            }
            SetCache(ckey, result == null ? true : result);

            return result;
        }

        /// <summary>
        /// 爬虫
        /// </summary>
        /// <returns></returns>
        public Entitys.BotEntity GetBot()
        {
            var ckey = $"ua:bot:{UserAgent.GetHashCode()}";
            if (memoryCache.Contains(ckey))
            {
                return memoryCache.Get(ckey) as Entitys.BotEntity;
            }

            Entitys.BotEntity result = null;
            foreach (var item in Regexes.ListBot)
            {
                var match = item.R.Match(UserAgent);
                if (match.Success)
                {
                    result = item;
                    break;
                }
            }
            SetCache(ckey, result == null ? true : result);

            return result;
        }

        /// <summary>
        /// 生成 XML
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXml(object obj)
        {
            if (obj == null) return null;

            using var sw = new StringWriter();
            var serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(sw, obj);
            return sw.ToString();
        }

        /// <summary>
        /// XML 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T ToEntity<T>(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return default;

            using var sr = new StringReader(xml);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(sr);
        }
    }
}
