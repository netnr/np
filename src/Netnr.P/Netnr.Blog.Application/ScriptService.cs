using Netnr.Core;
using Netnr.SharedFast;
using Newtonsoft.Json.Linq;

namespace Netnr.Blog.Application
{
    /// <summary>
    /// 脚本服务
    /// </summary>
    public class ScriptService
    {
        public static JArray Nav
        {
            get
            {
                var fullPath = PathTo.Combine(GlobalTo.WebRootPath, "file/ss/nav.json");
                return File.ReadAllText(fullPath).ToJArray();
            }
        }

        /// <summary>
        /// 返回一项
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static JToken Find(string action)
        {
            foreach (var gi in Nav)
            {
                foreach (JToken jt in gi["items"])
                {
                    if (jt["url"].ToString().TrimStart('/') == action)
                    {
                        return jt;
                    }
                }
            }

            return null;
        }
    }
}
